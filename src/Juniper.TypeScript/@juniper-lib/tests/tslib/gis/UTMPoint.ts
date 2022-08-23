import { TestCase } from "@juniper-lib/testing/tdd/TestCase";
import { LatLngPoint } from "@juniper-lib/tslib/gis/LatLngPoint";
import { GlobeHemisphere, UTMPoint } from "@juniper-lib/tslib/gis/UTMPoint";
import { HalfPi } from "@juniper-lib/tslib/math";

const K = 0.00000001;

export class UTMPointTests extends TestCase {

    override setup() {
    }

    private conversionTest(lat: number, lng: number, alt: number, easting: number, northing: number, zone: number, hemisphere: GlobeHemisphere) {
        const llExpected = new LatLngPoint(lat, lng, alt);
        const utmActual = llExpected.toUTM();
        const utmExpected = new UTMPoint(easting, northing, alt, zone);
        const llActual = utmExpected.toLatLng();

        this.areApprox(utmActual.easting, utmExpected.easting, "Easting");
        this.areApprox(utmActual.northing, utmExpected.northing, "Northing");
        this.areApprox(utmActual.altitude, utmExpected.altitude, "Altitude");
        this.areApprox(utmActual.zone, utmExpected.zone, "Zone");
        this.areExact(utmActual.hemisphere, utmExpected.hemisphere, "Hemisphere 1");
        this.areExact(utmActual.hemisphere, hemisphere, "Hemisphere 2");
        this.areApprox(llActual.lat, llExpected.lat, "Latitude");
        this.areApprox(llActual.lng, llExpected.lng, "Longitude");
        this.areApprox(llActual.alt, llExpected.alt, "Altitude");
    }

    test_JonesPoint() {
        this.conversionTest(38.790342, -77.040682, 0, 322765.32, 4295488.80, 18, "northern");
    }

    test_ZeroLatLng() {
        this.conversionTest(0, 0, 0, 166021.443, 0, 31, "northern");
    }

    test_NorthernHemisphereIsAllPositiveNorthings() {
        for (let lat = 0; lat <= 90; ++lat) {
            const ll = new LatLngPoint(lat, 0);
            const utm = ll.toUTM();
            this.isGreaterThanEqual(utm.northing, 0, `Latitude ${lat}`);
        }
    }

    test_SouthernHemisphereIsAllNegativeNorthings() {
        for (let lat = 0; lat >= -90; --lat) {
            const ll = new LatLngPoint(lat, 0);
            const utm = ll.toUTM();
            this.isLessThanEqual(utm.northing, 0, `Latitude ${lat}`);
        }
    }

    test_ZoneBoundaries() {
        const ll1 = new LatLngPoint(0, 0 - K, 0);
        const ll2 = new LatLngPoint(0, 0, 0);
        const ll3 = new LatLngPoint(0, K, 0);
        const ll4 = new LatLngPoint(0, 6 - 2 * K, 0);
        const ll5 = new LatLngPoint(0, 6 - K, 0);
        const ll6 = new LatLngPoint(0, 6, 0);
        const utm2 = ll2.toUTM();
        const utm1 = ll1.toUTM().rezone(utm2.zone);
        const utm3 = ll3.toUTM();
        const utm4 = ll4.toUTM();
        const utm5 = ll5.toUTM();
        const utm6 = ll6.toUTM().rezone(utm5.zone);

        const dEasting1 = utm2.easting - utm1.easting;
        const dEasting2 = utm3.easting - utm2.easting;
        const dEasting3 = utm5.easting - utm4.easting;
        const dEasting4 = utm6.easting - utm5.easting;
        this.areApprox(dEasting1, dEasting2, "A", 0.01);
        this.areApprox(dEasting1, dEasting3, "B", 0.01);
        this.areApprox(dEasting1, dEasting4, "C", 0.01);
        this.areApprox(dEasting2, dEasting3, "D", 0.01);
        this.areApprox(dEasting2, dEasting4, "E", 0.01);
        this.areApprox(dEasting3, dEasting4, "F", 0.01);
    }

    private centroidTest(r: number, dy: number) {
        const numPoints = 4;
        const lls = new Array<LatLngPoint>();
        for (let i = 0; i < numPoints; ++i) {
            const a = i * HalfPi;
            const x = r * Math.cos(a);
            const y = r * Math.sin(a);
            lls.push(new LatLngPoint(y + dy, x + 3))
        }

        const centLL = LatLngPoint.centroid(lls);

        const utms = lls.map((ll) => ll.toUTM());

        const centUTM = UTMPoint.centroid(utms);
        const centUTMll = centUTM.toLatLng();

        if (!this.areApprox(centUTMll.lat, centLL.lat, "Latitude", 0.01)
            || !this.areApprox(centUTMll.lng, centLL.lng, "Longitude", 0.01)) {
            console.log({ centUTMll, centUTM, centLL })
        }
    }

    test_Centroid_InOneZone_InOneHemisphere() {
        this.centroidTest(2.5, 3);
    }

    test_Centroid_InOneZone_SpanningHemispheres() {
        this.centroidTest(2.5, 0);
    }

    test_Centroid_SpanningZones_InOneHemisphere() {
        this.centroidTest(3.5, 4);
    }

    test_Centroid_SpanningZones_SpanningHemispheres() {
        this.centroidTest(3.5, 0);
    }
}