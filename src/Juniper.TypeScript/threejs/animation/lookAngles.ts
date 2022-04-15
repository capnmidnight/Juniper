import { angleClamp } from "@juniper/math";

const D = new THREE.Vector3();

export function getLookHeading(dir: THREE.Vector3) {
    D.copy(dir);
    D.y = 0;
    D.normalize();
    return angleClamp(Math.atan2(D.x, D.z));
}

export function getLookPitch(dir: THREE.Vector3) {
    return angleClamp(Math.asin(dir.y));
}