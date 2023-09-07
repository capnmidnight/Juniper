import type { TextImageOptions } from "@juniper-lib/graphics2d/TextImage";
import { TextImage } from "@juniper-lib/graphics2d/TextImage";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { RayTarget } from "../eventSystem/RayTarget";
import { TextMesh } from "./TextMesh";
export declare class TextMeshButton extends RayTarget {
    protected readonly env: BaseEnvironment;
    readonly image: TextImage;
    readonly enabledImage: TextMesh;
    readonly disabledImage: TextMesh;
    constructor(env: BaseEnvironment, name: string, value: string, textImageOptions?: Partial<TextImageOptions>);
    private createImage;
    get disabled(): boolean;
    set disabled(v: boolean);
}
//# sourceMappingURL=TextMeshButton.d.ts.map