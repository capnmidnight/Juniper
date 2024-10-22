import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
import { CancelledEvent } from "./OnCancelled";
import { DeleteEvent } from "./OnDelete";
import { OpenedEvent } from "./OnOpened";
import { UpdatedEvent } from "./OnUpdated";
type HiddenEditorEventMap = {
    "opened": OpenedEvent<HiddenEditorElement>;
    "updated": UpdatedEvent<HiddenEditorElement>;
    "cancelled": CancelledEvent<HiddenEditorElement>;
    "delete": DeleteEvent<HiddenEditorElement>;
};
export declare class HiddenEditorElement extends TypedHTMLElement<HiddenEditorEventMap> {
    #private;
    static observedAttributes: string[];
    constructor();
    get open(): boolean;
    set open(v: boolean);
    get disabled(): boolean;
    set disabled(value: boolean);
    get deletable(): boolean;
    set deletable(value: boolean);
    get cancelable(): boolean;
    set cancelable(value: boolean);
    get readOnly(): boolean;
    set readOnly(value: boolean);
    attributeChangedCallback(_name: string, oldValue: string, newValue: string): void;
    connectedCallback(): void;
    static install(): import("@juniper-lib/dom").ElementFactory<HiddenEditorElement>;
}
export declare function HiddenEditor(...rest: ElementChild<HiddenEditorElement>[]): HiddenEditorElement;
export {};
//# sourceMappingURL=HiddenEditorElement.d.ts.map