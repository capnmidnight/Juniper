import { deg2rad } from "@juniper-lib/tslib/dist/math";
import { isDefined, isObject } from "@juniper-lib/tslib/dist/typeChecks";
import { Vec2, Vec3 } from "gl-matrix/dist/esm";
import { DatumWGS_84 } from "./Datum";
import { LatLngPoint } from "./LatLngPoint";
/**
 * The Universal Transverse Mercator (UTM) conformal projection uses a 2-dimensional Cartesian
 * coordinate system to give locations on the surface of the Earth. Like the traditional method
 * of latitude and longitude, it is a horizontal position representation, i.e. it is used to
 * identify locations on the Earth independently of vertical position. However, it differs from
 * that method in several respects.
 *
 * The UTM system is not a single map projection. The system instead divides the Earth into sixty
 * zones, each being a six-degree band of longitude, and uses a secant transverse Mercator
 * projection in each zone.
 **/
export class UTMPoint {
    static centroid(points) {
        const zoneCounts = new Map();
        points.forEach((p) => zoneCounts.set(p.zone, (zoneCounts.get(p.zone) || 0) + 1));
        let maxZone = 0;
        let maxZoneCount = 0;
        for (const [zone, count] of zoneCounts) {
            if (count > maxZoneCount) {
                maxZone = zone;
                maxZoneCount = count;
            }
        }
        const scale = 1 / points.length;
        const vec = points
            .map((p) => p
            .rezone(maxZone)
            .toVec3())
            .reduce((a, b) => a.scaleAndAdd(b, scale), new Vec3());
        return new UTMPoint()
            .fromVec3(vec, maxZone)
            // reset the zone
            .toLatLng()
            .toUTM();
    }
    /**
     * The east/west component of the coordinate.
     **/
    get easting() {
        return this._easting;
    }
    /**
     * The north/south component of the coordinate.
     **/
    get northing() {
        return this._northing;
    }
    /**
     * An altitude component.
     **/
    get altitude() {
        return this._altitude;
    }
    /**
     * The UTM Zone for which this coordinate represents.
     **/
    get zone() {
        return this._zone;
    }
    /**
     * The hemisphere in which the UTM point sits.
     **/
    get hemisphere() {
        return this.northing >= 0
            ? "northern"
            : "southern";
    }
    constructor(eastingOrCopy, northing, altitude, zone) {
        if (isObject(eastingOrCopy)) {
            this._easting = eastingOrCopy.easting;
            this._northing = eastingOrCopy.northing;
            this._altitude = eastingOrCopy.altitude;
            this._zone = eastingOrCopy.zone;
        }
        else {
            this._easting = eastingOrCopy || 0;
            this._northing = northing || 0;
            this._altitude = altitude || 0;
            this._zone = zone || 0;
        }
    }
    toJSON() {
        return JSON.stringify({
            easting: this.easting,
            northing: this.northing,
            altitude: this.altitude,
            zone: this.zone,
            hemisphere: this.hemisphere
        });
    }
    toString() {
        return `(${this.easting}, ${this.northing}, ${this.altitude}) zone ${this.zone}`;
    }
    equals(other) {
        return isDefined(other)
            && this.hemisphere == other.hemisphere
            && this.easting == other.easting
            && this.northing == other.northing
            && this.altitude == other.altitude
            && this.zone == other.zone;
    }
    static A(cosPhi, lng, utmz) {
        const zcm = 3 + (6 * (utmz - 1)) - 180;
        return deg2rad(lng - zcm) * cosPhi;
    }
    static getZoneWidthAtLatitude(lat) {
        const phi = deg2rad(lat);
        const sinPhi = Math.sin(phi);
        const cosPhi = Math.cos(phi);
        const tanPhi = sinPhi / cosPhi;
        const ePhi = DatumWGS_84.e * sinPhi;
        const N = DatumWGS_84.equatorialRadius / Math.sqrt(1 - (ePhi * ePhi));
        // Easting
        const T = tanPhi * tanPhi;
        const C = DatumWGS_84.e0sq * cosPhi * cosPhi;
        const Tsqr = T * T;
        const A = deg2rad(3) * cosPhi;
        const Asqr = A * A;
        const x0 = 1
            - T
            + C;
        const x1 = 5
            - 18 * T
            + Tsqr + 72 * C
            - 58 * DatumWGS_84.e0sq;
        const x2 = x0 / 6
            + Asqr * x1 / 120;
        const x3 = 1
            + Asqr * x2;
        const width = 2 * DatumWGS_84.pointScaleFactor * N * A * x3;
        return width;
    }
    /**
     * Converts this UTMPoint to a Latitude/Longitude point using the WGS-84 datum. The
     * coordinate pair's units will be in meters, and should be usable to make distance
     * calculations over short distances.
     * reference: http://www.uwgb.edu/dutchs/usefuldata/utmformulas.htm
     **/
    fromLatLng(latLng) {
        const phi = deg2rad(latLng.lat);
        const sinPhi = Math.sin(phi);
        const cosPhi = Math.cos(phi);
        const cosPhi2 = 2 * cosPhi;
        const sin2Phi = cosPhi2 * sinPhi;
        const cos2Phi = cosPhi2 * cosPhi - 1;
        const cos2Phi2 = 2 * cos2Phi;
        const sin4Phi = cos2Phi2 * sin2Phi;
        const cos4Phi = cos2Phi2 * cos2Phi - 1;
        const sin6Phi = sin4Phi * cos2Phi
            + cos4Phi * sin2Phi;
        const tanPhi = sinPhi / cosPhi;
        const ePhi = DatumWGS_84.e * sinPhi;
        const N = DatumWGS_84.equatorialRadius / Math.sqrt(1 - (ePhi * ePhi));
        const M = DatumWGS_84.equatorialRadius * (phi * DatumWGS_84.alpha1
            - sin2Phi * DatumWGS_84.alpha2
            + sin4Phi * DatumWGS_84.alpha3
            - sin6Phi * DatumWGS_84.alpha4);
        const utmz = 1 + ((latLng.lng + 180) / 6) | 0;
        const A = UTMPoint.A(cosPhi, latLng.lng, utmz);
        const Asqr = A * A;
        // Easting
        const T = tanPhi * tanPhi;
        const C = DatumWGS_84.e0sq * cosPhi * cosPhi;
        const Tsqr = T * T;
        const x0 = 1
            - T
            + C;
        const x1 = 5
            - 18 * T
            + Tsqr
            + 72 * C
            - 58 * DatumWGS_84.e0sq;
        const x2 = Asqr * x1 / 120;
        const x3 = x0 / 6
            + x2;
        const x4 = 1
            + Asqr * x3;
        const easting = DatumWGS_84.pointScaleFactor * N * A * x4
            + DatumWGS_84.E0;
        // Northing
        const y0 = 5
            - T
            + 9 * C
            + 4 * C * C;
        const y1 = 61
            - 58 * T
            + Tsqr
            + 600 * C
            - 330 * DatumWGS_84.e0sq;
        const y2 = y0 / 24
            + Asqr * y1 / 720;
        const y3 = 0.5
            + Asqr * y2;
        const y4 = M
            + N * tanPhi * Asqr * y3;
        const northing = DatumWGS_84.pointScaleFactor * y4;
        this._easting = easting;
        this._northing = northing;
        this._altitude = latLng.alt;
        this._zone = utmz;
        return this;
    }
    rezone(newZone) {
        if (!(1 <= newZone && newZone <= 60)) {
            throw new Error(`Zones must be on the range [1, 60]. Given: ${newZone}`);
        }
        if (newZone !== this.zone) {
            const deltaZone = newZone - this.zone;
            const ll = this.toLatLng();
            const width = UTMPoint.getZoneWidthAtLatitude(ll.lat);
            return new UTMPoint(this.easting - width * deltaZone, this.northing, this.altitude, newZone);
        }
        else {
            return this;
        }
    }
    /**
     * Converts this UTMPoint to a Latitude/Longitude point using the WGS-84 datum. The
     * coordinate pair's units will be in meters, and should be usable to make distance
     * calculations over short distances.
     * reference: http://www.uwgb.edu/dutchs/usefuldata/utmformulas.htm
     **/
    toLatLng() {
        return new LatLngPoint().fromUTM(this);
    }
    toVec2() {
        return new Vec2(this.easting, -this.northing);
    }
    fromVec2(arr, zone) {
        this._easting = arr.x;
        this._northing = -arr.y;
        this._altitude = 0;
        this._zone = zone;
        return this;
    }
    toVec3() {
        return new Vec3(this.easting, this.altitude, -this.northing);
    }
    fromVec3(arr, zone) {
        this._easting = arr.x;
        this._altitude = arr.y;
        this._northing = -arr.z;
        this._zone = zone;
        return this;
    }
    copy(other) {
        this._easting = other.easting;
        this._northing = other.northing;
        this._altitude = other.altitude;
        this._zone = other.zone;
        return this;
    }
    clone() {
        return new UTMPoint(this);
    }
}
//# sourceMappingURL=UTMPoint.js.map