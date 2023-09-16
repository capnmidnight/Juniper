import { isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
export function y2g(p) {
    if (isNullOrUndefined(p)) {
        return null;
    }
    return {
        lat: p.lat,
        lng: p.lng
    };
}
export function g2y(p) {
    if (isNullOrUndefined(p)) {
        return null;
    }
    if (p instanceof google.maps.LatLng) {
        return {
            lat: p.lat(),
            lng: p.lng()
        };
    }
    else {
        return {
            lat: p.lat,
            lng: p.lng
        };
    }
}
//# sourceMappingURL=conversion.js.map