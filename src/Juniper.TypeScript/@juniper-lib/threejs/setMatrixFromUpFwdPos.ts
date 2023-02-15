import type { Matrix4 } from "three";
import { Vector3 } from "three";

const R = new Vector3();

export function setMatrixFromUpFwdPos(U: Vector3, F: Vector3, P: Vector3, matrix: Matrix4) {
    R.crossVectors(F, U);
    U.crossVectors(R, F);
    R.normalize();
    U.normalize();
    F.normalize();
    matrix.set(
        R.x, U.x, -F.x, P.x,
        R.y, U.y, -F.y, P.y,
        R.z, U.z, -F.z, P.z,
        0, 0, 0, 1);
}
