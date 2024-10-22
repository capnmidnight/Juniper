import { formatUSDate, isDefined, isNullOrUndefined, singleton, stringRandom } from "@juniper-lib/util";
import { Anchor, Button, ClassList, ClassName, Div, ElementChild, For, ID, Label, OnInput, PlaceHolder, Popover, PopoverTargetAction, PopoverTargetAttr, SingletonStyleBlob, SlotAttr, Time, TitleAttr, TypedHTMLElement, TypedHtmlProp, backgroundColor, border, borderRadius, boxShadow, columnGap, content, display, elementSetDisplay, em, flexDirection, fontSize, fontWeight, height, marginLeft, maxWidth, padding, position, pt, px, registerFactory, right, rule, top, width } from "@juniper-lib/dom";
import { northEastArrow } from "@juniper-lib/emoji";
import { DeleteEvent, HiddenEditor, HiddenEditorElement, LabelField, OnCancelled, OnDelete, OnItemSelected, OnOpened, OnUpdated, PropertyGroup, PropertyList, PropertyListElement, SortKeyField, TypedSelect, TypedSelectElement, UpdatedEvent, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../adapters";
import { DataType, EntityModel, PropertyModel, PropertyTypeModel, StorageType } from "../models";
import { PropertyEditorFactories, isFileEditor } from "./Editors";
import { IPropertyEditorElement } from "./Editors/IPropertyEditorFactory";
import { ReferenceDataManager, ReferenceEditor, ReferenceEditorElement } from "./ReferenceEditor";
import { IHasTemplateName } from "./TemplateName";

export function PropertyTypes(types: Map<string, PropertyTypeModel>) {
    return TypedHtmlProp<PropertyEditorElement>("propertyTypesByName", types);
}

export function AllowedPropertyTypes(propertyTypes: PropertyTypeModel[]) {
    return TypedHtmlProp<PropertyEditorElement>("allowedPropertyTypes", propertyTypes);
}

export function PropertyType(propertyType: PropertyTypeModel) {
    return TypedHtmlProp<PropertyEditorElement>("propertyType", propertyType);
}

export function LockPropertyType(locked: boolean) {
    return TypedHtmlProp<PropertyEditorElement>("lockPropertyType", locked);
}

export function Property(property: PropertyModel) {
    return TypedHtmlProp<PropertyEditorElement>("property", property);
}

export function ValidValues(values: Map<number, string[]>) {
    return TypedHtmlProp<PropertyEditorElement>("validValues", values);
}

export function Units(units: Map<string, string[]>) {
    return TypedHtmlProp<PropertyEditorElement>("units", units);
}

export function UnitAbbreviations(abbrevs: Map<string, string>) {
    return TypedHtmlProp<PropertyEditorElement>("unitAbbreviations", abbrevs);
}

type PropertyEditorEventMap = {
    "updated": UpdatedEvent<PropertyEditorElement>
    "delete": DeleteEvent<PropertyEditorElement>
}

export class PropertyEditorElement extends TypedHTMLElement<PropertyEditorEventMap> implements IHasTemplateName {

    static observedAttributes = [
        "disabled",
        "deletable",
        "cancelable",
        "open",
        "lockpropertytype",
        "propertyid",
        "propertytypename",
        "readonly"
    ];

    readonly #label: HTMLLabelElement;
    readonly #updateMessage: HTMLElement;
    readonly #propertyList: PropertyListElement;
    readonly #typeSelector: TypedSelectElement<PropertyTypeModel>;
    readonly #referencePopover: HTMLElement;
    readonly #valueLabel: HTMLElement;
    readonly #referenceButton: HTMLButtonElement;
    readonly #referenceInput: ReferenceEditorElement;
    readonly #unitsSelector: TypedSelectElement<string>;
    readonly #editor: HiddenEditorElement;
    readonly #editorContainer: HTMLElement;
    readonly #refMgr: ReferenceDataManager;
    readonly #updated = new UpdatedEvent<PropertyEditorElement>(true);
    readonly #delete = new DeleteEvent<PropertyEditorElement>(true);

    #valueInput: IPropertyEditorElement = null;


    #propertyTypesByName: Map<string, PropertyTypeModel<DataType, StorageType>> = null;
    get propertyTypesByName() { return this.#propertyTypesByName; }
    set propertyTypesByName(v) {
        this.#propertyTypesByName = v;
        this.#refreshEditor(true);
    }

    #validValuesByPropertyTypeId: Map<number, string[]> = null;
    get validValues() { return this.#validValuesByPropertyTypeId; }
    set validValues(v) {
        this.#validValuesByPropertyTypeId = v;
        this.#refreshEditor(true);
    }

    #units: Map<string, string[]> = null;
    get units() { return this.#units; }
    set units(v) {
        this.#units = v;
        this.#refreshUnitsSelector();
    }


    #abbreviations: Map<string, string> = null;
    get unitAbbreviations() { return this.#abbreviations; }
    set unitAbbreviations(v) {
        this.#abbreviations = v;
        this.#refreshEditor(true);
    }

    constructor() {
        super();

        SingletonStyleBlob("Juniper::Cedrus::PropertyEditorElement", () =>
            rule("property-editor",
                display("contents"),

                rule("> div",
                    display("flex"),
                    flexDirection("row"),
                    columnGap(em(.25))
                ),

                rule("> label",
                    fontWeight("bold"),

                    rule("::after",
                        content("':'")
                    )
                ),

                rule(" select",
                    height(px(26)),
                    maxWidth(em(15))
                ),

                rule(" textarea",
                    width("35vw"),
                    height("15vh")
                ),

                rule(" .editor-container",
                    display("flex"),
                    flexDirection("column")
                ),

                rule(" .ref-path",
                    position("relative"),
                    marginLeft(em(.25)),
                    top(px(-5)),
                    right(0)
                ),

                rule(" [popover]",
                    backgroundColor("antiquewhite"),
                    fontSize(pt(14)),
                    border("solid 3px #773"),
                    borderRadius(px(3)),
                    padding(em(1.5)),
                    boxShadow("rgba(0, 0, 0, 35%) 5px 5px 5px")
                )
            )
        );

        const id = stringRandom(12);
        this.#label = Label(For(id));

        this.#typeSelector = TypedSelect(
            ValueField<PropertyTypeModel>(v => v.id.toFixed()),
            LabelField<PropertyTypeModel>(v => v.name),
            SortKeyField<PropertyTypeModel>(v => v.name),
            PlaceHolder("Select property type..."),
            TitleAttr("Property Type"),
            OnItemSelected<PropertyTypeModel>(evt => {
                this.propertyType = evt.item;
            })
        );


        this.#editor = HiddenEditor(
            this.#valueLabel = Div(SlotAttr("default")),

            this.#referencePopover = Div(
                ID(id + "RefPopover"),
                Anchor(id + "RefPopoverButton"),
                Popover("auto")
            ),

            this.#referenceButton = Button(
                ID(id + "RefPopoverButton"),
                SlotAttr("closed-buttons"),
                ClassList("borderless"),
                TitleAttr("View references..."),
                northEastArrow.value,
                PopoverTargetAttr(id + "RefPopover"),
                PopoverTargetAction("toggle")
            ),

            this.#propertyList = PropertyList(
                SlotAttr("editor"),

                Label(For(id + "Input"), "Value"),
                this.#editorContainer = Div(
                    ClassName("editor-container"),
                    this.#valueInput = PropertyEditorFactories.String.Single(ID(id + "Input"))
                ),

                PropertyGroup("Units",
                    Label(For(id + "Units"), "Units"),
                    this.#unitsSelector = TypedSelect(
                        ID(id + "Units"),
                        PlaceHolder("Select units..."),
                        TitleAttr("Units"),
                        OnInput(() => this.#convertValue())
                    )
                ),

                PropertyGroup("Reference",
                    Label(For(id + "Reference"), "Reference"),
                    Div(
                        this.#referenceInput = ReferenceEditor()
                    )
                ),

                PropertyGroup("Last update",
                    Label("Last update"),
                    this.#updateMessage = Div(),
                )
            ),

            OnOpened(() => {
                this.open = this.#editor.open;
            }),
            OnCancelled(() => {
                this.open = this.#editor.open;
                this.#refreshEditor(false);
            }),
            OnUpdated(() => {
                if (this.#property) {
                    this.#property.value = this.#valueInput.outputValue;
                    this.#property.units = this.#unitsSelector.selectedItem;
                    this.#property.reference = this.selectedReference;
                }
                this.#onUpdate();
            }),
            OnDelete(() => {
                if (this.#property) {
                    this.#onDelete();
                }
            })

        );

        this.#refMgr = ReferenceDataManager.instance;
        this.#refMgr.addEventListener("updated", () => this.#refreshEditor(false));

        this.#propertyList.setGroupVisible("Units", false);
    }

    get showLabel() {
        const style = getComputedStyle(this.#label);
        return style.display !== "none";
    }

    set showLabel(v) {
        elementSetDisplay(this.#label, v);
    }

    #onUpdate() {
        this.dispatchEvent(this.#updated);
    }

    #onDelete() {
        this.dispatchEvent(this.#delete);
    }

    connectedCallback() {
        if (!this.#label.isConnected) {
            this.append(
                this.#label,
                this.#editor,

            );
            this.#refreshLabel();
            this.#refreshEditor(true);
        }
    }

    attributeChangedCallback(name: string, oldValue: string, newValue: string) {
        if (oldValue === newValue) return;

        switch (name) {
            case "open":
                this.#editor.open = this.open;
                break;
            case "deletable":
                this.#editor.deletable = this.deletable;
                break;
            case "cancelable":
                this.#editor.cancelable = this.cancelable;
                break;
            case "lockpropertytype":
            case "propertytypename":
                this.#refreshLabel();
                break;
            case "propertytypename":
                this.#refreshUnitsSelector();
                break;
            case "propertytypename":
            case "propertyid":
            case "readonly":
                this.#refreshEditor(true);
                break;
            case "disabled":
                this.#typeSelector.disabled = this.disabled;

                this.#valueInput.disabled
                    = this.#unitsSelector.disabled
                    = this.#editor.disabled
                    = this.#referenceInput.disabled
                    = this.disabled || !this.propertyType;
                break;
        }
    }

    #refreshLabel() {
        if (this.lockPropertyType && this.propertyType) {
            this.#label.replaceChildren(this.propertyType.name);
        }
        else {
            this.#label.replaceChildren(this.#typeSelector);
        }
    }

    #refreshUnitsSelector() {
        this.#propertyList.setGroupVisible("Units", isDefined(this.propertyType) && this.propertyType.unitsCategory !== "None");

        this.#typeSelector.selectedItem = this.propertyType;

        if (this.propertyType && this.units) {
            this.#unitsSelector.title = this.propertyType.unitsCategory;
            this.#unitsSelector.data = this.units.get(this.propertyType.unitsCategory);
        }
    }

    refresh() {
        this.#refreshEditor(true);
    }

    #refreshEditor(preservePreviousValue: boolean) {
        elementSetDisplay(this.#referenceButton, isDefined(this.property?.reference?.id));
        this.#valueLabel.innerHTML = "";
        this.#editor.readOnly = this.readOnly;
        this.#propertyList.setGroupVisible("Last update", isDefined(this.property?.updatedOn));
        this.#label.removeAttribute("title");
        this.#valueInput.disabled = !this.propertyType;

        if (this.propertyType) {
            const oldId = this.#valueInput.id;
            const oldValue = preservePreviousValue
                ? this.#valueInput.inputValue
                : null;

            const type = this.property?.type ?? this.propertyType;
            const value = this.property?.value ?? oldValue;

            const input = PropertyEditorFactories[type.type][type.storage](
                PlaceHolder(`Input ${type.name}...`),
                TitleAttr(type.name)
            );

            if (isNullOrUndefined(input)) {
                throw new Error(`Could not create input for ${type.type} ${type.storage}.`);
            }

            this.#valueInput = input;
            this.#valueInput.id = oldId;
            if ("setValidValues" in this.#valueInput) {
                this.#valueInput.setValidValues(this.#validValuesByPropertyTypeId.get(type.id));
            }

            if (isDefined(value)) {
                this.#valueInput.inputValue = value;
            }

            if (isDefined(this.propertyType)
                && isFileEditor(this.#valueInput)) {
                const contentTypeMatch = this.propertyType.description.match(/\[content-type: ([^\]]+)\]/);
                this.#valueInput.accept = contentTypeMatch && contentTypeMatch[1] || "";
            }

            this.#editorContainer.replaceChildren(this.#valueInput);
            this.#referenceInput.selectedReference = this.#property?.reference;

            if (this.property) {
                this.#lastUnits
                    = this.#unitsSelector.value
                    = this.property.units;

                this.#valueLabel.append(this.#valueInput.getPreviewElement(this.property));

                if (this.property.updatedOn) {
                    this.#updateMessage.replaceChildren(
                        "Last updated on ",
                        Time(this.property.updatedOn, formatUSDate(this.property.updatedOn)),
                        " by ",
                        this.property.updatedBy.name
                    );
                    this.#label.title = this.#updateMessage.textContent;
                }

                if (this.property.reference?.id) {
                    this.#referencePopover.replaceChildren(ReferenceEditorElement.format(this.property.reference))
                }
            }

            if (this.#abbreviations
                && this.propertyType?.type !== "Currency") {
                const units = this.#unitsSelector.value;
                const abbrev = this.#abbreviations.get(units);
                if (abbrev) {
                    this.#valueLabel.append(" " + abbrev);
                }
            }
        }
    }

    get open() { return this.hasAttribute("open"); }
    set open(value) { this.toggleAttribute("open", value); }

    get disabled() { return this.hasAttribute("disabled"); }
    set disabled(value) { this.toggleAttribute("disabled", value); }

    get deletable() { return this.hasAttribute("deletable"); }
    set deletable(value) { this.toggleAttribute("deletable", value); }

    get cancelable() { return this.hasAttribute("cancelable"); }
    set cancelable(value) { this.toggleAttribute("cancelable", value); }

    get readOnly() { return this.hasAttribute("readonly"); }
    set readOnly(value) { this.toggleAttribute("readonly", value); }

    get lockPropertyType() { return this.hasAttribute("lockpropertytype"); }
    set lockPropertyType(value) { this.toggleAttribute("lockpropertytype", value); }

    get allowedPropertyTypes() { return this.#typeSelector.data; }
    set allowedPropertyTypes(v) { this.#typeSelector.data = v; }

    #property: PropertyModel;
    get property() { return this.#property; }
    set property(v) {
        this.#property = v;
        this.propertyType = v?.type;
        this.lockPropertyType = true;
        this.propertyId = v?.id;
        this.#refreshLabel();
    }

    get propertyId() { return parseFloat(this.getAttribute("propertyid")); }
    set propertyId(v) {
        if (isNullOrUndefined(v)) {
            this.removeAttribute("propertyid");
        }
        else {
            this.setAttribute("propertyid", v.toFixed());
        }
    }

    get propertyType() { return this.#propertyTypesByName?.get(this.propertyTypeName); }
    set propertyType(v) { this.propertyTypeName = v?.name; }

    get propertyTypeName() { return this.getAttribute("propertytypename"); }
    set propertyTypeName(name) {
        if (isNullOrUndefined(name)) {
            this.removeAttribute("propertytypename");
        }
        else {
            this.setAttribute("propertytypename", name);
        }
    }

    get templateName() { return this.getAttribute("templatename"); }
    set templateName(v) {
        if (isNullOrUndefined(v)) {
            this.removeAttribute("templatename");
        }
        else {
            this.setAttribute("templatename", v);
        }
    }

    #lastUnits: string = null;

    async #convertValue() {
        if (this.#valueInput instanceof HTMLInputElement
            && this.#valueInput.type === "number") {
            if (this.#lastUnits) {
                const ds = await CedrusDataAPI.dataSourceTask;
                const newValue = await ds.convertUnits(this.#valueInput.valueAsNumber, this.#lastUnits, this.#unitsSelector.value);
                this.#valueInput.valueAsNumber = newValue;
            }

            this.#lastUnits = this.#unitsSelector.value;
        }
    }

    get selectedPropertyType() { return this.#typeSelector.selectedItem; }
    get selectedUnitOfMeasure() { return this.#unitsSelector.value || "None"; }
    get selectedReference() { return this.#referenceInput.selectedReference; }

    async saveProperty(ds: CedrusDataAPI, entity: EntityModel, readOnly: boolean) {
        this.property = await this.#valueInput.save(
            ds,
            entity,
            this.selectedPropertyType,
            this.selectedUnitOfMeasure,
            this.selectedReference
        );

        this.cancelable = true;
        this.open = false;
        this.deletable = this.templateName === "Other"
            && !readOnly;
        this.refresh();
    }

    static install() {
        return singleton("Juniper::Cedrus::PropertyEditorElement", () => registerFactory("property-editor", PropertyEditorElement));
    }
}

export function PropertyEditor(...rest: ElementChild<PropertyEditorElement>[]) {
    return PropertyEditorElement.install()(...rest);
}
