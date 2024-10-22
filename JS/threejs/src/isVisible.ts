import { Object3D } from "three";

export function isVisible(obj: Object3D) {
    while (obj != null) {
        if (!obj.visible) {
            return false;
        }

        obj = obj.parent;
    }

    return true;
}