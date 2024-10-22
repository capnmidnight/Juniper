import type { Matrix4, Vector3 } from "three";

export function setUpFwdPosFromMatrix(matrix: Matrix4, U: Vector3, F: Vector3, P: Vector3) {
    const m = matrix.elements;
    U.set(m[4], m[5], m[6]);
    F.set(-m[8], -m[9], -m[10]);
    P.set(m[12], m[13], m[14]);
    U.normalize();
    F.normalize();
}