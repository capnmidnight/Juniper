import { isArray, isClosable, isDisposable } from "juniper-tslib";
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
            else if (here.isMaterial) {
                cleanupQ.push(...Object.values(here));
            }

            if (here.isObject3D) {
                cleanupQ.push(...here.children);
                here.clear();
                removeScaledObj(here);
            }

            if (isDisposable(here)) {
                here.dispose();
            }
            else if (isClosable(here)) {
                here.close();
            }
            else if (isArray(here)) {
                cleanupQ.push(...here);
            }
        }
    }

    cleanupSeen.clear();
}
