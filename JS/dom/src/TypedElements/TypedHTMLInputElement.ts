import { isDefined } from "@juniper-lib/util";
import { TypedEventMap } from "@juniper-lib/events";
import { Input } from "../tags";
import { TypedHTMLElement } from "./TypedHTMLElement";

export class TypedHTMLInputElement<EventMapT extends TypedEventMap<string> = TypedEventMap<string>> extends TypedHTMLElement<EventMapT> implements HTMLInputElement {

    static observedAttributes = [
        "accept",
        "alt",
        "autocapitalize",
        "autocomplete",
        "capture",
        "checked",
        "dirname",
        "disabled",
        "form",
        "formaction",
        "formenctype",
        "formmethod",
        "formnovalidate",
        "formtarget",
        "height",
        "list",
        "max",
        "maxlength",
        "min",
        "minlength",
        "multiple",
        "name",
        "pattern",
        "placeholder",
        "popovertarget",
        "popovertargetaction",
        "readonly",
        "required",
        "size",
        "src",
        "step",
        "type",
        "value",
        "width",

        "onabort",
        "onautocomplete",
        "onautocompleteerror",
        "onblur",
        "oncancel",
        "oncanplay",
        "oncanplaythrough",
        "onchange",
        "onclick",
        "onclose",
        "oncontextmenu",
        "oncuechange",
        "ondblclick",
        "ondrag",
        "ondragend",
        "ondragenter",
        "ondragleave",
        "ondragover",
        "ondragstart",
        "ondrop",
        "ondurationchange",
        "onemptied",
        "onended",
        "onerror",
        "onfocus",
        "oninput",
        "oninvalid",
        "onkeydown",
        "onkeypress",
        "onkeyup",
        "onload",
        "onloadeddata",
        "onloadedmetadata",
        "onloadstart",
        "onmousedown",
        "onmouseenter",
        "onmouseleave",
        "onmousemove",
        "onmouseout",
        "onmouseover",
        "onmouseup",
        "onmousewheel",
        "onpause",
        "onplay",
        "onplaying",
        "onprogress",
        "onratechange",
        "onreset",
        "onresize",
        "onscroll",
        "onseeked",
        "onseeking",
        "onselect",
        "onshow",
        "onsort",
        "onstalled",
        "onsubmit",
        "onsuspend",
        "ontimeupdate",
        "ontoggle",
        "onvolumechange",
        "onwaiting",

        "accesskey",
        "anchor",
        "autocapitalize",
        "autofocus",
        "class",
        "contenteditable",
        "data-*",
        "dir",
        "draggable",
        "enterkeyhint",
        "exportparts",
        "hidden",
        "id",
        "inert",
        "inputmode",
        "is",
        "itemid",
        "itemprop",
        "itemref",
        "itemscope",
        "itemtype",
        "lang",
        "nonce",
        "part",
        "popover",
        "role",
        "slot",
        "spellcheck",
        "style",
        "tabindex",
        "title",
        "translate",
        "virtualkeyboardpolicy",
        "writingsuggestions"
    ];

    attributeChangedCallback(name: string, _oldValue: string, newValue: string) {
        this.element.setAttribute(name, newValue);
    }

    protected readonly element: HTMLInputElement;

    constructor() {
        super();

        this.element = Input();
    }

    #ready = false;
    connectedCallback() {
        if (!this.#ready) {
            this.append(this.element);
        }
    }

    get accept() { return this.element.accept; }
    set accept(v) { this.element.accept = v; }

    get align() { return this.element.align; }
    set align(v) { this.element.align = v; }

    get alt() { return this.element.alt; }
    set alt(v) { this.element.alt = v; }

    get autocomplete() { return this.element.autocomplete; }
    set autocomplete(v) { this.element.autocomplete = v; }

    get capture() { return this.element.capture; }
    set capture(v) { this.element.capture = v; }

    get checked() { return this.element.checked; }
    set checked(v) { this.element.checked = v; }

    get defaultChecked() { return this.element.defaultChecked; }
    set defaultChecked(v) { this.element.defaultChecked = v; }

    get defaultValue() { return this.element.defaultValue; }
    set defaultValue(v) { this.element.defaultValue = v; }

    get dirName() { return this.element.dirName; }
    set dirName(v) { this.element.dirName = v; }

    get disabled() { return this.element.disabled; }
    set disabled(v) { this.element.disabled = v; }

    get files() { return this.element.files; }
    set files(v) { this.element.files = v; }

    get form() { return this.element.form; }

    get formAction() { return this.element.formAction; }
    set formAction(v) { this.element.formAction = v; }

