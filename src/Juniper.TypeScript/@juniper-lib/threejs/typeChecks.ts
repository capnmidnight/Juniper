import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { Color, CubeTexture, Euler, Material, Mesh, MeshBasicMaterial, MeshPhongMaterial, MeshPhysicalMaterial, MeshStandardMaterial, Object3D, Quaternion, Texture } from "three";

export function isMesh(obj: any): obj is Mesh {
    return isDefined(obj)
        && obj.isMesh;
}

export function isMaterial(obj: any): obj is Material {
    return isDefined(obj)
        && obj.isMaterial;
}

function isNamedMaterial<T extends Material>(name: string, obj: any): obj is T {
    return isMaterial(obj)
        && obj.type === name;
}

export function isMeshBasicMaterial(obj: any): obj is MeshBasicMaterial {
    return isNamedMaterial("MeshBasicMaterial", obj);
}

export function isMeshStandardMaterial(obj: any): obj is MeshStandardMaterial {
    return isNamedMaterial("MeshStandardMaterial", obj);
}

export function isMeshPhongMaterial(obj: any): obj is MeshPhongMaterial {
    return isNamedMaterial("MeshPhongMaterial", obj);
}

export function isMeshPhysicalMaterial(obj: any): obj is MeshPhysicalMaterial {
    return isNamedMaterial("MeshPhysicalMaterial", obj);
}

export function isTexture(obj: any): obj is Texture {
    return isDefined(obj)
        && (obj as Texture).isTexture;
}

export function isColor(obj: any): obj is Color {
    return isDefined(obj)
        && (obj as Color).isColor;
}

export function isCubeTexture(obj: any): obj is CubeTexture {
    return isDefined(obj)
        && obj.isCubeTexture;
}

export function isObject3D(obj: any): obj is Object3D {
    return isDefined(obj)
        && (obj as Object3D).isObject3D;
}

export function isQuaternion(obj: any): obj is Quaternion {
    return isDefined(obj)
        && (obj as Quaternion).isQuaternion;
}

export function isEuler(obj: any): obj is Euler {
    return isDefined(obj)
        && (obj as Euler).isEuler;
}