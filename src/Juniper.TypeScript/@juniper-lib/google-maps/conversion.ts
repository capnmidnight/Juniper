import type { ILatLngPoint } from "@juniper-lib/gis/LatLngPoint";
import { isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";

export function y2g(p: ILatLngPoint): google.maps.LatLngLiteral {
    if (isNullOrUndefined(p)) {
        return null;
    }

    return {
        lat: p.lat,
        lng: p.lng
    };
}

export function g2y(p: google.maps.LatLng): ILatLngPoint;
export function g2y(p: google.maps.LatLngLiteral): ILatLngPoint;
export function g2y(p: google.maps.LatLng | google.maps.LatLngLiteral): ILatLngPoint {
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