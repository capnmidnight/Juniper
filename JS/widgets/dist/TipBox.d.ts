import { TypedHTMLElement } from "@juniper-lib/dom";
export declare class TipBoxElement extends TypedHTMLElement {
    connectedCallback(): void;
    static install(): import("@juniper-lib/dom").ElementFactory<TipBoxElement>;
}
export declare function TipBox(tipBoxID: string, ...tips: string[]): TipBoxElement;
//# sourceMappingURL=TipBox.d.ts.map