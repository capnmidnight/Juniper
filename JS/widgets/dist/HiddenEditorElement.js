import { singleton } from "@juniper-lib/util";
import { alignItems, border, Button, ClassList, Clear, columnGap, display, elementSetDisplay, em, flexBasis, flexDirection, HtmlRender, HtmlTag, I, justifyContent, Name, registerFactory, rule, SingletonStyleBlob, SlotTag, TemplateInstance, TitleAttr, TypedHTMLElement, width } from "@juniper-lib/dom";
import { crossMark, floppyDisk, pencil, wastebasket } from "@juniper-lib/emoji";
import { IKEA } from "./IKEA";
import { CancelledEvent } from "./OnCancelled";
import { DeleteEvent } from "./OnDelete";
import { OpenedEvent } from "./OnOpened";
import { UpdatedEvent } from "./OnUpdated";
export class HiddenEditorElement extends TypedHTMLElement {
    static { this.observedAttributes = [
        "open",
        "disabled",
        "deletable",
        "cancelable",
        "readonly"
    ]; }
    #template;
    #opened = new OpenedEvent();
    #cancelled = new CancelledEvent();
    #updated = new UpdatedEvent();
    #delete = new DeleteEvent();
    #defaultBlock = null;
    #editorBlock = null;
    #openButtonsBlock = null;
    #closedButtonsBlock = null;
    #editorButtonsBlock = null;
    #deleteButton = null;
    #saveButton = null;
    #editButton = null;
    #cancelButton = null;
    constructor() {
        super();
        SingletonStyleBlob("Juniper::Widgets::HiddenEditorElement", () => rule("hidden-editor", display("flex"), flexDirection("row"), columnGap(em(.25)), justifyContent("space-between"), width("100%"), flexBasis("0"), rule("> [slot='default']", width("100%")), rule("> button-group", display("flex"), alignItems("center")), rule("[open]", border("solid 1px #ccc"))));
        this.#template = TemplateInstance("Juniper::Widgets::HiddenEditorElement", () => [
            SlotTag(Name("default")),
            SlotTag(Name("editor")),
            HtmlTag("button-group", SlotTag(Name("open-buttons")), SlotTag(Name("closed-buttons")), Button(ClassList("borderless"), TitleAttr("Delete"), I(wastebasket.value)), Button(ClassList("borderless"), TitleAttr("Save"), I(floppyDisk.value)), Button(ClassList("borderless"), TitleAttr("Cancel"), I(crossMark.value)), Button(ClassList("borderless"), TitleAttr("Edit"), I(pencil.value)))
        ]);
    }
    get open() { return this.hasAttribute("open"); }
    set open(v) { this.toggleAttribute("open", v); }
    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(value) { this.toggleAttribute("disabled", value); }
    get deletable() { return this.hasAttribute("deletable"); }
    set deletable(value) { this.toggleAttribute("deletable", value); }
    get cancelable() { return this.hasAttribute("cancelable"); }
    set cancelable(value) { this.toggleAttribute("cancelable", value); }
    get readOnly() { return this.hasAttribute("readonly"); }
    set readOnly(value) { this.toggleAttribute("readonly", value); }
    attributeChangedCallback(_name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        this.#render();
    }
    #allSetup = false;
    connectedCallback() {
        if (!this.#allSetup) {
            this.append(this.#template);
            IKEA(this);
            this.#defaultBlock = this.querySelector("slot[name='default']");
            this.#editorBlock = this.querySelector("slot[name='editor']");
            this.#openButtonsBlock = this.querySelector("slot[name='open-buttons']");
            this.#closedButtonsBlock = this.querySelector("slot[name='closed-buttons']");
            this.#editorButtonsBlock = this.querySelector("button-group");
            this.#editButton = this.querySelector("button[title='Edit']");
            this.#editButton.addEventListener("click", () => {
                this.open = true;
                if (this.open) {
                    this.dispatchEvent(this.#opened);
                }
            });
            this.#cancelButton = this.querySelector("button[title='Cancel']");
            this.#cancelButton.addEventListener("click", () => {
                this.open = false;
                if (!this.open) {
                    this.dispatchEvent(this.#cancelled);
                }
            });
            this.#deleteButton = this.querySelector("button[title='Delete']");
            this.#deleteButton.addEventListener("click", () => {
                this.dispatchEvent(this.#delete);
            });
            this.#saveButton = this.querySelector("button[title='Save']");
            this.#saveButton.addEventListener("click", () => {
                this.dispatchEvent(this.#updated);
            });
            this.#render();
            this.#allSetup = true;
        }
    }
    #render() {
        if (this.#saveButton) {
            const open = this.open && !this.readOnly;
            elementSetDisplay(this.#editorBlock, open);
            elementSetDisplay(this.#defaultBlock, !open);
            if (open) {
                HtmlRender(this.#editorButtonsBlock, Clear(), this.#openButtonsBlock, this.#deleteButton, this.#saveButton, this.#cancelButton);
            }
            else if (this.readOnly) {
                HtmlRender(this.#editorButtonsBlock, Clear(), this.#closedButtonsBlock);
            }
            else {
                HtmlRender(this.#editorButtonsBlock, Clear(), this.#closedButtonsBlock, this.#editButton);
            }
            this.#cancelButton.style.visibility = this.cancelable ? "" : "hidden";
            this.#deleteButton.style.visibility = this.deletable ? "" : "hidden";
            this.#editButton.disabled
                = this.#cancelButton.disabled
                    = this.#saveButton.disabled
                        = this.#deleteButton.disabled
                            = this.disabled;
        }
    }
    static install() {
        return singleton("Juniper::Widgets::HiddenEditorElement", () => registerFactory("hidden-editor", HiddenEditorElement));
    }
}
export function HiddenEditor(...rest) {
    return HiddenEditorElement.install()(...rest);
}
//# sourceMappingURL=HiddenEditorElement.js.map