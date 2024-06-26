import { compareBy } from "@juniper-lib/collections/src/arrays";
import { debounce } from "@juniper-lib/events/src/debounce";
import { Vec2, Vec3 } from "gl-matrix/dist/esm";
import { ForceDirectedNode } from "./ForceDirectedNode";
function distinct(arr) {
    return Array.from(new Set(arr));
}
const delta = new Vec2();
function findIntersection(sx, sy, ex, ey, size) {
    const dx = ex - sx;
    const dy = ey - sy;
    delta.x = dx;
    delta.y = dy;
    const len = delta.magnitude;
    const hw = size.x / 2;
    const hh = size.y / 2;
    const lx = dx - hw;
    const rx = dx + hw;
    const ty = dy - hh;
    const by = dy + hh;
    const points = [
        intersect(dx, dy, len, lx, ty, rx, ty, size.x),
        intersect(dx, dy, len, lx, by, rx, by, size.x),
        intersect(dx, dy, len, lx, ty, lx, by, size.y),
        intersect(dx, dy, len, rx, ty, rx, by, size.y)
    ].filter(v => v);
    if (points.length === 0) {
        return null;
    }
    points.sort(compareBy(v => v[2]));
    return points[0];
}
function intersect(d1x, d1y, len1, v1x, v1y, v2x, v2y, len2) {
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
    delta.x = d1x;
    delta.y = d1y;
    const len = delta.magnitude;
    return new Vec3(d1x, d1y, len);
}
const style = document.createElement("style");
document.head.append(style);
let ruleIndex = null;
function setNodeScale(scale) {
    if (ruleIndex !== null) {
        style.sheet.deleteRule(ruleIndex);
    }
    ruleIndex = style.sheet.insertRule(`.force-directed-graph .graph-node { transform: scale(${scale}); }`);
}
setNodeScale(1);
export class ForceDirectedGraph {
    get running() { return this._running; }
    get scale() {
        return this._scale;
    }
    set scale(v) {
        this._scale = Math.max(.1, Math.min(10, v));
        this.resize();
    }
    get showArrows() {
        return this._showArrows;
    }
    set showArrows(v) {
        if (v !== this._showArrows) {
            this._showArrows = v;
            if (!this._showArrows) {
                this.g.clearRect(0, 0, this.g.canvas.width, this.g.canvas.height);
            }
        }
    }
    constructor(container, getWeightMod, makeElementClass, makeContent) {
        this.container = container;
        this.getWeightMod = getWeightMod;
        this.makeElementClass = makeElementClass;
        this.makeContent = makeContent;
        this.graph = new Map();
        this.elementToNode = new Map();
        this.mouseDown = false;
        this.mousePoint = new Vec2();
        this.displayCenter = new Vec2();
        this.size = new Vec2();
        this._running = false;
        this._scale = 1;
        this._showArrows = true;
        this.displayCount = 0;
        this.selectedNode = null;
        this.data = null;
        this.timer = null;
        this.performLayout = true;
        this.displayDepth = -1;
        this.limit = 10;
        this.cooling = false;
        this.attract = 1;
        this.repel = 1;
        this.centeringGravity = .1;
        this.halfSize = new Vec2();
        this.render = this._render.bind(this);
        this.container.classList.add("force-directed-graph");
        this.container.addEventListener("wheel", evt => {
            this.setMouse(evt);
            delta.copy(this.mousePoint)
                .sub(this.displayCenter)
                .scale(this.scale);
            const start = new Vec2(delta);
            this.scale -= 0.001 * evt.deltaY;
            this.setMouse(evt);
            delta.copy(this.mousePoint)
                .sub(this.displayCenter)
                .scale(this.scale);
            delta.sub(start)
                .scale(1 / this.scale)
                .add(this.displayCenter);
            this.displayCenter.copy(delta);
        });
        this.container.addEventListener("mousedown", evt => {
            this.setMouse(evt);
            this.mouseDown = true;
            if (evt.target instanceof HTMLElement) {
                let nextGrabbed;
                let here = evt.target;
                while (!nextGrabbed && here) {
                    nextGrabbed = this.elementToNode.get(here);
                    here = here.parentElement;
                }
                if (nextGrabbed && nextGrabbed.pinner.contains(evt.target)) {
                    nextGrabbed.pinned = !nextGrabbed.pinned;
                }
                else {
                    const lastGrabbedElement = container.querySelector(".top-most");
                    const lastGrabbed = lastGrabbedElement && this.elementToNode.get(lastGrabbedElement);
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
                        nextGrabbed.setMouseOffset(this.mousePoint);
                    }
                }
            }
        });
        this.container.addEventListener("mousemove", evt => {
            const lastGrabbedElement = container.querySelector(".top-most");
            const lastGrabbed = lastGrabbedElement && this.elementToNode.get(lastGrabbedElement);
            if (lastGrabbed) {
                this.setMouse(evt);
                evt.preventDefault();
                lastGrabbed.moving = true;
            }
            else if (this.mouseDown) {
                delta.copy(this.mousePoint);
                this.setMouse(evt);
                delta.sub(this.mousePoint);
                this.displayCenter.sub(delta);
            }
        });
        this.container.addEventListener("mouseup", (evt) => {
            this.mouseDown = false;
            const lastGrabbedElement = container.querySelector(".top-most");
            const lastGrabbed = lastGrabbedElement && this.elementToNode.get(lastGrabbedElement);
            if (lastGrabbed) {
                evt.preventDefault();
                lastGrabbed.moving = false;
                lastGrabbed.grabbed = false;
            }
        });
        this.content = document.createElement("div");
        this.content.classList.add("content");
        this.container.append(this.content);
        this.connectorsCanvas = document.createElement("canvas");
        this.content.append(this.connectorsCanvas);
        this.g = this.connectorsCanvas.getContext("2d");
        this.resize = debounce(() => {
            const zoom = 100 / this.scale;
            const offset = (100 - zoom) / 2;
            this.content.style.width = zoom + "%";
            this.content.style.height = zoom + "%";
            this.content.style.left = offset + "%";
            this.content.style.top = offset + "%";
            this.content.style.transform = `scale(${this.scale})`;
            this.size.x = this.connectorsCanvas.clientWidth;
            this.size.y = this.connectorsCanvas.clientHeight;
            this.halfSize.copy(this.size).scale(0.5);
            this.connectorsCanvas.width = this.size.x * devicePixelRatio * this.scale;
            this.connectorsCanvas.height = this.size.y * devicePixelRatio * this.scale;
        });
        const resizer = new ResizeObserver((evts) => {
            for (const evt of evts) {
                if (evt.target === this.connectorsCanvas) {
                    this.resize();
                }
            }
        });
        resizer.observe(this.connectorsCanvas);
        this.resize();
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
    showCycles(show, strict) {
        for (const node of this.graph.values()) {
            node.element.classList.remove("cycled", "not-cycled");
            node.hidden = false;
        }
        if (show) {
            const wasVisitedBy = new Map();
            for (const node of this.graph.values()) {
                wasVisitedBy.set(node, new Set());
            }
            for (const node of this.graph.values()) {
                const visited = new Set();
                ;
                const notVisited = (node) => !visited.has(node);
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
            for (const node of this.graph.values()) {
                const visitors = wasVisitedBy.get(node);
                const visitedSelf = visitors.has(node);
                node.element.classList.add(visitedSelf
                    ? "cycled"
                    : "not-cycled");
            }
            for (const node of this.graph.values()) {
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
        return this.data;
    }
    set values(v) {
        if (v !== this.data) {
            this.content.innerHTML = "";
            this.graph.clear();
            this.elementToNode.clear();
            this.selectedNode = null;
            this.data = v;
            if (this.data) {
                this.content.append(this.connectorsCanvas);
                for (const value of this.data) {
                    if (!this.graph.has(value)) {
                        const node = new ForceDirectedNode(value, this.makeElementClass(value), this.makeContent(value));
                        this.graph.set(value, node);
                        this.elementToNode.set(node.element, node);
                        this.content.append(node.element);
                        node.computeBounds(1 / this.scale);
                    }
                }
                this.reset();
            }
        }
    }
    connect(connections, mode = "directed") {
        for (const [fromValue, toValue] of connections) {
            const fromNode = this.graph.get(fromValue);
            const toNode = this.graph.get(toValue);
            if (mode === "directed" || mode === "undirected") {
                fromNode.connectTo(toNode);
            }
            if (mode === "reverse-directed" || mode === "undirected") {
                toNode.connectTo(fromNode);
            }
        }
        this.updateDepths();
    }
    reset() {
        const R = Math.min(...this.size) / 2;
        for (const node of this.graph.values()) {
            if (!node.pinned) {
                do {
                    node.position.x = Math.random() * 2 - 1;
                    node.position.y = Math.random() * 2 - 1;
                } while (node.position.magnitude > 1);
                node.position.scale(R);
            }
        }
    }
    _render() {
        this.timer = requestAnimationFrame(this.render);
        if (this.data) {
            this.fr91();
            this.draw();
        }
    }
    setMouse(evt) {
        this.mousePoint.x = (evt.pageX - this.container.offsetLeft) / this.scale;
        this.mousePoint.y = (evt.pageY - this.container.offsetTop) / this.scale;
    }
    draw() {
        delta.copy(this.halfSize)
            .add(this.displayCenter);
        if (this.showArrows) {
            this.g.clearRect(0, 0, this.g.canvas.width, this.g.canvas.height);
            this.g.fillStyle = "black";
            this.g.strokeStyle = "black";
            this.g.lineWidth = 2;
            this.g.save();
            this.g.scale(devicePixelRatio, devicePixelRatio);
            this.g.scale(this.scale, this.scale);
            this.g.translate(delta.x, delta.y);
            for (const n1 of this.graph.values()) {
                if (n1.canDrawArrow(this.displayDepth)) {
                    const [p1x, p1y] = n1.position;
                    this.g.save();
                    this.g.translate(p1x, p1y);
                    for (const n2 of n1.connections) {
                        if (n2.canDrawArrow(this.displayDepth)) {
                            const [p2x, p2y] = n2.position;
                            const inter = findIntersection(p1x, p1y, p2x, p2y, n2.size);
                            if (inter) {
                                let [dx, dy, len] = inter;
                                if (len > 0) {
                                    const angleRad = Math.atan2(dy, dx);
                                    this.g.save();
                                    this.g.rotate(angleRad);
                                    this.g.beginPath();
                                    this.g.moveTo(len - 5, 0);
                                    this.g.lineTo(len - 15, -6);
                                    this.g.lineTo(len - 12.5, 0);
                                    this.g.lineTo(len - 15, 6);
                                    this.g.lineTo(len - 5, 0);
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
        }
        delta.copy(this.halfSize)
            .add(this.displayCenter);
        for (const node of this.graph.values()) {
            node.updatePosition(delta, this.displayDepth);
        }
    }
    getElement(value) {
        const node = this.graph.get(value);
        return node?.element;
    }
    setElementClass(value, className) {
        if (value) {
            const node = this.graph.get(value);
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
    select(value) {
        this.selectedNode = this.setElementClass(value, "selected");
        if (this.selectedNode) {
            this.selectedNode.pinned = true;
            this.selectedNode.position.x = 0;
            this.selectedNode.position.y = 0;
        }
        this.updateDepths();
    }
    unpinAll() {
        for (const node of this.graph.values()) {
            if (node !== this.selectedNode) {
                node.pinned = false;
            }
        }
    }
    updateDepths() {
        if (this.selectedNode) {
            for (const node of this.graph.values()) {
                node.depth = Number.MAX_VALUE;
            }
            this.selectedNode.depth = 0;
            const visited = new Set();
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
    applyForces(attractFunc, repelFunc) {
        // calculate forces
        for (const n1 of this.graph.values()) {
            n1.applyForces(this.graph.values(), this.running, this.performLayout, this.displayDepth, this.centeringGravity, this.mousePoint, this.attract, this.repel, attractFunc, repelFunc, this.getWeightMod);
        }
        for (const n1 of this.graph.values()) {
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
        const k0 = c0 * Math.sqrt(c1 * area / this.displayCount);
        const k1 = c2 / k0;
        const k2 = c4 * Math.pow(k0, c5);
        this.applyForces((connected, len) => connected ? k1 * Math.pow(len, c3) : 0, (_, len) => k2 / len);
    }
}
//# sourceMappingURL=ForceDirectedGraph.js.map