import { stringRandom } from "@juniper-lib/tslib";

export class MeshLabel extends THREE.Object3D {
    private _disabled = false;

    protected readonly enabledMesh: THREE.Mesh;
    protected readonly disabledMesh: THREE.Mesh;

    constructor(name: string, geometry: THREE.BufferGeometry, enabledMaterial: THREE.Material, disabledMaterial: THREE.Material, size: number) {
        super();
        const id = stringRandom(16);

        this.name = name + id;

        this.enabledMesh = this.createMesh(`${this.name}-enabled`, geometry, enabledMaterial);
        this.disabledMesh = this.createMesh(`${this.name}-disabled`, geometry, disabledMaterial);
        this.disabledMesh.visible = false;
        this.size = size;
        this.add(this.enabledMesh, this.disabledMesh);
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

    get disabled(): boolean {
        return this._disabled;
    }

    set disabled(v: boolean) {
        if (v !== this.disabled) {
            this._disabled = v;
            this.enabledMesh.visible = !v;
            this.disabledMesh.visible = v;
        }
    }
}