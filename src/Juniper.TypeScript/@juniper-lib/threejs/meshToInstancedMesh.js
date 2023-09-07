import { InstancedMesh } from "three";
export function meshToInstancedMesh(count, mesh) {
    return new InstancedMesh(mesh.geometry, mesh.material, count);
}
//# sourceMappingURL=meshToInstancedMesh.js.map