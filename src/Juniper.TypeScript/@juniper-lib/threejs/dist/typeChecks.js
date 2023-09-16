import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
export function isMesh(obj) {
    return isDefined(obj)
        && obj.isMesh;
}
export function isMaterial(obj) {
    return isDefined(obj)
        && obj.isMaterial;
}
function isNamedMaterial(name, obj) {
    return isMaterial(obj)
        && obj.type === name;
}
export function isMeshBasicMaterial(obj) {
    return isNamedMaterial("MeshBasicMaterial", obj);
}
export function isMeshStandardMaterial(obj) {
    return isNamedMaterial("MeshStandardMaterial", obj);
}
export function isMeshPhongMaterial(obj) {
    return isNamedMaterial("MeshPhongMaterial", obj);
}
export function isMeshPhysicalMaterial(obj) {
    return isNamedMaterial("MeshPhysicalMaterial", obj);
}
export function isTexture(obj) {
    return isDefined(obj)
        && obj.isTexture;
}
export function isColor(obj) {
    return isDefined(obj)
        && obj.isColor;
}
export function isCubeTexture(obj) {
    return isDefined(obj)
        && obj.isCubeTexture;
}
export function isObject3D(obj) {
    return isDefined(obj)
        && obj.isObject3D;
}
export function isQuaternion(obj) {
    return isDefined(obj)
        && obj.isQuaternion;
}
export function isEuler(obj) {
    return isDefined(obj)
        && obj.isEuler;
}
//# sourceMappingURL=typeChecks.js.map