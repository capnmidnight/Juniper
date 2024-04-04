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
export declare abstract class Context3D extends TypedEventTarget<Context3DEvents> {
    #private;
    get canvas(): CanvasTypes;
    /**
     * Creates a wrapper around a Canvas to combine some
     * application lifecycle events that we always want to perform.
     * @param canvas - the canvas that we are drawing to.
     */
    constructor(canvas: CanvasTypes);
    resize(width: number, height: number): void;
    get width(): number;
    get height(): number;
}
export {};
//# sourceMappingURL=Context3D.d.ts.map