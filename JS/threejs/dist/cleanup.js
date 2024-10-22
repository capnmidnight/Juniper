import { dispose } from "@juniper-lib/dom/dist/canvas";
import { isArray } from "@juniper-lib/tslib/dist/typeChecks";
import { removeScaledObj } from "./animation/scaleOnHover";
export function cleanup(obj) {
    const cleanupQ = new Array();
    const cleanupSeen = new Set();
    cleanupQ.push(obj);
    while (cleanupQ.length > 0) {
        const here = cleanupQ.shift();
        if (here && !cleanupSeen.has(here)) {
            cleanupSeen.add(here);
            if (here.isMesh) {
                cleanupQ.push(here.material, here.geometry);
            }
            if (here.isMaterial) {
                cleanupQ.push(...Object.values(here));
            }
            if (here.isObject3D) {
                cleanupQ.push(...here.children);
                here.clear();
                removeScaledObj(here);
            }
            if (isArray(here)) {
                cleanupQ.push(...here);
            }
            dispose(here);
        }
    }
    cleanupSeen.clear();
}
//# sourceMappingURL=cleanup.js.map