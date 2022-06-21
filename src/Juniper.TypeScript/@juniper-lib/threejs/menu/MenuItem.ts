import { scaleOnHover } from "../animation/scaleOnHover";
import { RayTarget } from "../eventSystem/RayTarget";
import { Image2D } from "../Image2D";
import { IUpdatable } from "../IUpdatable";
import { objGraph } from "../objects";

export class MenuItem extends RayTarget
    implements IUpdatable {
    startX: number = 0;

    useWebXRLayers = false;

    constructor(width: number, height: number,
        name: string,
        public readonly front: Image2D,
        public readonly back: Image2D) {
        super(new THREE.Object3D());

        this.object.name = `MenuItem-${name}`;

        if (this.back) {
            this.back = back;
            this.back.renderOrder = 0;
            this.back.position.z = -0.125;
            this.addMesh(this.back.mesh);
        }

        if (this.front) {
            this.front = front;
            this.front.renderOrder = 5;

            if (!this.back) {
                this.addMesh(this.front.mesh);
            }
        }

        this.object.scale.x = width;
        this.object.scale.y = height;

        objGraph(this, this.back, this.front);
    }

    override get disabled() {
        return super.disabled;
    }

    override set disabled(v) {
        if (v !== this.disabled) {
            super.disabled = v;
            this.updateHover();
        }
    }

    override get clickable() {
        return super.clickable;
    }

    override set clickable(v) {
        if (v !== this.clickable) {
            super.clickable = v;
            this.updateHover();
        }
    }

    private updateHover() {
        scaleOnHover(this.object, this.clickable && this.enabled);
    }

    update(dt: number, frame?: XRFrame) {
        if (this.useWebXRLayers) {
            if (this.back) {
                this.back.update(dt, frame);
            }

            if (this.front) {
                this.front.update(dt, frame);
            }
        }
    }

    get width() {
        return this.object.scale.x;
    }

    get height() {
        return this.object.scale.y;
    }
}