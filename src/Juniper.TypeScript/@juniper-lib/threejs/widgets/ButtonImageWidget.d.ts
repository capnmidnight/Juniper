import { ButtonFactory } from "./ButtonFactory";
import { Widget } from "./widgets";
export declare class ButtonImageWidget extends Widget<HTMLButtonElement> {
    private mesh;
    constructor(buttons: ButtonFactory, setName: string, iconName: string);
    private load;
    get disabled(): boolean;
    set disabled(v: boolean);
    get visible(): boolean;
    set visible(v: boolean);
}
//# sourceMappingURL=ButtonImageWidget.d.ts.map