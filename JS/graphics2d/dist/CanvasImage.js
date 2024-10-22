import { isDefined } from "@juniper-lib/util";
import { createUICanvas } from "@juniper-lib/dom";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
export class CanvasImage extends TypedEventTarget {
    constructor(width, height, options) {
        super();
        this._scale = 250;
        this._visible = true;
        this.wasVisible = null;
        this.redrawnEvt = new TypedEvent("redrawn");
        if (isDefined(options)) {
            if (isDefined(options.scale)) {
                this._scale = options.scale;
            }
        }
        this._canvas = createUICanvas(width, height);
        this._g = this.canvas.getContext("2d");
    }
    fillRect(color, x, y, width, height, margin) {
        this.g.fillStyle = color;
        this.g.fillRect(x + margin, y + margin, width - 2 * margin, height - 2 * margin);
    }
    drawText(text, x, y, align) {
        this.g.textAlign = align;
        this.g.strokeText(text, x, y);
        this.g.fillText(text, x, y);
    }
    redraw() {
        if ((this.visible || this.wasVisible) && this.onRedraw()) {
            this.wasVisible = this.visible;
            this.dispatchEvent(this.redrawnEvt);
        }
    }
    onClear() {
        this.g.clearRect(0, 0, this.canvas.width, this.canvas.height);
    }
    clear() {
        this.onClear();
        this.dispatchEvent(this.redrawnEvt);
    }
    get canvas() {
        return this._canvas;
    }
    get g() {
        return this._g;
    }
    get imageWidth() {
        return this.canvas.width;
    }
    get imageHeight() {
        return this.canvas.height;
    }
    get aspectRatio() {
        return this.imageWidth / this.imageHeight;
    }
    get width() {
        return this.imageWidth / this.scale;
    }
    get height() {
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
}
//# sourceMappingURL=CanvasImage.js.map