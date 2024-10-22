import { isDefined, isObject } from "@juniper-lib/util";
import { isDisableable } from "@juniper-lib/dom";
import { BufferGeometry, Material, Mesh, Object3D, Vector3 } from "three";
import { isObject3D } from "./typeChecks";

export interface ErsatzObject<T extends Object3D = Object3D> {
    content3d: T;
}

export function isErsatzObject<T extends Object3D>(obj: unknown): obj is ErsatzObject<T> {
    return isObject(obj)
        && "content3d" in obj
        && isObject3D(obj.content3d);
}

export type Objects<T extends Object3D = Object3D> = T | ErsatzObject<T>;

export function isObjects(obj: any): obj is Objects {
    return isErsatzObject(obj)
        || isObject3D(obj);
}

export function objectResolve(obj: Objects): Object3D {
    if (isErsatzObject(obj)) {
        return obj.content3d;
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

export function objectToggleVisible(obj: Objects): void {
    objectSetVisible(obj, !objectIsVisible(obj));
}

export function objGraph<T extends Objects>(obj: T, ...children: Objects[]): T {
    const toAdd = children
        .filter(isDefined)
        .map(objectResolve);
    if (toAdd.length > 0) {
        objectResolve(obj)
            .add(...toAdd);
    }

    return obj;
}

export function objRemoveFromParent(obj: Objects) {
    obj = objectResolve(obj);
    if (isDefined(obj)) {
        obj.removeFromParent();
    }
}

export function obj(name: string, ...rest: Objects[]): Object3D {
    const obj = new Object3D();
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

export function objectSetWorldPosition(obj: Objects, pos: Vector3) {
    obj = objectResolve(obj);
    const parent = obj.parent;
    obj.removeFromParent();
    obj.position.copy(pos);
    if (isDefined(parent)) {
        parent.attach(obj);
    }
}

export function mesh<
    TGeometry extends BufferGeometry = BufferGeometry,
    TMaterial extends Material | Material[] = Material | Material[]
>(name: string, geom?: TGeometry, mat?: TMaterial): Mesh<TGeometry, TMaterial> {
    const mesh = new Mesh(geom, mat);
    mesh.name = name;
    return mesh;
}