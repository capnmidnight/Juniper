import { isDefined } from "@juniper-lib/tslib";

export function isMesh(obj: any): obj is THREE.Mesh {
    return isDefined(obj)
        && obj.isMesh;
}

export function isMaterial(obj: any): obj is THREE.Material {
    return isDefined(obj)
        && obj.isMaterial;
}

function isNamedMaterial<T extends THREE.Material>(name: string, obj: any): obj is T {
    return isMaterial(obj)
        && obj.type === name;
}

export function isMeshBasicMaterial(obj: any): obj is THREE.MeshBasicMaterial {
    return isNamedMaterial("MeshBasicMaterial", obj);
}

export function isMeshStandardMaterial(obj: any): obj is THREE.MeshStandardMaterial {
    return isNamedMaterial("MeshStandardMaterial", obj);
}

export function isMeshPhongMaterial(obj: any): obj is THREE.MeshPhongMaterial {
    return isNamedMaterial("MeshPhongMaterial", obj);
}

export function isMeshPhysicalMaterial(obj: any): obj is THREE.MeshPhysicalMaterial {
    return isNamedMaterial("MeshPhysicalMaterial", obj);
}

export function isTexture(obj: any): obj is THREE.Texture {
    return isDefined(obj)
        && (obj as THREE.Texture).isTexture;
}

export function isColor(obj: any): obj is THREE.Color {
    return isDefined(obj)
        && (obj as THREE.Color).isColor;
}

export function isCubeTexture(obj: any): obj is THREE.CubeTexture {
    return isDefined(obj)
        && obj.isCubeTexture;
}

export function isObject3D(obj: any): obj is THREE.Object3D {
    return isDefined(obj)
        && (obj as THREE.Object3D).isObject3D;
}

export function isQuaternion(obj: any): obj is THREE.Quaternion {
    return isDefined(obj)
        && (obj as THREE.Quaternion).isQuaternion;
}

export function isEuler(obj: any): obj is THREE.Euler {
    return isDefined(obj)
        && (obj as THREE.Euler).isEuler;
}