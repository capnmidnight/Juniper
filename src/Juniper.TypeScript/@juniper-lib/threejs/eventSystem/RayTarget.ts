import { TypedEventBase } from "@juniper-lib/tslib";
import { solidBlack } from "../materials";
import { isObject3D } from "../typeChecks";
import { EventSystemEvents } from "./EventSystemEvent";

const RAY_TARGET_KEY = "Juniper:ThreeJS:EventSystem:RayTarget";
const RAY_TARGETS_KEY = "Juniper:ThreeJS:EventSystem:RayTargets";
const RAY_TARGET_DISABLED_KEY = "Juniper:ThreeJS:EventSystem:RayTarget:Disabled";
const RAY_TARGET_CLICKABLE_KEY = "Juniper:ThreeJS:EventSystem:RayTarget:Clickable";
const RAY_TARGET_DRAGGABLE_KEY = "Juniper:ThreeJS:EventSystem:RayTarget:Draggable";

export class RayTarget<EventsT = void> extends TypedEventBase<EventsT & EventSystemEvents> {
    constructor(public readonly object: THREE.Object3D, public readonly mesh: THREE.Mesh) {
        super();

        this.mesh.userData[RAY_TARGET_KEY] = this;
        let targets = this.object.userData[RAY_TARGETS_KEY] as Array<RayTarget<EventsT>>;
        if (!targets) {
            this.object.userData[RAY_TARGETS_KEY] = targets = new Array<RayTarget<EventsT>>();
        }
        targets.push(this);
    }

    get disabled() {
        return this.object.userData[RAY_TARGET_DISABLED_KEY] as boolean;
    }

    set disabled(v) {
        this.object.userData[RAY_TARGET_DISABLED_KEY] = v;
    }

    get enabled() {
        return !this.disabled;
    }

    set enabled(v) {
        this.disabled = !v;
    }

    get clickable() {
        return this.object.userData[RAY_TARGET_CLICKABLE_KEY] as boolean;
    }

    set clickable(v) {
        this.object.userData[RAY_TARGET_CLICKABLE_KEY] = v;
    }

    get draggable() {
        return this.object.userData[RAY_TARGET_DRAGGABLE_KEY] as boolean;
    }

    set draggable(v) {
        this.object.userData[RAY_TARGET_DRAGGABLE_KEY] = v;
    }
}

export function getMeshTarget(objectOrHit: THREE.Object3D | THREE.Intersection): RayTarget {
    if (!objectOrHit) {
        return null;
    }

    const obj = isObject3D(objectOrHit) ? objectOrHit : objectOrHit.object;

    return obj && obj.userData[RAY_TARGET_KEY] as RayTarget;
}

export function getObjectTargets(obj: THREE.Object3D): Array<RayTarget> {
    if (!obj) {
        return null;
    }

    return obj.userData[RAY_TARGETS_KEY] as Array<RayTarget>;
}

export function makeRayTarget<T>(mesh: THREE.Mesh, obj?: THREE.Object3D): RayTarget<T> {
    obj = obj || mesh;
    return new RayTarget<T>(obj, mesh);
}

export function addRayTarget<T>(obj: THREE.Object3D, geom: THREE.BufferGeometry): RayTarget<T> {
    const mesh = new THREE.Mesh(geom, solidBlack);
    mesh.visible = false;
    obj.add(mesh);
    return makeRayTarget<T>(mesh, obj);
}