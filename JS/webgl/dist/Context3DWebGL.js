import { setContextSize } from "@juniper-lib/dom/dist/canvas";
import { Context3D } from "@juniper-lib/three-dee/dist/Context3D";
import { isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
// Let's start with the context.
export class Context3DWebGL extends Context3D {
    #gl = null;
    get gl() { return this.#gl; }
    /**
     * Creates a wrapper around a WebGLRenderingContext to combine some
     * application lifecycle events that we always want to perform.
     * @param canvas - the canvas that we are drawing to.
     * @param [opts] - rendering context options.
     */
    constructor(canvas, opts) {
        super(canvas);
        if (this.canvas) {
            if (isNullOrUndefined(opts)) {
                opts = {};
            }
            opts.xrCompatible = true;
            const gl = this.#gl = canvas.getContext("webgl2", opts);
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
        if (!this.gl) {
            throw new Error("Could not create context: could not discover WebGLRenderingContext from spec");
        }
    }
    resize(width, height) {
        setContextSize(this.gl, width, height);
        super.resize(width, height);
    }
}
//# sourceMappingURL=Context3DWebGL.js.map