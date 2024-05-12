import { isNullOrUndefined, singleton, stringRandom } from "@juniper-lib/util";
import { PrioritySet } from "@juniper-lib/collections";
import { ElementChild, HtmlTag, IDisableable, StyleBlob, alignItems, color, columnGap, content, display, elementSetDisplay, em, gridAutoFlow, gridAutoRows, gridColumn, gridTemplateColumns, isDisableable, isDisplayed, justifySelf, rowGap, rule, textAlign, whiteSpace } from "@juniper-lib/dom";
import { PropertyGroup, PropertyGroupElement } from "./PropertyGroupElement";



export type PropertyElement = string | ElementChild | [string, ElementChild];
const DEFAULT_PROPERTY_GROUP = "DefaultPropertyGroup" + stringRandom(16);

export class PropertyListElement extends HTMLElement {

    readonly #groups = new PrioritySet<string, HTMLElement>();
    readonly #controls = new Set<IDisableable>();

    readonly #defaultGroup: PropertyGroupElement;
    readonly #observer: MutationObserver;

    constructor() {
        super();
        this.#defaultGroup = PropertyGroup(DEFAULT_PROPERTY_GROUP);

        singleton("Juniper::PropertyListElement", () => {
            document.head.append(StyleBlob(
                rule("property-list",
                    display("grid"),
                    gridAutoFlow("row"),
                    gridTemplateColumns("auto", "1fr"),
                    gridAutoRows("auto"),
                    alignItems("center"),
                    columnGap(em(.5)),
                    rowGap(em(.25))
                ),

                rule("property-list[disabled]",
                    color("silver")
                ),

                rule("property-list > property-group *",
                    gridColumn(1, -1),
                    justifySelf("center")
                ),

                rule("property-list > property-group label",
                    gridColumn(1, 2),
                    justifySelf("end"),
                    whiteSpace("nowrap")
                ),


                rule("property-list > property-group label, property-list > property-group label > input",
                    textAlign("end")
                ),

                rule("property-list > property-group label + *:not(label)",
                    gridColumn(2, 3),
                    justifySelf("start")
                ),

                rule("property-list > property-group label::after",
                    content(":")
                ),

                rule("property-list input[type=number]",
                    textAlign("end")
                ),

                rule("property-group",
                    display("contents")
                ))
            );
            return true;
        });

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
    }

    connectedCallback() {
        if (!this.#defaultGroup.parentElement) {
            for (const child of Array.from(this.childNodes)) {
                this.#add(child);
            }
            this.insertAdjacentElement("afterbegin", this.#defaultGroup);
            this.#start();
        }
    }

    #start() {
        this.#observer.observe(this, { subtree: true, childList: true });
    }

    #stop() {
        this.#observer.disconnect();
    }

    #add(added: Node) {
        if (added.parentElement === this
            || added.parentElement instanceof PropertyGroupElement) {
            const queue = [added];
            while (queue.length > 0) {
                const here = queue.shift();

                if (isDisableable(here)) {
                    this.#controls.add(here);
                }

                if (here instanceof HTMLElement && !(here instanceof PropertyGroupElement)) {
                    const group = here.parentElement instanceof PropertyGroupElement
                        ? here.parentElement
                        : isNullOrUndefined(here.parentElement) || here.parentElement === this
                            ? this.#defaultGroup
                            : null;
                    if (group) {
                        this.#groups.add(group.name, here);
                        if (here.parentElement !== this.#defaultGroup && group === this.#defaultGroup) {
                            this.#stop();
                            this.#defaultGroup.appendChild(here);
                            this.#start();
                        }
                    }
                }

                if (here.childNodes.length > 0) {
                    queue.push(...here.childNodes);
                }
            }
        }
    }

    #remove(removed: Node) {
        const queue = [removed];
        while (queue.length > 0) {
            const here = queue.shift();

            if (isDisableable(here) && this.#controls.has(here)) {
                this.#controls.delete(here);
            }

            if (here instanceof HTMLElement && here.parentElement instanceof PropertyGroupElement) {
                this.#groups.remove(here.parentElement.name, here);
            }

            queue.push(...here.childNodes);
        }
    }

    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(v) {
        if (v !== this.disabled) {
            if (v) {
                this.setAttribute("disabled", "");
            }
            else {
                this.removeAttribute("disabled");
            }

            for (const control of this.#controls) {
                control.disabled = v;
            }
        }
    }

    get enabled() { return !this.disabled; }
    set enabled(v) { this.disabled = !v; }

    setGroupVisible(id: string, v: boolean): void {
        const elems = this.#groups.get(id);
        if (elems) {
            for (const elem of elems) {
                elementSetDisplay(elem, v);
            }
        }
    }

    getGroupVisible(id: string): boolean {
        const elems = this.#groups.get(id);
        if (elems) {
            for (const elem of elems) {
                return isDisplayed(elem);
            }
        }

        return false;
    }
}
customElements.define("property-list", PropertyListElement);

export function PropertyList(...rest: ElementChild[]) {
    return HtmlTag("property-list", ...rest) as PropertyListElement;
}
