import { isOffscreenCanvas } from "@juniper-lib/dom/dist/canvas";
import { border, height, left, margin, padding, perc, position, top, touchAction, width } from "@juniper-lib/dom/dist/css";
import { HtmlRender } from "@juniper-lib/dom/dist/tags";
import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
export class ResizeEvent extends Event {
    constructor(width, height) {
        super("resize");
        this.width = width;
        this.height = height;
        Object.seal(this);
    }
}
// Let's start with the context.
export class Context3D extends TypedEventTarget {
    #canvas = null;
    get canvas() { return this.#canvas; }
    /**
     * Creates a wrapper around a Canvas to combine some
     * application lifecycle events that we always want to perform.
     * @param canvas - the canvas that we are drawing to.
     */
    constructor(canvas) {
        super();
        this.#canvas = canvas;
        // If we have a canvas, get the graphics context
        if (canvas) {
            // If we're dealing with HTML canvases, then setup the auto-resizer.
            if (!isOffscreenCanvas(canvas)) {
                HtmlRender(canvas, position("fixed"), left(0), top(0), width(perc(100)), height(perc(100)), margin(0), padding(0), border(0), touchAction("none"));
                canvas.tabIndex = 1;
                const canv = canvas;
                const resize = () => {
                    this.resize(canv.clientWidth * devicePixelRatio, canv.clientHeight * devicePixelRatio);
                };
                window.addEventListener("resize", resize);
                setTimeout(resize);
            }
        }
    }
    resize(width, height) {
        this.dispatchEvent(new ResizeEvent(width, height));
    }
    get width() {
        return this.canvas.width;
    }
    get height() {
        return this.canvas.height;
    }
}
//# sourceMappingURL=Context3D.js.map