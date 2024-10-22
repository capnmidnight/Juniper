import type { Feature, FeatureCollection, MultiPolygon, Polygon } from "geojson";
export type GEOJSONS = FeatureCollection<Polygon | MultiPolygon>;
export type GEOJSON = Feature<Polygon | MultiPolygon>;
export type GEOJSONCallback<T = string> = (v: GEOJSON) => T;
export declare class GeoMapPathSelectedEvent extends Event {
    readonly pathId: string;
    constructor(pathId: string);
}
export declare class GeoMap extends EventTarget {
    #private;
    readonly svg: SVGSVGElement;
    constructor(svg: SVGSVGElement, movable: boolean, getRegionId: GEOJSONCallback, getRegionName: GEOJSONCallback);
    addFeature(feature: GEOJSON): void;
    getFeature(pathId: string): GEOJSON;
    getCentroid(pathId: string | GEOJSON): [number, number];
    setGeoJsonFile(geoJsonFile: GEOJSONS): void;
    loadGeoJsonFile(path: string | URL): Promise<void>;
    getPath(pathId: string): SVGPathElement;
    selectPath(pathId: string): void;
    clearTags(): void;
    tag(pathId: string, className: string): void;
    clearMarkers(): void;
    addMarker(marker: SVGElement): void;
}
//# sourceMappingURL=index.d.ts.map