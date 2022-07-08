import { isHTMLCanvas } from "@juniper-lib/dom/canvas";
import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import type { CanvasImage } from "@juniper-lib/graphics2d/CanvasImage";
import { IWebXRLayerManager } from "../IWebXRLayerManager";
import { objectSetVisible } from "../objects";
import { Image2D } from "./Image2D";
import type { Widget } from "./widgets";

const redrawnEvt = { type: "redrawn" };

export class CanvasImageMesh<T extends CanvasImage>
    extends Image2D
    implements Widget {

    private _image: T = null;

    private readonly _onRedrawn: () => void;

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

    constructor(env: IWebXRLayerManager, name: string, image: T, materialOptions?: THREE.MeshBasicMaterialParameters) {
        super(env, name, false, materialOptions);
        this._onRedrawn = this.onRedrawn.bind(this);
        this.image = image;
    }

    protected onRedrawn() {
        this.updateTexture();
        this.dispatchEvent(redrawnEvt);
    }

    get image(): T {
        return this._image;
    }

    set image(v: T) {
        if (this.image) {
            this.image.removeEventListener("redrawn", this._onRedrawn);
        }

        this._image = v;

        if (this.image) {
            this.image.addEventListener("redrawn", this._onRedrawn);
            this.setTextureMap(this.image.canvas);
            this.onRedrawn();
        }
    }

    override get imageWidth() {
        return this.image.width;
    }

    override get imageHeight() {
        return this.image.height;
    }

    override copy(source: this, recursive = true): this {
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
