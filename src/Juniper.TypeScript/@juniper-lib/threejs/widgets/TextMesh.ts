import { TextImage, TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { MeshBasicMaterialParameters } from "three";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { CanvasImageMesh } from "./CanvasImageMesh";


export class TextMesh extends CanvasImageMesh<TextImage> {
    constructor(env: BaseEnvironment, name: string, textOptions: TextImage | Partial<TextImageOptions>, materialOptions?: MeshBasicMaterialParameters) {
        let image: TextImage;
        if (textOptions instanceof TextImage) {
            image = textOptions;
        }
        else {
            image = new TextImage(textOptions);
        }

        super(env, name, "none", image, materialOptions);
    }

    protected override onRedrawn() {
        this.objectHeight = this.imageHeight;
        super.onRedrawn();
    }
}