import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
import { IDialog, StandardDialogEventsMap } from "./IDialog";
import { StandardDialogElement } from './StandardDialogElement';
export declare abstract class BaseDialogElement<InputT = void, OutputT = InputT> extends TypedHTMLElement<StandardDialogEventsMap<InputT, OutputT>> implements IDialog<InputT, OutputT> {
    #private;
    static observedAttributes: string[];
    protected dialog: StandardDialogElement<InputT, OutputT>;
    constructor(title: HTMLElement, body: HTMLElement, ...rest: ElementChild<StandardDialogElement<InputT, OutputT>>[]);
    protected Q<T extends Element>(selectors: string): T;
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get body(): HTMLElement;
    get form(): HTMLFormElement;
    get cancelButtonText(): string;
    set cancelButtonText(v: string);
    get saveButtonText(): string;
    set saveButtonText(v: string);
    get errorMessage(): string;
    set errorMessage(v: string);
    get open(): boolean;
    set open(v: boolean);
    get cancelable(): boolean;
    set cancelable(v: boolean);
    get title(): string;
    set title(v: string);
    show(value: InputT): Promise<OutputT>;
    showModal(value: InputT): Promise<OutputT>;
    close(): void;
    cancel(): void;
    confirm(): void;
    confirmed(): Promise<boolean>;
    cancelled(): Promise<boolean>;
}
//# sourceMappingURL=BaseDialogElement.d.ts.map