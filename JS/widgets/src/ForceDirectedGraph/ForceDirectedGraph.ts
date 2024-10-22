import { arrayClear, compareBy } from "@juniper-lib/util";
import { SingletonStyleBlob, height, overflow, pointerEvents, position, rule, userSelect, width, zIndex } from "@juniper-lib/dom";
import { debounce } from "@juniper-lib/events";
import { Vec2, Vec3 } from "gl-matrix";
import { ForceDirectedNode } from "./ForceDirectedNode";

const delta1 = /*@__PURE__*/(() => new Vec2())();
const delta2 = /*@__PURE__*/(() => new Vec2())();
const halfSize = /*@__PURE__*/(() => new Vec2())();
const topLeft = /*@__PURE__*/(() => new Vec2())();
const topRight = /*@__PURE__*/(() => new Vec2())();
const bottomLeft = /*@__PURE__*/(() => new Vec2())();
const bottomRight = /*@__PURE__*/(() => new Vec2())();
const intersections = [
    new Vec3(),
    new Vec3(),
    new Vec3(),
    new Vec3()
];

const points = new Array<Vec3>();

function findIntersection(start: Vec2, end: Vec2, size: Vec2): Vec3 {
    delta1.copy(end).sub(start);
    halfSize.copy(size).scale(0.5);

    const len = delta1.magnitude;

    topLeft.x = bottomLeft.x = delta1.x - halfSize.x;
    topRight.x = bottomRight.x = delta1.x + halfSize.x;
    topLeft.y = topRight.y = delta1.y - halfSize.y;
    bottomLeft.y = bottomRight.y = delta1.y + halfSize.y;

    intersect(delta1, len, topLeft, topRight, size.x, intersections[0]);
    intersect(delta1, len, bottomLeft, bottomRight, size.x, intersections[1]);
    intersect(delta1, len, topLeft, bottomLeft, size.y, intersections[2]);
    intersect(delta1, len, topRight, bottomRight, size.y, intersections[3]);

    arrayClear(points);
    for (const intersection of intersections) {
        if (intersection.z >= 0) {
            points.push(intersection);
        }
    }

    if (points.length === 0) {
        return null;
    }

    points.sort(compareBy(v => v.z));

    return points[0];
}

function intersect(d1: Vec2, len1: number, v1: Vec2, v2: Vec2, len2: number, intersection: Vec3): void {
    delta2.copy(v2).sub(v1);

    // Check for parallel lines;
    const dot = d1.dot(delta2);
    if (dot === len1 * len2) {
        intersection.z = -1;
        return;
    }

    const slope = d1.y / d1.x;

    if (delta2.x > 0) {
        d1.y = v1.y;
        d1.x = v1.y / slope;
        const p = (d1.x - v1.x) / delta2.x;
        if (p < 0 || p > 1) {
            intersection.z = -1;
            return;
        }
    }
    else if (delta2.y > 0) {
        d1.x = v1.x;
        d1.y = v1.x * slope;
        const p = (d1.y - v1.y) / delta2.y;
        if (p < 0 || p > 1) {
            intersection.z = -1;
            return;
        }
    }

    intersection.x = d1.x;
    intersection.y = d1.y;
    intersection.z = d1.magnitude;
}

type GraphMode = "directed" | "reverse-directed" | "undirected";

export class ForceDirectedGraph<T> {
    readonly #graph = new Map<T, ForceDirectedNode<T>>();
    readonly #elementToNode = new Map<HTMLElement, ForceDirectedNode<T>>();

    #mouseDown = false;
    readonly #mousePoint = new Vec2();
    readonly #displayCenter = new Vec2();

    readonly #content: HTMLElement;
    readonly #connectorsCanvas: HTMLCanvasElement;
    readonly #g: CanvasRenderingContext2D;
    readonly #render: FrameRequestCallback;
    readonly #resize: () => void;
    readonly #size = new Vec2();
    readonly #halfSize = new Vec2();
    readonly #container: HTMLElement;
    readonly #getWeightMod: (connected: boolean, a: T, b: T) => number;
    readonly #makeElementClass: (value: T) => string;
    readonly #makeContent: (value: T) => string | HTMLElement;

