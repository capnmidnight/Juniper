import { arrayClear, arrayRemove, arrayReplace, compareBy, compareCallback, eventHandler, isNullOrUndefined, singleton } from "@juniper-lib/util";
import { Button, ClassList, Disabled, Div, ElementChild, HtmlTag, Name, OnClick, Query, SlotTag, StyleBlob, Template, TitleAttr, display, flexDirection, flexWrap, justifyContent, rule } from "@juniper-lib/dom";
import { multiply, plus } from "@juniper-lib/emoji";
import { ArrayElementType, IArrayElementSelector, TypedElementSelector } from "../ArrayElementSelector";
import { fieldGetter, identityString, makeItemCallback } from "../FieldDef";
import { IKEA } from "../IKEA";

export function ArrayEditor<InputT extends IArrayElementSelector<DataT>, DataT = ArrayElementType<InputT>>(selector: InputT, ...rest: ElementChild[]): ArrayEditorElement<InputT, DataT>;
export function ArrayEditor<InputT extends IArrayElementSelector<DataT>, DataT = ArrayElementType<InputT>>(...rest: ElementChild[]): ArrayEditorElement<InputT, DataT>;
export function ArrayEditor<InputT extends IArrayElementSelector<DataT>, DataT = ArrayElementType<InputT>>(...rest: ElementChild[]) {
    return HtmlTag("array-editor", ...rest) as ArrayEditorElement<InputT, DataT>;
}

export class ArrayEditorElement<InputT extends IArrayElementSelector<DataT>, DataT = ArrayElementType<InputT>> extends HTMLElement implements TypedElementSelector<DataT> {

    static observedAttributes = [
        "valuefield",
        "labelfield",
        "sortkeyfield",
        "placeholder",
        "allowduplicates"
    ];

    readonly #template: DocumentFragment;
    readonly #itemsContainer: HTMLElement;
    readonly #addButton: HTMLButtonElement;
    readonly #observer: MutationObserver;
    readonly #onItemSelected: eventHandler<Event>;

    #selector: InputT;

    readonly #data: DataT[] = [];
    get data(): readonly DataT[] { return this.#data; }
    set data(v: readonly DataT[]) {
        arrayReplace(this.#data, v);
        this.#render();
    }

    readonly #values: DataT[] = [];
    get values() { return this.#values; }
    set values(v) {
        for (const item of v) {
            this.#addItem(item);
        }
    }

    constructor() {
        super();

        singleton("Juniper::ArrayEditor::Stylesheet", () => {
            document.head.append(StyleBlob(
                rule("array-editor",
                    display("flex"),
                    flexDirection("column")
                ),
                rule("array-editor > div.controls",
                    display("flex"),
                    flexDirection("row")
                ),
                rule("array-editor > div.items",
                    display("flex"),
                    flexDirection("column")
                ),
                rule("array-editor > div.items > div",
                    display("flex"),
                    flexDirection("row"),
                    flexWrap("nowrap"),
                    justifyContent("space-between")
                ),
                rule("array-editor button",

                ),
                rule("array-editor button.add"),
                rule("array-editor button.remove")
            ));
            return true;
        })

        this.#template = singleton("Foxgolve::ArrayEditor::Template", () =>
            Template(
                Div(
                    ClassList("controls", "input-group"),
                    SlotTag(Name("selector")),
                    Button(
                        plus.value,
                        ClassList("add"),
                        TitleAttr("Add selected item"),
                        Disabled(true)
                    )
                ),
                Div(ClassList("items"))
            )
        ).content.cloneNode(true) as DocumentFragment;

        this.#addButton = Button(
            Query(this.#template, "button[type='button']"),
            OnClick(() => {
                if (this.#selector?.selectedItem) {
                    this.#addItem(this.#selector.selectedItem);
                    this.#selector.selectedItem = null;
                }
            })
        );

        this.#itemsContainer = this.#template.querySelector("div:last-child");

        this.#onItemSelected = () =>
            this.#addButton.disabled = isNullOrUndefined(this.#selector.selectedItem);

        this.#observer = new MutationObserver(mutations => {
            for (const mutation of mutations) {
                for (const removed of mutation.removedNodes) {
                    this.#remove(removed);
                }
                for (const added of mutation.addedNodes) {
                    this.#add(added);
                }
            }
        });

        this.#start();
    }

