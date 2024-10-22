import { TextImage, TextImageOptions } from "@juniper-lib/graphics2d/dist/TextImage";
import { MeshBasicMaterialParameters } from "three";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { CanvasImageMesh } from "./CanvasImageMesh";
import { WebXRLayerType } from "./Image2D";


export class TextMesh extends CanvasImageMesh<TextImage> {
    constructor(env: BaseEnvironment, name: string, webXRLayerType: WebXRLayerType, textOptions: TextImage | Partial<TextImageOptions>, materialOptions?: MeshBasicMaterialParameters) {
        let image: TextImage;
        if (textOptions instanceof TextImage) {
            image = textOptions;
        }
        else {
            image = new TextImage(textOptions);
        }

        super(env, name, webXRLayerType, image, materialOptions);
    }

    protected override onRedrawn() {
        this.objectHeight = this.imageHeight;
        super.onRedrawn();
    }
}