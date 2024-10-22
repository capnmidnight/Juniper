import { stringRandom } from "@juniper-lib/util";
import { BufferGeometry, Material, Mesh } from "three";
import { scaleOnHover } from "../animation/scaleOnHover";
import { RayTarget } from "../eventSystem/RayTarget";
import { mesh, obj, objGraph } from "../objects";

export class MeshButton extends RayTarget {
    protected readonly mesh: Mesh;

    constructor(
        name: string,
        geometry: BufferGeometry,
        protected readonly enabledMaterial: Material,
        protected readonly disabledMaterial: Material,
        size: number) {
        name = name + stringRandom(16);
        super(obj(name));

        this.mesh = mesh(`Mesh-${name}-enabled`, geometry, enabledMaterial);
        this.size = size;
        objGraph(this, this.mesh);
        this.addMesh(this.mesh);

        this.clickable = true;
        this.disabled = this.disabled;

        scaleOnHover(this, true);
    }

    get size(): number {
        return this.mesh.scale.x;
    }

    set size(v: number) {
        this.mesh.scale.setScalar(v);
    }

    override get disabled(): boolean {
        return super.disabled;
    }

    override set disabled(v: boolean) {
        super.disabled = v;
        this.mesh.material = v
            ? this.disabledMaterial
            : this.enabledMaterial;
    }
}