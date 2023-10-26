import { debounce } from "@juniper-lib/events/src/debounce";
import { Tau } from "@juniper-lib/tslib/dist/math";
import { ForceDirectedNode } from "./ForceDirectedNode";
import "./index.css";

const size = 20;
export class ForceDirectedGraph<T> {

    private readonly connectorsCanvas: HTMLCanvasElement;
    private readonly g: CanvasRenderingContext2D;

    private readonly graph = new Map<number, ForceDirectedNode<T>>();
    private readonly elementToNode = new Map<HTMLElement, ForceDirectedNode<T>>();
    private readonly mousePoint = [0, 0];

    private data: T[] = null;

    private grabbed: ForceDirectedNode<T> = null;

    private timer: number = null;

    private render: FrameRequestCallback;

    limit = 5;
    cooling = false;
    attract = 1;
    repel = 1;
    private _running = false;
    get running() { return this._running; }

    get w() {
        return this.content.clientWidth - 2 * size - 200;
    }

    get h() {
        return this.content.clientHeight - 2 * size;
    }

    constructor(
        private readonly content: HTMLElement,
        private readonly getWeightMod: (a: T, b: T, connected: boolean) => number,
        private readonly getID1: (value: T) => number,
        private readonly getName1: (value: T) => string,
        private readonly getID2: (value: T) => number,
        private readonly getName2: (value: T) => string,
        private readonly makeLink: (value: T, id: number, name: string) => string | HTMLElement) {
        this.render = this._render.bind(this);

        this.content.addEventListener("mousedown", evt => {
            this.setMouse(evt);

            if (evt.target instanceof HTMLElement) {
                this.grabbed = this.elementToNode.get(evt.target);
                if (this.grabbed) {
                    this.grabbed.wasGrabbed = !this.grabbed.wasGrabbed;
                }
            }
        });

        this.content.classList.add("force-directed-graph");

        this.content.addEventListener("mousemove", evt => {
            this.setMouse(evt);
            evt.preventDefault();
        });

        this.content.addEventListener("mouseup", () => {
            this.grabbed = null;
        });

        this.connectorsCanvas = document.createElement("canvas");
        this.g = this.connectorsCanvas.getContext("2d");

        const resize = debounce(() => {
            this.connectorsCanvas.width = this.connectorsCanvas.clientWidth * devicePixelRatio;
            this.connectorsCanvas.height = this.connectorsCanvas.clientHeight * devicePixelRatio;
        });
        const resizer = new ResizeObserver((evts) => {
            for (const evt of evts) {
                if (evt.target == this.connectorsCanvas) {
                    resize();
                }
            }
        });
        resizer.observe(this.connectorsCanvas);

        if (this.running) {
            this.start();
        }
    }

    start() {
        this.timer = requestAnimationFrame(this.render);
        this._running = true;
    }

    stop() {
        if (this.timer) {
            cancelAnimationFrame(this.timer);
            this.timer = null;
            this._running = false;
        }
    }

    get values() {
        return this.data;
    }

    private addNode(value: T, id: number, name: string) {
        if (!this.graph.has(id)) {
            const element = document.createElement("div");
            element.classList.add("graph-node");
            element.append(this.makeLink(value, id, name));

            const node = new ForceDirectedNode(value, element);
            this.graph.set(id, node);
            this.elementToNode.set(element, node);
        }
    }

    set values(v) {
        if (v !== this.data) {
            this.content.innerHTML = "";
            this.content.append(this.connectorsCanvas);
            this.graph.clear();
            this.elementToNode.clear();
            this.grabbed = null;


            this.data = v;

            if (this.data) {
                for (const value of this.data) {
                    this.addNode(value, this.getID1(value), this.getName1(value));
                    this.addNode(value, this.getID2(value), this.getName2(value));
                }

                for (const value of this.data) {
                    const from = this.graph.get(this.getID1(value));
                    const to = this.graph.get(this.getID2(value));
                    from.connectTo(to);
                }

                this.reset();

                this.content.append(...this.elementToNode.keys());
            }
        }
    }

    reset() {
        const R = Math.min(this.w, this.h) / 2;
        const nodes = Array.from(this.graph.values());
        for (let i = 0; i < nodes.length; ++i) {
            const node = nodes[i];
            const a = Tau * i / nodes.length;
            const r = R;
            node.position[0] = r * Math.cos(a) + this.w / 2;
            node.position[1] = r * Math.sin(a) + this.h / 2;
        }
    }

    private _render() {
        this.timer = requestAnimationFrame(this.render);
        if (this.data) {
            this.fr91();
            this.draw();
        }
    }

    private setMouse(evt: MouseEvent) {
        this.mousePoint[0] = evt.pageX - this.content.offsetLeft - size;
        this.mousePoint[1] = evt.pageY - this.content.offsetTop - size;
    }

    private draw() {
        this.g.clearRect(0, 0, this.connectorsCanvas.width, this.connectorsCanvas.height);

        this.g.fillStyle = "black";
        this.g.strokeStyle = "black";

        this.g.save();
        this.g.translate(size, size);
        this.g.scale(devicePixelRatio, devicePixelRatio);

        for (const n1 of this.graph.values()) {
            const p1 = n1.position;
            for (const node2 of n1.connections) {
                const p2 = node2.position;
                this.g.beginPath();
                this.g.moveTo(p1[0], p1[1]);
                this.g.lineTo(p2[0], p2[1]);
                this.g.stroke();
            }
        }

        this.g.restore();

        for (const node of this.graph.values()) {
            node.updatePosition();
        }
    }

    getElement(id: number): HTMLElement {
        const node = this.graph.get(id);
        return node?.element;
    }

    private setElementClass(id: number, className: string) {
        if (id) {
            const node = this.graph.get(id);
            if (node) {
                node.element.classList.add(className);
            }
        }
        else {
            for (const node of this.graph.values()) {
                if (node.element.classList.contains(className)) {
                    node.element.classList.remove(className);
                }
            }
        }

    }

    select(id: number) {
        this.setElementClass(id, "selected");
    }

    highlight(id: number) {
        this.setElementClass(id, "highlighted");
    }

    private applyForces(
        attractFunc: (connected: boolean, len: number) => number,
        repelFunc: (connected: boolean, len: number) => number) {

        for (const node of this.graph.values()) {
            node.dynamicForce[0] = 0;
            node.dynamicForce[1] = 0;
        }

        // calculate forces
        for (const n1 of this.graph.values()) {
            if (n1 === this.grabbed) {
                n1.moveTo(this.mousePoint);
            }
            else if (!n1.wasGrabbed && this.running) {
                n1.updateForce(this.w, this.h);

                for (const n2 of this.graph.values()) {
                    if (n1 !== n2) {
                        n1.applyForce(n2, this.attract, attractFunc, this.repel, repelFunc, this.getWeightMod);
                    }
                }
            }
        }

        // limit
        for (const n1 of this.graph.values()) {
            n1.limit(this.limit, this.w, this.h);
        }

        if (this.cooling) {
            this.limit *= 0.975;
        }
    }

    fr91() {
        const area = this.w * this.h * 0.1;
        const c1 = 1;
        const c2 = 1.5;
        const c3 = 0.1;
        const k = c1 * Math.sqrt(area / this.graph.size);
        // Running this twice prevents oscillations from becoming visible.
        for (let i = 0; i < 2; ++i) {
            this.applyForces(
                (connected, len) => connected ? c2 * Math.sqrt(len) * len / k : 0,
                (_, len) => c3 * k * k / len);
        }
    }
}
