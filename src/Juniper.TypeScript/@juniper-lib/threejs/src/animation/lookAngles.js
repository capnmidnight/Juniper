import { radiansClamp } from "@juniper-lib/tslib/math";
import { Vector3 } from "three";
const D = new Vector3();
export function getLookHeadingRadians(dir) {
    D.copy(dir);
    D.y = 0;
    D.normalize();
    return radiansClamp(Math.atan2(D.x, D.z));
}
export function getLookPitchRadians(dir) {
    return radiansClamp(Math.asin(dir.y));
}
//# sourceMappingURL=lookAngles.js.map