import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";

const delta = [0, 0];

function clamp(a: number, b: number) {
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

export class ForceDirectedNode<T> extends GraphNode<T> {

    public readonly position = [0, 0];
    public readonly dynamicForce = [0, 0];
    public readonly staticForce = [0, 0];
    public wasGrabbed = false;

    constructor(value: T, public readonly element: HTMLElement) {
        super(value);
    }

    limit(t: number, w: number, h: number) {
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

    moveTo(mousePoint: number[]) {
        this.position[0] = mousePoint[0];
        this.position[1] = mousePoint[1];
    }

    updateForce(w: number, h: number) {
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

    applyForce(
        n2: ForceDirectedNode<T>,
        attract: number, attractFunc: (connected: boolean, len: number) => number,
        repel: number, repelFunc: (connected: boolean, len: number) => number,
        getWeightMod: (a: T, b: T, connected: boolean) => number) {
        const p2 = n2.position;
        delta[0] = p2[0] - this.position[0];
        delta[1] = p2[1] - this.position[1];

        const len = Math.sqrt(delta[0] * delta[0] + delta[1] * delta[1]);
        if (len > 0) {
            delta[0] /= len;
            delta[1] /= len;
            const connected = this.isConnectedTo(n2 as this);
            const weight = getWeightMod(this.value, n2.value, connected);
            const invWeight = 2 - weight;
            const f = weight * attract * attractFunc(connected, len)
                - invWeight * repel * repelFunc(connected, len);

            this.dynamicForce[0] += delta[0] * f;
            this.dynamicForce[1] += delta[1] * f;
        }
    }
}
