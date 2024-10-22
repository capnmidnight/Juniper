export function isVisible(obj) {
    while (obj != null) {
        if (!obj.visible) {
            return false;
        }
        obj = obj.parent;
    }
    return true;
}
//# sourceMappingURL=isVisible.js.map