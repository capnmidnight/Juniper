import { ElementChild, TypedHTMLInputElement } from "@juniper-lib/dom";
export declare class InputCurrencyElement extends TypedHTMLInputElement {
    #private;
    get valueAsNumber(): number;
    set valueAsNumber(v: number);
    get type(): string;
    set type(_v: string);
    connectedCallback(): void;
    static install(): import("@juniper-lib/dom").ElementFactory<InputCurrencyElement>;
}
export declare function InputCurrency(...rest: ElementChild<InputCurrencyElement>[]): InputCurrencyElement;
//# sourceMappingURL=InputCurrencyElement.d.ts.map