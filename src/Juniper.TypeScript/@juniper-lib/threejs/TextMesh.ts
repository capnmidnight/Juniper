import { TextImage } from "@juniper-lib/graphics2d/TextImage";
import { IWebXRLayerManager } from "./IWebXRLayerManager";
import { CanvasImageMesh } from "./widgets/CanvasImageMesh";


export class TextMesh extends CanvasImageMesh<TextImage> {
    constructor(env: IWebXRLayerManager, name: string, materialOptions?: THREE.MeshBasicMaterialParameters) {
        super(env, name, materialOptions);
    }

    protected override onRedrawn() {
        this.objectHeight = this.imageHeight;
        super.onRedrawn();
    }
}