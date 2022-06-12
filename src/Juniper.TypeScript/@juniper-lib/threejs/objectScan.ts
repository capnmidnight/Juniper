export function objectScan<T extends THREE.Object3D = THREE.Object3D>(obj: THREE.Object3D, test: (obj: THREE.Object3D) => boolean): T {
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