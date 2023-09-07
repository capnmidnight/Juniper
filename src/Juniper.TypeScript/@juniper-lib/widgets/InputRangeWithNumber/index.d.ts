import { ElementChild, ErsatzElement } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import "./style.css";
export declare class InputRangeWithNumber extends TypedEventTarget<{
    "input": TypedEvent<"input">;
}> implements ErsatzElement {
    readonly element: HTMLElement;
    private rangeInput;
    private numberInput;
    constructor(...rest: ElementChild[]);
    get value(): string;
    set value(v: string);
    get valueAsNumber(): number;
    set valueAsNumber(v: number);
    get disabled(): boolean;
    set disabled(v: boolean);
    get enabled(): boolean;
    set enabled(v: boolean);
}
//# sourceMappingURL=index.d.ts.map