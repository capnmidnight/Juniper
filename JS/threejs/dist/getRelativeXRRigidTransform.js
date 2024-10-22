import { Matrix4, Quaternion, Vector3, Vector4 } from "three";
const M = new Matrix4();
const P3 = new Vector3();
const P4 = new Vector4();
const Q = new Quaternion();
export function getRelativeXRRigidTransform(ref, obj, scale) {
    M.copy(ref.matrixWorld)
        .invert()
        .multiply(obj.matrixWorld)
        .decompose(P3, Q, scale);
    P4.set(P3.x, P3.y, P3.z, 1);
    return new XRRigidTransform(P4, Q);
}
//# sourceMappingURL=getRelativeXRRigidTransform.js.map