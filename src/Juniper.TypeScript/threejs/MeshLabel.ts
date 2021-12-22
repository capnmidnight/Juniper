import { stringRandom } from "juniper-tslib";

export class MeshLabel extends THREE.Object3D {
    private _disabled = false;

    protected readonly enabledMesh: THREE.Mesh;
    protected readonly disabledMesh: THREE.Mesh;

    constructor(name: string, geometry: THREE.BufferGeometry, enabledMaterial: THREE.Material, disabledMaterial: THREE.Material, size: number) {
        super();
        const id = stringRandom(16);

        this.name = name + id;

        this.enabledMesh = this.createMesh(`${this.name}-enabled`, geometry, enabledMaterial, size);
        this.disabledMesh = this.createMesh(`${this.name}-disabled`, geometry, disabledMaterial, size);
        this.disabledMesh.visible = false;

        this.add(this.enabledMesh, this.disabledMesh);
    }

    private createMesh(id: string, geometry: THREE.BufferGeometry, material: THREE.Material, size: number) {
        const mesh = new THREE.Mesh(geometry, material);
        mesh.name = "Mesh-" + id;
        mesh.scale.setScalar(size);
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
