import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
import { Vec2 } from "gl-matrix/dist/esm";
const delta = new Vec2();
function elementComputeSize(scale, element, size) {
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
export class ForceDirectedNode extends GraphNode {
    #pinned;
    get pinned() {
        return this.#pinned;
    }
    set pinned(v) {
        this.#pinned = v;
        this.element.classList.toggle("pinned", v);
        this.pinner.innerHTML = v ? pinned : unpinned;
    }
    #grabbed;
    get grabbed() {
        return this.#grabbed;
    }
    set grabbed(v) {
        this.#grabbed = v;
        this.element.classList.toggle("top-most", v);
    }
    #moving;
    get moving() {
        return this.#moving;
    }
    set moving(v) {
        this.#moving = v;
        this.element.classList.toggle("moving", v);
    }
    constructor(value, elementClass, content) {
        super(value);
        this.mouseOffset = new Vec2();
        this.position = new Vec2();
        this.size = new Vec2();
        this.halfSize = new Vec2();
        this.dynamicForce = new Vec2();
        this.depth = -1;
        this.hidden = false;
        this.#pinned = false;
        this.#grabbed = false;
        this.#moving = false;
        this.pinner = document.createElement("button");
        this.pinner.type = "button";
        this.pinner.style.float = "right";
        this.pinner.style.backgroundColor = "transparent";
        this.content = document.createElement("div");
        this.element = document.createElement("div");
        this.element.classList.add("graph-node");
        if (elementClass) {
            this.element.classList.add(elementClass);
        }
        this.element.append(this.pinner, this.content);
        this.setContent(content);
        this.pinned = false;
        this.grabbed = false;
        this.moving = false;
    }
    setContent(content) {
        this.content.innerHTML = "";
        this.content.append(content);
    }
    setMouseOffset(mousePoint) {
        this.mouseOffset
            .copy(mousePoint)
            .sub(this.position);
    }
    computeBounds(sz) {
        if (this.size.x * this.size.y === 0) {
            elementComputeSize(sz, this.element, this.size);
            this.halfSize.copy(this.size)
                .scale(0.5);
        }
    }
    applyForces(nodes, running, performLayout, displayDepth, centeringGravity, mousePoint, attract, repel, attractFunc, repelFunc, getWeightMod) {
        this.dynamicForce.x = 0;
        this.dynamicForce.y = 0;
        if (displayDepth < 0 || this.depth <= displayDepth) {
            if (this.grabbed) {
                this.moveTo(mousePoint);
            }
            else if (!this.pinned
                && running
                && performLayout) {
                this.dynamicForce.scaleAndAdd(this.position, -centeringGravity);
                for (const n2 of nodes) {
                    if (this !== n2
                        && (displayDepth < 0 || n2.depth <= displayDepth)) {
                        this.attractRepel(n2, attract, attractFunc, repel, repelFunc, getWeightMod);
                    }
                }
            }
        }
    }
    updatePosition(center, maxDepth) {
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
    moveTo(mousePoint) {
        this.position.copy(mousePoint)
            .sub(this.mouseOffset);
    }
    isVisible(maxDepth) {
        return !this.hidden
            && (maxDepth < 0
                || this.depth <= maxDepth);
    }
    canDrawArrow(maxDepth) {
        return this.isVisible(maxDepth)
            && !this.element.classList.contains("not-cycled");
    }
    attractRepel(n2, attract, attractFunc, repel, repelFunc, getWeightMod) {
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
    apply(t) {
        const len = this.dynamicForce.magnitude;
        if (len > 0) {
            // Restrict forces to a maximum magnitude
            const r = Math.min(t, len) / len;
            this.position.scaleAndAdd(this.dynamicForce, r);
        }
    }
}
//# sourceMappingURL=ForceDirectedNode.js.map