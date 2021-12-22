import { scaleOnHover } from "./animation/scaleOnHover";
import { Collider } from "./Collider";
import { MeshLabel } from "./MeshLabel";

export class MeshButton extends MeshLabel {
    readonly collider: Collider;
    readonly isDraggable = false;
    readonly isClickable = true;

    constructor(name: string, geometry: THREE.BufferGeometry, enabledMaterial: THREE.Material, disabledMaterial: THREE.Material, size: number) {
        super(name, geometry, enabledMaterial, disabledMaterial, size);

        this.collider = new Collider(geometry);
        this.collider.name = `Collider-${this.name}`;
        this.collider.scale.setScalar(size);

        this.add(this.collider);

        scaleOnHover(this);
    }
}