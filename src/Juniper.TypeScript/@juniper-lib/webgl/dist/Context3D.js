import { isOffscreenCanvas, setContextSize } from "@juniper-lib/dom/dist/canvas";
import { border, height, left, margin, padding, perc, position, top, touchAction, width } from "@juniper-lib/dom/dist/css";
import { HtmlRender } from "@juniper-lib/dom/dist/tags";
import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
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
    /**
     * Creates a wrapper around a WebGLRenderingContext to combine some
     * application lifecycle events that we always want to perform.
     * @param canvas - the canvas that we are drawing to.
     * @param [opts] - rendering context options.
     */
    constructor(canvas, opts) {
        super();
        this._gl = null;
        // If we have a canvas, get the graphics context
        if (this.gl === null && canvas !== null) {
            if (isNullOrUndefined(opts)) {
                opts = {};
            }
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
            opts.xrCompatible = true;
            const gl = this._gl = canvas.getContext("webgl2", opts);
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
            throw new Error("Could not create context: could not discover WebGLRenderingContext from spec");
        }
    }
    get gl() {
        return this._gl;
    }
    resize(width, height) {
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
//# sourceMappingURL=Context3D.js.map