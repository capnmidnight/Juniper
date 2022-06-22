import { TextImage, TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { IWebXRLayerManager } from "../IWebXRLayerManager";
import { CanvasImageMesh } from "./CanvasImageMesh";


export class TextMesh extends CanvasImageMesh<TextImage> {
    constructor(env: IWebXRLayerManager, name: string, textOptions: TextImage | Partial<TextImageOptions>, materialOptions?: THREE.MeshBasicMaterialParameters) {
        let image: TextImage;
        if (textOptions instanceof TextImage) {
            image = textOptions;
        }
        else {
            image = new TextImage(textOptions);
        }

        super(env, name, image, materialOptions);
    }

    protected override onRedrawn() {
        this.objectHeight = this.imageHeight;
        super.onRedrawn();
    }
}