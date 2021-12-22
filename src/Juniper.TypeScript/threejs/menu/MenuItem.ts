import { scaleOnHover } from "../animation/scaleOnHover";
import { PlaneCollider } from "../Plane";

export class MenuItem extends THREE.Object3D {
    disabled: boolean;
    collider: PlaneCollider;

    startX: number = 0;

    constructor(width: number, height: number, name: string, public front: THREE.Object3D, public back: THREE.Object3D, public isClickable: boolean, enabled: boolean) {
        super();

        this.name = `MenuItem-${name}`;

        this.disabled = !enabled;

        if (this.front) {
            this.front = front;
            this.front.renderOrder = 5;
            this.add(this.front);
        }

        if (this.back) {
            this.back = back;
            this.back.renderOrder = 0;
            this.back.position.z = -0.125;
            this.back.scale.x = width;
            this.back.scale.y = height;
            this.add(this.back);
        }

        this.collider = new PlaneCollider(width, height);
        this.collider.scale.x = width;
        this.collider.scale.y = height;
        this.add(this.collider);

        if (this.isClickable) {
            scaleOnHover(this);
        }
    }

    get width() {
        return this.collider.scale.x;
    }

    get height() {
        return this.collider.scale.y;
    }
}