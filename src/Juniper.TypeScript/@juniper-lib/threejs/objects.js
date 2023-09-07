import { isDisableable } from "@juniper-lib/dom/tags";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { Mesh, Object3D } from "three";
import { isObject3D } from "./typeChecks";
export function isErsatzObject(obj) {
    return isDefined(obj)
        && isObject3D(obj.object);
}
export function isObjects(obj) {
    return isErsatzObject(obj)
        || isObject3D(obj);
}
export function objectResolve(obj) {
    if (isErsatzObject(obj)) {
        return obj.object;
    }
    return obj;
}
export function objectSetVisible(obj, visible) {
    obj = objectResolve(obj);
    obj.visible = visible;
    return visible;
}
export function objectIsVisible(obj) {
    obj = objectResolve(obj);
    return obj.visible;
}
export function objectIsFullyVisible(obj) {
    if (!obj) {
        return false;
    }
    obj = objectResolve(obj);
    while (obj) {
        if (!obj.visible) {
            return false;
        }
        obj = obj.parent;
    }
    return true;
}
export function objectToggleVisible(obj) {
    objectSetVisible(obj, !objectIsVisible(obj));
}
export function objGraph(obj, ...children) {
    const toAdd = children
        .filter(isDefined)
        .map(objectResolve);
    if (toAdd.length > 0) {
        objectResolve(obj)
            .add(...toAdd);
    }
    return obj;
}
export function objRemoveFromParent(obj) {
    obj = objectResolve(obj);
    if (isDefined(obj)) {
        obj.removeFromParent();
    }
}
export function obj(name, ...rest) {
    const obj = new Object3D();
    obj.name = name;
    objGraph(obj, ...rest);
    return obj;
}
export function objectClearChildren(obj) {
    obj = objectResolve(obj);
    obj.clear();
}
export function objectSetEnabled(obj, enabled) {
    obj = objectResolve(obj);
    if (isDisableable(obj)) {
        obj.disabled = !enabled;
    }
}
export function objectSetWorldPosition(obj, pos) {
    obj = objectResolve(obj);
    const parent = obj.parent;
    obj.removeFromParent();
    obj.position.copy(pos);
    if (isDefined(parent)) {
        parent.attach(obj);
    }
}
export function mesh(name, geom, mat) {
    const mesh = new Mesh(geom, mat);
    mesh.name = name;
    return mesh;
}
//# sourceMappingURL=objects.js.map