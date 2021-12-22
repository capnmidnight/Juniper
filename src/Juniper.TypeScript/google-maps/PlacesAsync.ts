export class PlacesAsync {
    constructor(private places: google.maps.places.PlacesService) {
    }

    nearbySearch(request: google.maps.places.PlaceSearchRequest): Promise<google.maps.places.PlaceResult[]> {
        return new Promise((
            resolve: (results: google.maps.places.PlaceResult[]) => void,
            reject: (status: google.maps.places.PlacesServiceStatus) => void) => {
            this.places.nearbySearch(request, (results, status) => {
                if (status === google.maps.places.PlacesServiceStatus.OK) {
                    resolve(results);
                }
                else {
                    reject(status);
                }
            });
        });
    }

    textSearch(request: google.maps.places.TextSearchRequest): Promise<google.maps.places.PlaceResult[]> {
        return new Promise((
            resolve: (results: google.maps.places.PlaceResult[]) => void,
            reject: (status: google.maps.places.PlacesServiceStatus) => void) => {
            this.places.textSearch(request, (results, status) => {
                if (status === google.maps.places.PlacesServiceStatus.OK) {
                    resolve(results);
                }
                else {
                    reject(status);
                }
            });
        });
    }

    findPlaceFromQuery(request: google.maps.places.FindPlaceFromQueryRequest): Promise<google.maps.places.PlaceResult[]> {
        return new Promise((
            resolve: (results: google.maps.places.PlaceResult[]) => void,
            reject: (status: google.maps.places.PlacesServiceStatus) => void) => {
            this.places.findPlaceFromQuery(request, (results, status) => {
                if (status === google.maps.places.PlacesServiceStatus.OK) {
                    resolve(results);
                }
                else {
                    reject(status);
                }
            });
        });
    }
}