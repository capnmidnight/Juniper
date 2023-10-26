import { debounce } from "@juniper-lib/events/src/debounce";
import { Tau } from "@juniper-lib/tslib/dist/math";
import { ForceDirectedNode } from "./ForceDirectedNode";
import "./index.css";
const size = 20;
function elementComputeBounds(element, cache) {
    if (cache && cache.has(element)) {
        return cache.get(element);
    }
    const boundingRect = element.getBoundingClientRect();
    const styles = getComputedStyle(element);
    if (boundingRect.width * boundingRect.height === 0 || styles.display === "none") {
        return null;
    }
    const sMarginTop = parseFloat(styles.marginTop);
    const sMarginRight = parseFloat(styles.marginRight);
    const sMarginBottom = parseFloat(styles.marginBottom);
    const sMarginLeft = parseFloat(styles.marginLeft);
    const sPaddingTop = parseFloat(styles.paddingTop);
    const sPaddingRight = parseFloat(styles.paddingRight);
    const sPaddingBottom = parseFloat(styles.paddingBottom);
    const sPaddingLeft = parseFloat(styles.paddingLeft);
    const sBorderTop = parseFloat(styles.borderTopWidth);
    const sBorderRight = parseFloat(styles.borderRightWidth);
    const sBorderBottom = parseFloat(styles.borderBottomWidth);
    const sBorderLeft = parseFloat(styles.borderLeftWidth);
    const borderLeft = boundingRect.x;
    const borderTop = boundingRect.y;
    const borderWidth = boundingRect.width;
    const borderHeight = boundingRect.height;
    const borderRight = borderLeft + borderWidth;
    const borderBottom = borderTop + borderHeight;
    const marginLeft = borderLeft - sMarginLeft;
    const marginTop = borderTop - sMarginTop;
    const paddingLeft = borderLeft + sBorderLeft;
    const paddingTop = borderTop + sBorderTop;
    const interiorLeft = paddingLeft + sPaddingLeft;
    const interiorTop = paddingTop + sPaddingTop;
    const marginRight = borderRight + sMarginRight;
    const marginBottom = borderBottom + sMarginBottom;
    const paddingRight = borderRight - sBorderRight;
    const paddingBottom = borderBottom - sBorderBottom;
    const interiorRight = paddingRight - sPaddingRight;
    const interiorBottom = paddingBottom - sPaddingBottom;
    const marginWidth = marginRight - marginLeft;
    const marginHeight = marginBottom - marginTop;
    const paddingWidth = paddingRight - paddingLeft;
    const paddingHeight = paddingBottom - paddingTop;
    const interiorWidth = interiorRight - interiorLeft;
    const interiorHeight = interiorBottom - interiorTop;
    const bounds = {
        styles,
        margin: {
            left: marginLeft,
            right: marginRight,
            top: marginTop,
            bottom: marginBottom,
            width: marginWidth,
            height: marginHeight
        },
        border: {
            left: borderLeft,
            top: borderTop,
            right: borderRight,
            bottom: borderBottom,
            width: borderWidth,
            height: borderHeight
        },
        padding: {
            left: paddingLeft,
            top: paddingTop,
            right: paddingRight,
            bottom: paddingBottom,
            width: paddingWidth,
            height: paddingHeight
        },
        interior: {
            left: interiorLeft,
            top: interiorTop,
            right: interiorRight,
            bottom: interiorBottom,
            width: interiorWidth,
            height: interiorHeight
        }
    };
    if (cache) {
        cache.set(element, bounds);
    }
    return bounds;
}
export class ForceDirectedGraph {
    get running() { return this._running; }
    get w() {
        return this.content.clientWidth - 2 * size - 200;
    }
    get h() {
        return this.content.clientHeight - 2 * size;
    }
    constructor(content, getWeightMod, getID1, getName1, getID2, getName2, makeLink) {
        this.content = content;
        this.getWeightMod = getWeightMod;
        this.getID1 = getID1;
        this.getName1 = getName1;
        this.getID2 = getID2;
        this.getName2 = getName2;
        this.makeLink = makeLink;
        this.graph = new Map();
        this.elementToNode = new Map();
        this.mousePoint = [0, 0];
        this.data = null;
        this.grabbed = null;
        this.timer = null;
        this.limit = 5;
        this.cooling = false;
        this.attract = 1;
        this.repel = 1;
        this._running = false;
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
    addNode(value, id, name) {
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
    _render() {
        this.timer = requestAnimationFrame(this.render);
        if (this.data) {
            this.fr91();
            this.draw();
        }
    }
    setMouse(evt) {
        this.mousePoint[0] = evt.pageX - this.content.offsetLeft - size;
        this.mousePoint[1] = evt.pageY - this.content.offsetTop - size;
    }
    draw() {
        this.g.clearRect(0, 0, this.connectorsCanvas.width, this.connectorsCanvas.height);
        this.g.fillStyle = "black";
        this.g.strokeStyle = "black";
        this.g.save();
        this.g.translate(size, size);
        this.g.scale(devicePixelRatio, devicePixelRatio);
        const boundsCache = new Map();
        for (const n1 of this.graph.values()) {
            const p1 = n1.position;
            const bounds1 = elementComputeBounds(n1.element, boundsCache);
            const sx = p1[0] + bounds1.interior.width / 2;
            const sy = p1[1] + bounds1.interior.height / 2;
            for (const node2 of n1.connections) {
                const p2 = node2.position;
                const bounds2 = elementComputeBounds(node2.element, boundsCache);
                const ex = p2[0] + bounds2.interior.width / 2;
                const ey = p2[1] + bounds2.interior.height / 2;
                this.g.beginPath();
                this.g.moveTo(sx, sy);
                this.g.lineTo(ex, ey);
                this.g.stroke();
            }
        }
        this.g.restore();
        for (const node of this.graph.values()) {
            node.updatePosition();
        }
    }
    getElement(id) {
        const node = this.graph.get(id);
        return node?.element;
    }
    setElementClass(id, className) {
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
    select(id) {
        this.setElementClass(id, "selected");
    }
    highlight(id) {
        this.setElementClass(id, "highlighted");
    }
    applyForces(attractFunc, repelFunc) {
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
            this.applyForces((connected, len) => connected ? c2 * Math.sqrt(len) * len / k : 0, (_, len) => c3 * k * k / len);
        }
    }
}
//# sourceMappingURL=index.js.map