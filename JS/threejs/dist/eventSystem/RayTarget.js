import { arrayRemove } from "@juniper-lib/util";
import { TypedEventTarget } from "@juniper-lib/events";
import { objectIsFullyVisible, objectResolve } from "../objects";
const RAY_TARGET_KEY = "Juniper:ThreeJS:EventSystem:RayTarget";
export class RayTarget extends TypedEventTarget {
    constructor(content3d) {
        super();
        this.content3d = content3d;
        this.meshes = new Array();
        this._disabled = false;
        this._clickable = false;
        this._draggable = false;
        this._navigable = false;
        this.content3d.userData[RAY_TARGET_KEY] = this;
    }
    addMesh(mesh) {
        mesh.userData[RAY_TARGET_KEY] = this;
        this.meshes.push(mesh);
        return this;
    }
    removeMesh(mesh) {
        if (arrayRemove(this.meshes, mesh)) {
            delete mesh.userData[RAY_TARGET_KEY];
        }
        return this;
    }
    addMeshes(...meshes) {
        for (const mesh of meshes) {
            this.addMesh(mesh);
        }
        return this;
    }
    removeMeshes(...meshes) {
        for (const mesh of meshes) {
            this.removeMesh(mesh);
        }
        return this;
    }
    get disabled() {
        return this._disabled;
    }
    set disabled(v) {
        this._disabled = v;
    }
    get enabled() {
        return !this.disabled;
    }
    set enabled(v) {
        this.disabled = !v;
    }
    get clickable() {
        return this._clickable;
    }
    set clickable(v) {
        this._clickable = v;
    }
    get draggable() {
        return this._draggable;
    }
    set draggable(v) {
        this._draggable = v;
    }
    get navigable() {
        return this._navigable;
    }
    set navigable(v) {
        this._navigable = v;
    }
}
export function isRayTarget(obj) {
    return obj instanceof RayTarget;
}
export function getRayTarget(obj) {
    let target = null;
    if (obj) {
        if (isRayTarget(obj)) {
            target = obj;
        }
        else {
            obj = objectResolve(obj);
            if (obj) {
                target = obj.userData[RAY_TARGET_KEY];
            }
        }
        if (target && !objectIsFullyVisible(target)) {
            target = null;
        }
    }
    return target;
}
//# sourceMappingURL=RayTarget.js.map