import { ICloneable } from "@juniper-lib/tslib/using";
import { vec2, vec3 } from "gl-matrix";
import { IUTMPoint, UTMPoint } from "./UTMPoint";
export interface ILatLngPoint {
    lat: number;
    lng: number;
    alt?: number;
}
/**
 * A point in geographic space on a radial coordinate system.
 **/
export declare class LatLngPoint implements ILatLngPoint, ICloneable {
    static centroid(points: LatLngPoint[]): LatLngPoint;
    /**
     * An altitude value thrown in just for kicks. It makes some calculations and conversions
     * easier if we keep the Altitude value.
     **/
    get alt(): number;
    private _alt;
    /**
     * Lines of latitude run east/west around the globe, parallel to the equator, never
     * intersecting. They measure angular distance north/south.
     **/
    get lat(): number;
    private _lat;
    /**
     * Lines of longitude run north/south around the globe, intersecting at the poles. They
     * measure angular distance east/west.
     **/
    get lng(): number;
    private _lng;
    /**
     * Initializes a zero LatLngPoint.
     **/
    constructor();
    /**
     * Initializes a new LatLngPoint as a copy of another.
     * @param lat
     */
    constructor(lat: ILatLngPoint);
    /**
     * Initializes a LatLngPoint from the given components.
     * @param lat
     * @param lng
     * @param alt
     */
    constructor(lat: number, lng: number, alt?: number);
    toJSON(): string;
    private static parseDMS;
    private static parseDMSPair;
    private static parseDecimal;
    /**
     * Try to parse a string as a Latitude/Longitude.
     **/
    static parse(value: string): LatLngPoint | null;
    /**
     * Pretty-print the Degrees/Minutes/Second version of the Latitude/Longitude angles.
     * @param sigfigs
     */
    toDMS(sigfigs: number, withAltitude?: boolean): string;
    /**
     * Pretty-print the Degrees/Minutes/Second version of the Latitude/Longitude angles.
     * @param sigfigs
     */
    toDMSArray(sigfigs: number): [string, string];
    toDMSArray(sigfigs: number, withAltitude: false): [string, string];
    toDMSArray(sigfigs: number, withAltitude: true): [string, string, string];
    /**
     * Pretty-print the Degrees/Minutes/Second version of the Latitude/Longitude angles.
     * @param value The decimal degree value to format
     * @param negative The string prefix to use when the value is negative
     * @param positive The string prefix to use when the value is positive
     * @param sigfigs The number of significant figures to which to print the value
     */
    private static toDMS;
    /**
     * Pretty-print the LatLngPoint for easier debugging.
     * @param sigfigs
     */
    toString(sigfigs?: number): string;
    /**
     * Check two LatLngPoints to see if they overlap.
     * @param other
     */
    equals(other?: ILatLngPoint | null): boolean;
    compareTo(other?: ILatLngPoint | null): number;
    /**
     * Calculate a rough distance, in meters, between two LatLngPoints.
     * @param other
     */
    distance(other: LatLngPoint): number;
    /**
     * Converts this UTMPoint to a Latitude/Longitude point using the WGS-84 datum. The
     * coordinate pair's units will be in meters, and should be usable to make distance
     * calculations over short distances.
     * reference: http://www.uwgb.edu/dutchs/usefuldata/utmformulas.htm
     **/
    fromUTM(utm: IUTMPoint): LatLngPoint;
    /**
     * Converts this LatLngPoint to a Universal Transverse Mercator point using the WGS-84
     * datum. The coordinate pair's units will be in meters, and should be usable to make
     * distance calculations over short distances.
     *
     * @see http://www.uwgb.edu/dutchs/usefuldata/utmformulas.htm
     **/
    toUTM(): UTMPoint;
    toVec2(): vec2;
    fromVec2(v: vec2): this;
    toVec3(): vec3;
    fromVec3(v: vec3): this;
    toArray(): number[];
    fromArray(arr: [number, number, number]): this;
    copy(other: ILatLngPoint): LatLngPoint;
    clone(): LatLngPoint;
}
//# sourceMappingURL=LatLngPoint.d.ts.map