import { Promisifier } from "@juniper/events";

const statusTest = (_: google.maps.places.PlaceResult[], status: google.maps.places.PlacesServiceStatus) => status === google.maps.places.PlacesServiceStatus.OK;
const getResults = (results: google.maps.places.PlaceResult[], _: google.maps.places.PlacesServiceStatus) => results;
const getStatus = (_: google.maps.places.PlaceResult[], status: google.maps.places.PlacesServiceStatus) => status;

export class PlacesAsync {
    constructor(private places: google.maps.places.PlacesService) {
    }

    nearbySearch(request: google.maps.places.PlaceSearchRequest): Promise<google.maps.places.PlaceResult[]> {
        const task = new Promisifier<google.maps.places.PlaceResult[]>(statusTest, getResults, getStatus);
        this.places.nearbySearch(request, task.callback);
        return task;
    }

    textSearch(request: google.maps.places.TextSearchRequest): Promise<google.maps.places.PlaceResult[]> {
        const task = new Promisifier<google.maps.places.PlaceResult[]>(statusTest, getResults, getStatus);
        this.places.textSearch(request, task.callback);
        return task;
    }

    findPlaceFromQuery(request: google.maps.places.FindPlaceFromQueryRequest): Promise<google.maps.places.PlaceResult[]> {
        const task = new Promisifier<google.maps.places.PlaceResult[]>(statusTest, getResults, getStatus);
        this.places.findPlaceFromQuery(request, task.callback);
        return task;
    }
}