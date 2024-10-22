import { singleton } from "@juniper-lib/util";
import { columnGap, display, fr, gridTemplateColumns, InputNumber, InputRange, OnInput, px, registerFactory, rule, SingletonStyleBlob, TypedHTMLElement } from "@juniper-lib/dom";
export class InputRangeWithNumberElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "min",
        "max",
        "step",
        "value",
        "disabled"
    ]; }
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Widgets::InputRangeWithNumberElement", () => rule(".input-range-with-number", display("grid"), columnGap(px(5)), gridTemplateColumns(fr(1), 0)));
        this.rangeInput = InputRange(OnInput(() => {
            this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber;
        }));
        this.numberInput = InputNumber(OnInput(() => {
            this.rangeInput.valueAsNumber = this.numberInput.valueAsNumber;
        }));
        this.numberInput.min = this.rangeInput.min;
        this.numberInput.max = this.rangeInput.max;
        this.numberInput.step = this.rangeInput.step;
        this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber;
        this.numberInput.disabled = this.rangeInput.disabled;
        this.numberInput.placeholder = this.rangeInput.placeholder;
    }
    #ready = false;
    connectedCallback() {
        if (!this.#ready) {
            this.#ready = true;
            this.append(this.rangeInput, this.numberInput);
        }
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        switch (name) {
            case "min":
                this.numberInput.min = this.rangeInput.min = this.min;
                break;
            case "max":
                this.numberInput.max = this.rangeInput.max = this.max;
                break;
            case "value":
                this.numberInput.valueAsNumber = this.rangeInput.valueAsNumber = this.valueAsNumber;
                break;
            case "step":
                this.numberInput.step = this.rangeInput.step = this.step;
                break;
            case "disabled":
                this.numberInput.disabled = this.rangeInput.disabled = this.disabled;
                break;
        }
    }
    get min() { return this.getAttribute("min"); }
    set min(v) { this.setAttribute("min", v); }
    get max() { return this.getAttribute("max"); }
    set max(v) { this.setAttribute("max", v); }
    get step() { return this.getAttribute("step"); }
    set step(v) { this.setAttribute("step", v); }
    get value() { return this.getAttribute("value"); }
    set value(v) { this.setAttribute("value", v); }
    get valueAsNumber() { return parseFloat(this.value); }
    set valueAsNumber(v) { this.value = v.toString(); }
    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(v) { this.toggleAttribute("disabled", v); }
    get enabled() { return !this.disabled; }
    set enabled(v) { this.disabled = !v; }
    static install() { return singleton("Juniper::Widgets::InputRangeWithNumberElement", () => registerFactory("input-range", InputRangeWithNumberElement)); }
}
export function InputRangeWithNumber(...rest) {
    return InputRangeWithNumberElement.install()(...rest);
}
//# sourceMappingURL=index.js.map