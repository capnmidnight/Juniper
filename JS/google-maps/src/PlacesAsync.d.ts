export declare class PlacesAsync {
    private places;
    constructor(places: google.maps.places.PlacesService);
    nearbySearch(request: google.maps.places.PlaceSearchRequest): Promise<google.maps.places.PlaceResult[]>;
    textSearch(request: google.maps.places.TextSearchRequest): Promise<google.maps.places.PlaceResult[]>;
    findPlaceFromQuery(request: google.maps.places.FindPlaceFromQueryRequest): Promise<google.maps.places.PlaceResult[]>;
}
//# sourceMappingURL=PlacesAsync.d.ts.map