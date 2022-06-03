import { isDisableable } from "@juniper-lib/dom/tags";
import { isDefined } from "@juniper-lib/tslib";
import { isObject3D } from "./typeChecks";

export interface ErsatzObject {
    object: THREE.Object3D;
}

export function isErsatzObject(obj: any): obj is ErsatzObject {
    return isDefined(obj)
        && isObject3D(obj.object);
}

export type Objects = THREE.Object3D | ErsatzObject;

export function isObjects(obj: any): obj is Objects {
    return isErsatzObject(obj)
        || isObject3D(obj);
}

export function objectResolve(obj: Objects): THREE.Object3D {
    if (isErsatzObject(obj)) {
        return obj.object;
    }

    return obj;
}

export function objectSetVisible(obj: Objects, visible: boolean): boolean {
    obj = objectResolve(obj);
    obj.visible = visible;
    return visible;
}

export function objectIsVisible(obj: Objects): boolean {
    obj = objectResolve(obj);
    return obj.visible;
}

export function objectIsFullyVisible(obj: Objects) {
    obj = objectResolve(obj);
    while (obj) {
        if (!obj.visible) {
            return false;
        }
        obj = obj.parent;
    }

    return true;
}

export function objectToggleVisible(obj: Objects): void {
    objectSetVisible(obj, !objectIsVisible(obj));
}

export function objGraph(obj: Objects, ...children: Objects[]): Objects {
    const toAdd = children
        .filter(isDefined)
        .map(objectResolve);
    if (toAdd.length > 0) {
        objectResolve(obj)
            .add(...toAdd);
    }

    return obj;
}

export function objectRemove(obj: Objects, ...children: Objects[]): void {
    const toRemove = children
        .filter(isDefined)
        .map(objectResolve);
    if (toRemove.length > 0) {
        objectResolve(obj)
            .remove(...toRemove);
    }
}

export function obj(name: string, ...rest: Objects[]): THREE.Object3D {
    const obj = new THREE.Object3D();
    obj.name = name;
    objGraph(obj, ...rest);
    return obj;
}

export function objectClearChildren(obj: Objects) {
    obj = objectResolve(obj);
    obj.clear();
}

export function objectSetEnabled(obj: Objects, enabled: boolean) {
    obj = objectResolve(obj);
    if (isDisableable(obj)) {
        obj.disabled = !enabled;
    }
}

export function objectSetWorldPosition(obj: Objects, pos: THREE.Vector3) {
    obj = objectResolve(obj);
    const parent = obj.parent;
    obj.removeFromParent();
    obj.position.copy(pos);
    if (isDefined(parent)) {
        parent.attach(obj);
    }
}