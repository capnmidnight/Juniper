import { DialogBox } from "@juniper-lib/widgets/dist/DialogBox";
import type { Environment } from "../environment/Environment";
import { TextMesh } from "./TextMesh";
import type { IWidget } from "./widgets";
export declare class ConfirmationDialog extends DialogBox implements IWidget {
    private readonly env;
    readonly object: import("three").Object3D<import("three").Event>;
    private readonly root;
    readonly mesh: TextMesh;
    private readonly confirmButton3D;
    private readonly cancelButton3D;
    private readonly animator;
    private a;
    private b;
    private readonly onTick;
    constructor(env: Environment, fontFamily: string);
    get name(): string;
    get visible(): boolean;
    set visible(visible: boolean);
    update(dt: number): void;
    private showHide;
    private get use3D();
    protected onShowing(): Promise<void>;
    onShown(): void;
    protected onClosing(): Promise<void>;
    prompt(title: string, message: string): Promise<boolean>;
}
//# sourceMappingURL=ConfirmationDialog.d.ts.map