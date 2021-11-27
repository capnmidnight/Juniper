import { ElementChild, ErsatzElement } from "./tags";
export declare class InputRangeWithNumberElement implements ErsatzElement {
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
}
export declare function InputRangeWithNumber(...rest: ElementChild[]): InputRangeWithNumberElement;
