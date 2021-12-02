import { TypedEvent, TypedEventBase } from "juniper-tslib";
import type { CanvasTypes, Context2D } from "./canvas";
import type { ErsatzElement } from "./tags";
interface CanvasImageEvents {
    redrawn: TypedEvent<"redrawn">;
}
export interface ICanvasImage extends TypedEventBase<CanvasImageEvents> {
    canvas: CanvasTypes;
    scale: number;
    width: number;
    height: number;
    aspectRatio: number;
}
export interface CanvasImageOptions {
    scale: number;
}
export declare function isCanvasImage(obj: any): obj is ICanvasImage;
export declare abstract class CanvasImage<T> extends TypedEventBase<CanvasImageEvents & T> implements ICanvasImage, ErsatzElement {
    private _canvas;
    private _scale;
    private _g;
    private redrawnEvt;
    readonly element: HTMLCanvasElement;
    constructor(width: number, height: number, options?: Partial<CanvasImageOptions>);
    protected fillRect(color: string, x: number, y: number, width: number, height: number, margin: number): void;
    protected drawText(text: string, x: number, y: number, align: CanvasTextAlign): void;
    protected redraw(): void;
    get canvas(): CanvasTypes;
    protected get g(): Context2D;
    get imageWidth(): number;
    get imageHeight(): number;
    get aspectRatio(): number;
    get width(): number;
    get height(): number;
    get scale(): number;
    set scale(v: number);
    protected abstract onRedraw(): boolean;
}
export {};
