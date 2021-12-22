import type { TextImageOptions } from "juniper-2d/TextImage";
import { TextImage } from "juniper-2d/TextImage";
import { Image2DMesh } from "./Image2DMesh";

const redrawnEvt = { type: "redrawn" };

export class TextMesh extends Image2DMesh {
    private _textImage: TextImage = null;

    private _onRedrawn: () => void;

    constructor(name: string, materialOptions?: THREE.MeshBasicMaterialParameters) {
        super(name, materialOptions);

        this._onRedrawn = this.onRedrawn.bind(this);
    }

    private onRedrawn() {
        this.updateTexture();
        this.scale.set(this._textImage.width, this._textImage.height, 0.01);
        this.dispatchEvent(redrawnEvt);
    }


    get textImage() {
        return this._textImage;
    }

    set textImage(v) {
        if (v !== this.textImage) {
            if (this.textImage) {
                this.textImage.removeEventListener("redrawn", this._onRedrawn);
            }

            this._textImage = v;

            if (this.textImage) {
                this.textImage.addEventListener("redrawn", this._onRedrawn);
                this.setImage(this.textImage.canvas);
                this._onRedrawn();
            }
        }
    }

    createTextImage(textImageOptions?: Partial<TextImageOptions>) {
        this.textImage = new TextImage(textImageOptions);
    }

    get wrapWords() {
        return this._textImage.wrapWords;
    }

    set wrapWords(v) {
        this._textImage.wrapWords = v;
    }

    get minWidth() {
        return this._textImage.minWidth;
    }

    set minWidth(v) {
        this._textImage.minWidth = v;
    }

    get maxWidth() {
        return this._textImage.maxWidth;
    }

    set maxWidth(v) {
        this._textImage.maxWidth = v;
    }

    get minHeight() {
        return this._textImage.minHeight;
    }

    set minHeight(v) {
        this._textImage.minHeight = v;
    }

    get maxHeight() {
        return this._textImage.maxHeight;
    }

    set maxHeight(v) {
        this._textImage.maxHeight = v;
    }

    get textDirection() {
        return this._textImage.textDirection;
    }

    set textDirection(v) {
        this._textImage.textDirection = v;
    }

    get textScale() {
        return this._textImage.scale;
    }

    set textScale(v) {
        this._textImage.scale = v;
    }

    get textWidth() {
        return this._textImage.width;
    }

    get textHeight() {
        return this._textImage.height;
    }

    get textPadding() {
        return this._textImage.padding;
    }

    set textPadding(v) {
        this._textImage.padding = v;
    }

    get fontStyle() {
        return this._textImage.fontStyle;
    }

    set fontStyle(v) {
        this._textImage.fontStyle = v;
    }

    get fontVariant() {
        return this._textImage.fontVariant;
    }

    set fontVariant(v) {
        this._textImage.fontVariant = v;
    }

    get fontWeight() {
        return this._textImage.fontWeight;
    }

    set fontWeight(v) {
        this._textImage.fontWeight = v;
    }

    get fontSize() {
        return this._textImage.fontSize;
    }

    set fontSize(v) {
        this._textImage.fontSize = v;
    }

    get fontFamily() {
        return this._textImage.fontFamily;
    }

    set fontFamily(v) {
        this._textImage.fontFamily = v;
    }

    get textFillColor() {
        return this._textImage.textFillColor;
    }

    set textFillColor(v) {
        this._textImage.textFillColor = v;
    }

    get textStrokeColor() {
        return this._textImage.textStrokeColor;
    }

    set textStrokeColor(v) {
        this._textImage.textStrokeColor = v;
    }

    get textStrokeSize() {
        return this._textImage.textStrokeSize;
    }

    set textStrokeSize(v) {
        this._textImage.textStrokeSize = v;
    }

    get textBgColor() {
        return this._textImage.bgFillColor;
    }

    set textBgColor(v) {
        this._textImage.bgFillColor = v;
    }

    get value() {
        return this._textImage.value;
    }

    set value(v) {
        this._textImage.value = v;
    }
}