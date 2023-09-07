import { scaleOnHover } from "../animation/scaleOnHover";
import { RayTarget } from "../eventSystem/RayTarget";
import { obj, objGraph } from "../objects";
export class MenuItem extends RayTarget {
    constructor(width, height, name, front, back) {
        super(obj(`MenuItem-${name}`));
        this.front = front;
        this.back = back;
        this.startX = 0;
        if (this.back) {
            this.back = back;
            this.back.renderOrder = 0;
            this.back.position.z = -0.05;
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
    get disabled() {
        return super.disabled;
    }
    set disabled(v) {
        if (v !== this.disabled) {
            super.disabled = v;
            this.updateHover();
        }
    }
    get clickable() {
        return super.clickable;
    }
    set clickable(v) {
        if (v !== this.clickable) {
            super.clickable = v;
            this.updateHover();
        }
    }
    updateHover() {
        scaleOnHover(this, this.clickable && this.enabled);
    }
    get width() {
        return this.back.scale.x;
    }
    get height() {
        return this.back.scale.y;
    }
}
//# sourceMappingURL=MenuItem.js.map