import { singleton } from "@juniper-lib/util";
import { Button, ClassList, ElementChild, HtmlTag, Name, SlotTag, StyleBlob, Template, TitleAttr, TypedHTMLElement, columnGap, display, em, flexDirection, flexGrow, perc, rule, width } from "@juniper-lib/dom";
import { floppyDisk, pencil } from "@juniper-lib/emoji";
import { IKEA } from "../IKEA";
import { UpdatedEvent } from "../OnUpdated";

export function HiddenEditor(defaultDisplayBlock: HTMLElement, editDisplayBlock: HTMLElement, ...rest: ElementChild[]) {
    defaultDisplayBlock.slot = "default";
    editDisplayBlock.slot = "editor";
    return HtmlTag(
        "hidden-editor",
        defaultDisplayBlock,
        editDisplayBlock,
        ...rest
    ) as HiddenEditorElement;
}

type HiddenEditorEventMap = {
    "updated": UpdatedEvent<HiddenEditorElement>
}

export class HiddenEditorElement extends TypedHTMLElement<HiddenEditorEventMap> {

    static observedAttributes = [
        "open"
    ];

    readonly #template: DocumentFragment;
    readonly #updated = new UpdatedEvent();

    #defaultBlock: HTMLElement;
    #editorBlock: HTMLElement;
    #button: HTMLButtonElement;

    constructor() {
        super();

        singleton("Juniper::HiddenEditorElement::Style", () => {
            document.head.append(StyleBlob(
                rule("hidden-editor",
                    display("flex"),
                    flexDirection("row"),
                    width(perc(100)),
                    columnGap(em(.25))
                ),
                rule("hidden-editor > [slot='default']",
                    flexGrow(1)
                )
            ));

            return true;
        });

        this.#template = singleton("Juniper::HiddenEditorElement::Template", () =>
            Template(
                SlotTag(Name("default")),
                SlotTag(Name("editor")),
                Button(
                    ClassList("borderless"),
                    TitleAttr("Edit"),
                    pencil.value
                )
            )
        ).content.cloneNode(true) as DocumentFragment;
    }

    get open() { return this.hasAttribute("open"); }
    set open(v) {
        if (v) {
            this.setAttribute("open", "");
        }
        else {
            this.removeAttribute("open");
        }
    }

    attributeChangedCallback(name: string) {
        if (name === "open") {
            this.#defaultBlock.style.display = this.open ? "none" : "";
            this.#editorBlock.style.display = this.open ? "" : "none";
            this.#button.title = this.open ? "Save" : "Edit";
            this.#button.replaceChildren(this.open
                ? floppyDisk.value
                : pencil.value
            );
        }
    }

    connectedCallback() {
        if (!this.#template.isConnected) {
            this.append(this.#template);
            IKEA(this);

            this.#defaultBlock = this.querySelector("[slot='default']");
            this.#editorBlock = this.querySelector("[slot='editor']");
            this.#editorBlock.style.display = "none";

            this.#button = this.querySelector(":scope > button");
            this.#button.addEventListener("click", () => {
                this.open = !this.open;
                if (!this.open) {
                    this.dispatchEvent(this.#updated);
                }
            });
        }
    }
}

customElements.define("hidden-editor", HiddenEditorElement);