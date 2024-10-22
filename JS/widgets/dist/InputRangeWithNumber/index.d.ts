import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
export declare class InputRangeWithNumberElement extends TypedHTMLElement<{
    "input": TypedEvent<"input">;
}> {
    #private;
    static observedAttributes: string[];
    private rangeInput;
    private numberInput;
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get min(): string;
    set min(v: string);
    get max(): string;
    set max(v: string);
    get step(): string;
    set step(v: string);
    get value(): string;
    set value(v: string);
    get valueAsNumber(): number;
    set valueAsNumber(v: number);
    get disabled(): boolean;
    set disabled(v: boolean);
    get enabled(): boolean;
    set enabled(v: boolean);
    static install(): import("@juniper-lib/dom").ElementFactory<InputRangeWithNumberElement>;
}
export declare function InputRangeWithNumber(...rest: ElementChild[]): InputRangeWithNumberElement;
//# sourceMappingURL=index.d.ts.map