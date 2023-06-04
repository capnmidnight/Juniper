import type { CanvasTypes, Context2D } from "@juniper-lib/dom/canvas";
import { createUICanvas, isHTMLCanvas } from "@juniper-lib/dom/canvas";
import type { ErsatzElement } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventBase } from "@juniper-lib/events/EventBase";
import { isDefined } from "@juniper-lib/tslib/typeChecks";

interface CanvasImageEvents {
    redrawn: TypedEvent<"redrawn">;
}

export interface CanvasImageOptions {
    scale: number;
}

export abstract class CanvasImage
    extends TypedEventBase<CanvasImageEvents>
    implements ErsatzElement {

    private _canvas: CanvasTypes;
    private _scale = 250;
    private _g: Context2D;
    private _visible = true;
    private wasVisible: boolean = null;

    protected redrawnEvt = new TypedEvent("redrawn");
    readonly element: HTMLCanvasElement = null;

    constructor(width: number, height: number, options?: Partial<CanvasImageOptions>) {
        super();

        if (isDefined(options)) {

            if (isDefined(options.scale)) {
                this._scale = options.scale;
            }

        }

        this._canvas = createUICanvas(width, height);
        this._g = this.canvas.getContext("2d") as Context2D;

        if (isHTMLCanvas(this._canvas)) {
            this.element = this._canvas;
        }
    }

    protected fillRect(color: string, x: number, y: number, width: number, height: number, margin: number) {
        this.g.fillStyle = color;
        this.g.fillRect(x + margin, y + margin, width - 2 * margin, height - 2 * margin);
    }

    protected drawText(text: string, x: number, y: number, align: CanvasTextAlign) {
        this.g.textAlign = align;
        this.g.strokeText(text, x, y);
        this.g.fillText(text, x, y);
    }

    protected redraw(): void {
        if ((this.visible || this.wasVisible) && this.onRedraw()) {
            this.wasVisible = this.visible;
            this.dispatchEvent(this.redrawnEvt);
        }
    }

    protected onClear(): void {
        this.g.clearRect(0, 0, this.canvas.width, this.canvas.height);
    }


    clear() {
        this.onClear();
        this.dispatchEvent(this.redrawnEvt);
    }

    get canvas() {
        return this._canvas;
    }

    protected get g() {
        return this._g;
    }

    get imageWidth(): number {
        return this.canvas.width;
    }

    get imageHeight(): number {
        return this.canvas.height;
    }

    get aspectRatio(): number {
        return this.imageWidth / this.imageHeight;
    }

    get width(): number {
        return this.imageWidth / this.scale;
    }

    get height(): number {
        return this.imageHeight / this.scale;
    }

    get scale() {
        return this._scale;
    }

    set scale(v) {
        if (this.scale !== v) {
            this._scale = v;
            this.redraw();
        }
    }

    get visible() {
        return this._visible;
    }

    set visible(v) {
        if (this.visible !== v) {
            this.wasVisible = this._visible;
            this._visible = v;
            this.redraw();
        }
    }

    protected abstract onRedraw(): boolean;
}
