import { stringRandom } from "@juniper-lib/tslib";
import { scaleOnHover } from "../animation/scaleOnHover";
import { RayTarget } from "../eventSystem/RayTarget";

export class MeshButton extends RayTarget {
    protected readonly enabledMesh: THREE.Mesh;
    protected readonly disabledMesh: THREE.Mesh;

    constructor(name: string, geometry: THREE.BufferGeometry, enabledMaterial: THREE.Material, disabledMaterial: THREE.Material, size: number) {
        super(new THREE.Object3D());
        const id = stringRandom(16);

        this.object.name = name + id;

        this.enabledMesh = this.createMesh(`${this.object.name}-enabled`, geometry, enabledMaterial);
        this.disabledMesh = this.createMesh(`${this.object.name}-disabled`, geometry, disabledMaterial);
        this.disabledMesh.visible = false;
        this.size = size;
        this.object.add(this.enabledMesh, this.disabledMesh);
        this.addMesh(this.enabledMesh);
        this.addMesh(this.disabledMesh);

        this.clickable = true;
        this.disabled = this.disabled;

        scaleOnHover(this, true);
    }

    get size(): number {
        return this.enabledMesh.scale.x;
    }

    set size(v: number) {
        this.enabledMesh.scale.setScalar(v);
        this.disabledMesh.scale.setScalar(v);
    }

    private createMesh(id: string, geometry: THREE.BufferGeometry, material: THREE.Material) {
        const mesh = new THREE.Mesh(geometry, material);
        mesh.name = "Mesh-" + id;
        return mesh;
    }

    override get disabled(): boolean {
        return super.disabled;
    }

    override set disabled(v: boolean) {
        super.disabled = v;
        this.enabledMesh.visible = !v;
        this.disabledMesh.visible = v;
    }
}