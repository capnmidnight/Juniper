import { angleClamp } from "@juniper-lib/tslib";
import { Vector3 } from "three";

const D = new Vector3();

export function getLookHeading(dir: Vector3) {
    D.copy(dir);
    D.y = 0;
    D.normalize();
    return angleClamp(Math.atan2(D.x, D.z));
}

export function getLookPitch(dir: Vector3) {
    return angleClamp(Math.asin(dir.y));
}