    connectedCallback() {
        if (!this.#template.isConnected) {
            this.append(this.#template);
            this.#add(this.querySelector("[slot='selector']"));
        }
    }

    attributeChangedCallback(name: string) {
        const value = this.getAttribute(name);
        switch (name.toLowerCase()) {
            case "valuefield":
                this.valueField = value && fieldGetter(value) || identityString;
                break;
            case "labelfield":
                this.labelField = value && fieldGetter(value) || identityString;
                break;
            case "sortkeyfield":
                this.sortKeyField = value && compareBy(fieldGetter(value)) || null;
                break;
            default:
                this.#render();
                break;
        }
    }

    #start() {
        this.#observer.observe(this, { childList: true });
    }

    #stop() {
        this.#observer.disconnect();
    }

    #add(added: Node) {
        if (added instanceof Element && added.slot === "selector") {
            this.#stop();
            this.#selector = added as InputT;
            IKEA(this);
            this.#selector.addEventListener("input", this.#onItemSelected);

            if (this.data.length === 0 && this.#selector.data.length > 0) {
                arrayReplace(this.#data, this.#selector.data);
            }

            if (!this.placeholder && this.#selector.placeholder) {
                this.setAttribute("placeholder", this.#selector.placeholder)
            }

            if (this.#getLabel === identityString) {
                this.#getLabel = this.#selector.labelField;
            }

            if (this.#getValue === identityString) {
                this.#getValue = this.#selector.valueField;
            }

            this.#getSortKey ??= this.#selector.sortKeyField;

            this.#render();
            this.#start();
        }
    }

    #remove(removed: Node) {
        if (this.#selector && removed === this.#selector) {
            this.#selector.removeEventListener("input", this.#onItemSelected);
            this.#selector = null;
        }
    }


    get placeholder() { return this.getAttribute("placeholder"); }
    set placeholder(v) { this.setAttribute("placeholder", v); }


    #getLabel: makeItemCallback<DataT> = identityString;
    get labelField(): makeItemCallback<DataT> { return this.#getLabel; }
    set labelField(v: makeItemCallback<DataT>) {
        this.#getLabel = v || identityString;
        this.#render();
    }

    #getValue: makeItemCallback<DataT> = identityString;
    get valueField(): makeItemCallback<DataT> { return this.#getValue; }
    set valueField(v: makeItemCallback<DataT>) {
        this.#getValue = v || identityString;
        this.#render();
    }

    #getSortKey: compareCallback<DataT> = null;
    get sortKeyField(): compareCallback<DataT> { return this.#getSortKey; }
    set sortKeyField(v: compareCallback<DataT>) {
        this.#getSortKey = v;
        this.#render();
    }

    get allowDuplicates() { return this.hasAttribute("allowduplicates"); }
    set allowDuplicates(v) {
        if (v) {
            this.setAttribute("allowduplicates", "");
        }
        else {
            this.removeAttribute("allowduplicates");
        }
    }

    #addItem(item: DataT) {
        if (this.#values.indexOf(item) === -1 || this.allowDuplicates) {
            this.#values.push(item);
            this.#render();
        }
    }

    #removeItem(item: DataT) {
        arrayRemove(this.#values, item);
        this.#render();
    }

    clear() {
        arrayClear(this.#values);
        this.#render();
    }

    #render() {
        if (this.#selector) {
            this.#selector.labelField = this.#getLabel;
            this.#selector.valueField = this.#getValue;
            this.#selector.sortKeyField = this.#getSortKey;
            this.#selector.placeholder = this.placeholder;
            if (this.allowDuplicates) {
                this.#selector.data = this.#data;
            }
            else {
                this.#selector.data = this.#data.filter(v => this.#values.indexOf(v) === -1);
            }
        }

        if (this.#values.length === 0) {
            this.#itemsContainer.replaceChildren("No items");
        }
        else {
            if (this.#getSortKey) {
                this.#values.sort(this.#getSortKey);
            }

            this.#itemsContainer.replaceChildren(
                ...this.#values.map(item => Div(
                    this.#selector.labelField(item),
                    Button(
                        multiply.value,
                        ClassList("remove"),
                        TitleAttr("Remove item"),
                        OnClick(() => this.#removeItem(item))
                    )
                ))
            );
        }
    }
}

customElements.define("array-editor", ArrayEditorElement);