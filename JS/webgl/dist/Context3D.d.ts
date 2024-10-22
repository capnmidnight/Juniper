import type { CanvasTypes } from "@juniper-lib/dom/dist/canvas";
import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
export declare class ResizeEvent extends Event {
    width: number;
    height: number;
    constructor(width: number, height: number);
}
type Context3DEvents = {
    resize: ResizeEvent;
};
export declare class Context3D extends TypedEventTarget<Context3DEvents> {
    private _gl;
    /**
     * Creates a wrapper around a WebGLRenderingContext to combine some
     * application lifecycle events that we always want to perform.
     * @param canvas - the canvas that we are drawing to.
     * @param [opts] - rendering context options.
     */
    constructor(canvas: CanvasTypes, opts: WebGLContextAttributes);
    get gl(): WebGL2RenderingContext;
    resize(width: number, height: number): void;
    get width(): number;
    get height(): number;
}
export {};
//# sourceMappingURL=Context3D.d.ts.map