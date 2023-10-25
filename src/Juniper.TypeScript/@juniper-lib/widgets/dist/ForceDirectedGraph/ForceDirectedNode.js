import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
const delta = [0, 0];
function clamp(a, b) {
    if (a < 0) {
        return 0;
    }
    else if (a > b) {
        return b;
    }
    else if (!Number.isFinite(a)) {
        console.trace("To infinity... and beyond!");
        return b;
    }
    else {
        return a;
    }
}
export class ForceDirectedNode extends GraphNode {
    constructor(value, element) {
        super(value);
        this.element = element;
        this.position = [0, 0];
        this.dynamicForce = [0, 0];
        this.staticForce = [0, 0];
        this.wasGrabbed = false;
    }
    limit(t, w, h) {
        const f1 = this.dynamicForce;
        f1[1] *= h / w;
        const len = Math.sqrt(f1[0] * f1[0] + f1[1] * f1[1]);
        if (len > 0) {
            const r = Math.min(t, len) / len;
            f1[0] *= r;
            f1[1] *= r;
            const p1 = this.position;
            p1[0] = clamp(p1[0] + f1[0], w);
            p1[1] = clamp(p1[1] + f1[1], h);
        }
    }
    updatePosition() {
        const { position, element } = this;
        element.style.left = `${position[0]}px`;
        element.style.top = `${position[1]}px`;
    }
    moveTo(mousePoint) {
        this.position[0] = mousePoint[0];
        this.position[1] = mousePoint[1];
    }
    updateForce(w, h) {
        const f1 = this.dynamicForce;
        const f0 = this.staticForce;
        if (f0) {
            f1[0] += f0[0];
            f1[1] += f1[0];
        }
        delta[0] = w * -0.5 + this.position[0];
        delta[1] = h * -0.5 + this.position[1];
        const len = Math.sqrt(delta[0] * delta[0] + delta[1] * delta[1]);
        if (len > 0) {
            delta[0] /= len;
            delta[1] /= len;
            let f = -10000 * len;
            f = Math.sign(f) * Math.pow(Math.abs(f), 0.2);
            f1[0] += delta[0] * f;
            f1[1] += delta[1] * f;
        }
    }
    applyForce(n2, attract, attractFunc, repel, repelFunc, getWeightMod) {
        const p2 = n2.position;
        delta[0] = p2[0] - this.position[0];
        delta[1] = p2[1] - this.position[1];
        const len = Math.sqrt(delta[0] * delta[0] + delta[1] * delta[1]);
        if (len > 0) {
            delta[0] /= len;
            delta[1] /= len;
            const connected = this.isConnectedTo(n2);
            const weight = getWeightMod(this.value, n2.value, connected);
            const invWeight = 2 - weight;
            const f = weight * attract * attractFunc(connected, len)
                - invWeight * repel * repelFunc(connected, len);
            this.dynamicForce[0] += delta[0] * f;
            this.dynamicForce[1] += delta[1] * f;
        }
    }
}
//# sourceMappingURL=ForceDirectedNode.js.map