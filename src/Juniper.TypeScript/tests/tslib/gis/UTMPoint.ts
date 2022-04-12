import { vec3 } from "gl-matrix";
import { TestCase } from "juniper-tdd/tdd";
import { LatLngPoint, UTMPoint } from "juniper-tslib";


export class UTMPointTests extends TestCase {

    override setup() {
    }

    test_JonesPoint() {
        const jonesPointExpected = new LatLngPoint(38.790342, -77.040682, 0);
        const jonesPointUTMActual = jonesPointExpected.toUTM();
        const jonesPointUTMExpected = new UTMPoint(322765.32, 4295488.80, 0, 18, "northern");
        const jonesPointActual = jonesPointUTMExpected.toLatLng();

        this.areApprox(jonesPointUTMActual.easting, jonesPointUTMExpected.easting, "Easting");
        this.areApprox(jonesPointUTMActual.northing, jonesPointUTMExpected.northing, "Northing");
        this.areApprox(jonesPointUTMActual.altitude, jonesPointUTMExpected.altitude, "Altitude");
        this.areApprox(jonesPointUTMActual.zone, jonesPointUTMExpected.zone, "Zone");
        this.areExact(jonesPointUTMActual.hemisphere, jonesPointUTMExpected.hemisphere, "Hemisphere");
        this.areApprox(jonesPointActual.lat, jonesPointExpected.lat, "Latitude");
        this.areApprox(jonesPointActual.lng, jonesPointExpected.lng, "Longitude");
        this.areApprox(jonesPointActual.alt, jonesPointExpected.alt, "Altitude");
    }

    test_Zone1() {
        const uA1 = new UTMPoint(0, 0, 0, 0, "northern");
        const uB1 = new UTMPoint(-1, 0, 0, 0, "northern");
        const lA1 = uA1.toLatLng();
        const lB1 = uB1.toLatLng();
        const uA2 = lA1.toUTM();
        const uB2 = lB1.toUTM();
        const vA1 = uA1.toVec3();
        const vB1 = uB1.toVec3();
        const vA2 = uA2.toVec3();
        const vB2 = uB2.toVec3();
        const vD1 = vec3.sub(vec3.create(), vB1, vA1);
        const vD2 = vec3.sub(vec3.create(), vB2, vA2);
        const ln1 = vec3.len(vD1);
        const ln2 = vec3.len(vD2);
        console.log({ lA1, lB1, uA1, uA2, uB1, uB2, vA1, vA2, vB1, vB2, vD1, vD2 });
        this.areExact(ln1, 1);
        this.areExact(ln2, 1);
        this.areExact(ln1, ln2);
    }
}