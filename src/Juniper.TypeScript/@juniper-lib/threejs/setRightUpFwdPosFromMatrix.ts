export function setRightUpFwdPosFromMatrix(matrix: THREE.Matrix4, R: THREE.Vector3, U: THREE.Vector3, F: THREE.Vector3, P: THREE.Vector3) {
    const m = matrix.elements;
    R.set(m[0], m[1], m[2]);
    U.set(m[4], m[5], m[6]);
    F.set(-m[8], -m[9], -m[10]);
    P.set(m[12], m[13], m[14]);
    R.normalize();
    U.normalize();
    F.normalize();
}