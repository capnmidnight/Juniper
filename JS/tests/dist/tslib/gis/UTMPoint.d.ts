import { TestCase } from "@juniper-lib/testing/dist/tdd/TestCase";
export declare class UTMPointTests extends TestCase {
    setup(): void;
    private conversionTest;
    test_JonesPoint(): void;
    test_ZeroLatLng(): void;
    test_NorthernHemisphereIsAllPositiveNorthings(): void;
    test_SouthernHemisphereIsAllNegativeNorthings(): void;
    test_ZoneBoundaries(): void;
    private centroidTest;
    test_Centroid_InOneZone_InOneHemisphere(): void;
    test_Centroid_InOneZone_SpanningHemispheres(): void;
    test_Centroid_SpanningZones_InOneHemisphere(): void;
    test_Centroid_SpanningZones_SpanningHemispheres(): void;
}
//# sourceMappingURL=UTMPoint.d.ts.map