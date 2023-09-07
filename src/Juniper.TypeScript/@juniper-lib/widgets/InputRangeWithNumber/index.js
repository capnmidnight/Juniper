import { ClassList } from "@juniper-lib/dom/attrs";
import { onInput } from "@juniper-lib/dom/evts";
import { Div, InputNumber, InputRange } from "@juniper-lib/dom/tags";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import "./style.css";
export class InputRangeWithNumber extends TypedEventTarget {
    constructor(...rest) {
        super();
        this.element = Div(ClassList("input-range-with-number"), this.rangeInput = InputRange(onInput(() => {
            this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber;
            this.dispatchEvent(new TypedEvent("input"));
        }), ...rest), this.numberInput = InputNumber(onInput(() => {
            this.rangeInput.valueAsNumber = this.numberInput.valueAsNumber;
            this.rangeInput.dispatchEvent(new Event("input"));
        })));
        this.numberInput.min = this.rangeInput.min;
        this.numberInput.max = this.rangeInput.max;
        this.numberInput.step = this.rangeInput.step;
        this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber;
        this.numberInput.disabled = this.rangeInput.disabled;
        this.numberInput.placeholder = this.rangeInput.placeholder;
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
    get enabled() {
        return !this.disabled;
    }
    set enabled(v) {
        this.disabled = !v;
    }
}
//# sourceMappingURL=index.js.map