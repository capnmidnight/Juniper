import type { CanvasImage } from "@juniper-lib/2d/CanvasImage";
import { isHTMLCanvas } from "@juniper-lib/dom/canvas";
import { elementIsDisplayed, elementSetDisplay } from "@juniper-lib/dom/tags";
import { Image2DMesh } from "../Image2DMesh";
import { IWebXRLayerManager } from "../IWebXRLayerManager";
import { objectSetVisible } from "../objects";
import type { Widget } from "./widgets";


export class CanvasImageMesh<T extends CanvasImage>
    extends Image2DMesh
    implements Widget {

    get object() {
        return this;
    }

    constructor(env: IWebXRLayerManager, name: string, public image: T) {
        super(env, name, false);

        if (this.mesh) {
            this.setImage(image);
        }
    }

    private setImage(image: T): void {
        this.mesh.setImage(image.canvas);
        this.mesh.objectHeight = 0.1;
        this.mesh.updateTexture();
        image.addEventListener("redrawn", () => this.mesh.updateTexture());
    }

    override copy(source: this, recursive = true): this {
        super.copy(source, recursive);
        this.setImage(source.image);
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
