import { TypedEventBase } from "juniper-tslib";
import { className } from "./attrs";
import { display, gridAutoFlow, gridTemplateColumns, rule } from "./css";
import { Div, InputNumber, Style } from "./tags";
Style(rule(".input-range-with-text", display("grid"), gridAutoFlow("column"), gridTemplateColumns("1fr auto")));
const inputEvt = new InputEvent("input");
export class InputRangeWithText extends TypedEventBase {
    rangeInput;
    numberInput;
    constructor(rangeInput) {
        super();
        this.rangeInput = rangeInput;
        const placeHolder = Div();
        this.rangeInput.replaceWith(Div(className("input-range-with-text"), placeHolder, this.numberInput = InputNumber()));
        placeHolder.replaceWith(this.rangeInput);
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
        this.rangeInput.valueAsNumber
            = this.numberInput.valueAsNumber
                = v;
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
