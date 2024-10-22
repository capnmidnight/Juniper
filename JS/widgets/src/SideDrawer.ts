import { singleton } from "@juniper-lib/util";
import { backdropFilter, backgroundColor, border, borderRadius, boxShadow, Button, ClassList, display, Div, ElementChild, flexDirection, I, justifyContent, left, margin, OnClick, padding, position, Query, registerFactory, rule, SingletonStyleBlob, TemplateInstance, TitleAttr, top, width, zIndex } from "@juniper-lib/dom";

export class SideDrawerElement extends HTMLElement {

    readonly #template: DocumentFragment;
    readonly #interior: HTMLElement;

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Widgets::SideDrawer", () =>
            rule("side-drawer",
                position("relative"),
                rule(" .controls",
                    display("flex"),
                    justifyContent("flex-start")
                ),
                rule(">.side-drawer-elements",
                    position("absolute"),
                    top("0"),
                    left("0"),
                    width("max-content"),
                    zIndex(9001),
                    padding("1em"),
                    backgroundColor("rgba(100%, 100%, 100%, 50%)"),
                    backdropFilter("blur(10px)"),
                    boxShadow("5px 5px 5px rgba(0, 0, 0, 25%)"),
                    border("solid 1px black"),
                    borderRadius("3px"),
                    rule(">.controls",
                        flexDirection("row-reverse"),
                        margin("-1em")
                    )
                ),
                rule(":not([open])>.side-drawer-elements",
                    display("none")
                )
            )
        );


        this.#template = TemplateInstance("Juniper:Widgets:SideDrawer", () => [
            Div(
                ClassList("controls"),
                Button(
                    ClassList("btn", "btn-dark", "side-drawer-open-btn"),
                    TitleAttr("Open filter panel"),
                    I(ClassList("fa", "fa-filter"))
                )
            ),
            Div(
                ClassList("side-drawer-elements"),
                Div(
                    ClassList("controls"),
                    Button(
                        ClassList("btn", "side-drawer-close-btn"),
                        TitleAttr("Close filter panel"),
                        I(ClassList("fa", "fa-xmark"))
                    )
                )
            )
        ]);

        this.#interior = this.#template.querySelector(".side-drawer-elements");
    }

    connectedCallback() {
        if (!this.#template.parentElement) {
            this.#interior.append(...this.children);
            super.append(this.#template);

            Button(
                Query(this, ".side-drawer-open-btn"),
                OnClick(() => this.open = true)
            );

            Button(
                Query(this, ".side-drawer-close-btn"),
                OnClick(() => this.open = false)
            );
        }
    }

    get open() {
        return this.hasAttribute("open");
    }

    set open(v) { this.toggleAttribute("open", v); }

    override append(...nodes: (string | Node)[]): void {
        this.#interior.append(...nodes);
    }

    override appendChild<T extends Node>(node: T): T {
        return this.#interior.appendChild(node);
    }

    override insertAdjacentElement(where: InsertPosition, element: Element): Element {
        return this.#interior.insertAdjacentElement(where, element);
    }

    override insertAdjacentHTML(position: InsertPosition, text: string): void {
        this.#interior.insertAdjacentHTML(position, text);
    }

    override insertAdjacentText(where: InsertPosition, data: string): void {
        this.#interior.insertAdjacentText(where, data);
    }

    override replaceChildren(...nodes: (string | Node)[]): void {
        this.#interior.replaceChildren(...nodes);
    }

    override replaceChild<T extends Node>(node: Node, child: T): T {
        return this.#interior.replaceChild(node, child);
    }

    override insertBefore<T extends Node>(node: T, child: Node): T {
        return this.#interior.insertBefore(node, child);
    }

    static install() {
        return singleton("Juniper::Widgets::SideDrawerElement", () => registerFactory("side-drawer", SideDrawerElement));
    }
}

export function SideDrawer(...rest: ElementChild<SideDrawerElement>[]) {
    return SideDrawerElement.install()(...rest);
}
