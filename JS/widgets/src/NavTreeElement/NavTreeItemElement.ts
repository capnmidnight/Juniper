import { ClassList, Div, ElementChild, HtmlAttr, I, Label, SingletonStyleBlob, TypedHTMLElement, backgroundColor, cursor, display, fontWeight, marginLeft, opacity, registerFactory, rule } from "@juniper-lib/dom";
import { blackDownwardsEquilateralArrowhead, blackRightwardsEquilateralArrowhead, fileFolder, openFileFolder, pageWithCurl } from "@juniper-lib/emoji";
import { TypedItemSelectedEvent } from "../TypedItemSelectedEvent";
import { singleton } from "@juniper-lib/util";

export function Selectable(selectable: boolean) {
    return new HtmlAttr<boolean, NavTreeItemElement>("selectable", selectable)
}

export class NavTreeItemElement extends TypedHTMLElement<{
    "itemselected": TypedItemSelectedEvent<NavTreeItemElement>;
}> {
    static observedAttributes = [
        "open",
        "selectable",
        "selected"
    ];

    readonly #collapser: HTMLElement;
    readonly #selectedEvt: TypedItemSelectedEvent<NavTreeItemElement>;
    readonly #controls: HTMLElement;
    #label: HTMLLabelElement;

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Widgets::NavTreeItem", () =>
            rule("nav-tree-item",
                display("block"),
                marginLeft("1em"),

                rule("[selectable]>.controls",
                    rule(">label",
                        cursor("pointer")
                    ),

                    rule(">i",
                        cursor("default")
                    )
                ),

                rule(":has(>nav-tree-item)",
                    rule("[open]",
                        rule(">nav-tree-item",
                            display("block")
                        ),

                        rule(">.controls>i",
                            cursor("zoom-out")
                        )
                    ),

                    rule(":not([open])",
                        rule(">nav-tree-item",
                            display("none")
                        ),

                        rule(">.controls>i",
                            cursor("zoom-in")
                        )
                    ),
                ),

                rule("[selected]>.controls",
                    fontWeight("bold"),
                    backgroundColor("rgba(255 255 255 / 25%)")
                ),

                rule(".highlighted>.controls",
                    fontWeight("bold")
                ),

                rule(":has(>nav-tree-item).dehighlighted",
                    opacity(0.5)
                ),

                rule(":not(:has(>nav-tree-item)).dehighlighted",
                    display("none").important(true)
                )
            )
        );

        this.addEventListener("click", evt => {
            evt.preventDefault();
            evt.stopPropagation();
            if (evt.target === this.#collapser) {
                this.open = !this.open;
            }
            else if (evt.target === this.#label) {
                if (this.selectable) {
                    this.selected = true;
                }
            }
            else {
                evt.target.dispatchEvent(this.#selectedEvt);
            }
        });

        this.#controls = Div(
            ClassList("controls"),
            this.#collapser = I(pageWithCurl.value),
            this.#label = Label()
        );

        this.#selectedEvt = new TypedItemSelectedEvent(this);
    }

    #setup = false;
    connectedCallback() {
        if (!this.#setup) {
            this.#setup = true;
            const controls = Array.from(this.children)
                .filter(e => e.slot === "controls");
            this.#controls.append(...controls);
            this.insertBefore(this.#controls, this.childNodes[0]);
        }

        this._refresh();
    }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        switch (name) {
            case "open":
            case "selectable":
                this._refresh();
                break;
            case "selected":
                if (this.selected) {
                    this.dispatchEvent(this.#selectedEvt);
                }
                break;
        }
    }

    _refresh() {
        this.#collapser.replaceChildren(this.#icon);

        for (const child of Array.from(this.childNodes)) {
            if (child instanceof HTMLLabelElement && child !== this.#label) {
                this.#label.replaceWith(child);
                this.#label = child;
            }
        }
    }

    get type() { return this.getAttribute("type"); }
    set type(v) { this.setAttribute("type", v); }

    get value() { return this.getAttribute("value"); }
    set value(v) { this.setAttribute("value", v); }

    get label() { return this.#label.textContent; }

    get selectable() { return this.hasAttribute("selectable"); }
    set selectable(v) { this.toggleAttribute("selectable", v); }

    get selected() { return this.hasAttribute("selected"); }
    set selected(v) { this.toggleAttribute("selected", v); }

    get open() { return this.hasAttribute("open"); }
    set open(v) { this.toggleAttribute("open", v); }

    get #icon() {
        if (this.querySelectorAll("nav-tree-item").length === 0) {
            return pageWithCurl.value;
        }

        if (this.open) {
            return blackDownwardsEquilateralArrowhead.value + openFileFolder.value;
        }

        return blackRightwardsEquilateralArrowhead.value + fileFolder.value;
    }

    static install() {
        return singleton("Juniper::Widgets::NavTreeItemElement", () => registerFactory("nav-tree-item", NavTreeItemElement));
    }
}

export function NavTreeItem(...rest: ElementChild<NavTreeItemElement>[]) {
    return NavTreeItemElement.install()(...rest);
}
