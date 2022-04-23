import { TypedEventBase } from "@juniper/tslib";
import { className } from "./attrs";
import { display, gridAutoFlow, gridTemplateColumns, rule } from "./css";
import { Div, elementSwap, InputNumber, Style } from "./tags";

Style(
    rule(".input-range-with-text",
        display("grid"),
        gridAutoFlow("column"),
        gridTemplateColumns("1fr auto"))
);

interface InputRangeWithTextEvents {
    input: InputEvent;
}

const inputEvt = new InputEvent("input");

export class InputRangeWithText extends TypedEventBase<InputRangeWithTextEvents> {
    private numberInput: HTMLInputElement;

    constructor(private rangeInput: HTMLInputElement) {
        super();

        elementSwap(this.rangeInput, (placeholder) =>
            Div(
                className("input-range-with-text"),
                placeholder,
                this.numberInput = InputNumber()));

        this.numberInput.min = this.rangeInput.min;
        this.numberInput.max = this.rangeInput.max;
        this.numberInput.step = this.rangeInput.step;
        this.numberInput.value = this.rangeInput.value;
        this.numberInput.disabled = this.rangeInput.disabled;
        this.numberInput.placeholder = this.rangeInput.placeholder;

        this.rangeInput.addEventListener("input", () => {
            this.numberInput.value = this.rangeInput.value;
            this.dispatchEvent(inputEvt);
        });

        this.numberInput.addEventListener("input", () => {
            this.rangeInput.value = this.numberInput.value;
            this.dispatchEvent(inputEvt);
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
        this.rangeInput.valueAsNumber
            = this.numberInput.valueAsNumber
            = v;
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