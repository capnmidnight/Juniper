import { stringRandom } from "@juniper-lib/tslib/strings/stringRandom";
import { BufferGeometry, Material, Mesh } from "three";
import { scaleOnHover } from "../animation/scaleOnHover";
import { RayTarget } from "../eventSystem/RayTarget";
import { mesh, obj, objGraph } from "../objects";

export class MeshButton extends RayTarget {
    protected readonly enabledMesh: Mesh;
    protected readonly disabledMesh: Mesh;

    constructor(name: string, geometry: BufferGeometry, enabledMaterial: Material, disabledMaterial: Material, size: number) {
        name = name + stringRandom(16);
        super(obj(name));

        this.enabledMesh = mesh(`Mesh-${name}-enabled`, geometry, enabledMaterial);
        this.disabledMesh = mesh(`Mesh-${name}-disabled`, geometry, disabledMaterial);
        this.disabledMesh.visible = false;
        this.size = size;
        objGraph(this, this.enabledMesh, this.disabledMesh);
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

    override get disabled(): boolean {
        return super.disabled;
    }

    override set disabled(v: boolean) {
        super.disabled = v;
        this.enabledMesh.visible = !v;
        this.disabledMesh.visible = v;
    }
}