import { TypedEventBase } from "juniper-tslib";
interface InputRangeWithTextEvents {
    input: InputEvent;
}
export declare class InputRangeWithText extends TypedEventBase<InputRangeWithTextEvents> {
    private rangeInput;
    private numberInput;
    constructor(rangeInput: HTMLInputElement);
    get value(): string;
    set value(v: string);
    get valueAsNumber(): number;
    set valueAsNumber(v: number);
    get disabled(): boolean;
    set disabled(v: boolean);
}
export {};
