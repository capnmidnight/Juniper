import { isHTMLCanvas } from "@juniper-lib/dom/canvas";
import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import type { CanvasImage } from "@juniper-lib/graphics2d/CanvasImage";
import { Image2D } from "../Image2D";
import { IWebXRLayerManager } from "../IWebXRLayerManager";
import { objectSetVisible } from "../objects";
import type { Widget } from "./widgets";


export class CanvasImageMesh<T extends CanvasImage>
    extends Image2D
    implements Widget {

    get object() {
        return this;
    }

    constructor(env: IWebXRLayerManager, name: string, public image: T) {
        super(env, name, false);

        if (this.mesh) {
            this.setCanvas(image);
        }
    }

    private setCanvas(image: T): void {
        this.setImage(image.canvas);
        this.objectHeight = 0.1;
        this.updateTexture();
        image.addEventListener("redrawn", () => this.updateTexture());
    }

    override copy(source: this, recursive = true): this {
        super.copy(source, recursive);
        this.setCanvas(source.image);
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
