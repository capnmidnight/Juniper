export function isVisible(obj: THREE.Object3D) {
    while (obj != null) {
        if (!obj.visible) {
            return false;
        }

        obj = obj.parent;
    }

    return true;
}