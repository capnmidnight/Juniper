const R = new THREE.Vector3();

export function setMatrixFromUpFwdPos(U: THREE.Vector3, F: THREE.Vector3, P: THREE.Vector3, matrix: THREE.Matrix4) {
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
