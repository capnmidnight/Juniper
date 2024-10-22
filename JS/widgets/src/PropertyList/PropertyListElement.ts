import { isNullOrUndefined, singleton } from "@juniper-lib/util";
import { PrioritySet } from "@juniper-lib/collections";
import { ElementChild, IDisableable, SingletonStyleBlob, alignItems, color, columnGap, display, elementSetDisplay, em, gridAutoFlow, gridAutoRows, gridTemplateColumns, isDisableable, margin, registerFactory, rowGap, rule, textAlign, width } from "@juniper-lib/dom";
import { PropertyGroup, PropertyGroupElement } from "./PropertyGroupElement";



export type PropertyElement = string | ElementChild | [string, ElementChild];
const DEFAULT_PROPERTY_GROUP = "Default"

export class PropertyListElement extends HTMLElement {

    static observedAttributes = [
        "disabled"
    ];

    readonly #groups = new PrioritySet<string, HTMLElement>();
    readonly #groupVisible = new Map<string, boolean>();
    readonly #controls = new Set<IDisableable>();

    readonly #defaultGroup: PropertyGroupElement;
    readonly #observer: MutationObserver;

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Widgets::PropertyListElement", () => [
            rule("property-list",
                display("grid"),
                gridAutoFlow("row"),
                gridTemplateColumns("auto", "1fr"),
                gridAutoRows("auto"),
                alignItems("center"),
                columnGap(em(.5)),
                rowGap(em(.25)),

                rule("[disabled]",
                    color("silver")
                ),

                rule(" input[type=number]",
                    textAlign("end")
                ),

                rule(">property-group property-list",
                    width("100%"),
                    margin("0.5em"),
                    rule(" .editor-container",
                        width("100%")
                    )
                )
            )
        ]);

        this.#defaultGroup = PropertyGroup(DEFAULT_PROPERTY_GROUP);

        this.#observer = new MutationObserver(mutations => {
            for (const mutation of mutations) {
                for (const removed of mutation.removedNodes) {
                    this.#remove(removed);
                }
                for (const added of mutation.addedNodes) {
                    this.#add(added);
                }
            }
            this.#updateVisibility();
        });
    }

    connectedCallback() {
        if (!this.#defaultGroup.parentElement) {
            for (const child of Array.from(this.childNodes)) {
                this.#add(child);
            }
            this.insertAdjacentElement("afterbegin", this.#defaultGroup);
            this.#start();
            this.#updateVisibility();
        }
    }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        if (name === "disabled") {
            for (const control of this.#controls) {
                control.disabled = this.disabled;
            }
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
                this.#groups.delete(here.parentElement.name, here);
            }

            queue.push(...here.childNodes);
        }
    }

    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(v) { this.toggleAttribute("disabled", v); }

    get enabled() { return !this.disabled; }
    set enabled(v) { this.disabled = !v; }

    setGroupVisible(id: string, v: boolean): void {
        this.#groupVisible.set(id, v);
        this.#updateVisibility();
    }

    getGroupVisible(id: string): boolean {
        return !this.#groupVisible.has(id)
            || this.#groupVisible.get(id);
    }

    #updateVisibility() {
        for (const [name, group] of this.#groups) {
            const visible = this.getGroupVisible(name);
            for (const elem of group) {
                elementSetDisplay(elem, visible);
            }
        }
    }

    static install() {
        PropertyGroupElement.install();

        return singleton("Juniper::Widgets::PropertyListElement", () => registerFactory("property-list", PropertyListElement));
    }
}

export function PropertyList(...rest: ElementChild<PropertyListElement>[]) {
    return PropertyListElement.install()(...rest);
}
