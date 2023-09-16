export function setUpFwdPosFromMatrix(matrix, U, F, P) {
    const m = matrix.elements;
    U.set(m[4], m[5], m[6]);
    F.set(-m[8], -m[9], -m[10]);
    P.set(m[12], m[13], m[14]);
    U.normalize();
    F.normalize();
}
//# sourceMappingURL=setUpFwdPosFromMatrix.js.map