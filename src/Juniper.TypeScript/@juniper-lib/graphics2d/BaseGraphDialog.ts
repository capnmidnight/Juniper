import {
    max,
    min,
    step,
    value
} from "@juniper-lib/dom/attrs";
import { resizeCanvas } from "@juniper-lib/dom/canvas";
import {
    alignItems,
    columnGap,
    display, gridTemplateColumns,
    height,
    styles,
    width
} from "@juniper-lib/dom/css";
import {
    Canvas,
    Div,
    elementApply,
    InputCheckbox,
    InputNumber, PreLabeled
} from "@juniper-lib/dom/tags";
import { GraphNode } from "@juniper-lib/tslib/collections/GraphNode";
import { mapBuild } from "@juniper-lib/tslib/collections/mapBuild";
import { Tau } from "@juniper-lib/tslib/math";
import { stringRandom } from "@juniper-lib/tslib/strings/stringRandom";
import { RequestAnimationFrameTimer } from "@juniper-lib/tslib/timers/RequestAnimationFrameTimer";
import { DialogBox } from "@juniper-lib/widgets/DialogBox";
import { vec2 } from "gl-matrix";

const size = 20;
const mid = size / 2;
const delta = vec2.create();

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

export class BaseGraphDialog<T> extends DialogBox {

    private readonly t: HTMLInputElement;
    private readonly cooling: HTMLInputElement;
    private readonly attract: HTMLInputElement;
    private readonly repel: HTMLInputElement;
    private readonly canvas: HTMLCanvasElement;
    private readonly g: CanvasRenderingContext2D;

    private readonly timer = new RequestAnimationFrameTimer();
    private readonly positions = new Map<GraphNode<T>, vec2>();
    private readonly forces = new Map<GraphNode<T>, vec2>();
    private readonly mousePoint = vec2.create();

    private grabbed: GraphNode<T> = null;
    private mouseForce = vec2.create();

    private graph: Array<GraphNode<T>> = null;
    private origin: GraphNode<T> = null;

    get w() {
        return this.canvas.width - 2 * size - 200;
    }

    get h() {
        return this.canvas.height - 2 * size;
    }

