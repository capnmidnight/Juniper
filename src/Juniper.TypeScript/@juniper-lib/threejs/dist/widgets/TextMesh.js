import { TextImage } from "@juniper-lib/graphics2d/dist/TextImage";
import { CanvasImageMesh } from "./CanvasImageMesh";
export class TextMesh extends CanvasImageMesh {
    constructor(env, name, webXRLayerType, textOptions, materialOptions) {
        let image;
        if (textOptions instanceof TextImage) {
            image = textOptions;
        }
        else {
            image = new TextImage(textOptions);
        }
        super(env, name, webXRLayerType, image, materialOptions);
    }
    onRedrawn() {
        this.objectHeight = this.imageHeight;
        super.onRedrawn();
    }
}
//# sourceMappingURL=TextMesh.js.map