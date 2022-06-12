export function meshToInstancedMesh(count: number, mesh: THREE.Mesh) {
    return new THREE.InstancedMesh(mesh.geometry, mesh.material, count);
}