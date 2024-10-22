import { isDefined, isString } from "@juniper-lib/util";
import { Select } from "../tags";
import { TypedHTMLElement } from "./TypedHTMLElement";
export class TypedHTMLSelectElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "autocomplete",
        "autofocus",
        "disabled",
        "form",
        "multiple",
        "name",
        "required",
        "size",
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
    ]; }
    attributeChangedCallback(name, _oldValue, newValue) {
        this.element.setAttribute(name, newValue);
    }
    constructor() {
        super();
        this.element = Select();
        return new Proxy(this, {
            get(target, name) {
                if (isString(name) && /^\d+$/.test(name)) {
                    const index = parseFloat(name);
                    return target.element[index];
                }
                return Reflect.get(target, name);
            },
            set(target, name, value) {
                if (isString(name) && /^\d+$/.test(name)) {
                    const index = parseFloat(name);
                    target.element[index] = value;
                }
                else {
                    Reflect.set(target, name, value);
                }
                return true;
            }
        });
    }
    get autocomplete() { return this.element.autocomplete; }
    set autocomplete(v) { this.element.autocomplete = v; }
    get disabled() { return this.element.disabled; }
    set disabled(v) { this.element.disabled = v; }
    get form() { return this.element.form; }
    get labels() { return this.element.labels; }
    get length() { return this.element.length; }
    set length(v) { this.element.length = v; }
    get multiple() { return this.element.multiple; }
    set multiple(v) { this.element.multiple = v; }
    get name() { return this.element.name; }
    set name(v) { this.element.name = v; }
    get options() { return this.element.options; }
    get required() { return this.element.required; }
    set required(v) { this.element.required = v; }
    get selectedIndex() { return this.element.selectedIndex; }
    set selectedIndex(v) { this.element.selectedIndex = v; }
    get selectedOptions() { return this.element.selectedOptions; }
    get size() { return this.element.size; }
    set size(v) { this.element.size = v; }
    get type() { return this.element.type; }
    get validationMessage() { return this.element.validationMessage; }
    get validity() { return this.element.validity; }
    get value() { return this.element.value; }
    set value(v) { this.element.value = v; }
    get willValidate() { return this.element.willValidate; }
    add(element, before) {
        this.element.add(element, before);
    }
    checkValidity() {
        return this.element.checkValidity();
    }
    item(index) {
        return this.element.item(index);
    }
    namedItem(name) {
        return this.element.namedItem(name);
    }
    remove(index) {
        if (isDefined(index)) {
            this.element.remove(index);
        }
        else {
            super.remove();
        }
    }
    reportValidity() {
        return this.element.reportValidity();
    }
    setCustomValidity(error) {
        this.element.setCustomValidity(error);
    }
    showPicker() {
        this.element.showPicker();
    }
    [Symbol.iterator]() {
        return this.element[Symbol.iterator]();
    }
    #ready = false;
    connectedCallback() {
        if (!this.#ready) {
            this.append(this.element);
        }
    }
}
//# sourceMappingURL=TypedHTMLSelectElement.js.map