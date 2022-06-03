import { isDefined } from "@juniper-lib/tslib";
import { InteractiveObject3D, isCollider, isInteractiveObject3D } from "./InteractiveObject3D";

export function resolveObj(hit: THREE.Intersection): InteractiveObject3D{
    if (!hit
        || !isCollider(hit.object)) {
        return null;
    }

    let obj = hit.object;
    while (isDefined(obj)
        && !isInteractiveObject3D(obj)) {
        obj = obj.parent;
    }

    return obj;
}
