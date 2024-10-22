import { stringCallback } from "@juniper-lib/util";
import { ElementChild, HtmlProp } from "@juniper-lib/dom";
export declare function DateFormatter(callback: stringCallback<any>): HtmlProp<"dateFormatter", stringCallback<any>, Node & Record<"dateFormatter", stringCallback<any>>>;
export declare class InputDateRangeElement extends HTMLElement {
    #private;
    get dateFormatter(): stringCallback<any>;
    set dateFormatter(v: stringCallback<any>);
    constructor();
    connectedCallback(): void;
    /** Sets or retrieves a text alternative to the graphic. */
    get alt(): string;
    set alt(v: string);
    /** Sets or retrieves the initial contents of the object. */
    get defaultValue(): string;
    set defaultValue(v: string);
    get dirName(): string;
    set dirName(v: string);
    get disabled(): boolean;
    set disabled(v: boolean);
    /** Retrieves a reference to the form that the object is embedded in. */
    get form(): HTMLFormElement;
    /**
     * Overrides the action attribute (where the data on a form is sent) on the parent form element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formAction)
     */
    get formAction(): string;
    set formAction(v: string);
    /**
     * Used to override the encoding (formEnctype attribute) specified on the form element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formEnctype)
     */
    get formEnctype(): string;
    set formEnctype(v: string);
    /**
     * Overrides the submit method attribute previously specified on a form element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formMethod)
     */
    get formMethod(): string;
    set formMethod(v: string);
    /**
     * Overrides any validation or required attributes on a form or form elements to allow it to be submitted without validation. This can be used to create a "save draft"-type submit option.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formNoValidate)
     */
    get formNoValidate(): boolean;
    set formNoValidate(v: boolean);
    /**
     * Overrides the target attribute on a form element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/formTarget)
     */
    get formTarget(): string;
    set formTarget(v: string);
    /**
     * Sets or retrieves the height of the object.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/height)
     */
    get height(): number;
    set height(v: number);
    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/labels) */
    get labels(): NodeListOf<HTMLLabelElement>;
    /** Defines the maximum acceptable value for an input element with type="number".When used with the min and step attributes, lets you control the range and increment (such as only even numbers) that the user can enter into an input field. */
    get max(): string;
    set max(v: string);
    /** Defines the minimum acceptable value for an input element with type="number". When used with the max and step attributes, lets you control the range and increment (such as even numbers only) that the user can enter into an input field. */
    get min(): string;
    set min(v: string);
    /** Sets or retrieves the name of the object. */
    get name(): string;
    set name(v: string);
    /**
     * Gets or sets a string containing a regular expression that the user's input must match.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/pattern)
     */
    get pattern(): string;
    set pattern(v: string);
    /**
     * Gets or sets a text string that is displayed in an input field as a hint or prompt to users as the format or type of information they need to enter.The text appears in an input field until the user puts focus on the field.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/placeholder)
     */
    get placeholder(): string;
    set placeholder(v: string);
    get readOnly(): boolean;
    set readOnly(v: boolean);
    /**
     * When present, marks an element that can't be submitted without a value.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/required)
     */
    get required(): boolean;
    set required(v: boolean);
    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/selectionDirection) */
    get selectionDirection(): "none" | "forward" | "backward";
    set selectionDirection(v: "none" | "forward" | "backward");
    /** Gets or sets the end position or offset of a text selection. */
    get selectionEnd(): number;
    set selectionEnd(v: number);
    /** Gets or sets the starting position or offset of a text selection. */
    get selectionStart(): number;
    set selectionStart(v: number);
    get size(): number;
    set size(v: number);
    get type(): string;
    /**
     * Returns the error message that would be displayed if the user submits the form, or an empty string if no error message. It also triggers the standard error message, such as "this is a required field". The result is that the user sees validation messages without actually submitting.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/validationMessage)
     */
    get validationMessage(): string;
    /**
     * Returns a  ValidityState object that represents the validity states of an element.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/validity)
     */
    get validity(): ValidityState;
    /** Returns the value of the data at the cursor's current position. */
    get value(): string;
    set value(v: string);
    get minValue(): string;
    set minValue(v: string);
    get maxValue(): string;
    set maxValue(v: string);
    /** Returns a Date object representing the form control's value, if applicable; otherwise, returns null. Can be set, to change the value. Throws an "InvalidStateError" DOMException if the control isn't date- or time-based. */
    get valuesAsDates(): Date[];
    set valuesAsDates(dates: Date[]);
    get minValueAsDate(): Date;
    set minValueAsDate(v: Date);
    get maxValueAsDate(): Date;
    set maxValueAsDate(v: Date);
    /** Returns the input field value as a number. */
    get valuesAsNumbers(): number[];
    set valuesAsNumbers(numbers: number[]);
    get minValueAsNumber(): number;
    set minValueAsNumber(v: number);
    get maxValueAsNumber(): number;
    set maxValueAsNumber(v: number);
    /**
     * Sets or retrieves the width of the object.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/width)
     */
    get width(): number;
    set width(v: number);
    /**
     * Returns whether an element will successfully validate based on forms validation rules and constraints.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/willValidate)
     */
    get willValidate(): boolean;
    /**
     * Returns whether a form will validate when it is submitted, without having to submit it.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/checkValidity)
     */
    checkValidity(): boolean;
    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/reportValidity) */
    reportValidity(): boolean;
    /**
     * Makes the selection equal to the current object.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/select)
     */
    select(): void;
    /**
     * Sets a custom error message that is displayed when a form is submitted.
     * @param error Sets a custom error message that is displayed when a form is submitted.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/setCustomValidity)
     */
    setCustomValidity(error: string): void;
    /**
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/setRangeText)
     */
    setRangeText(replacement: string, start: number, end: number, selectionMode: SelectionMode): void;
    /**
     * Sets the start and end positions of a selection in a text field.
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/setSelectionRange)
     */
    setSelectionRange(start: number, end: number, direction?: "forward" | "backward" | "none"): void;
    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/HTMLInputElement/showPicker) */
    showPicker(): void;
    static install(): import("@juniper-lib/dom").ElementFactory<InputDateRangeElement>;
}
export declare function InputDateRange(...rest: ElementChild<InputDateRangeElement>[]): InputDateRangeElement;
//# sourceMappingURL=InputDateRangeElement.d.ts.map