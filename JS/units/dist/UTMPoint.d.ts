import { ICloneable } from "@juniper-lib/util";
import { Vec2, Vec3 } from "gl-matrix";
import { ILatLngPoint, LatLngPoint } from "./LatLngPoint";
/**
 * The globe hemispheres in which the UTM point could sit.
 **/
export type GlobeHemisphere = "northern" | "southern";
export interface IUTMPoint {
    easting: number;
    northing: number;
    altitude: number;
    zone: number;
    hemisphere: GlobeHemisphere;
}
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
export declare class UTMPoint implements IUTMPoint, ICloneable {
    #private;
    static centroid(points: UTMPoint[]): UTMPoint;
    /**
     * The east/west component of the coordinate.
     **/
    get easting(): number;
    /**
     * The north/south component of the coordinate.
     **/
    get northing(): number;
    /**
     * An altitude component.
     **/
    get altitude(): number;
    /**
     * The UTM Zone for which this coordinate represents.
     **/
    get zone(): number;
    /**
     * The hemisphere in which the UTM point sits.
     **/
    get hemisphere(): "northern" | "southern";
    /**
     * Initialize a zero UTMPoint
     */
    constructor();
    /**
     * Initializes a UTMPoint as a copy of another UTMPoint
     * @param copy
     */
    constructor(copy: IUTMPoint);
    /**
     * InitialnorthingTMPoint from the given components
     * @param easting
     * @param northing
     * @param altitude
     * @param zone
     * @param hemisphere
     */
    constructor(easting: number, northing: number, altitude: number, zone: number);
    toJSON(): string;
    toString(): string;
    equals(other: IUTMPoint): boolean;
    /**
     * Converts this UTMPoint to a Latitude/Longitude point using the WGS-84 datum. The
     * coordinate pair's units will be in meters, and should be usable to make distance
     * calculations over short distances.
     * reference: http://www.uwgb.edu/dutchs/usefuldata/utmformulas.htm
     **/
    fromLatLng(latLng: ILatLngPoint): UTMPoint;
    rezone(newZone: number): UTMPoint;
    /**
     * Converts this UTMPoint to a Latitude/Longitude point using the WGS-84 datum. The
     * coordinate pair's units will be in meters, and should be usable to make distance
     * calculations over short distances.
     * reference: http://www.uwgb.edu/dutchs/usefuldata/utmformulas.htm
     **/
    toLatLng(): LatLngPoint;
    toVec2(): Vec2;
    fromVec2(arr: Vec2, zone: number): UTMPoint;
    toVec3(): Vec3;
    fromVec3(arr: Vec3, zone: number): UTMPoint;
    copy(other: IUTMPoint): UTMPoint;
    clone(): UTMPoint;
}
//# sourceMappingURL=UTMPoint.d.ts.map