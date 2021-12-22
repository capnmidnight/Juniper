import { isDefined } from "juniper-tslib";

export function isMesh(obj: any): obj is THREE.Mesh {
    return isDefined(obj)
        && obj.isMesh;
}

export function isMaterial(obj: any): obj is THREE.Material {
    return isDefined(obj)
        && obj.isMaterial;
}

export function isMeshBasicMaterial(obj: any): obj is THREE.MeshBasicMaterial {
    return isMaterial(obj)
        && obj.type === "MeshBasicMaterial";
}

export function isMeshStandardMaterial(obj: any): obj is THREE.MeshStandardMaterial {
    return isMaterial(obj)
        && obj.type === "MeshStandardMaterial";
}

export function isMeshPhysicalMaterial(obj: any): obj is THREE.MeshPhysicalMaterial {
    return isMaterial(obj)
        && obj.type === "MeshPhysicalMaterial";
}

export function isTexture(obj: any): obj is THREE.Texture {
    return isDefined(obj)
        && obj.isTexture;
}

export function isCubeTexture(obj: any): obj is THREE.CubeTexture {
    return isDefined(obj)
        && obj.isCubeTexture;
}

export function isObject3D(obj: any): obj is THREE.Object3D {
    return isDefined(obj)
        && obj.isObject3D;
}

export function isQuaternion(obj: any): obj is THREE.Quaternion {
    return isDefined(obj)
        && obj.isQuaternion;
}

export function isEuler(obj: any): obj is THREE.Euler {
    return isDefined(obj)
        && obj.isEuler;
}