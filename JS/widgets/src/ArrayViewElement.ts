import { arrayClear, arrayRemove, arrayReplace, compareBy, compareCallback, eventHandler, isNullOrUndefined, singleton } from "@juniper-lib/util";
import { Button, ClassList, Div, ElementChild, HtmlEvent, OnClick, SingletonStyleBlob, StyleAttr, TitleAttr, TypedHTMLElement, border, display, em, flexDirection, flexWrap, gap, justifyContent, maxHeight, overflowY, padding, registerFactory, rule } from "@juniper-lib/dom";
import { multiply } from "@juniper-lib/emoji";
import { TypedEvent } from "@juniper-lib/events";
import { fieldGetter, identityString, makeItemCallback } from "./FieldDef";
import { TypedItemSelectedEvent } from "./TypedItemSelectedEvent";

export function OnRemoving<DataT, TargetT extends EventTarget = EventTarget>(callback: eventHandler<RemovingEvent<DataT, TargetT>>, opts?: boolean | AddEventListenerOptions) {
    return new HtmlEvent("removing", callback, opts, false);
}

export class RemovingEvent<DataT, TargetT extends EventTarget = EventTarget> extends TypedEvent<"removing", TargetT> {
    readonly #item: DataT;
    get item() { return this.#item; }
    constructor(item: DataT) {
        super("removing", { bubbles: true });
        this.#item = item;
    }
}

type ArrayViewEventMap<DataT> = {
    "removing": RemovingEvent<DataT, ArrayViewElement<DataT>>;
    "itemselected": TypedItemSelectedEvent<DataT, ArrayViewElement<DataT>>;
};

export class ArrayViewElement<DataT> extends TypedHTMLElement<ArrayViewEventMap<DataT>> implements TypedHTMLElement<ArrayViewEventMap<DataT>> {

    static observedAttributes = [
        "labelfield",
        "sortkeyfield"
    ];

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Widgets::ArrayView", () =>
            rule("array-view",
                display("flex"),
                flexDirection("column"),
                border("inset 2px #ccc"),
                padding(em(.25)),
                maxHeight(em(20)),
                overflowY("auto"),

                rule(" > div",
                    display("flex"),
                    flexDirection("row"),
                    flexWrap("nowrap"),
                    justifyContent("space-between"),
                    gap(em(.5)),

                    rule(".selected",
                        border("dashed 2px black")
                    )
                ),

                rule(" button",
                    rule(".remove")
                )
            )
        );
    }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        switch (name.toLowerCase()) {
            case "labelfield":
                this.labelField = newValue && fieldGetter(newValue) || identityString;
                break;
            case "sortkeyfield":
                this.sortKeyField = newValue && compareBy(fieldGetter(newValue)) || null;
                break;
            default:
                this.#render();
                break;
        }
    }

    #labelField: makeItemCallback<DataT> = identityString;
    get labelField(): makeItemCallback<DataT> { return this.#labelField; }
    set labelField(v: makeItemCallback<DataT>) {
        this.#labelField = v || identityString;
        this.#render();
    }

    #sortKeyField: compareCallback<DataT> = null;
    get sortKeyField(): compareCallback<DataT> { return this.#sortKeyField; }
    set sortKeyField(v: compareCallback<DataT>) {
        this.#sortKeyField = v;
        this.#render();
    }

    readonly #values: DataT[] = [];
    get values() { return this.#values; }
    set values(v) {
        if (isNullOrUndefined(v)) {
            this.clear();
        }
        else {
            arrayReplace(this.#values, v);
            this.#render();
        }
    }

    readonly #idvalues: string[] = [];
    get idvalues() { return this.#idvalues; }
    set idvalues(v) {
        if (isNullOrUndefined(v)) {
            this.clear();
        }
        else {
            arrayReplace(this.#idvalues, v);
            this.#render();
        }
    }


    #selectedItem: DataT = null;
    get selectedItem() { return this.#selectedItem; }
    set selectedItem(v) {
        this.#selectedItem = v;
        this.#render();
    }

    addItem(item: DataT) {
        this.#values.push(item);
        this.#idvalues.push(this.#labelField(item));
        this.#render();
    }

    removeItem(item: DataT) {
        arrayRemove(this.#values, item);
        arrayRemove(this.#idvalues, this.#labelField(item));
        this.#render();
    }

    clear() {
        arrayClear(this.#values);
        arrayClear(this.#idvalues);
        this.#render();
    }

    readonly #itemToElement = new Map<DataT, HTMLElement>();
    readonly #elementToItem = new Map<HTMLElement, DataT>();

    #render() {
        let viewcolor: string;
        if (this.#values.length === 0) {
            this.replaceChildren("No items");
        }
        else {
            if (this.#sortKeyField) {
                this.#values.sort(this.#sortKeyField);
            }

            this.#itemToElement.clear();
            this.#elementToItem.clear();

            for (const item of this.#values) {
                if (this.#idvalues.includes(this.#labelField(item))) {
                    viewcolor = "red";
                } else {
                    viewcolor = "--bs-body-color";
                }
                const label = this.#labelField(item);
                const element = Div(
                    label,
                    Button(
                        multiply.value,
                        ClassList("remove"),
                        TitleAttr("Remove item"),
                        OnClick(() => this.dispatchEvent(new RemovingEvent(item)))
                    ),
                    OnClick(() => {
                        for (const element of this.#itemToElement.values()) {
                            element.classList.remove("selected");
                        }

                        if (item === this.#selectedItem) {
                            this.#selectedItem = null;
                        }
                        else {
                            this.#selectedItem = item;
                            element.classList.add("selected");
                        }

                        this.dispatchEvent(new TypedItemSelectedEvent(this.#selectedItem));
                    }),
                    StyleAttr({ color: viewcolor })
                );

                this.#itemToElement.set(item, element);
                this.#elementToItem.set(element, item);
            }

            if (this.#selectedItem) {
                if (this.#itemToElement.has(this.#selectedItem)) {
                    this.#itemToElement
                        .get(this.#selectedItem)
                        .classList
                        .add("selected");
                }
                else {
                    this.#selectedItem = null;
                }
            }

            this.replaceChildren(
                ...this.#values.map(item =>
                    this.#itemToElement.get(item))
            );
        }
    }

    static install() {
        return singleton("Juniper::Widgets::ArrayViewElement", () => registerFactory("array-view", ArrayViewElement));
    }
}

export function ArrayView<DataT>(...rest: ElementChild<ArrayViewElement<DataT>>[]) {
    return (ArrayViewElement.install() as any)(...rest) as ArrayViewElement<DataT>;
}
