import { isNullOrUndefined } from "@juniper-lib/util";
import { CanvasTypes, setContextSize } from "@juniper-lib/dom";
import { Context3D } from "@juniper-lib/three-dee"

// Let's start with the context.
export class Context3DWebGL extends Context3D {

    #gl: WebGL2RenderingContext = null;
    get gl() { return this.#gl; }

    /**
     * Creates a wrapper around a WebGLRenderingContext to combine some
     * application lifecycle events that we always want to perform.
     * @param canvas - the canvas that we are drawing to.
     * @param [opts] - rendering context options.
     */
    constructor(canvas: CanvasTypes, opts: WebGLContextAttributes) {
        super(canvas);

        if (this.canvas) {
            if (isNullOrUndefined(opts)) {
                opts = {};
            }

            (opts as any).xrCompatible = true;

            const gl = this.#gl = canvas.getContext("webgl2", opts) as WebGL2RenderingContext;
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
            throw new Error(
                "Could not create context: could not discover WebGLRenderingContext from spec"
            );
        }
    }

    override resize(width: number, height: number) {
        setContextSize(this.gl, width, height);
        super.resize(width, height);
    }
}