import { border, CanvasTypes, height, HtmlRender, isOffscreenCanvas, left, margin, padding, perc, position, top, touchAction, width } from "@juniper-lib/dom";
import { TypedEventTarget } from "@juniper-lib/events";

export class ResizeEvent extends Event {
    constructor(
        public width: number,
        public height: number) {
        super("resize");
        Object.seal(this);
    }
}

type Context3DEvents = {
    resize: ResizeEvent;
}

// Let's start with the context.
export abstract class Context3D extends TypedEventTarget<Context3DEvents> {

    #canvas: CanvasTypes = null;
    get canvas() { return this.#canvas; }

    
    /**
     * Creates a wrapper around a Canvas to combine some
     * application lifecycle events that we always want to perform.
     * @param canvas - the canvas that we are drawing to.
     */
    constructor(canvas: CanvasTypes) {
        super();
        
        this.#canvas = canvas;
        
        // If we have a canvas, get the graphics context
        if (canvas) {
            // If we're dealing with HTML canvases, then setup the auto-resizer.
            if (!isOffscreenCanvas(canvas)) {
                HtmlRender(canvas, 
                    position("fixed"),
                    left(0),
                    top(0),
                    width(perc(100)),
                    height(perc(100)),
                    margin(0),
                    padding(0),
                    border(0),
                    touchAction("none")
                );

                canvas.tabIndex = 1;

                const canv = canvas;
                const resize = () => {
                    this.resize(
                        canv.clientWidth * devicePixelRatio,
                        canv.clientHeight * devicePixelRatio
                    );
                };

                window.addEventListener("resize", resize);
                setTimeout(resize);
            }
        }
    }

    resize(width: number, height: number) {
        this.dispatchEvent(new ResizeEvent(width, height));
    }

    get width() {
        return this.canvas.width;
    }

    get height() {
        return this.canvas.height;
    }
}