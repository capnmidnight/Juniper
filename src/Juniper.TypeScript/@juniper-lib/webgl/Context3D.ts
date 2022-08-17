import type { CanvasTypes } from "@juniper-lib/dom/canvas";
import { isOffscreenCanvas, setContextSize } from "@juniper-lib/dom/canvas";
import { border, height, left, margin, padding, position, styles, top, touchAction, width } from "@juniper-lib/dom/css";
import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";

export class ResizeEvent extends Event {
    constructor(
        public width: number,
        public height: number) {
        super("resize");
        Object.seal(this);
    }
}

interface Context3DEvents {
    resize: ResizeEvent;
}

// Let's start with the context.
export class Context3D extends TypedEventBase<Context3DEvents> {
    private _gl: WebGL2RenderingContext = null;

    /**
     * Creates a wrapper around a WebGLRenderingContext to combine some
     * application lifecycle events that we always want to perform.
     * @param canvas - the canvas that we are drawing to.
     * @param [opts] - rendering context options.
     */
    constructor(canvas: CanvasTypes, opts: WebGLContextAttributes) {
        super();
        
        // If we have a canvas, get the graphics context
        if (this.gl === null && canvas !== null) {
            if (isNullOrUndefined(opts)) {
                opts = {};
            }

            // If we're dealing with HTML canvases, then setup the auto-resizer.
            if (!isOffscreenCanvas(canvas)) {

                styles(
                    position("fixed"),
                    left(0),
                    top(0),
                    width("100%"),
                    height("100%"),
                    margin(0),
                    padding(0),
                    border(0),
                    touchAction("none")
                ).applyToElement(canvas);

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

            (opts as any).xrCompatible = true;

            const gl = this._gl = canvas.getContext("webgl2", opts) as WebGL2RenderingContext;
            gl.clearColor(0, 0, 0, 1);
            gl.clearDepth(1);

            gl.enable(gl.CULL_FACE);
            gl.cullFace(gl.BACK);

            gl.enable(gl.DEPTH_TEST);
            gl.depthFunc(gl.LEQUAL);

            gl.enable(gl.BLEND);
            gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
            gl.blendEquation(gl.FUNC_ADD);
        }

        // Finally, validate the graphics context
        if (this.gl === null) {
            throw new Error(
                "Could not create context: could not discover WebGLRenderingContext from spec"
            );
        }
    }

    get gl() {
        return this._gl;
    }

    resize(width: number, height: number) {
        setContextSize(this.gl, width, height);
        this.dispatchEvent(new ResizeEvent(width, height));
    }

    get width() {
        return this.gl.canvas.width;
    }

    get height() {
        return this.gl.canvas.height;
    }
}