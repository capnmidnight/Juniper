import { isHTMLCanvas } from "@juniper-lib/dom/canvas";
import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import type { CanvasImage } from "@juniper-lib/graphics2d/CanvasImage";
import { MeshBasicMaterialParameters } from "three";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { objectSetVisible } from "../objects";
import { Image2D, WebXRLayerType } from "./Image2D";
import type { IWidget } from "./widgets";

const redrawnEvt = { type: "redrawn" };

export class CanvasImageMesh<T extends CanvasImage>
    extends Image2D
    implements IWidget {

    private _image: T = null;

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

    constructor(env: BaseEnvironment, name: string, webXRLayerType: WebXRLayerType, image: T, materialOptions?: MeshBasicMaterialParameters) {
        super(env, name, webXRLayerType, materialOptions);
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
            this.image.removeScope(this);
        }

        this._image = v;

        if (this.image) {
            this.image.addScopedEventListener(this, "redrawn", () => this.onRedrawn());
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
