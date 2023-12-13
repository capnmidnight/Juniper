import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
import { Vec2 } from "gl-matrix/dist/esm";

const delta = new Vec2();

function elementComputeSize(scale: number, element: Element, size: Vec2): void {
    const boundingRect = element.getBoundingClientRect();
    const styles = getComputedStyle(element);
    if (boundingRect.width * boundingRect.height === 0 || styles.display === "none") {
        return null;
    }

    const sBorderTop = parseFloat(styles.borderTopWidth) * scale;
    const sBorderRight = parseFloat(styles.borderRightWidth) * scale;
    const sBorderBottom = parseFloat(styles.borderBottomWidth) * scale;
    const sBorderLeft = parseFloat(styles.borderLeftWidth) * scale;

    const borderLeft = boundingRect.x * scale;
    const borderTop = boundingRect.y * scale;
    const borderWidth = boundingRect.width * scale;
    const borderHeight = boundingRect.height * scale;

    const borderRight = borderLeft + borderWidth;
    const borderBottom = borderTop + borderHeight;

    const paddingLeft = borderLeft + sBorderLeft;
    const paddingTop = borderTop + sBorderTop;

    const paddingRight = borderRight - sBorderRight;
    const paddingBottom = borderBottom - sBorderBottom;

    size.x = paddingRight - paddingLeft;
    size.y = paddingBottom - paddingTop;
}

const unpinned = "<i>\u{d83d}\u{dccc}</i>";
const pinned = "<i>\u{d83d}\u{dccd}</i>";

export class ForceDirectedNode<T> extends GraphNode<T> {
    private readonly content: HTMLElement;
    public readonly pinner: HTMLButtonElement;
    public readonly element: HTMLElement;

    public readonly mouseOffset = new Vec2();
    public readonly position = new Vec2();
    public readonly size = new Vec2();
    public readonly halfSize = new Vec2();
    public readonly dynamicForce = new Vec2();

    private _pinned = false;

    public depth = -1;
    public hidden = false;

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
        this.mouseOffset
            .copy(mousePoint)
            .sub(this.position);
    }

    computeBounds(sz: number) {
        if (this.size.x * this.size.y === 0) {
            elementComputeSize(sz, this.element, this.size);
            this.halfSize.copy(this.size)
                .scale(0.5);
        }
    }

    updatePosition(center: Vec2, maxDepth: number) {
        const { position, element } = this;
        element.style.display = this.isVisible(maxDepth) ? "" : "none";
        if (this.isVisible(maxDepth)) {
            delta.copy(position)
                .add(center)
                .sub(this.halfSize);
            element.style.left = `${delta.x}px`;
            element.style.top = `${delta.y}px`;
            element.classList.toggle("deemphasized", maxDepth >= 0 && this.depth >= maxDepth);
        }
    }

    moveTo(mousePoint: Vec2) {
        this.position.copy(mousePoint)
            .sub(this.mouseOffset);
    }

    resetForce() {
        this.dynamicForce.x = 0;
        this.dynamicForce.y = 0;
    }

    private isVisible(maxDepth: number) {
        return !this.hidden
            && (maxDepth < 0
                || this.depth <= maxDepth);
    }

    canDrawArrow(maxDepth: number): boolean {
        return this.isVisible(maxDepth)
            && !this.element.classList.contains("not-cycled");
    }

    gravitate(gravity: number) {
        this.dynamicForce.scaleAndAdd(this.position, -gravity);
    }

    attractRepel(
        n2: ForceDirectedNode<T>,
        attract: number, attractFunc: (connected: boolean, len: number) => number,
        repel: number, repelFunc: (connected: boolean, len: number) => number,
        getWeightMod: (connected: boolean, dist: number, a: T, b: T) => number) {

        // Displacement between this node and the other node
        delta.copy(n2.position)
            .sub(this.position);

        const distance = delta.magnitude;

        if (distance > 0) {
            const connected = n2.isConnectedTo(this);
            const weight = getWeightMod(connected, distance, this.value, n2.value);
            const invWeight = 1 - weight;
            const f = weight * attract * attractFunc(connected, distance)
                - invWeight * repel * repelFunc(connected, distance);

            // Convert the displacement vector to the calculated force vector
            // and accumulate forces.
            this.dynamicForce.scaleAndAdd(delta, f / distance);
        }
    }

    apply(t: number) {
        const len = this.dynamicForce.magnitude;
        if (len > 0) {
            // Restrict forces to a maximum magnitude
            const r = Math.min(t, len) / len;
            this.position.scaleAndAdd(this.dynamicForce, r);
        }
    }
}
