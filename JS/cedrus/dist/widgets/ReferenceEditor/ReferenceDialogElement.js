import { arrayScan, dateISOToLocal, isNullOrUndefined, singleton, stringRandom } from "@juniper-lib/util";
import { ClassList, display, For, gap, H1, ID, InputDate, InputText, Label, OnInput, PlaceHolder, registerFactory, Required, rule, SingletonStyleBlob, TitleAttr, TypeAttr } from "@juniper-lib/dom";
import { BaseDialogElement, Cancelable, DataAttr, DataColumn, DataTable, FieldAttr, FileInput, PropertyGroup, PropertyList, TabPane, TabPanel, TypedSelect } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../../adapters";
import { ENTITY_TYPE, PROPERTY_TYPE } from "../../models";
import { ReferenceDataManager, referencePropertyTypes } from "./ReferenceDataManager";
const protocols = [
    "file://",
    "http://",
    "https://"
];
export class ReferenceDialogElement extends BaseDialogElement {
    #tabs;
    #properties;
    #references;
    #nameInput;
    #authorsInput;
    #publicationDateInput;
    #typeSelector;
    #protocolInput;
    #pathInput;
    #fileInput;
    #dataManager;
    constructor() {
        SingletonStyleBlob("Juniper::Cedrus::ReferenceDialogElement", () => [
            rule(".reference-editor-dialog", rule(" .modal-section-container", display("grid"), gap("11px")))
        ]);
        const id = stringRandom(12);
        const tabs = TabPanel(TabPane("Select existing", DataTable(ID(id + "References"), ClassList("table", "table-striped", "table-borderless", "table-hover"), DataColumn(TypeAttr("text"), FieldAttr("name"), TitleAttr("Name")), DataColumn(TypeAttr("text"), FieldAttr("authors"), TitleAttr("Authors")), DataColumn(TypeAttr("date"), FieldAttr("date"), TitleAttr("Published")), DataColumn(TypeAttr("text"), FieldAttr("value"), TitleAttr("Value")))), TabPane("Upload new", PropertyList(ID(id + "Properties"), ClassList("modal-section-container"), Label(For(id + "Name"), "Name"), InputText(ID(id + "Name"), Required(true), PlaceHolder("Enter title..."), TitleAttr("Reference Document Title")), Label(For(id + "Authors"), "Authors"), InputText(ID(id + "Authors"), Required(true), PlaceHolder("Enter authors..."), TitleAttr("Reference Document Authors")), Label(For(id + "Date"), "Publication Date"), InputDate(ID(id + "Date"), PlaceHolder("Select publication date..."), TitleAttr("Reference Publication Date")), Label(For(id + "Type"), "Type"), TypedSelect(ID(id + "Type"), Required(true), PlaceHolder("Select reference type..."), TitleAttr("Reference Type"), DataAttr(referencePropertyTypes), OnInput(() => {
            const curTypeName = this.#typeSelector.selectedItem;
            for (const typeName of referencePropertyTypes) {
                this.#properties.setGroupVisible(typeName, typeName === curTypeName);
            }
            this.#fileInput.required = curTypeName === "ReferenceFile";
            this.#protocolInput.required
                = this.#pathInput.required
                    = curTypeName === "ReferenceLink";
        })), PropertyGroup("ReferenceFile", Label(For(id + "File"), "File"), FileInput(ID(id + "File"), Required(true))), PropertyGroup("ReferenceLink", Label(For(id + "Protocol"), "Protocol"), TypedSelect(ID(id + "Protocol"), DataAttr(protocols)), Label(For(id + "Link"), "Path"), InputText(ID(id + "Link"), OnInput(() => {
            const v = this.#pathInput.value;
            const matchProtocol = v?.match(/^(\w+:\/\/)(.*)$/);
            const protocol = matchProtocol && matchProtocol[1]
                || /^([a-zA-Z]:\\|\\\\|\/)/.test(v) && "file://"
                || this.#protocolInput.value;
            const refValue = matchProtocol && matchProtocol[2] || v;
            this.#protocolInput.value = protocol;
            this.#pathInput.value = refValue;
        }))))));
        super(H1("Add New Reference"), tabs, Cancelable(true));
        this.#tabs = tabs;
        this.#references = this.Q(`#${id}References`);
        this.#properties = this.Q(`#${id}Properties`);
        this.#nameInput = this.Q(`#${id}Name`);
        this.#authorsInput = this.Q(`#${id}Authors`);
        this.#publicationDateInput = this.Q(`#${id}Date`);
        this.#typeSelector = this.Q(`#${id}Type`);
        this.#fileInput = this.Q(`#${id}File`);
        this.#protocolInput = this.Q(`#${id}Protocol`);
        this.#pathInput = this.Q(`#${id}Link`);
        this.#references.alterColumns(columns => columns.setVisible(3, false));
        this.#properties.setGroupVisible("ReferenceLink", false);
        this.#dataManager = ReferenceDataManager.instance;
        this.#dataManager.addEventListener("updated", () => this.#refresh());
        this.form.addEventListener("reset", () => {
            this.#fileInput.clear();
        });
        CedrusDataAPI.dataSourceTask.then(() => this.#refresh());
        this.dialog.addEventListener("showing", evt => {
            this.selectedReference = evt.value;
            evt.resolve();
        });
        this.dialog.addEventListener("validating", evt => {
            if (this.#tabs.selectedTab === "Select existing"
                && isNullOrUndefined(this.#references.selectedValue)) {
                evt.preventDefault();
            }
        });
        this.dialog.addEventListener("submit", async (evt) => {
            if (this.#tabs.selectedTab === "Upload new") {
                const ds = await CedrusDataAPI.dataSourceTask;
                const entity = await ds.setEntity(this.#nameInput.value, ENTITY_TYPE("Reference"));
                const value = this.#typeSelector.selectedItem === "ReferenceFile"
                    ? (await ds.mergeFiles(this.#fileInput.getFiles()))[0]
                    : this.#protocolInput.value + this.#pathInput.value;
                const publicationDate = dateISOToLocal(this.#publicationDateInput.valueAsDate);
                const properties = await Promise.all([
                    ds.setProperty(entity, PROPERTY_TYPE("Name"), this.#nameInput.value, "None"),
                    ds.setProperty(entity, PROPERTY_TYPE("Authors"), this.#authorsInput.value, "None"),
                    ds.setProperty(entity, PROPERTY_TYPE("Publication Date"), publicationDate, "None"),
                    ds.setProperty(entity, PROPERTY_TYPE(this.#typeSelector.selectedItem), value, "None")
                ]);
                const reference = this.#dataManager.addReference(entity, properties);
                this.#references.selectedValue = reference;
            }
            evt.resolve(this.selectedReference);
        });
    }
    #refresh() {
        const curId = this.#references.selectedValue?.id;
        this.#references.data = this.#dataManager.references;
        this.#references.selectedValue = this.#dataManager.getReference(curId);
    }
    get selectedReference() {
        if (this.#tabs.selectedTab === "Upload new") {
            return null;
        }
        return arrayScan(this.#dataManager.entities, ref => ref.id === this.#references.selectedValue?.id);
    }
    set selectedReference(v) {
        this.#references.selectedValue = arrayScan(this.#references.data, ref => ref.id === v?.id);
    }
    static install() {
        return singleton("Juniper::Cedrus::ReferenceDialogElement", () => registerFactory("reference-dialog", ReferenceDialogElement));
    }
}
export function ReferenceDialog(...rest) {
    return ReferenceDialogElement.install()(...rest);
}
//# sourceMappingURL=ReferenceDialogElement.js.map