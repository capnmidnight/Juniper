import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
type CanvasImageEvents = {
    redrawn: TypedEvent<"redrawn">;
};
export interface CanvasImageOptions {
    scale: number;
}
export declare abstract class CanvasImage extends TypedEventTarget<CanvasImageEvents> {
    private _canvas;
    private _scale;
    private _g;
    private _visible;
    private wasVisible;
    protected redrawnEvt: any;
    constructor(width: number, height: number, options?: Partial<CanvasImageOptions>);
    protected fillRect(color: string, x: number, y: number, width: number, height: number, margin: number): void;
    protected drawText(text: string, x: number, y: number, align: CanvasTextAlign): void;
    protected redraw(): void;
    protected onClear(): void;
    clear(): void;
    get canvas(): CanvasTypes;
    protected get g(): Context2D;
    get imageWidth(): number;
    get imageHeight(): number;
    get aspectRatio(): number;
    get width(): number;
    get height(): number;
    get scale(): number;
    set scale(v: number);
    get visible(): boolean;
    set visible(v: boolean);
    protected abstract onRedraw(): boolean;
}
export {};
//# sourceMappingURL=CanvasImage.d.ts.map