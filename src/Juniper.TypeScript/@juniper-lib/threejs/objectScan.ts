import { Object3D } from "three";

export function objectScan<T extends Object3D = Object3D>(obj: Object3D, test: (obj: Object3D) => boolean): T {
    const queue = [obj];
    while (queue.length > 0) {
        const here = queue.shift();
        if (test(here)) {
            return here as T;
        }

        if (here.children.length > 0) {
            queue.push(...here.children);
        }
    }

    return null;
}