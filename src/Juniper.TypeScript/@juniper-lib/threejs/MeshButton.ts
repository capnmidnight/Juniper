import { scaleOnHover } from "./animation/scaleOnHover";
import { assureRayTarget, RayTarget } from "./eventSystem/RayTarget";
import { MeshLabel } from "./MeshLabel";

export class MeshButton extends MeshLabel {
    readonly target: RayTarget;

    constructor(name: string, geometry: THREE.BufferGeometry, enabledMaterial: THREE.Material, disabledMaterial: THREE.Material, size: number) {
        super(name, geometry, enabledMaterial, disabledMaterial, size);

        this.target = assureRayTarget(this.enabledMesh);
        this.target.clickable = true;
        this.target.disabled = this.disabled;

        scaleOnHover(this, true);
    }

    override get disabled() {
        return super.disabled;
    }

    override set disabled(v) {
        this.target.disabled = super.disabled = v;
    }
}