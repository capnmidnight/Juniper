import { first, formatDate, isDefined, last, singleton, stringCallback } from "@juniper-lib/util";
import { AutoComplete, ClassList, ElementChild, HtmlProp, InputDate, InputText, OnChange, OnClick, OnInput, SingletonStyleBlob, Value, border, boxShadow, fontSize, padding, perc, position, registerFactory, rem, rule, visibility } from "@juniper-lib/dom";

export function DateFormatter(callback: stringCallback<any>) {
    return new HtmlProp("dateFormatter", callback);
}

export class InputDateRangeElement extends HTMLElement {

    #front;
    #minDate;
    #maxDate;

    #inputs: HTMLInputElement[];

    #dateFormatter: stringCallback<any>;
    get dateFormatter() { return this.#dateFormatter; }
    set dateFormatter(v) {
        if (v !== this.#dateFormatter) {
            this.#dateFormatter = v;
            this.#render();
        }
    }

    constructor() {
        super();

        this.#dateFormatter = formatDate;

        const inputEvt = new Event("input");

        SingletonStyleBlob("Juniper::Widgets::InputDateRange", () => [
            rule(".date-range",
                position("relative"),
                padding(0)
            ),

            rule(".date-range:focus-within",
                boxShadow("0 0 0 0.25rem rgba(13, 110, 253, .25)")
            ),

            rule(".date-range input[type=date]",
                position("absolute"),
                visibility("hidden")
            ),

            rule(".date-range input[type=text]",
                position("relative"),
                fontSize(perc(80)),
                border("none"),
                padding(rem(.25), rem(.5))
            ),

            rule(".date-range input[type=text]:focus",
                boxShadow("none")
            )
        ]);

        this.#inputs = [
            this.#front = InputText(
                AutoComplete("off"),
                Value("min - max"),
                OnClick((evt) => {
                    if (evt.offsetX < this.#front.offsetWidth / 2) {
                        this.#selectMin();
                        this.#minDate.showPicker();
                    }
                    else {
                        this.#selectMax();
                        this.#maxDate.showPicker();
                    }
                    evt.preventDefault();
                }),
                OnChange(() => {
                    this.dispatchEvent(inputEvt);
                })
            ),
            this.#minDate = InputDate(
                OnInput(() => {
                    this.#render();
                    this.#selectMin();
                }),
                OnChange(() => {
                    this.#maxDate.showPicker();
                    this.dispatchEvent(inputEvt);
                })
            ),
            this.#maxDate = InputDate(
                OnInput(() => {
                    this.#render();
                    this.#selectMax();
                }),
                OnChange(() => {
                    this.dispatchEvent(inputEvt);
                })
            )
        ];
    }

    #selectMin() {
        const separator = this.#front.value.indexOf("-");
        this.#front.setSelectionRange(0, separator - 1, "forward");
    }

    #selectMax() {
        const separator = this.#front.value.indexOf("-");
        this.#front.setSelectionRange(separator + 2, this.#front.value.length, "forward");
    }

    #render() {
        if (isDefined(this.maxValueAsDate)
            && isDefined(this.minValueAsDate)
            && this.maxValueAsDate < this.minValueAsDate) {
            this.valuesAsDates = [this.maxValueAsDate, this.minValueAsDate];
        }

        this.#front.value = this.valuesAsDates
            .map(this.#dateFormatter)
            .map((v, i) => v || ["min", "max"][i])
            .join(" - ");
    }

    connectedCallback() {
        if (!this.#minDate.isConnected) {
            this.append(
                this.#minDate,
                this.#maxDate,
                this.#front
            );
        }

        for (const qualifiedName of this.getAttributeNames()) {
            const value = this.getAttribute(qualifiedName);
            switch (qualifiedName) {
                case "form":
                    this.#inputs.forEach(input => input.setAttribute(qualifiedName, value));
                    break;
                case "className":
                    continue;
                default:
                    this.#front.setAttribute(qualifiedName, value);
                    break;
            }
        }
    }

    /** Sets or retrieves a text alternative to the graphic. */
    get alt() { return this.#front.alt; }
    set alt(v) { this.#front.alt = v; }

    /** Sets or retrieves the initial contents of the object. */
    get defaultValue() { return this.#front.defaultValue; }
    set defaultValue(v) { this.#front.defaultValue = v; }

    get dirName() { return this.#front.dirName; }
    set dirName(v) { this.#front.dirName = v; }

    get disabled() { return this.#front.disabled; }
    set disabled(v) { this.#front.disabled = v; }

    /** Retrieves a reference to the form that the object is embedded in. */
    get form() { return this.#front.form; }

    /**
     * Overrides the action attribute (where the data on a form is sent) on the parent form element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formAction)
     */
    get formAction() { return this.#front.formAction; }
    set formAction(v) { this.#front.formAction = v; }

    /**
     * Used to override the encoding (formEnctype attribute) specified on the form element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formEnctype)
     */
    get formEnctype() { return this.#front.formEnctype; }
    set formEnctype(v) { this.#front.formEnctype = v; }

    /**
     * Overrides the submit method attribute previously specified on a form element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formMethod)
     */
    get formMethod() { return this.#front.formMethod; }
    set formMethod(v) { this.#front.formMethod = v; }

    /**
     * Overrides any validation or required attributes on a form or form elements to allow it to be submitted without validation. This can be used to create a "save draft"-type submit option.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formNoValidate)
     */
    get formNoValidate() { return this.#front.formNoValidate; }
    set formNoValidate(v) { this.#front.formNoValidate = v; }

    /**
     * Overrides the target attribute on a form element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formTarget)
     */
    get formTarget() { return this.#front.formTarget; }
    set formTarget(v) { this.#front.formTarget = v; }

    /**
     * Sets or retrieves the height of the object.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/height)
     */
    get height() { return this.#front.height; }
    set height(v) { this.#front.height = v; }

    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/labels) */
    get labels() { return this.#front.labels; }

    /** Defines the maximum acceptable value for an input element with type="number".When used with the min and step attributes, lets you control the range and increment (such as only even numbers) that the user can enter into an input field. */
    get max() { return this.#front.max; }
    set max(v) { this.#front.max = v; }

    /** Defines the minimum acceptable value for an input element with type="number". When used with the max and step attributes, lets you control the range and increment (such as even numbers only) that the user can enter into an input field. */
    get min() { return this.#front.min; }
    set min(v) { this.#front.min = v; }

    /** Sets or retrieves the name of the object. */
    get name() { return this.#front.name; }
    set name(v) { this.#front.name = v; }

    /**
     * Gets or sets a string containing a regular expression that the user's input must match.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/pattern)
     */
    get pattern() { return this.#front.pattern; }
    set pattern(v) { this.#front.pattern = v; }

    /**
     * Gets or sets a text string that is displayed in an input field as a hint or prompt to users as the format or type of information they need to enter.The text appears in an input field until the user puts focus on the field.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/placeholder)
     */
    get placeholder() { return this.#front.placeholder; }
    set placeholder(v) { this.#front.placeholder = v; }

    get readOnly() { return this.#front.readOnly; }
    set readOnly(v) {
        for (const input of this.#inputs) {
            input.readOnly = v;
        }
    }

    /**
     * When present, marks an element that can't be submitted without a value.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/required)
     */
    get required() { return this.#front.required; }
    set required(v) { this.#front.required = v; }

    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/selectionDirection) */
    get selectionDirection() { return this.#front.selectionDirection; }
    set selectionDirection(v) { this.#front.selectionDirection = v; }

    /** Gets or sets the end position or offset of a text selection. */
    get selectionEnd() { return this.#front.selectionEnd; }
    set selectionEnd(v) { this.#front.selectionEnd = v; }

    /** Gets or sets the starting position or offset of a text selection. */
    get selectionStart() { return this.#front.selectionStart; }
    set selectionStart(v) { this.#front.selectionStart = v; }

    get size() { return this.#front.size; }
    set size(v) { this.#front.size = v; }

    // /** Returns the content type of the object. */
    get type() { return "date-range"; }

    /**
     * Returns the error message that would be displayed if the user submits the form, or an empty string if no error message. It also triggers the standard error message, such as "this is a required field". The result is that the user sees validation messages without actually submitting.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/validationMessage)
     */
    get validationMessage() { return this.#front.validationMessage; }

    /**
     * Returns a  ValidityState object that represents the validity states of an element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/validity)
     */
    get validity() { return this.#front.validity; }

    /** Returns the value of the data at the cursor's current position. */
    get value() {
        if (this.#front.value === "min - max") {
            return "";
        }
        return this.#front.value;
    }
    set value(v) { this.#front.value = v; }

    get minValue() { return this.#minDate.value; }
    set minValue(v) { this.#minDate.value = v; }

    get maxValue() { return this.#maxDate.value; }
    set maxValue(v) { this.#maxDate.value = v; }

    /** Returns a Date object representing the form control's value, if applicable; otherwise, returns null. Can be set, to change the value. Throws an "InvalidStateError" DOMException if the control isn't date- or time-based. */
    get valuesAsDates() {
        return [
            new Date(this.#minDate.value + " 00:00:00"),
            new Date(this.#maxDate.value + " 23:59:59")
        ];
    }
    set valuesAsDates(dates) {
        this.#minDate.valueAsDate = first(dates);
        this.#maxDate.valueAsDate = last(dates);
    }

    get minValueAsDate() { return this.#minDate.valueAsDate; }
    set minValueAsDate(v) { this.#minDate.valueAsDate = v; }

    get maxValueAsDate() { return this.#maxDate.valueAsDate; }
    set maxValueAsDate(v) { this.#maxDate.valueAsDate = v; }

    /** Returns the input field value as a number. */
    get valuesAsNumbers() {
        return [
            this.#minDate.valueAsNumber,
            this.#maxDate.valueAsNumber
        ];
    }
    set valuesAsNumbers(numbers) {
        this.#minDate.valueAsNumber = first(numbers);
        this.#maxDate.valueAsNumber = last(numbers);
    }

    get minValueAsNumber() { return this.#minDate.valueAsNumber; }
    set minValueAsNumber(v) { this.#minDate.valueAsNumber = v; }

    get maxValueAsNumber() { return this.#maxDate.valueAsNumber; }
    set maxValueAsNumber(v) { this.#maxDate.valueAsNumber = v; }

    /**
     * Sets or retrieves the width of the object.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/width)
     */
    get width() { return this.#front.width; }
    set width(v) { this.#front.width = v; }

    /**
     * Returns whether an element will successfully validate based on forms validation rules and constraints.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/willValidate)
     */
    get willValidate() {
        return this.#inputs
            .every(input => input.willValidate);
    }

    /**
     * Returns whether a form will validate when it is submitted, without having to submit it.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/checkValidity)
     */
    checkValidity() {
        return this.#inputs
            .every(input => input.checkValidity());
    }

    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/reportValidity) */
    reportValidity() {
        return this.#inputs
            .every(input => input.reportValidity());
    }

    /**
     * Makes the selection equal to the current object.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/select)
     */
    select() { this.#front.select(); };

    /**
     * Sets a custom error message that is displayed when a form is submitted.
     * @param error Sets a custom error message that is displayed when a form is submitted.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/setCustomValidity)
     */
    setCustomValidity(error: string) {
        this.#front.setCustomValidity(error);
    }


    /**
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/setRangeText)
     */
    setRangeText(replacement: string, start: number, end: number, selectionMode: SelectionMode) {
        this.#front.setRangeText(replacement, start, end, selectionMode);
    }

    /**
     * Sets the start and end positions of a selection in a text field.
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/setSelectionRange)
     */
    setSelectionRange(start: number, end: number, direction?: "forward" | "backward" | "none") {
        this.#front.setSelectionRange(start, end, direction);
    }

    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/showPicker) */
    showPicker() {
        this.#minDate.showPicker();
    }

    // addEventListener<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLInputElement, ev: HTMLElementEventMap[K]) => any, options?: boolean | AddEventListenerOptions): void;
    // addEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    // removeEventListener<K extends keyof HTMLElementEventMap>(type: K, listener: (this: HTMLInputElement, ev: HTMLElementEventMap[K]) => any, options?: boolean | EventListenerOptions): void;
    // removeEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | EventListenerOptions): void;

    static install() {
        return singleton("Juniper::Widgets::InputDateRangeElement", () => registerFactory("input-date-range", InputDateRangeElement, ClassList("date-range")));
    }
}

export function InputDateRange(...rest: ElementChild<InputDateRangeElement>[]) {
    return InputDateRangeElement.install()(...rest);
}
