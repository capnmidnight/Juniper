import { identity, singleton } from "@juniper-lib/util";
import { Disabled, ElementChild, HistoryManager, registerFactory } from "@juniper-lib/dom";
import { TabPanelElement } from "@juniper-lib/widgets";
import { DataBrowserPaneElement } from "./DataBrowserPane";
import { EndpointTab } from "./EndpointTab";
import { EntityTab } from "./EntityTab";
import { EntityTypeTab } from "./EntityTypeTab";
import { FileTab } from "./FileTab";
import { PropertyTab } from "./PropertyTab";
import { PropertyTemplateTab } from "./PropertyTemplateTab";
import { PropertyTypeTab } from "./PropertyTypeTab";
import { PropertyValidValuesTab } from "./PropertyTypeValidValuesTab";
import { RelationshipTab } from "./RelationshipTab";
import { RelationshipTemplateTab } from "./RelationshipTemplateTab";
import { RelationshipTypeTab } from "./RelationshipTypeTab";
import { RoleTab } from "./RoleTab";
import { UserTab } from "./UserTab";

export class DataBrowserElement extends TabPanelElement {

    static override observedAttributes = [
        "disabled"
    ];

    readonly #navHistory: HistoryManager<string>;
    #initialized = false;

    constructor() {
        super();
        this.#navHistory = new HistoryManager<string>({
            queryStringParam: "tab",
            stateSerializer: identity,
            stateDeserializer: identity
        });
        this.addEventListener("tabshown", () => {
            this.#navHistory.push(this.selectedTab);
        });

        this.#navHistory.addEventListener("statechanged", (evt) => {
            if (evt.state !== this.selectedTab) {
                this.selectedTab = evt.state || "Files";
            }
        });
    }

    override connectedCallback() {
        if (!this.#initialized) {
            const panes = Array.from(this.querySelectorAll<DataBrowserPaneElement>("data-browser-pane"));
            for (const pane of panes) {
                pane.remove();
            }
            
            this.append(
                FileTab(),
                EntityTypeTab(),
                EntityTab(),
                PropertyTypeTab(),
                PropertyValidValuesTab(),
                PropertyTemplateTab(),
                PropertyTab(),
                RelationshipTypeTab(),
                RelationshipTemplateTab(),
                RelationshipTab(),
                UserTab(),
                RoleTab(),
                EndpointTab()
            );
            this.#initialized = true;
        }
        super.connectedCallback();
        this.#navHistory.start();
        this.disabled = false;
    }

    override attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        super.attributeChangedCallback(name, oldValue, newValue);

        if (name === "disabled") {
            for (const pane of this.querySelectorAll<DataBrowserPaneElement>("data-browser-pane")) {
                pane.disabled = this.disabled;
            }
        }
    }

    override show(tabName: string, emitEvent: boolean) {
        super.show(tabName, emitEvent);
        const tab = this.getTab(tabName);
        if (tab instanceof DataBrowserPaneElement) {
            tab.show();
        }
    }

    static override install() {
        return singleton("Juniper::Cedrus::DataBrowserElement", () => registerFactory("data-browser", DataBrowserElement, Disabled(true)));
    }
}

export function DataBrowser(...rest: ElementChild<DataBrowserElement>[]) {
    return DataBrowserElement.install()(...rest);
}