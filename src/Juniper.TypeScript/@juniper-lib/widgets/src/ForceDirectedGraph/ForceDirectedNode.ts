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

const unpinned = "\u{d83d}\u{dccc}";
const pinned = "\u{d83d}\u{dccd}";

export class ForceDirectedNode<T> extends GraphNode<T> {
    private readonly content: HTMLElement;

    public readonly pinner: HTMLButtonElement;
    public readonly element: HTMLElement;
    public readonly mouseOffset: [number, number] = [0, 0];
    public readonly position: [number, number] = [0, 0];
    public readonly dynamicForce: [number, number] = [0, 0];
    public readonly staticForce: [number, number] = [0, 0];

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

    constructor(value: T, public readonly name: string, content: string | HTMLElement) {
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
        this.element.append(this.pinner, this.content);

        this.setContent(content);
    }

    setContent(content: string | HTMLElement) {
        this.content.innerHTML = "";
        this.content.append(content);
    }

    setMouseOffset(mousePoint: [number, number]) {
        this.mouseOffset[0] = mousePoint[0] - this.position[0];
        this.mouseOffset[1] = mousePoint[1] - this.position[1];
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

    updatePosition(maxDepth: number) {
        const { position, element } = this;
        element.style.display = maxDepth < 0 || this.depth <= maxDepth ? "" : "none";
        if (this.canDrawArrow(maxDepth)) {
            element.style.display = "";
            element.style.left = `${position[0] - this.hw}px`;
            element.style.top = `${position[1] - this.hh}px`;
            element.style.opacity = maxDepth < 0 || this.depth < maxDepth
                ? "1"
                : ".5";
        }
    }

    moveTo(mousePoint: number[]) {
        this.position[0] = mousePoint[0] - this.mouseOffset[0];
        this.position[1] = mousePoint[1] - this.mouseOffset[1];
    }

    resetForce() {
        this.dynamicForce[0] = 0;
        this.dynamicForce[1] = 0;
    }

    canDrawArrow(maxDepth: number): boolean {
        return this.bounds
            && (maxDepth < 0
                || this.depth <= maxDepth);
    }

    updateForce(w: number, h: number, wallForce: number) {
        const f1 = this.dynamicForce;
        const f0 = this.staticForce;
        if (f0) {
            f1[0] += f0[0];
            f1[1] += f1[0];
        }

        delta[0] = 0.5 * w - this.position[0];
        delta[1] = 0.5 * h - this.position[1];

        const len = Math.sqrt(delta[0] * delta[0] + delta[1] * delta[1]);
        if (len > 0) {
            delta[0] /= len;
            delta[1] /= len;
            let f = wallForce * len;
            f = Math.sign(f) * Math.pow(Math.abs(f), 0.3);
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
            const connected = n2.isConnectedTo(this);
            const weight = getWeightMod(this.value, n2.value, connected);
            const invWeight = 2 - weight;
            const f = weight * attract * attractFunc(connected, len)
                - invWeight * repel * repelFunc(connected, len);

            this.dynamicForce[0] += delta[0] * f;
            this.dynamicForce[1] += delta[1] * f;
        }
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
}
