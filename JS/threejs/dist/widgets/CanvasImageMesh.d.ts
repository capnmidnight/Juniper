import type { CanvasImage } from "@juniper-lib/graphics2d/dist/CanvasImage";
import { MeshBasicMaterialParameters } from "three";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { Image2D, WebXRLayerType } from "./Image2D";
import type { IWidget } from "./widgets";
export declare class CanvasImageMesh<T extends CanvasImage> extends Image2D implements IWidget {
    private _image;
    get object(): this;
    get element(): HTMLCanvasElement;
    get canvas(): import("@juniper-lib/dom/dist/canvas").CanvasTypes;
    constructor(env: BaseEnvironment, name: string, webXRLayerType: WebXRLayerType, image: T, materialOptions?: MeshBasicMaterialParameters);
    protected onRedrawn(): void;
    get image(): T;
    set image(v: T);
    get imageWidth(): number;
    get imageHeight(): number;
    copy(source: this, recursive?: boolean): this;
    get isVisible(): boolean;
    set isVisible(v: boolean);
}
//# sourceMappingURL=CanvasImageMesh.d.ts.map