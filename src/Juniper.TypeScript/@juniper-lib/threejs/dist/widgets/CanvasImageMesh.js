import { isHTMLCanvas } from "@juniper-lib/dom/dist/canvas";
import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/dist/tags";
import { objectSetVisible } from "../objects";
import { Image2D } from "./Image2D";
const redrawnEvt = { type: "redrawn" };
export class CanvasImageMesh extends Image2D {
    get object() {
        return this;
    }
    get element() {
        if (isHTMLCanvas(this.image.canvas)) {
            return this.image.canvas;
        }
        else {
            return null;
        }
    }
    get canvas() {
        return this._image.canvas;
    }
    constructor(env, name, webXRLayerType, image, materialOptions) {
        super(env, name, webXRLayerType, materialOptions);
        this._image = null;
        this.image = image;
    }
    onRedrawn() {
        this.updateTexture();
        this.dispatchEvent(redrawnEvt);
    }
    get image() {
        return this._image;
    }
    set image(v) {
        if (this.image) {
            this.image.removeScope(this);
        }
        this._image = v;
        if (this.image) {
            this.image.addScopedEventListener(this, "redrawn", () => this.onRedrawn());
            this.setTextureMap(this.image.canvas);
            this.onRedrawn();
        }
    }
    get imageWidth() {
        return this.image.width;
    }
    get imageHeight() {
        return this.image.height;
    }
    copy(source, recursive = true) {
        super.copy(source, recursive);
        this.image = source.image;
        return this;
    }
    get isVisible() {
        return elementIsDisplayed(this);
    }
    set isVisible(v) {
        elementSetDisplay(this, v, "inline-block");
        objectSetVisible(this, v);
        objectSetVisible(this.mesh, v);
        this.image.visible = v;
    }
}
//# sourceMappingURL=CanvasImageMesh.js.map