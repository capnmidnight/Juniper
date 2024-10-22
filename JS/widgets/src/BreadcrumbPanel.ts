import { singleton } from "@juniper-lib/util";
import { A, ClassName, Clear, display, ElementChild, flexDirection, flexGrow, H2, HtmlRender, Name, registerFactory, rule, SingletonStyleBlob, SlotTag, TemplateInstance } from "@juniper-lib/dom";


export class BreadcrumbPanelChangedEvent extends Event {
    constructor(public readonly panel: HTMLElement) {
        super("panelchanged");
    }
}

export class BreadcrumbPanelElement extends HTMLElement {

    readonly #template: DocumentFragment;
    readonly #navPathContainer: HTMLDivElement;
    readonly #panels = new Array<HTMLElement>();
    readonly #panelNames = new Array<string>();
    readonly #panelDisplayTypes = new Array<string>();
    readonly #panelIndexes = new Map<string, number>();
    readonly #stepTitles = new Array<string>();

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Widgets::BreadcrumbPanel", () =>
            rule("breadcrumb-panel",
                display("flex"),
                flexDirection("column"),
                flexGrow(1),

                rule("> .navigationPathContainer, slot[name=sub-panels]",
                    display("block")
                )
            )
        );

        this.#template = TemplateInstance("Juniper::Widgets::BreadcrumbPanel", () => [
            H2(ClassName("navigationPathContainer")),
            SlotTag(Name("sub-panels"))
        ]);

        this.#navPathContainer = this.#template.querySelector(".navigationPathContainer");
    }

    connectedCallback() {
        if (!this.#template.parentElement) {
            this.#panels.push(...this.children as HTMLCollectionOf<HTMLElement>);

            for (let i = 0; i < this.#panels.length; ++i) {
                const panel = this.#panels[i];
                const panelName = panel.dataset.panelName;

                this.#panelIndexes.set(panelName, i);
                this.#panelNames.push(panelName);

                const panelStyle = getComputedStyle(panel);
                this.#panelDisplayTypes.push(panelStyle.display);
            }

            const subPanels = this.#template.querySelector("slot[name=sub-panels]");
            subPanels.append(...this.children);
            this.append(this.#template);

            this.#refresh();
        }
    }

    getPanel(index: number | string): HTMLElement {
        if (typeof index === "string") {
            if (!this.#panelIndexes.has(index)) {
                console.warn(`Panel "${index}" does not exist.`);
                return null;
            }

            index = this.#panelIndexes.get(index);
        }
        return this.#panels[index];
    }

    get currentStep() {
        return this.#stepTitles.length;
    }

    get currentPanel() {
        return this.#panels[this.#stepTitles.length];
    }

    next(title: string) {
        if (this.#stepTitles.length < this.#panels.length - 1) {
            this.#stepTitles.push(title);
            this.#refresh();
            this.dispatchEvent(new BreadcrumbPanelChangedEvent(this.currentPanel));
        }
    }

    back() {
        this.#goto(this.#stepTitles.length - 1);
    }

    reset() {
        this.#goto(0);
    }

    get panelCount() {
        return this.#panels.length;
    }

    #goto(index: number) {
        if (this.#stepTitles.length > 0) {
            if (typeof index === "string") {
                if (!this.#panelIndexes.has(index)) {
                    console.warn(`Panel "${index}" does not exist.`)
                    return;
                }
                index = this.#panelIndexes.get(index);
            }
            index = Math.min(index || 0, this.#stepTitles.length);
            if (index < this.#stepTitles.length) {
                this.#stepTitles.splice(index, this.#stepTitles.length);
                this.#refresh();
                this.dispatchEvent(new BreadcrumbPanelChangedEvent(this.currentPanel));
            }
        }
    }

    #stepLink(i: number) {
        const link = A(this.#panelNames[i]);

        if (this.#stepTitles.length > i) {
            link.href = "#";
            link.addEventListener("click", (evt) => {
                evt.preventDefault();
                this.#goto(i);
            });
        }

        return link;
    }

    #refresh() {
        HtmlRender(
            this.#navPathContainer,
            Clear(),
            this.#stepLink(0),
            ...this.#stepTitles.flatMap((title, i) => [
                " > ",
                this.#stepLink(i + 1),
                ": ",
                title
            ])
        );

        for (let i = 0; i < this.#panels.length; ++i) {
            const panel = this.#panels[i];
            const displayType = this.#panelDisplayTypes[i];
            panel.style.display = i === this.#stepTitles.length ? displayType : "none";
        }
    }

    static install() {
        return singleton("Juniper::Widgets::BreadCrumbPanelElement", () => registerFactory("breadcrumb-panel", BreadcrumbPanelElement));
    }
}

export function BreadcrumbPanel(...rest: ElementChild<BreadcrumbPanelElement>[]) {
    return BreadcrumbPanelElement.install()(...rest);
}
