import { InstancedMesh, Mesh } from "three";

export function meshToInstancedMesh(count: number, mesh: Mesh) {
    return new InstancedMesh(mesh.geometry, mesh.material, count);
}