    #running = false;
    #scale = 1;
    #showArrows = true;

    #displayCount = 0;
    #selectedNode: ForceDirectedNode<T> = null;
    #data: T[] = null;
    #timer: number = null;

    performLayout = true;

    #displayDepth = Number.MAX_SAFE_INTEGER;
    get displayDepth() { return this.#displayDepth; }
    set displayDepth(v) { this.#displayDepth = Math.max(0, Math.min(Number.MAX_SAFE_INTEGER, v)); }


    limit = 10;
    cooling = false;
    attract = 1;
    repel = 1;
    centeringGravity = .1;

    get running() { return this.#running; }

    get scale() {
        return this.#scale;
    }

    set scale(v) {
        this.#scale = Math.max(.1, Math.min(10, v));
        this.#resize();
    }

    get showArrows() {
        return this.#showArrows;
    }

    set showArrows(v) {
        if (v !== this.#showArrows) {
            this.#showArrows = v;
            if (!this.#showArrows) {
                this.#g.clearRect(0, 0, this.#g.canvas.width, this.#g.canvas.height);
            }
        }
    }

    constructor(
        container: HTMLElement,
        getWeightMod: (connected: boolean, a: T, b: T) => number,
        makeElementClass: (value: T) => string,
        makeContent: (value: T) => string | HTMLElement) {

        SingletonStyleBlob("Juniper:Widgets:ForceDirectedGraph", () =>
            rule(".force-directed-graph",
                position("relative"),
                userSelect("none"),

                rule(" .content",
                    position("absolute"),
                    overflow("hidden")
                ),

                rule(" canvas",
                    position("absolute"),
                    width("100%"),
                    height("100%"),
                    pointerEvents("none"),
                    zIndex(1)
                ),

                rule(" .graph-node",
                    position("absolute"),
                    zIndex(2),

                    rule(".selected",
                        zIndex(3)
                    ),

                    rule(".top-most",
                        zIndex(4)
                    ),

                    rule(".moving a",
                        pointerEvents("none")
                    )
                )
            )
        );

        this.#container = container;
        this.#getWeightMod = getWeightMod;
        this.#makeElementClass = makeElementClass;
        this.#makeContent = makeContent;

        this.#render = this.#_render.bind(this);

        this.#container.classList.add("force-directed-graph");

        this.#container.addEventListener("wheel", evt => {
            this.#setMouse(evt);
            delta1.copy(this.#mousePoint)
                .sub(this.#displayCenter)
                .scale(this.scale);
            const start = new Vec2(delta1);

            this.scale -= 0.001 * evt.deltaY;

            this.#setMouse(evt);
            delta1.copy(this.#mousePoint)
                .sub(this.#displayCenter)
                .scale(this.scale);

            delta1.sub(start)
                .scale(1 / this.scale)
                .add(this.#displayCenter);

            this.#displayCenter.copy(delta1);
        });

        this.#container.addEventListener("mousedown", evt => {
            this.#setMouse(evt);
            this.#mouseDown = true;
            if (evt.target instanceof HTMLElement) {
                let nextGrabbed: ForceDirectedNode<T>;
                let here = evt.target;
                while (!nextGrabbed && here) {
                    nextGrabbed = this.#elementToNode.get(here);
                    here = here.parentElement;
                }

                if (nextGrabbed && nextGrabbed.pinner.contains(evt.target)) {
                    nextGrabbed.pinned = !nextGrabbed.pinned;
                }
                else {
                    const lastGrabbedElement = this.#container.querySelector<HTMLElement>(".top-most");
                    const lastGrabbed = lastGrabbedElement && this.#elementToNode.get(lastGrabbedElement);

                    if (nextGrabbed !== lastGrabbed) {
                        if (lastGrabbed) {
                            lastGrabbed.grabbed = false;
                        }

                        if (nextGrabbed) {
                            nextGrabbed.grabbed = true;
                            nextGrabbed.pinned = true;
                        }
                    }

                    if (nextGrabbed) {
                        nextGrabbed.setMouseOffset(this.#mousePoint);
                    }
                }
            }
        });

        this.#container.addEventListener("mousemove", evt => {
            const lastGrabbedElement = this.#container.querySelector<HTMLElement>(".top-most");
            const lastGrabbed = lastGrabbedElement && this.#elementToNode.get(lastGrabbedElement);
            if (lastGrabbed) {
                this.#setMouse(evt);
                evt.preventDefault();
                lastGrabbed.moving = true;
            }
            else if (this.#mouseDown) {
                delta1.copy(this.#mousePoint);
                this.#setMouse(evt);
                delta1.sub(this.#mousePoint);
                this.#displayCenter.sub(delta1);
            }
        });

        this.#container.addEventListener("mouseup", (evt) => {
            this.#mouseDown = false;
            const lastGrabbedElement = this.#container.querySelector<HTMLElement>(".top-most");
            const lastGrabbed = lastGrabbedElement && this.#elementToNode.get(lastGrabbedElement);
            if (lastGrabbed) {
                evt.preventDefault();
                lastGrabbed.moving = false;
                lastGrabbed.grabbed = false;
            }
        });

        this.#content = document.createElement("div");
        this.#content.classList.add("content");
        this.#container.append(this.#content);

        this.#connectorsCanvas = document.createElement("canvas");
        this.#content.append(this.#connectorsCanvas);
        this.#g = this.#connectorsCanvas.getContext("2d");

        this.#resize = debounce(() => {
            const zoom = 100 / this.scale;
            const offset = (100 - zoom) / 2;
            this.#content.style.width = zoom + "%";
            this.#content.style.height = zoom + "%";
            this.#content.style.left = offset + "%";
            this.#content.style.top = offset + "%";
            this.#content.style.transform = `scale(${this.scale})`;
            this.#size.x = this.#connectorsCanvas.clientWidth;
            this.#size.y = this.#connectorsCanvas.clientHeight;
            this.#halfSize.copy(this.#size).scale(0.5);
            this.#connectorsCanvas.width = this.#size.x * devicePixelRatio * this.scale;
            this.#connectorsCanvas.height = this.#size.y * devicePixelRatio * this.scale;
        });

        const resizer = new ResizeObserver((evts) => {
            for (const evt of evts) {
                if (evt.target === this.#connectorsCanvas) {
                    this.#resize();
                }
            }
        });

        resizer.observe(this.#connectorsCanvas);

        this.#resize();
    }

    start() {
        this.#timer = requestAnimationFrame(this.#render);
        this.#running = true;
    }

    stop() {
        if (this.#timer) {
            cancelAnimationFrame(this.#timer);
            this.#timer = null;
            this.#running = false;
        }
    }

    showCycles(show: boolean, strict: boolean) {
        for (const node of this.#graph.values()) {
            node.element.classList.remove("cycled", "not-cycled");
            node.hidden = false;
        }

        if (show) {
            const wasVisitedBy = new Map<ForceDirectedNode<T>, Set<ForceDirectedNode<T>>>();
            for (const node of this.#graph.values()) {
                wasVisitedBy.set(node, new Set());
            }

            for (const node of this.#graph.values()) {
                const visited = new Set<ForceDirectedNode<T>>();
                const notVisited = (node: ForceDirectedNode<T>) => !visited.has(node);
                const queue = [...node.connections];
                if (!strict && node.reverseConnections.length > 0) {
                    queue.push(...node.reverseConnections);
                }
                while (queue.length > 0) {
                    const here = queue.shift();
                    if (!visited.has(here)) {
                        visited.add(here);
                        wasVisitedBy.get(here).add(node);
                        const next = here.connections.filter(notVisited);
                        if (!strict) {
                            const reverse = here.reverseConnections.filter(notVisited);
                            if (reverse.length > 0) {
                                next.push(...reverse);
                            }
                        }

                        if (next.length > 0) {
                            queue.push(...next);
                        }
                    }
                }
            }

            for (const node of this.#graph.values()) {
                const visitors = wasVisitedBy.get(node);
                const visitedSelf = visitors.has(node);
                node.element.classList.add(visitedSelf
                    ? "cycled"
                    : "not-cycled");
            }

            for (const node of this.#graph.values()) {
                if (node.element.classList.contains("not-cycled")) {
                    const connections = [
                        ...node.connections,
                        ...node.reverseConnections
                    ];

                    node.hidden = connections
                        .filter(c => !c.element.classList.contains("cycled"))
                        .length == 0;
                }
            }
        }
    }

    get values() {
        return this.#data;
    }

    set values(v) {
        if (v !== this.#data) {
            this.#content.innerHTML = "";
            this.#graph.clear();
            this.#elementToNode.clear();
            this.#selectedNode = null;

            this.#data = v;

            if (this.#data) {
                this.#content.append(this.#connectorsCanvas);

                for (const value of this.#data) {
                    if (!this.#graph.has(value)) {
                        const node = new ForceDirectedNode(
                            value,
                            this.#makeElementClass(value),
                            this.#makeContent(value)
                        );
                        this.#graph.set(value, node);
                        this.#elementToNode.set(node.element, node);
                        this.#content.append(node.element);
                        node.computeBounds(1 / this.scale);
                    }
                }

                this.reset();
            }

            this.#draw();
        }
    }

    clear() {
        this.values = [];
    }

    connect(connections: [T, T][], mode: GraphMode = "directed") {
        for (const [fromValue, toValue] of connections) {
            const fromNode = this.#graph.get(fromValue);
            const toNode = this.#graph.get(toValue);

            if (mode === "directed" || mode === "undirected") {
                fromNode.connectTo(toNode);
            }

            if (mode === "reverse-directed" || mode === "undirected") {
                toNode.connectTo(fromNode);
            }
        }

        this.#updateDepths();

        for (const node of this.#graph.values()) {
            node.updateWeights(this.#getWeightMod, this.#graph.values());
        }
    }

    reset() {
        const R = Math.min(...this.#size) / 2;
        for (const node of this.#graph.values()) {
            if (!node.pinned) {
                do {
                    node.position.x = Math.random() * 2 - 1;
                    node.position.y = Math.random() * 2 - 1;
                } while (node.position.magnitude > 1);

                node.position.scale(R);
            }
        }
    }

    #_render() {
        this.#timer = requestAnimationFrame(this.#render);
        if (this.#data) {
            this.fr91();
            this.#draw();
        }
    }

    #setMouse(evt: MouseEvent) {
        this.#mousePoint.x = (evt.pageX - this.#container.offsetLeft) / this.scale;
        this.#mousePoint.y = (evt.pageY - this.#container.offsetTop) / this.scale;
    }

    #draw() {
        delta1.copy(this.#halfSize)
            .add(this.#displayCenter);

        if (this.showArrows) {
            this.#g.clearRect(0, 0, this.#g.canvas.width, this.#g.canvas.height);

            this.#g.fillStyle = "black";
            this.#g.strokeStyle = "black";
            this.#g.lineWidth = 2;

            this.#g.save();
            this.#g.scale(devicePixelRatio, devicePixelRatio);
            this.#g.scale(this.scale, this.scale);
            this.#g.translate(delta1.x, delta1.y);

            for (const n1 of this.#graph.values()) {
                if (n1.canDrawArrow(this.#displayDepth)) {
                    this.#g.save();
                    this.#g.translate(n1.position.x, n1.position.y);
                    for (const n2 of n1.connections) {
                        if (n2.canDrawArrow(this.#displayDepth)) {
                            const inter = findIntersection(n1.position, n2.position, n2.size);
                            if (inter) {
                                const angleRad = Math.atan2(inter.y, inter.x);
                                this.#g.save();
                                this.#g.rotate(angleRad);
                                this.#g.beginPath();
                                this.#g.moveTo(inter.z - 5, 0);
                                this.#g.lineTo(inter.z - 15, -6);
                                this.#g.lineTo(inter.z - 12.5, 0);
                                this.#g.lineTo(inter.z - 15, 6);
                                this.#g.lineTo(inter.z - 5, 0);
                                this.#g.fill();
                                this.#g.lineTo(0, 0);
                                this.#g.stroke();
                                this.#g.restore();
                            }
                        }
                    }
                    this.#g.restore();
                }
            }

            this.#g.restore();
        }

        delta1.copy(this.#halfSize)
            .add(this.#displayCenter);
        for (const node of this.#graph.values()) {
            node.updatePosition(delta1, this.#displayDepth);
        }
    }

    getElement(value: T): HTMLElement {
        const node = this.#graph.get(value);
        return node?.element;
    }

    #setElementClass(value: T, className: string) {
        if (value) {
            const node = this.#graph.get(value);
            if (node) {
                node.element.classList.add(className);
            }
            return node;
        }
        else {
            for (const node of this.#graph.values()) {
                if (node.element.classList.contains(className)) {
                    node.element.classList.remove(className);
                }
            }
            return null;
        }

    }

    select(value: T) {
        this.#selectedNode = this.#setElementClass(value, "selected");
        if (this.#selectedNode) {
            this.#selectedNode.pinned = true;
            this.#selectedNode.position.x = 0;
            this.#selectedNode.position.y = 0;
        }
        this.#updateDepths();
    }

    unpinAll() {
        for (const node of this.#graph.values()) {
            if (node !== this.#selectedNode) {
                node.pinned = false;
            }
        }
    }

    #updateDepths() {
        if (this.#selectedNode) {
            for (const node of this.#graph.values()) {
                node.depth = Number.MAX_SAFE_INTEGER;
            }

            this.#selectedNode.depth = 0;
            const visited = new Set<ForceDirectedNode<T>>();
            const toVisit = [this.#selectedNode];

            while (toVisit.length > 0) {
                const here = toVisit.shift();
                if (!visited.has(here)) {
                    visited.add(here);

                    const next = Array.from(new Set([
                        ...here.connections,
                        ...here.reverseConnections
                    ]));

                    if (next.length > 0) {
                        for (const n of next) {
                            n.depth = Math.min(n.depth, here.depth + 1);
                        }

                        toVisit.push(...next);
                    }
                }
            }


            this.#displayCount = 0;
            for (const node of this.#graph.values()) {
                if (node.depth <= this.#displayDepth) {
                    ++this.#displayCount;
                }
            }
        }
        else {
            for (const node of this.#graph.values()) {
                node.depth = -1;
            }
            this.#displayCount = this.#graph.size;
        }
    }

    #applyForces(
        attractFunc: (connected: boolean, len: number) => number,
        repelFunc: (connected: boolean, len: number) => number) {

        // calculate forces
        for (const n1 of this.#graph.values()) {
            n1.applyForces(
                this.#graph.values(),
                this.running, this.performLayout, this.#displayDepth, this.centeringGravity,
                this.#mousePoint,
                this.attract, this.repel, attractFunc, repelFunc);
        }

        for (const n1 of this.#graph.values()) {
            n1.apply(this.limit);
        }

        if (this.cooling) {
            this.limit *= 0.975;
        }
    }

    fr91() {
        const area = 5e7;
        const c0 = 1;
        const c1 = 0.1;
        const c2 = 1.25;
        const c3 = 1.25;
        const c4 = 0.2;
        const c5 = 2;
        const k = c0 * Math.sqrt(c1 * area / this.#displayCount);
        this.#applyForces(
            (connected, len) => connected ? c2 * Math.pow(len, c3) / k : 0,
            (_, len) => c4 * Math.pow(k, c5) / len);
    }
}
