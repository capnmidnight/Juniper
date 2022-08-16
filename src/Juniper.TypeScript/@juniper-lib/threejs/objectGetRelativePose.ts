import { Matrix4, Object3D, Quaternion, Vector3, Vector4 } from "three";

const M = new Matrix4();
const P = new Vector3();

export function objectGetRelativePose(ref: Object3D, obj: Object3D, position: Vector4, quaternion: Quaternion, scale: Vector3): void {
    M.copy(ref.matrixWorld)
        .invert()
        .multiply(obj.matrixWorld)
        .decompose(P, quaternion, scale);
    position.set(P.x, P.y, P.z, 1);
}