    get formEnctype() { return this.element.formEnctype; }
    set formEnctype(v) { this.element.formEnctype = v; }

    get formMethod() { return this.element.formMethod; }
    set formMethod(v) { this.element.formMethod = v; }

    get formNoValidate() { return this.element.formNoValidate; }
    set formNoValidate(v) { this.element.formNoValidate = v; }

    get formTarget() { return this.element.formTarget; }
    set formTarget(v) { this.element.formTarget = v; }

    get height() { return this.element.height; }
    set height(v) { this.element.height = v; }

    get indeterminate() { return this.element.indeterminate; }
    set indeterminate(v) { this.element.indeterminate = v; }

    get labels() { return this.element.labels; }

    get list() { return this.element.list; }

    get max() { return this.element.max; }
    set max(v) { this.element.max = v; }

    get maxLength() { return this.element.maxLength; }
    set maxLength(v) { this.element.maxLength = v; }

    get min() { return this.element.min; }
    set min(v) { this.element.min = v; }

    get minLength() { return this.element.minLength; }
    set minLength(v) { this.element.minLength = v; }

    get multiple() { return this.element.multiple; }
    set multiple(v) { this.element.multiple = v; }

    get name() { return this.element.name; }
    set name(v) { this.element.name = v; }

    get pattern() { return this.element.pattern; }
    set pattern(v) { this.element.pattern = v; }

    get placeholder() { return this.element.placeholder; }
    set placeholder(v) { this.element.placeholder = v; }

    get readOnly() { return this.element.readOnly; }
    set readOnly(v) { this.element.readOnly = v; }

    get required() { return this.element.required; }
    set required(v) { this.element.required = v; }

    get selectionDirection() { return this.element.selectionDirection; }
    set selectionDirection(v) { this.element.selectionDirection = v; }

    get selectionEnd() { return this.element.selectionEnd; }
    set selectionEnd(v) { this.element.selectionEnd = v; }

    get selectionStart() { return this.element.selectionStart; }
    set selectionStart(v) { this.element.selectionStart = v; }

    get size() { return this.element.size; }
    set size(v) { this.element.size = v; }

    get src() { return this.element.src; }
    set src(v) { this.element.src = v; }

    get step() { return this.element.step; }
    set step(v) { this.element.step = v; }

    get type() { return this.element.type; }
    set type(v) { this.element.type = v; }

    get useMap() { return this.element.useMap; }
    set useMap(v) { this.element.useMap = v; }

    get validationMessage() { return this.element.validationMessage; }

    get validity() { return this.element.validity; }

    get value() { return this.element.value; }
    set value(v) { this.element.value = v; }

    get valueAsDate() { return this.element.valueAsDate; }
    set valueAsDate(v) { this.element.valueAsDate = v; }

    get valueAsNumber() { return this.element.valueAsNumber; }
    set valueAsNumber(v) { this.element.valueAsNumber = v; }

    get webkitEntries() { return this.element.webkitEntries; }

    get webkitdirectory() { return this.element.webkitdirectory; }
    set webkitdirectory(v) { this.element.webkitdirectory = v; }

    get width() { return this.element.width; }
    set width(v) { this.element.width = v; }

    get willValidate() { return this.element.willValidate; }

    checkValidity(): boolean {
        return this.element.checkValidity();
    }

    reportValidity(): boolean {
        return this.element.reportValidity();
    }

    select(): void {
        return this.element.select();
    }

    setCustomValidity(error: string): void {
        this.element.setCustomValidity(error);
    }

    setRangeText(replacement: string): void;
    setRangeText(replacement: string, start: number, end: number, selectionMode?: SelectionMode): void;
    setRangeText(replacement: string, start?: number, end?: number, selectionMode?: SelectionMode): void {
        if (isDefined(start)) {
            this.element.setRangeText(replacement, start, end, selectionMode);
        }
        else {
            this.element.setRangeText(replacement);
        }
    }

    setSelectionRange(start: number | null, end: number | null, direction?: "forward" | "backward" | "none"): void {
        this.setSelectionRange(start, end, direction);
    }

    showPicker(): void {
        this.showPicker();
    }

    stepDown(n?: number): void {
        this.stepDown(n);
    }

    stepUp(n?: number): void {
        this.stepUp(n);
    }


    get popoverTargetAction() { return this.element.popoverTargetAction; }
    set popoverTargetAction(v) { this.element.popoverTargetAction = v; }

    get popoverTargetElement() { return this.element.popoverTargetElement; }
    set popoverTargetElement(v) { this.element.popoverTargetElement = v; }
}