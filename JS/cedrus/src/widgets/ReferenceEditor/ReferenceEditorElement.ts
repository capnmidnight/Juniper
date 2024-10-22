import { formatUSDate, isNullOrUndefined, singleton, stringRandom } from "@juniper-lib/util";
import { A, Button, ClassList, ElementChild, For, ID, Label, OnClick, SingletonStyleBlob, SpanTag, Time, TypedHTMLElement, border, borderRadius, copyToClipboard, elementSetDisplay, marginLeft, registerFactory, rule } from "@juniper-lib/dom";
import { northEastArrow } from "@juniper-lib/emoji";
import { UpdatedEvent } from "@juniper-lib/widgets";
import { EntityModel } from "../../models";
import { ReferenceDataManager } from "./ReferenceDataManager";
import { ReferenceDialog, ReferenceDialogElement } from "./ReferenceDialogElement";


type ReferenceEditorEventMap = {
    "updated": UpdatedEvent<ReferenceEditorElement>
}

export class ReferenceEditorElement extends TypedHTMLElement<ReferenceEditorEventMap> {

    static observedAttributes = [
        "disabled",
        "readonly"
    ];

    static format(refEntity: EntityModel) {
        const mgr = ReferenceDataManager.instance;
        const ref = mgr.getReference(refEntity?.id);
        const container = SpanTag();
        if (isNullOrUndefined(ref)) {
            container.append("No reference.");
        }
        else {
            const id = stringRandom(12);
            const link = A(
                ID(id),
                ref.name
            );

            if (ref.type === "ReferenceFile"
                || !ref.value.startsWith("file://")) {
                link.target = "_blank";
                link.href = ref.value;
            }
            else {
                link.href = "#";
                const path = ref.value.replace("file://", "")
                    .split("/")
                    .join("\\");
                link.addEventListener("click", async evt => {
                    evt.preventDefault();
                    await copyToClipboard(path);
                    alert(`File path ${path} copied to clipboard`);
                });
            }

            container.append(
                link,
                Label(
                    For(id),
                    ", ",
                    ref.authors,
                    ", ",
                    ref.date
                        ? Time(ref.date, formatUSDate(ref.date))
                        : "(Unknown date)",
                    "."
                )
            );
        }

        return container;
    }

    readonly #addButton: HTMLButtonElement;
    readonly #dialog: ReferenceDialogElement;
    readonly #dataManager: ReferenceDataManager;
    #content: HTMLElement = null;

    #selectedReference: EntityModel = null;
    get selectedReference() { return this.#selectedReference; }
    set selectedReference(v) {
        this.#selectedReference = v;
        this.#refresh();
    }

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Cedrus::ReferenceEditorElement", () => [
            rule("reference-editor",

                rule(".form-control input[is='typed-input']",
                    border("none")
                ),

                rule(" button",
                    borderRadius("5px"),
                    marginLeft("2px")
                )
            )
        ]);

        this.#content = SpanTag();

        this.#addButton = Button(
            ClassList("borderless"),
            northEastArrow.value,
            OnClick(async () => {
                this.selectedReference = await this.showDialog();
            }));

        this.#dialog = singleton("ReferenceEditorElement::Dialog", () => {
            const dialog = ReferenceDialog();
            document.body.append(dialog);
            return dialog;
        });

        this.#dataManager = ReferenceDataManager.instance;
        this.#dataManager.addEventListener("updated", () => this.#refresh());
    }

    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(value) { this.toggleAttribute("disabled", value); }

    get readOnly() { return this.hasAttribute("readonly"); }
    set readOnly(value) { this.toggleAttribute("readonly", value); }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        switch (name) {
            case "disabled":
                this.#addButton.disabled = this.disabled;
                break;
            case "readonly":
                elementSetDisplay(this.#addButton, !this.readOnly);
                break;
        }
    }

    #allSetup = false;

    connectedCallback() {
        if (!this.#allSetup) {
            this.append(
                this.#content,
                this.#addButton
            );

            this.#dialog.saveButtonText = "OK";
            this.#allSetup = true;
        }
    }

    #refresh() {
        this.#dialog.selectedReference = this.#selectedReference;
        const content = ReferenceEditorElement.format(this.#selectedReference);
        this.#content.replaceWith(content);
        this.#content = content;
    }

    showDialog() {
        return this.#dialog.showModal(this.#selectedReference);
    }

    static install() {
        ReferenceDialogElement.install();
        return singleton("Juniper::Cedrus::ReferenceEditorElement", () => registerFactory("reference-editor", ReferenceEditorElement));
    }
}

export function ReferenceEditor(...rest: ElementChild<ReferenceEditorElement>[]) {
    return ReferenceEditorElement.install()(...rest);
}