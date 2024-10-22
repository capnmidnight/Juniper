import { ElementChild, HtmlProp } from "@juniper-lib/dom";
import { BaseDialogElement } from "@juniper-lib/widgets";
import { BaseEnvironment } from "../environment";
import { TextMesh } from "./TextMesh";
import type { IWidget } from "./widgets";
export declare function EnvironmentAttr(env: BaseEnvironment): HtmlProp<"env", BaseEnvironment<unknown>, Node & Record<"env", BaseEnvironment<unknown>>>;
export declare class ConfirmationDialogElement extends BaseDialogElement<void, boolean> implements IWidget {
    #private;
    get content(): this;
    readonly content3d: import("three").Object3D<import("three").Event>;
    private readonly root;
    readonly mesh: TextMesh;
    private readonly confirmButton3D;
    private readonly cancelButton3D;
    private readonly animator;
    private a;
    private b;
    private readonly onTick;
    get env(): BaseEnvironment<unknown>;
    set env(v: BaseEnvironment<unknown>);
    constructor(fontFamily: string);
    get name(): string;
    get visible(): boolean;
    set visible(visible: boolean);
    update(dt: number): void;
    private showHide;
    private get use3D();
    prompt(title: string, message: string): Promise<boolean>;
    static install(): import("@juniper-lib/dom").ElementFactory<ConfirmationDialogElement>;
}
export declare function ConfirmationDialog(...rest: ElementChild<ConfirmationDialogElement>[]): ConfirmationDialogElement;
//# sourceMappingURL=ConfirmationDialog.d.ts.map