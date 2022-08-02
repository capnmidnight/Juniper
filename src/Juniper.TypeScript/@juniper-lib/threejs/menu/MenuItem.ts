import { scaleOnHover } from "../animation/scaleOnHover";
import { RayTarget } from "../eventSystem/RayTarget";
import { obj, objGraph } from "../objects";
import { Image2D } from "../widgets/Image2D";

export class MenuItem extends RayTarget {
    startX: number = 0;

    constructor(width: number, height: number,
        name: string,
        public readonly front: Image2D,
        public readonly back: Image2D) {
        super(obj(`MenuItem-${name}`));

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

        this.back.scale.x = width;
        this.back.scale.y = height;

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
        scaleOnHover(this, this.clickable && this.enabled);
    }

    get width() {
        return this.back.scale.x;
    }

    get height() {
        return this.back.scale.y;
    }
}