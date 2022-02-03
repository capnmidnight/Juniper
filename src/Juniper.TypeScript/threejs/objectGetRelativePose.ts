const M = new THREE.Matrix4();
const P = new THREE.Vector3();

export function objectGetRelativePose(ref: THREE.Object3D, obj: THREE.Object3D, position: THREE.Vector4, quaternion: THREE.Quaternion, scale: THREE.Vector3): void {
    M.copy(ref.matrixWorld)
        .invert()
        .multiply(obj.matrixWorld)
        .decompose(P, quaternion, scale);
    position.set(P.x, P.y, P.z, 1);
}