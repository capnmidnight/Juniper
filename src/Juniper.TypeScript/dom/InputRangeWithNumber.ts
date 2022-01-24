import { TypedEvent, TypedEventBase } from "../tslib";
import { className } from "./attrs";
import { columnGap, display, gridAutoFlow, gridTemplateColumns, rule } from "./css";
import { Div, ElementChild, ErsatzElement, InputNumber, InputRange, Style } from "./tags";

Style(
    rule(".input-range-with-number",
        display("grid"),
        gridAutoFlow("column"),
        columnGap("5px"),
        gridTemplateColumns("1fr auto"))
);

export class InputRangeWithNumberElement
    extends TypedEventBase<{
        "input": TypedEvent<"input">;
    }>
    implements ErsatzElement {
    public readonly element: HTMLElement;
    private rangeInput: HTMLInputElement;
    private numberInput: HTMLInputElement;

    constructor(...rest: ElementChild[]) {
        super();

        this.element = Div(
            className("input-range-with-number"),
            this.rangeInput = InputRange(...rest),
            this.numberInput = InputNumber());

        this.numberInput.min = this.rangeInput.min;
        this.numberInput.max = this.rangeInput.max;
        this.numberInput.step = this.rangeInput.step;
        this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber;
        this.numberInput.disabled = this.rangeInput.disabled;
        this.numberInput.placeholder = this.rangeInput.placeholder;

        this.numberInput.addEventListener("input", () => {
            this.rangeInput.valueAsNumber = this.numberInput.valueAsNumber;
            this.rangeInput.dispatchEvent(new Event("input"));
        });

        this.rangeInput.addEventListener("input", () => {
            this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber;
            this.dispatchEvent(new TypedEvent("input"));
        });
    }

    get value(): string {
        return this.rangeInput.value;
    }

    set value(v: string) {
        this.rangeInput.value
            = this.numberInput.value
            = v;
    }

    get valueAsNumber(): number {
        return this.rangeInput.valueAsNumber;
    }

    set valueAsNumber(v: number) {
        this.rangeInput.valueAsNumber = this.numberInput.valueAsNumber = v;
    }

    get disabled(): boolean {
        return this.rangeInput.disabled;
    }

    set disabled(v: boolean) {
        this.rangeInput.disabled
            = this.numberInput.disabled
            = v;
    }
}

export function InputRangeWithNumber(...rest: ElementChild[]) {
    return new InputRangeWithNumberElement(...rest);
}