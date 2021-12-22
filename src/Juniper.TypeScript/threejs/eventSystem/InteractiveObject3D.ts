import { isBoolean, isDefined } from "juniper-tslib";
import { isVisible } from "../isVisible";
import { isObject3D } from "../typeChecks";

type InteractiveObject3DEvents = THREE.Event & {
    type: "enter"
    | "exit"
    | "move"
    | "up"
    | "down"
    | "dragstart"
    | "drag"
    | "dragend"
    | "dragcancel"
    | "click";
};

export type InteractiveObject3D = THREE.Object3D<InteractiveObject3DEvents> & {
    disabled?: boolean;
    isDraggable?: boolean;
    isClickable?: boolean;
    isCollider?: boolean;
}

export function isCollider(obj: any): boolean {
    return isObject3D(obj)
        && isBoolean((obj as any).isCollider)
        && isDefined(obj.parent);
}

export function isInteractiveHit(hit: THREE.Intersection): boolean {
    return isDefined(hit)
        && isCollider(hit.object);
}

export function isObjVisible(hit: THREE.Intersection): boolean {
    return isDefined(hit)
        && isCollider(hit.object)
        && (isInteractiveObject3D(hit.object) && isVisible(hit.object)
            || isInteractiveObject3D(hit.object.parent) && isVisible(hit.object.parent));
}

export function isInteractiveObject3D(obj: any): obj is InteractiveObject3D {
    return isObject3D(obj)
        && (isBoolean((obj as InteractiveObject3D).disabled)
            || isBoolean((obj as InteractiveObject3D).isDraggable)
            || isBoolean((obj as InteractiveObject3D).isClickable));
}

function checkClickable(obj: any): boolean {
    return isInteractiveObject3D(obj)
        && obj.isClickable
        && !obj.disabled;
}

export function isClickable(hit: THREE.Intersection): boolean {
    return isInteractiveHit(hit)
        && isCollider(hit.object)
        && (checkClickable(hit.object)
            || checkClickable(hit.object.parent));
}

function checkDraggable(obj: any): boolean {
    return isInteractiveObject3D(obj)
        && obj.isDraggable
        && !obj.disabled;
}

export function isDraggable(hit: THREE.Intersection): boolean {
    return isInteractiveHit(hit)
        && isCollider(hit.object)
        && (checkDraggable(hit.object)
            || checkDraggable(hit.object.parent));
}

function checkDisabled(obj: any): boolean {
    return isInteractiveObject3D(obj)
        && obj.disabled;
}

export function isDisabled(hit: THREE.Intersection): boolean {
    return isInteractiveHit(hit)
        && isCollider(hit.object)
        && (checkDisabled(hit.object)
            || checkDisabled(hit.object.parent));
}