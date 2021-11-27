import { className } from "./attrs";
import { columnGap, display, gridAutoFlow, gridTemplateColumns, rule } from "./css";
import { Div, InputNumber, InputRange, Style } from "./tags";
Style(rule(".input-range-with-number", display("grid"), gridAutoFlow("column"), columnGap("5px"), gridTemplateColumns("1fr auto")));
export class InputRangeWithNumberElement {
    element;
    rangeInput;
    numberInput;
    constructor(...rest) {
        this.element = Div(className("input-range-with-number"), this.rangeInput = InputRange(...rest), this.numberInput = InputNumber());
        this.numberInput.min = this.rangeInput.min;
        this.numberInput.max = this.rangeInput.max;
        this.numberInput.step = this.rangeInput.step;
        this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber;
        this.numberInput.disabled = this.rangeInput.disabled;
        this.numberInput.placeholder = this.rangeInput.placeholder;
        this.rangeInput.addEventListener("input", () => this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber);
        this.numberInput.addEventListener("input", () => {
            this.rangeInput.valueAsNumber = this.numberInput.valueAsNumber;
            this.rangeInput.dispatchEvent(new Event("input"));
        });
    }
    get value() {
        return this.rangeInput.value;
    }
    set value(v) {
        this.rangeInput.value
            = this.numberInput.value
                = v;
    }
    get valueAsNumber() {
        return this.rangeInput.valueAsNumber;
    }
    set valueAsNumber(v) {
        this.rangeInput.valueAsNumber = this.numberInput.valueAsNumber = v;
    }
    get disabled() {
        return this.rangeInput.disabled;
    }
    set disabled(v) {
        this.rangeInput.disabled
            = this.numberInput.disabled
                = v;
    }
}
export function InputRangeWithNumber(...rest) {
    return new InputRangeWithNumberElement(...rest);
}
