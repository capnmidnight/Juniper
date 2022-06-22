import { TextImage, TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { IWebXRLayerManager } from "./IWebXRLayerManager";
import { CanvasImageMesh } from "./widgets/CanvasImageMesh";


export class TextMesh extends CanvasImageMesh<TextImage> {
    constructor(env: IWebXRLayerManager, name: string, textOptions: TextImage | Partial<TextImageOptions>, materialOptions?: THREE.MeshBasicMaterialParameters) {
        super(env, name, materialOptions);

        if (textOptions instanceof TextImage) {
            this.image = textOptions;
        }
        else {
            this.image = new TextImage(textOptions);
        }
    }

    protected override onRedrawn() {
        this.objectHeight = this.imageHeight;
        super.onRedrawn();
    }
}