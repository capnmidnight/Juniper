import { ElementChild, HtmlProp, TypedHTMLElement } from "@juniper-lib/dom";
import { IDialog, StandardDialogEventsMap } from "./IDialog";
export declare function CancelButtonText(value: string): HtmlProp<"cancelButtonText", string, Node & Record<"cancelButtonText", string>>;
export declare function SaveButtonText(value: string): HtmlProp<"saveButtonText", string, Node & Record<"saveButtonText", string>>;
export declare class StandardDialogElement<InputT = void, OutputT = InputT> extends TypedHTMLElement<StandardDialogEventsMap<InputT, OutputT>> implements IDialog<InputT, OutputT> {
    #private;
    static observedAttributes: string[];
    static import<InputT = void, OutputT = InputT>(path: string): Promise<StandardDialogElement<InputT, OutputT>>;
    get body(): HTMLElement;
    get form(): HTMLFormElement;
    get cancelButtonText(): string;
    set cancelButtonText(v: string);
    get saveButtonText(): string;
    set saveButtonText(v: string);
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get cancelable(): boolean;
    set cancelable(v: boolean);
    get title(): string;
    set title(v: string);
    get errorMessage(): string;
    set errorMessage(msg: string);
    get open(): boolean;
    set open(v: boolean);
    show(value: InputT): Promise<OutputT>;
    showModal(value: InputT): Promise<OutputT>;
    close(): void;
    cancel(): void;
    confirm(): void;
    confirmed(): Promise<boolean>;
    cancelled(): Promise<boolean>;
    static install(): import("@juniper-lib/dom").ElementFactory<StandardDialogElement<void, void>>;
}
export declare function StandardDialog<InputT, OutputT = InputT>(...rest: ElementChild<StandardDialogElement<InputT, OutputT>>[]): StandardDialogElement<InputT, OutputT>;
//# sourceMappingURL=StandardDialogElement.d.ts.map