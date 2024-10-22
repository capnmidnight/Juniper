
import { singleton } from "@juniper-lib/util";
import { ElementChild, H2, SingletonStyleBlob, SpanTag, backgroundColor, border, borderRadius, display, elementSetDisplay, flexDirection, float, margin, padding, px, registerFactory, rule } from "@juniper-lib/dom";

export class NamedPanelElement extends HTMLElement {

    static observedAttributes = [
        "title",
        "open"
    ];

    private readonly header: HTMLHeadingElement;
    private readonly titleText: HTMLSpanElement;

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Widgets::NamedPanelElement", () => 
            rule("named-panel",
                display("flex"),
                flexDirection("column"),
                border("2px outset silver"),
                borderRadius("5px"),
            
                rule(">H2",
                    margin(0),
                    padding(px(3), px(6)),
                    backgroundColor("silver"),

                    rule(">button",
                        float("right")
                    )
                )
            )
        );

        this.header = H2(
            this.titleText = SpanTag()
        );

        Object.seal(this);
    }

    #ready = false;
    connectedCallback() {
        if (!this.#ready) {
            this.#ready = true;
            this.insertBefore(this.header, this.children[0]);
        }
    }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        switch (name) {
            case "title":
                this.titleText.replaceChildren(this.title);
                break;
            case "open":
                this.header.classList.toggle("closed", !this.open);
                for(const child of this.children){
                    elementSetDisplay(child as HTMLElement, child === this.header || this.open);
                }
                break;
        }
    }

    get open() { return this.hasAttribute("open"); }

    set open(v) { this.toggleAttribute("open", v); }

    static install() { return singleton("Juniper::Widgets::NamedPanelElement", () => registerFactory("named-panel", NamedPanelElement)); }
}

export function NamedPanel(...rest: ElementChild[]) {
    return NamedPanelElement.install()(...rest);
}
