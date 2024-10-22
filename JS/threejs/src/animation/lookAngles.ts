import { radiansClamp } from "@juniper-lib/util";
import { Vector3 } from "three";

const D = new Vector3();

export function getLookHeadingRadians(dir: Vector3) {
    D.copy(dir);
    D.y = 0;
    D.normalize();
    return radiansClamp(Math.atan2(D.x, D.z));
}

export function getLookPitchRadians(dir: Vector3) {
    return radiansClamp(Math.asin(dir.y));
}