    constructor(title: string, private readonly getNodeName: (value: T) => string) {
        super(title);

        this.cancelButton.style.display = "none";

        const idPostfix = stringRandom(5);

        elementApply(this.container,
            styles(
                width("100%"),
                height("100%")));

        elementApply(
            this.contentArea,
            Div(
                styles(
                    display("grid"),
                    gridTemplateColumns("repeat(9, auto) 1fr"),
                    columnGap("5px"),
                    alignItems("center")
                ),

                ...PreLabeled(
                    "limit" + idPostfix,
                    "Limit",
                    this.t = InputNumber(
                        min(0),
                        max(1000),
                        step(0.1),
                        value(5)
                    )
                ),

                ...PreLabeled(
                    "cooling" + idPostfix,
                    "Cooling",
                    this.cooling = InputCheckbox()
                ),

                ...PreLabeled(
                    "attract" + idPostfix,
                    "Attract",
                    this.attract = InputNumber(
                        min(-100),
                        max(100),
                        step(0.1),
                        value(1)
                    )
                ),

                ...PreLabeled(
                    "repel" + idPostfix,
                    "Repel",
                    this.repel = InputNumber(
                        min(-100),
                        max(100),
                        step(0.1),
                        value(1)
                    )
                )
            ),

            this.canvas = Canvas(
                styles(
                    display("block"),
                    width("100%"),
                    height("calc(100% - 2em)")
                )
            )
        );

        this.g = this.canvas.getContext("2d");
        this.g.textAlign = "center";
        this.g.textBaseline = "middle";

        this.timer.addTickHandler(() => {
            resizeCanvas(this.canvas);
            this.fr91();
            this.draw();
        });

        const delta = vec2.create();
        this.canvas.addEventListener("mousedown", (evt) => {
            this.setMouse(evt);

            this.grabbed = null;
            let dist = 0.70710678118654752440084436210485 * size;
            for (const node of this.graph) {
                const point = this.positions.get(node);
                vec2.sub(delta, point, this.mousePoint);
                const d = vec2.length(delta);
                if (d < dist) {
                    this.grabbed = node;
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
    }

    private setMouse(evt: MouseEvent) {
        const x = evt.offsetX * this.canvas.width / this.canvas.clientWidth - size;
        const y = evt.offsetY * this.canvas.height / this.canvas.clientHeight - size;
        vec2.set(this.mousePoint, x, y);
    }

    private draw() {
        this.g.clearRect(0, 0, this.canvas.width, this.canvas.height);

        this.g.fillStyle = "#000000";
        this.g.strokeStyle = "#000000";

        this.g.save();
        this.g.translate(size, size);

        for (const n1 of this.graph) {
            const p1 = this.positions.get(n1);
            for (const node2 of n1.connections) {
                const p2 = this.positions.get(node2);
                this.g.beginPath();
                this.g.moveTo(p1[0], p1[1]);
                this.g.lineTo(p2[0], p2[1]);
                this.g.stroke();
            }
        }

        this.g.strokeStyle = "#d5d5d5";
        this.g.save();
        this.g.translate(-mid, -mid);

        for (const n1 of this.graph) {
            this.g.fillStyle = "#f3f3f3";
            const p1 = this.positions.get(n1);
            this.g.fillRect(p1[0], p1[1], size, size);
            this.g.strokeRect(p1[0], p1[1], size, size);

            this.g.fillStyle = "#000000";
            this.g.fillText(this.getNodeName(n1.value), p1[0] + mid, p1[1] + mid);
        }
        this.g.restore();
        this.g.restore();
    }

    private applyForces(
        attract: (connected: boolean, len: number) => number,
        repel: (connected: boolean, len: number) => number) {
        const forces = mapBuild(this.graph, () => vec2.create());

        // calculate forces
        for (const n1 of this.graph) {
            const p1 = this.positions.get(n1);
            if (n1 === this.origin) {
                vec2.set(p1, this.w / 2, this.h / 2);
            }
            else {
                const f1 = forces.get(n1);

                const f0 = this.forces.get(n1);
                if (f0) {
                    vec2.add(f1, f1, f0);
                }

                if (n1 === this.grabbed) {
                    vec2.sub(this.mouseForce, this.mousePoint, p1);
                    vec2.scaleAndAdd(f1, f1, this.mouseForce, 10);
                }

                for (const n2 of this.graph) {
                    if (n1 !== n2) {
                        const p2 = this.positions.get(n2);
                        vec2.sub(delta, p2, p1);
                        const len = vec2.length(delta);
                        if (len > 0) {
                            vec2.normalize(delta, delta);
                            const connected = n1.isConnectedTo(n2);
                            const f = this.attract.valueAsNumber * attract(connected, len)
                                - this.repel.valueAsNumber * repel(connected, len);
                            vec2.scaleAndAdd(f1, f1, delta, f);
                        }
                    }
                }
            }
        }

        // limit
        for (const n1 of this.graph) {
            const f1 = forces.get(n1);
            f1[1] *= this.h / this.w;
            const len = vec2.length(f1);
            if (len > 0) {
                vec2.scale(f1, f1, Math.min(this.t.valueAsNumber, len) / len);
                const p1 = this.positions.get(n1);
                vec2.add(p1, p1, f1);
                p1[0] = clamp(p1[0], this.w);
                p1[1] = clamp(p1[1], this.h);
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
            this.applyForces(
                (connected, len) => connected ? c2 * Math.sqrt(len) * len / k : 0,
                (_, len) => c3 * k * k / len);
        }
    }

    override onShown() {
        super.onShown();

        this.t.valueAsNumber = 5;
        this.positions.clear();
        resizeCanvas(this.canvas);

        this.forces.clear();

        const R = Math.min(this.w, this.h) / 2;
        for (let i = 0; i < this.graph.length; ++i) {
            const node = this.graph[i];
            if (node === this.origin) {
                this.positions.set(node, vec2.fromValues(this.w / 2, this.h / 2));
            }
            else {
                const a = Tau * i / this.graph.length;
                const r = R;
                const x = r * Math.cos(a) + this.w / 2;
                const y = r * Math.sin(a) + this.h / 2;
                this.positions.set(node, vec2.fromValues(x, y));
            }
        }

        if (!this.timer.isRunning) {
            this.timer.start();
        }
    }

    protected setGraph(graph: GraphNode<T>[]) {
        this.graph = graph;
    }

    protected setOrigin(node: GraphNode<T>) {
        this.origin = node;
    }

    protected override onClosed() {
        this.timer.stop();
        super.onClosed();
    }
}
