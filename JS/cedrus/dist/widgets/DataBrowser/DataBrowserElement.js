import { identity, singleton } from "@juniper-lib/util";
import { Disabled, HistoryManager, registerFactory } from "@juniper-lib/dom";
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
    static { this.observedAttributes = [
        "disabled"
    ]; }
    #navHistory;
    #initialized = false;
    constructor() {
        super();
        this.#navHistory = new HistoryManager({
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
    connectedCallback() {
        if (!this.#initialized) {
            const panes = Array.from(this.querySelectorAll("data-browser-pane"));
            for (const pane of panes) {
                pane.remove();
            }
            this.append(FileTab(), EntityTypeTab(), EntityTab(), PropertyTypeTab(), PropertyValidValuesTab(), PropertyTemplateTab(), PropertyTab(), RelationshipTypeTab(), RelationshipTemplateTab(), RelationshipTab(), UserTab(), RoleTab(), EndpointTab());
            this.#initialized = true;
        }
        super.connectedCallback();
        this.#navHistory.start();
        this.disabled = false;
    }
    attributeChangedCallback(name, oldValue, newValue) {
        if (oldValue === newValue)
            return;
        super.attributeChangedCallback(name, oldValue, newValue);
        if (name === "disabled") {
            for (const pane of this.querySelectorAll("data-browser-pane")) {
                pane.disabled = this.disabled;
            }
        }
    }
    show(tabName, emitEvent) {
        super.show(tabName, emitEvent);
        const tab = this.getTab(tabName);
        if (tab instanceof DataBrowserPaneElement) {
            tab.show();
        }
    }
    static install() {
        return singleton("Juniper::Cedrus::DataBrowserElement", () => registerFactory("data-browser", DataBrowserElement, Disabled(true)));
    }
}
export function DataBrowser(...rest) {
    return DataBrowserElement.install()(...rest);
}
//# sourceMappingURL=DataBrowserElement.js.map