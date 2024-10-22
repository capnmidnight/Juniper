import { TextImage, TextImageOptions } from "@juniper-lib/graphics2d";
import { MeshBasicMaterialParameters } from "three";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { CanvasImageMesh } from "./CanvasImageMesh";
import { WebXRLayerType } from "./Image2D";
export declare class TextMesh extends CanvasImageMesh<TextImage> {
    constructor(env: BaseEnvironment, name: string, webXRLayerType: WebXRLayerType, textOptions: TextImage | Partial<TextImageOptions>, materialOptions?: MeshBasicMaterialParameters);
    protected onRedrawn(): void;
}
//# sourceMappingURL=TextMesh.d.ts.map