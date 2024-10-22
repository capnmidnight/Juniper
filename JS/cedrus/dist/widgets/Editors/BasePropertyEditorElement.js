import { isDefined } from "@juniper-lib/util";
const forwardedAttributes = new Set([
    "disabled",
    "readonly",
    "placeholder",
    "title"
]);
export class BasePropertyEditorElement extends HTMLElement {
    static { this.observedAttributes = Array.from(forwardedAttributes); }
    constructor(makeInput) {
        super();
        this._input = makeInput();
    }
    _setInputValue(_input, _value) {
    }
    _setOptions(_input, _values) {
    }
    #setup = false;
    connectedCallback() {
        if (!this.#setup) {
            this.append(this._input);
            this.#setup = true;
        }
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        if (forwardedAttributes.has(name)) {
            if (isDefined(newValue)) {
                this._input.setAttribute(name, newValue);
            }
            else {
                this._input.removeAttribute(name);
            }
        }
    }
    get id() {
        return this._input.id;
    }
    set id(v) {
        this._input.id = v;
    }
    getPreviewElement(property) {
        return this._getPreviewElement(property);
    }
    get inputValue() {
        return this._getInputValue(this._input);
    }
    set inputValue(v) {
        this._setInputValue?.call(null, this._input, v);
    }
    #outputValue;
    get outputValue() { return this.#outputValue; }
    setValidValues(v) {
        this._setOptions?.call(null, this._input, v);
    }
    get disabled() {
        return this._input.disabled;
    }
    set disabled(v) {
        this._input.disabled = v;
    }
    get readOnly() {
        return this._input.readOnly;
    }
    set readOnly(v) {
        this._input.readOnly = v;
    }
    get placeholder() {
        return this._input.placeholder;
    }
    set placeholder(v) {
        this._input.placeholder = v;
    }
    get title() {
        return this._input.title;
    }
    set title(v) {
        this._input.title = v;
    }
    async save(ds, entity, propertyType, unitOfMeasure, reference) {
        const inputValue = this._getInputValue(this._input);
        const outputValue = await this._inputToOutput(inputValue, ds);
        return await ds.setProperty(entity, propertyType, outputValue, unitOfMeasure, reference);
    }
}
//# sourceMappingURL=BasePropertyEditorElement.js.map