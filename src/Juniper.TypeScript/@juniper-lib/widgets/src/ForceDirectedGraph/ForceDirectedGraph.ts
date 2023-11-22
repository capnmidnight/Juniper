import { arrayRandom, arrayRemove, compareBy } from "@juniper-lib/collections/src/arrays";
import { debounce } from "@juniper-lib/events/src/debounce";
import { Tau } from "@juniper-lib/tslib/dist/math";
import { ForceDirectedNode, IFullBounds } from "./ForceDirectedNode";

function length(x: number, y: number) {
    return Math.sqrt(x * x + y * y);
}

function distinct<T>(arr: T[]) {
    return Array.from(new Set(arr));
}

function findIntersection(sx: number, sy: number, ex: number, ey: number, bounds: IFullBounds): [number, number] {
    const dx = ex - sx;
    const dy = ey - sy;
    const len = length(dx, dy);
    const box = bounds.padding;
    const hw = box.width / 2;
    const hh = box.height / 2;
    const lx = dx - hw;
    const rx = dx + hw;
    const ty = dy - hh;
    const by = dy + hh;
    const points = [
        intersect(dx, dy, len, lx, ty, rx, ty, box.width),
        intersect(dx, dy, len, lx, by, rx, by, box.width),
        intersect(dx, dy, len, lx, ty, lx, by, box.height),
        intersect(dx, dy, len, rx, ty, rx, by, box.height)
    ].filter(v => v);

    if (points.length === 0) {
        return null;
    }

    points.sort(compareBy(v => v[2]));

    return [points[0][0], points[0][1]];
}

function intersect(d1x: number, d1y: number, len1: number, v1x: number, v1y: number, v2x: number, v2y: number, len2: number): [number, number, number] {
    const d2x = v2x - v1x;
    const d2y = v2y - v1y;

    // Check for parallel lines;
    const dot = d1x * d2x + d1y * d2y;
    if (dot === len1 * len2) {
        return null;
    }

    const slope = d1y / d1x;

    if (d2x > 0) {
        d1y = v1y;
        d1x = v1y / slope;
        const p = (d1x - v1x) / d2x;
        if (p < 0 || p > 1) {
            return null;
        }
    }
    else if (d2y > 0) {
        d1x = v1x;
        d1y = v1x * slope;
        const p = (d1y - v1y) / d2y;
        if (p < 0 || p > 1) {
            return null;
        }
    }

    return [d1x, d1y, length(d1x, d1y)];
}

export class ForceDirectedGraph<T> {
    private readonly graph = new Map<number, ForceDirectedNode<T>>();
    private readonly elementToNode = new Map<HTMLElement, ForceDirectedNode<T>>();
    private readonly mousePoint: [number, number] = [0, 0];

    private readonly connectorsCanvas: HTMLCanvasElement;
    private readonly g: CanvasRenderingContext2D;
    private readonly render: FrameRequestCallback;

    private _running = false;

    private displayCount = 0;
    private selectedNode: ForceDirectedNode<T> = null;
    private data: T[] = null;
    private grabbed: ForceDirectedNode<T> = null;
    private timer: number = null;

    public performLayout = true;
    public displayDepth = -1;
    public limit = 5;
    public cooling = false;
    public attract = 1;
    public repel = 1;

    get running() { return this._running; }

    get w() {
        return this.content.clientWidth;
    }

