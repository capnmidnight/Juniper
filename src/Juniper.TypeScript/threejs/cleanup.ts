import { dispose, isArray } from "@juniper-lib/tslib";
import { removeScaledObj } from "./animation/scaleOnHover";

export function cleanup(obj: any) {
    const cleanupQ = new Array<any>();
    const cleanupSeen = new Set<any>();

    cleanupQ.push(obj);
    while (cleanupQ.length > 0) {
        const here = cleanupQ.shift();
        if (here && !cleanupSeen.has(here)) {
            cleanupSeen.add(here);

            if (here.isMesh) {
                cleanupQ.push(
                    here.material,
                    here.geometry);
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
