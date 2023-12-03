import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
export function length(xOrVec, y) {
    let x;
    if (typeof xOrVec === "number") {
        x = xOrVec;
    }
    else {
        [x, y] = xOrVec;
    }
    return Math.sqrt(x * x + y * y);
}
export function add(a, b, out) {
    out[0] = a[0] + b[0];
    out[1] = a[1] + b[1];
}
export function sub(a, b, out) {
    out[0] = a[0] - b[0];
    out[1] = a[1] - b[1];
}
export function zero(a) {
    a[0] = a[1] = 0;
}
export function scale(a, b, out) {
    out[0] = a * b[0];
    out[1] = a * b[1];
}
export function copy(a, out) {
    out[0] = a[0];
    out[1] = a[1];
}
const delta = [0, 0];
function elementComputeBounds(scale, element, cache) {
    if (cache && cache.has(element)) {
        return cache.get(element);
    }
    const boundingRect = element.getBoundingClientRect();
    const styles = getComputedStyle(element);
    if (boundingRect.width * boundingRect.height === 0 || styles.display === "none") {
        return null;
    }
    const sMarginTop = parseFloat(styles.marginTop) * scale;
    const sMarginRight = parseFloat(styles.marginRight) * scale;
    const sMarginBottom = parseFloat(styles.marginBottom) * scale;
    const sMarginLeft = parseFloat(styles.marginLeft) * scale;
    const sPaddingTop = parseFloat(styles.paddingTop) * scale;
    const sPaddingRight = parseFloat(styles.paddingRight) * scale;
    const sPaddingBottom = parseFloat(styles.paddingBottom) * scale;
    const sPaddingLeft = parseFloat(styles.paddingLeft) * scale;
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
export class ForceDirectedNode extends GraphNode {
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
    constructor(value, elementClass, content) {
        super(value);
        this.mouseOffset = [0, 0];
        this.position = [0, 0];
        this.dynamicForce = [0, 0];
        this.hw = null;
        this.hh = null;
        this._pinned = false;
        this.bounds = null;
        this.depth = -1;
        this.hidden = false;
        this.pinner = document.createElement("button");
        this.pinner.type = "button";
        this.pinner.innerHTML = unpinned;
        this.pinner.style.float = "right";
        this.pinner.style.backgroundColor = "transparent";
        this.pinner.addEventListener("click", () => this.pinned = !this.pinned);
        this.content = document.createElement("div");
        this.element = document.createElement("div");
        this.element.classList.add("graph-node");
        if (elementClass) {
            this.element.classList.add(elementClass);
        }
        this.element.append(this.pinner, this.content);
        this.setContent(content);
    }
    setContent(content) {
        this.content.innerHTML = "";
        this.content.append(content);
    }
    setMouseOffset(mousePoint) {
        sub(mousePoint, this.position, this.mouseOffset);
    }
    computeBounds(scale, boundsCache) {
        this.bounds = elementComputeBounds(scale, this.element, boundsCache);
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
    updatePosition(cx, cy, maxDepth) {
        const { position, element } = this;
        element.style.display = this.isVisible(maxDepth) ? "" : "none";
        if (this.canDrawArrow(maxDepth)) {
            element.style.left = `${position[0] - this.hw + cx}px`;
            element.style.top = `${position[1] - this.hh + cy}px`;
            element.style.opacity = maxDepth < 0 || this.depth < maxDepth
                ? "1"
                : ".5";
        }
    }
    moveTo(mousePoint) {
        sub(mousePoint, this.mouseOffset, this.position);
    }
    resetForce() {
        zero(this.dynamicForce);
    }
    isVisible(maxDepth) {
        return !this.hidden
            && (maxDepth < 0
                || this.depth <= maxDepth);
    }
    canDrawArrow(maxDepth) {
        return this.bounds
            && this.isVisible(maxDepth);
    }
    gravitate(gravity) {
        // Get displacement from center
        scale(-1, this.position, delta);
        const distance = length(delta);
        if (distance > 0) {
            scale(gravity, delta, delta);
            add(this.dynamicForce, delta, this.dynamicForce);
        }
    }
    attractRepel(n2, attract, attractFunc, repel, repelFunc, getWeightMod) {
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
    apply(t, w, h) {
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
//# sourceMappingURL=ForceDirectedNode.js.map