    get h() {
        return this.content.clientHeight;
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
                const lastGrabbed = this.grabbed;
                this.grabbed = null;
                let here = evt.target;
                while (!this.grabbed && here) {
                    this.grabbed = this.elementToNode.get(here);
                    here = here.parentElement;
                }

                if (!this.grabbed || this.grabbed.pinner === evt.target) {
                    this.grabbed = lastGrabbed;
                }
                else {
                    this.grabbed.setMouseOffset(this.mousePoint);

                    if (this.grabbed !== lastGrabbed) {
                        this.grabbed.element.remove();
                        this.content.append(this.grabbed.element);
                    }
                }
            }
        });

        this.content.classList.add("force-directed-graph");

        this.content.addEventListener("mousemove", evt => {
            this.setMouse(evt);
            if (this.grabbed) {
                this.grabbed.pinned = true;
            }
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

    set values(v) {
        if (v !== this.data) {
            this.content.innerHTML = "";
            this.graph.clear();
            this.elementToNode.clear();
            this.selectedNode = null;
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

                this.updateDepths();

                this.content.append(
                    this.connectorsCanvas,
                    ...this.elementToNode.keys()
                );
            }
        }
    }

    private addNode(value: T, id: number, name: string) {
        if (!this.graph.has(id)) {
            const node = new ForceDirectedNode(
                value,
                name,
                this.makeLink(value, id, name)
            );
            this.graph.set(id, node);
            this.elementToNode.set(node.element, node);
        }
    }

    reset() {
        const R = Math.min(this.w, this.h) / 2;
        const nodes = Array.from(this.graph.values());
        while (nodes.length > 0) {
            const node = arrayRandom(nodes);
            arrayRemove(nodes, node);
            const i = nodes.length;
            if (!node.pinned) {
                const a = Tau * i / this.graph.size;
                const r = R;
                node.position[0] = r * Math.cos(a) + this.w / 2;
                node.position[1] = r * Math.sin(a) + this.h / 2;
            }
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
        this.mousePoint[0] = evt.pageX - this.content.offsetLeft;
        this.mousePoint[1] = evt.pageY - this.content.offsetTop;
    }

    private readonly boundsCache = new Map<Element, IFullBounds>();

    private draw() {
        this.g.clearRect(0, 0, this.connectorsCanvas.width, this.connectorsCanvas.height);

        this.g.fillStyle = "black";
        this.g.strokeStyle = "black";
        this.g.lineWidth = 2;

        this.g.save();
        this.g.scale(devicePixelRatio, devicePixelRatio);

        this.boundsCache.clear();
        for (const n1 of this.graph.values()) {
            n1.computeBounds(this.boundsCache);
        }

        for (const n1 of this.graph.values()) {
            if (n1.canDrawArrow(this.displayDepth)) {
                const [p1x, p1y] = n1.position;
                this.g.save();
                this.g.translate(p1x, p1y);
                for (const n2 of n1.connections) {
                    if (n2.canDrawArrow(this.displayDepth)) {
                        const [p2x, p2y] = n2.position;
                        const inter = findIntersection(p1x, p1y, p2x, p2y, n2.bounds);
                        if (inter) {
                            const [dx, dy] = inter;
                            const len = length(dx, dy);
                            if (len > 0) {
                                const angleRad = Math.atan2(dy, dx);
                                this.g.save();
                                this.g.rotate(angleRad);
                                this.g.beginPath();
                                this.g.moveTo(len, 0);
                                this.g.lineTo(len - 8, -6);
                                this.g.lineTo(len - 8, 6);
                                this.g.lineTo(len, 0);
                                this.g.fill();
                                this.g.lineTo(0, 0);
                                this.g.stroke();
                                this.g.restore();
                            }
                        }
                    }
                }
                this.g.restore();
            }
        }

        this.g.restore();

        for (const node of this.graph.values()) {
            node.updatePosition(this.displayDepth);
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
            return node;
        }
        else {
            for (const node of this.graph.values()) {
                if (node.element.classList.contains(className)) {
                    node.element.classList.remove(className);
                }
            }
            return null;
        }

    }

    select(id: number) {
        this.selectedNode = this.setElementClass(id, "selected");
        if (this.selectedNode) {
            this.selectedNode.pinned = true;
            this.selectedNode.position[0] = this.w / 2;
            this.selectedNode.position[1] = this.h / 2;
            this.selectedNode.element.remove();
            this.content.append(this.selectedNode.element);
        }
        this.updateDepths();
    }

    highlight(id: number) {
        this.setElementClass(id, "highlighted");
    }

    private updateDepths() {
        if (this.selectedNode) {
            for (const node of this.graph.values()) {
                node.depth = Number.MAX_VALUE;
            }

            this.selectedNode.depth = 0;

            const visited = new Set<ForceDirectedNode<T>>();
            const toVisit = [this.selectedNode];
            while (toVisit.length > 0) {
                const here = toVisit.shift();
                if (!visited.has(here)) {
                    visited.add(here);

                    const next = distinct([
                        ...here.connections,
                        ...here.reverseConnections
                    ].filter(n => !visited.has(n)));

                    for (const n of next) {
                        n.depth = Math.min(n.depth, here.depth + 1);
                    }

                    if (next.length > 0) {
                        toVisit.push(...next);
                    }
                }
            }

            this.displayCount = 0;
            for (const node of this.graph.values()) {
                if (this.displayDepth < 0 || node.depth <= this.displayDepth) {
                    ++this.displayCount;
                }
            }
        }
        else {
            for (const node of this.graph.values()) {
                node.depth = -1;
            }
            this.displayCount = this.graph.size;
        }
    }

    private applyForces(
        attractFunc: (connected: boolean, len: number) => number,
        repelFunc: (connected: boolean, len: number) => number) {

        for (const node of this.graph.values()) {
            node.resetForce();
        }

        // Don't know why this works, but seems to help
        // account for larger canvases versus smaller ones.
        const wallForce = Math.sqrt(this.w * this.h) / this.displayCount;

        // calculate forces
        for (const n1 of this.graph.values()) {
            if (this.displayDepth < 0 || n1.depth <= this.displayDepth) {
                if (n1 === this.grabbed) {
                    n1.moveTo(this.mousePoint);
                }
                else if (!n1.pinned
                    && this.running
                    && this.performLayout) {

                    n1.updateForce(this.w, this.h, wallForce);

                    for (const n2 of this.graph.values()) {
                        if (n1 !== n2
                            && (this.displayDepth < 0 || n2.depth <= this.displayDepth)) {
                            n1.applyForce(n2, this.attract, attractFunc, this.repel, repelFunc, this.getWeightMod);
                        }
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
        const area = this.w * this.h;
        const c0 = 1;
        const c1 = 0.1;
        const c2 = 1.25;
        const c3 = 1.25;
        const c4 = 0.2;
        const c5 = 2;
        const k = c0 * Math.sqrt(c1 * area / this.graph.size);
        // Running this twice prevents oscillations from becoming visible.
        for (let i = 0; i < 2; ++i) {
            this.applyForces(
                (connected, len) => connected ? c2 * Math.pow(len, c3) / k : 0,
                (_, len) => c4 * Math.pow(k, c5) / len);
        }
    }
}
