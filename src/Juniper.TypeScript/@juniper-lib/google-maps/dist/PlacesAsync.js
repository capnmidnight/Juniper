import { Promisifier } from "@juniper-lib/events/dist/Promisifier";
const statusTest = (_, status) => status === google.maps.places.PlacesServiceStatus.OK;
const getResults = (results, _) => results;
const getStatus = (_, status) => status;
export class PlacesAsync {
    constructor(places) {
        this.places = places;
    }
    nearbySearch(request) {
        const task = new Promisifier(statusTest, getResults, getStatus);
        this.places.nearbySearch(request, task.callback);
        return task;
    }
    textSearch(request) {
        const task = new Promisifier(statusTest, getResults, getStatus);
        this.places.textSearch(request, task.callback);
        return task;
    }
    findPlaceFromQuery(request) {
        const task = new Promisifier(statusTest, getResults, getStatus);
        this.places.findPlaceFromQuery(request, task.callback);
        return task;
    }
}
//# sourceMappingURL=PlacesAsync.js.map