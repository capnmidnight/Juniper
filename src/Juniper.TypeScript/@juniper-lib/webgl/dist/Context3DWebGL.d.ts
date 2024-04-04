import type { CanvasTypes } from "@juniper-lib/dom/dist/canvas";
import { Context3D } from "@juniper-lib/three-dee/dist/Context3D";
export declare class Context3DWebGL extends Context3D {
    #private;
    get gl(): WebGL2RenderingContext;
    /**
     * Creates a wrapper around a WebGLRenderingContext to combine some
     * application lifecycle events that we always want to perform.
     * @param canvas - the canvas that we are drawing to.
     * @param [opts] - rendering context options.
     */
    constructor(canvas: CanvasTypes, opts: WebGLContextAttributes);
    resize(width: number, height: number): void;
}
//# sourceMappingURL=Context3DWebGL.d.ts.map