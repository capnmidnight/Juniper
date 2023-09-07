import { ButtonFactory } from "./ButtonFactory";
import { MeshButton } from "./MeshButton";
import { Widget } from "./widgets";
export declare class ToggleButton extends Widget<HTMLButtonElement> {
    private readonly buttons;
    private readonly setName;
    private readonly activeName;
    private readonly inactiveName;
    private enterButton;
    private exitButton;
    private readonly btnImage;
    private _isAvailable;
    private _isEnabled;
    private _isActive;
    constructor(buttons: ButtonFactory, setName: string, activeName: string, inactiveName: string);
    private load;
    get mesh(): MeshButton;
    get available(): boolean;
    set available(v: boolean);
    get active(): boolean;
    set active(v: boolean);
    get enabled(): boolean;
    set enabled(v: boolean);
    get visible(): boolean;
    set visible(v: boolean);
    private refreshState;
}
//# sourceMappingURL=ToggleButton.d.ts.map