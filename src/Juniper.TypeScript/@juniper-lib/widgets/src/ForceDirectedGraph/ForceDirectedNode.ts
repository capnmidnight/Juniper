import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";

export type Vec2 = [number, number];
export type Vec2WithLen = [number, number, number];
export function length(vec: Vec2): number
export function length(x: number, y: number): number;
export function length(xOrVec: number | Vec2, y?: number): number {
    let x: number;
    if (typeof xOrVec === "number") {
        x = xOrVec;
    }
    else {
        [x, y] = xOrVec
    }

    return Math.sqrt(x * x + y * y);
}

export function add(a: Vec2, b: Vec2, out: Vec2) {
    out[0] = a[0] + b[0];
    out[1] = a[1] + b[1];
}

export function sub(a: Vec2, b: Vec2, out: Vec2) {
    out[0] = a[0] - b[0];
    out[1] = a[1] - b[1];
}

export function zero(a: Vec2) {
    a[0] = a[1] = 0;
}

export function scale(a: number, b: Vec2, out: Vec2) {
    out[0] = a * b[0];
    out[1] = a * b[1];
}

export function copy(a: Vec2, out: Vec2) {
    out[0] = a[0];
    out[1] = a[1];
}

const delta: Vec2 = [0, 0];
interface IBounds {
    left: number;
    top: number;
    right: number;
    bottom: number;
    width: number;
    height: number;
}

export interface IFullBounds {
    styles: CSSStyleDeclaration;
    margin: IBounds;
    border: IBounds;
    padding: IBounds;
    interior: IBounds;
}

type BoundsCache = Map<Element, IFullBounds>;

function elementComputeBounds(element: Element, cache?: BoundsCache): IFullBounds {
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

const unpinned = "<i>\u{d83d}\u{dccc}</i>";
const pinned = "<i>\u{d83d}\u{dccd}</i>";

export class ForceDirectedNode<T> extends GraphNode<T> {
    private readonly content: HTMLElement;
    public readonly pinner: HTMLButtonElement;
    public readonly element: HTMLElement;

    public readonly mouseOffset: Vec2 = [0, 0];
    public readonly position: Vec2 = [0, 0];
    public readonly dynamicForce: Vec2 = [0, 0];

    private hw: number = null;
    private hh: number = null;
    private _pinned = false;

    public bounds: IFullBounds = null;
    public depth = -1;

    get pinned() {
        return this._pinned;
    }

    set pinned(v) {
        this._pinned = v;
        this.pinner.innerHTML = this.pinned ? pinned : unpinned;
    }

    get moving() {
        return this.element.classList.contains("moving");
    }

    set moving(v) {
        this.element.classList.toggle("moving", v);
    }

    constructor(value: T, elementClass: string, content: string | HTMLElement) {
        super(value);

        this.pinner = document.createElement("button");
        this.pinner.type = "button";
        this.pinner.innerHTML = unpinned;
        this.pinner.style.float = "right";
        this.pinner.style.backgroundColor = "transparent";
        this.pinner.addEventListener("click", () =>
            this.pinned = !this.pinned);


        this.content = document.createElement("div");
        this.element = document.createElement("div");
        this.element.classList.add("graph-node");
        if (elementClass) {
            this.element.classList.add(elementClass);
        }
        this.element.append(this.pinner, this.content);

        this.setContent(content);
    }

    setContent(content: string | HTMLElement) {
        this.content.innerHTML = "";
        this.content.append(content);
    }

    setMouseOffset(mousePoint: Vec2) {
        sub(mousePoint, this.position, this.mouseOffset);
    }

    computeBounds(boundsCache: BoundsCache) {
        this.bounds = elementComputeBounds(this.element, boundsCache);
        if (this.bounds) {
            const b = this.bounds.padding;
            this.hw = b.width / 2;
            this.hh = b.height / 2;
        }
        else {
            this.hw = null;
            this.hh = null;
        }
    }

    updatePosition(cx: number, cy: number, maxDepth: number) {
        const { position, element } = this;
        element.style.display = maxDepth < 0 || this.depth <= maxDepth ? "" : "none";
        if (this.canDrawArrow(maxDepth)) {
            element.style.display = "";
            element.style.left = `${position[0] - this.hw + cx}px`;
            element.style.top = `${position[1] - this.hh + cy}px`;
            element.style.opacity = maxDepth < 0 || this.depth < maxDepth
                ? "1"
                : ".5";
        }
    }

    moveTo(mousePoint: Vec2) {
        sub(mousePoint, this.mouseOffset, this.position);
    }

    resetForce() {
        zero(this.dynamicForce);
    }

    canDrawArrow(maxDepth: number): boolean {
        return this.bounds
            && (maxDepth < 0
                || this.depth <= maxDepth);
    }

    gravitate(gravity: number) {
        // Get displacement from center
        scale(-1, this.position, delta);

        const distance = length(delta);
        if (distance > 0) {
            scale(gravity, delta, delta);
            add(this.dynamicForce, delta, this.dynamicForce);
        }
    }

    attractRepel(
        n2: ForceDirectedNode<T>,
        attract: number, attractFunc: (connected: boolean, len: number) => number,
        repel: number, repelFunc: (connected: boolean, len: number) => number,
        getWeightMod: (connected: boolean, dist: number, a: T, b: T) => number) {

        // Displacement between this node and the other node
        sub(n2.position, this.position, delta);

        const distance = length(delta);

        if (distance > 0) {
            const connected = n2.isConnectedTo(this);
            const weight = getWeightMod(connected, distance, this.value, n2.value);
            const invWeight = 1 - weight;
            const f = weight * attract * attractFunc(connected, distance)
                - invWeight * repel * repelFunc(connected, distance);

            // Convert the displacement vector to the calculated force vector
            // and accumulate forces.
            scale(f / distance, delta, delta);
            add(this.dynamicForce, delta, this.dynamicForce);
        }
    }

    apply(t: number, w: number, h: number) {
        // squarify the force
        this.dynamicForce[1] *= h / w;
        const len = length(this.dynamicForce);
        if (len > 0) {
            // Restrict forces to a maximum magnitude
            const r = Math.min(t, len) / len;
            scale(r, this.dynamicForce, this.dynamicForce);

            // Apply the force
            add(this.position, this.dynamicForce, this.position);
        }
    }
}
