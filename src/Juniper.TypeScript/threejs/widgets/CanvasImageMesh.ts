import { isHTMLCanvas } from "juniper-dom/canvas";
import type { ICanvasImage } from "juniper-dom/CanvasImage";
import { elementIsDisplayed, elementSetDisplay } from "juniper-dom/tags";
import { Image2DMesh } from "../Image2DMesh";
import { obj, objectSetVisible } from "../objects";
import type { Widget } from "./widgets";


export class CanvasImageMesh<T extends ICanvasImage> implements Widget {
    readonly object: THREE.Object3D;
    readonly mesh: Image2DMesh;

    constructor(name: string, public readonly image: T) {

        this.object = obj(name,
            this.mesh = new Image2DMesh(`${name}-image`));
        this.mesh.setImage(image.canvas);
        this.mesh.objectHeight = 0.1;
        this.mesh.updateTexture();

        image.addEventListener("redrawn", () => this.mesh.updateTexture());
    }

    get element() {
        if (isHTMLCanvas(this.image.canvas)) {
            return this.image.canvas;
        }
        else {
            return null;
        }
    }

    get name() {
        return this.object.name;
    }

    get visible() {
        return elementIsDisplayed(this);
    }

    set visible(v) {
        elementSetDisplay(this, v, "inline-block");
        objectSetVisible(this, v);
        objectSetVisible(this.mesh, v);
    }
}
