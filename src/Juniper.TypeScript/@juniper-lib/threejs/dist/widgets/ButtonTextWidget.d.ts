import { TextImageOptions } from "@juniper-lib/graphics2d/dist/TextImage";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { TextMeshButton } from "./TextMeshButton";
import { Widget } from "./widgets";
export declare class ButtonTextWidget extends Widget<HTMLButtonElement> {
    protected readonly env: BaseEnvironment;
    readonly mesh: TextMeshButton;
    constructor(env: BaseEnvironment, name: string, text: string, textButtonStyle: Partial<TextImageOptions>);
    get visible(): boolean;
    set visible(v: boolean);
}
//# sourceMappingURL=ButtonTextWidget.d.ts.map