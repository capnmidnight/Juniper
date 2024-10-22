import { stringRandom, Tau } from "@juniper-lib/util";
import { alignItems, Canvas, Checked, columnGap, display, Div, For, fr, gridTemplateColumns, H1, ID, InputCheckbox, InputNumber, Label, Max, Min, minHeight, perc, px, resizeContext, rgb, Step, Value, width } from "@juniper-lib/dom";
import { RequestAnimationFrameTimer } from "@juniper-lib/timers";
import { BaseDialogElement } from "@juniper-lib/widgets";
import { Vec2 } from "gl-matrix";
const size = 20;
const mid = size / 2;
const delta = new Vec2();
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
export class BaseGraphDialog extends BaseDialogElement {
    get w() {
        return this.canvas.width - 2 * size - 200;
    }
    get h() {
        return this.canvas.height - 2 * size;
    }
    constructor(title, getNodeName, getNodeColor, getWeightMod) {
        const id = stringRandom(12);
        super(H1(title), Div(display("grid"), gridTemplateColumns("repeat(11, auto)", fr(1)), columnGap(px(5)), alignItems("center"), Label(For(id + "Limit"), "Limit"), InputNumber(ID(id + "Limit"), Min(0), Max(1000), Step(0.1), Value(5)), Label(For(id + "Cooling"), "Cooling"), InputCheckbox(ID(id + "Cooling")), Label(For(id + "Attract"), "Attract"), InputNumber(ID(id + "Attract"), Min(-100), Max(100), Step(0.1), Value(1)), Label(For(id + "Repel"), "Repel"), InputNumber(ID(id + "Repel"), Min(-100), Max(100), Step(0.1), Value(1)), Label(For(id + "HideBare"), "Hide bare nodes"), InputCheckbox(ID(id + "HideBare"), Checked(true)), Canvas(display("block"), width(perc(100)), minHeight("calc(100% - 2em)"))));
        this.getNodeName = getNodeName;
        this.getNodeColor = getNodeColor;
        this.getWeightMod = getWeightMod;
        this.timer = new RequestAnimationFrameTimer();
        this.positions = new Map();
        this.forces = new Map();
        this.wasGrabbed = new Set();
        this.mousePoint = new Vec2();
        this.grabbed = null;
        this.graph = null;
        this.t = this.Q(`#${id}Limit`);
        this.cooling = this.Q(`#${id}Cooling`);
        this.attract = this.Q(`#${id}Attract`);
        this.repel = this.Q(`#${id}Repel`);
        this.hideBare = this.Q(`#${id}HideBare`);
        this.canvas = this.Q("canvas");
        this.g = this.canvas.getContext("2d");
        this.g.textAlign = "center";
        this.g.textBaseline = "middle";
        this.timer.addTickHandler(() => {
            this.fr91();
            this.draw();
        });
        const resizer = new ResizeObserver(() => {
            resizeContext(this.g);
        });
        resizer.observe(this.canvas);
        const delta = new Vec2();
        this.canvas.addEventListener("mousedown", (evt) => {
            this.setMouse(evt);
            this.grabbed = null;
            let dist = 0.7071067811865475 * size;
            for (const node of this.graph) {
                const point = this.positions.get(node);
                delta.copy(point)
                    .sub(this.mousePoint);
                const d = delta.magnitude;
                if (d < dist) {
                    this.grabbed = node;
                    if (this.wasGrabbed.has(node)) {
                        this.wasGrabbed.delete(node);
                    }
                    else {
                        this.wasGrabbed.add(node);
                    }
                    dist = d;
                }
            }
        });
        this.canvas.addEventListener("mousemove", (evt) => {
            this.setMouse(evt);
        });
        this.canvas.addEventListener("mouseup", () => {
            this.grabbed = null;
        });
        this.dialog.addEventListener("showing", evt => {
            this.refreshData();
            this.t.valueAsNumber = 5;
            if (!this.timer.isRunning) {
                this.timer.start();
            }
            evt.resolve();
        });
        this.dialog.addEventListener("closing", () => {
            this.timer.stop();
        });
    }
    setMouse(evt) {
        this.mousePoint.x = evt.offsetX * this.canvas.width / this.canvas.clientWidth - size;
        this.mousePoint.y = evt.offsetY * this.canvas.height / this.canvas.clientHeight - size;
    }
    draw() {
        this.g.clearRect(0, 0, this.canvas.width, this.canvas.height);
        this.g.fillStyle = "black";
        this.g.strokeStyle = "black";
        this.g.save();
        this.g.translate(size, size);
        for (const n1 of this.graph) {
            const p1 = this.positions.get(n1);
            for (const node2 of n1.connections) {
                const p2 = this.positions.get(node2);
                this.g.beginPath();
                this.g.moveTo(p1.x, p1.y);
                this.g.lineTo(p2.x, p2.y);
                this.g.stroke();
            }
        }
        this.g.strokeStyle = "lightgrey";
        this.g.save();
        this.g.translate(-mid, -mid);
        for (const n1 of this.graph) {
            if (n1.isConnected || !this.hideBare.checked) {
                this.g.fillStyle = rgb(243, 243, 243);
                const p1 = this.positions.get(n1);
                this.g.fillStyle = this.getNodeColor(n1.value);
                this.g.fillRect(p1.x, p1.y, size, size);
                this.g.strokeRect(p1.x, p1.y, size, size);
                this.g.fillStyle = "black";
                this.g.fillText(this.getNodeName(n1.value), p1.x + mid, p1.y + mid);
            }
        }
        this.g.restore();
        this.g.restore();
    }
    applyForces(attract, repel) {
        const forces = new Map(this.graph.map(n => [n, new Vec2()]));
        // calculate forces
        for (const n1 of this.graph) {
            const p1 = this.positions.get(n1);
            if (n1 === this.grabbed) {
                p1.copy(this.mousePoint);
            }
            else if (!this.wasGrabbed.has(n1)) {
                const f1 = forces.get(n1);
                const f0 = this.forces.get(n1);
                if (f0) {
                    f1.add(f0);
                }
                delta.x = this.w;
                delta.y = this.h;
                delta.scale(-0.5)
                    .add(p1);
                const len = delta.magnitude;
                if (len > 0) {
                    delta.normalize();
                    let f = -10000 * len;
                    f = Math.sign(f) * Math.pow(Math.abs(f), 0.2);
                    f1.scaleAndAdd(delta, f);
                }
                for (const n2 of this.graph) {
                    if (n1 !== n2) {
                        const p2 = this.positions.get(n2);
                        delta.copy(p2).sub(p1);
                        const len = delta.magnitude;
                        if (len > 0) {
                            delta.normalize();
                            const connected = n1.isConnectedTo(n2);
                            const weight = this.getWeightMod(n1.value, n2.value, connected);
                            const invWeight = 2 - weight;
                            const f = weight * this.attract.valueAsNumber * attract(connected, len)
                                - invWeight * this.repel.valueAsNumber * repel(connected, len);
                            f1.scaleAndAdd(delta, f);
                        }
                    }
                }
            }
        }
        // limit
        for (const n1 of this.graph) {
            const f1 = forces.get(n1);
            f1.y *= this.h / this.w;
            const len = f1.magnitude;
            if (len > 0) {
                f1.scale(Math.min(this.t.valueAsNumber, len) / len);
                const p1 = this.positions.get(n1);
                p1.add(f1);
                p1.x = clamp(p1.x, this.w);
                p1.y = clamp(p1.y, this.h);
            }
        }
        if (this.cooling.checked) {
            this.t.valueAsNumber *= 0.975;
        }
    }
    fr91() {
        const area = this.w * this.h * 0.1;
        const c1 = 1;
        const c2 = 1.5;
        const c3 = 0.1;
        const k = c1 * Math.sqrt(area / this.graph.length);
        // Running this twice prevents oscillations from becoming visible.
        for (let i = 0; i < 2; ++i) {
            this.applyForces((connected, len) => connected ? c2 * Math.sqrt(len) * len / k : 0, (_, len) => c3 * k * k / len);
        }
    }
    refreshData() {
        this.positions.clear();
        this.forces.clear();
        this.wasGrabbed.clear();
        const R = Math.min(this.w, this.h) / 2;
        for (let i = 0; i < this.graph.length; ++i) {
            const node = this.graph[i];
            const a = Tau * i / this.graph.length;
            const r = R;
            const x = r * Math.cos(a) + this.w / 2;
            const y = r * Math.sin(a) + this.h / 2;
            this.positions.set(node, new Vec2(x, y));
        }
    }
    setGraph(graph) {
        this.graph = graph;
    }
}
//# sourceMappingURL=BaseGraphDialog.js.map