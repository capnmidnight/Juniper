import { groupBy, singleton, stringRandom } from "@juniper-lib/util";
import { A, AbstractAppliable, Div, ElementChild, For, HRef, HtmlTag, ID, Input, Label, OnInput, PlaceHolder, SlotAttr, SpanTag, StyleBlob, TitleAttr, TypedHTMLElement, columnGap, display, em, flexDirection, rule } from "@juniper-lib/dom";
import { HiddenEditor, HiddenEditorElement, LabelField, OnUpdated, SortKeyField, TypedSelect, TypedSelectElement, UpdatedEvent, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../adapters";
import { PropertyModel, PropertyTypeModel, SetPropertyInput, isPropertyModel } from "../models";
import { getPropertyValueFromInput, makeInputForProperty, makeInputForPropertyType } from "../widgets";
import { ClassificationEditor, ClassificationEditorElement } from "./ClassificationEditorElement";

export function PropertyEditor(...rest: ElementChild[]) {
    return HtmlTag("property-editor", ...rest) as PropertyEditorElement;
}

class PropertyTypeAttr extends AbstractAppliable<PropertyEditorElement> {

    #types;

    constructor(types: PropertyTypeModel[]) {
        super();
        this.#types = types;
    }

    apply(editor: PropertyEditorElement) {
        editor.propertyTypes = this.#types;
    }
}

export function PropertyTypes(types: PropertyTypeModel[]) {
    return new PropertyTypeAttr(types);
}

class ItemTypeAttr extends AbstractAppliable<PropertyEditorElement>{
    #itemType: PropertyTypeModel;
    constructor(itemType: PropertyTypeModel) {
        super();
        this.#itemType = itemType;
    }

    apply(editor: PropertyEditorElement) {
        editor.itemType = this.#itemType;
    }
}

export function ItemType(itemType: PropertyTypeModel) {
    return new ItemTypeAttr(itemType);
}

class ItemAttr extends AbstractAppliable<PropertyEditorElement>{
    #item: PropertyModel;
    constructor(item: PropertyModel) {
        super();
        this.#item = item;
    }

    apply(editor: PropertyEditorElement) {
        editor.item = this.#item;
    }
}

export function Item(item: PropertyModel) {
    return new ItemAttr(item);
}

type PropertyEditorEventMap = {
    "updated": UpdatedEvent<PropertyEditorElement>
}

export class PropertyEditorElement extends TypedHTMLElement<PropertyEditorEventMap> {

    readonly #label: HTMLLabelElement;
    readonly #typeSelector: TypedSelectElement<PropertyTypeModel>;
    readonly #infoLabel: HTMLElement;
    readonly #unitsSelector: TypedSelectElement<string>;
    readonly #classifier: ClassificationEditorElement;
    readonly #updated = new UpdatedEvent(true);

    #valueInput: HTMLInputElement | HTMLSelectElement;
    #onInput: (evt: Event) => void;
    #validValuesByPropertyTypeId: Map<number, string[]>;
    #unitsData: Map<string, string[]>;
    #editor: HiddenEditorElement;
    #abbreviations: Map<string, string>;

    constructor() {
        super();

        singleton("PropertyEditorElement::StyleSheet", () => {
            document.head.append(StyleBlob(
                rule("property-editor",
                    display("contents")
                ),
                rule("property-editor > div",
                    display("flex"),
                    flexDirection("row"),
                    columnGap(em(.25))
                )
            ));
            return true;
        });

        this.#onInput = this.#_onInput.bind(this);
        const id = stringRandom(10);
        this.#label = Label(For(id));

        this.#typeSelector = TypedSelect(
            LabelField<PropertyTypeModel>(v => v.name),
            ValueField<PropertyTypeModel>(v => v.name),
            SortKeyField<PropertyTypeModel>(v => v.name),
            PlaceHolder("Property Type"),
            TitleAttr("Property Type"),
            OnInput(() => {
                this.#typeSelector.title = this.#typeSelector.selectedItem?.dataType ?? "Property Type";
                this.#render();
            })
        );

        this.#editor = HiddenEditor(
            this.#infoLabel = Div(SlotAttr("default")),
            Div(SlotAttr("editor"),
                this.#valueInput = Input(ID(id)),
                this.#unitsSelector = TypedSelect(
                    PlaceHolder("Units"),
                    TitleAttr("Units"),
                    OnInput(() => this.#convertValue())
                ),
                this.#classifier = ClassificationEditor()
            ),
            OnUpdated(() => {
                if (this.#item) {
                    this.#item.value = getPropertyValueFromInput(this.#valueInput);
                    this.#item.units = this.#unitsSelector.selectedItem;
                    this.#item.classification = this.#classifier.selectedClassification;
                }
                this.#render();
                this.#onUpdate();
            })
        );

        this.#getData();
    }

    async #getData() {
        const [values, units, abbreviations] = await singleton("PropertyEditorElement::Data", () =>
            CedrusDataAPI
                .dataSourceTask
                .then(ds =>
                    Promise.all([
                        ds.getPropertyTypeValidValues(),
                        ds.getUnitsData(),
                        ds.getUnitAbbreviations()
                    ])));

        this.#validValuesByPropertyTypeId = groupBy(
            values,
            v => v.propertyType.id,
            v => v.value
        )
        this.#unitsData = units;
        this.#abbreviations = abbreviations;
        this.#render();
    }

    #onUpdate() {
        this.dispatchEvent(this.#updated);
    }

    get propertyTypes() { return this.#typeSelector.data; }
    set propertyTypes(v) { this.#typeSelector.data = v; }

    connectedCallback() {
        if (!this.#label.isConnected) {
            this.append(
                this.#label,
                this.#editor
            );
        }
    }

    #item: PropertyModel;
    get item() { return this.#item; }
    set item(v) {
        this.#item = v;
        this.#lastUnits = null;
        this.#render();
    }

    #itemType: PropertyTypeModel;

    get itemType() {
        return this.#item?.type
            ?? this.#itemType
            ?? this.#typeSelector.selectedItem;
    }

    set itemType(v) {
        this.#itemType = v;
        this.#lastUnits = null;
        this.#render();
    }

    #render() {
        if (this.#validValuesByPropertyTypeId && this.#unitsData) {
            this.#infoLabel.innerHTML = "";

            if (this.#item?.type || this.#itemType) {
                this.#label.replaceChildren(
                    SpanTag(
                        TitleAttr(this.itemType.dataType),
                        this.itemType.name
                    )
                );
            }
            else if (!this.#typeSelector.isConnected) {
                this.#label.replaceChildren(this.#typeSelector);
            }

            if (this.itemType) {
                const oldInput = this.#valueInput;
                const oldValue = oldInput.value;
                oldInput.removeEventListener("input", this.#onInput);

                const newInput = this.item
                    ? makeInputForProperty(this.item, this.#validValuesByPropertyTypeId)
                    : makeInputForPropertyType(this.itemType, this.#validValuesByPropertyTypeId);

                newInput.id = oldInput.id;
                newInput.addEventListener("input", this.#onInput);

                oldInput.replaceWith(newInput);

                this.#valueInput = newInput;

                this.#unitsSelector.title = this.itemType.unitsCategory;
                this.#unitsSelector.data = this.#unitsData.get(this.itemType.unitsCategory);
                this.#unitsSelector.style.visibility = this.itemType.unitsCategory === "None"
                    ? "hidden"
                    : "visible";

                this.#typeSelector.selectedItem = this.itemType;

                if (this.item) {
                    this.#lastUnits = this.#unitsSelector.value = this.item.units;
                    this.#classifier.selectedClassification = this.item.classification;
                    if (isPropertyModel(this.item, "File") || isPropertyModel(this.item, "Link")) {
                        this.#infoLabel.append(A(
                            HRef(this.item.value),
                            "link"
                        ));
                    }
                }
                else if (newInput.type !== "file") {
                    newInput.value = oldValue;
                }
            }

            this.#infoLabel.append(this.#valueInput.value);
            const units = this.#unitsSelector.value;
            const abbrev = this.#abbreviations.get(units) ?? ""
            this.#infoLabel.append(" " + abbrev);

            if (this.#classifier.selectedClassification) {
                this.#infoLabel.append(` (${this.#classifier.selectedClassification.name})`);
            }
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

    get setPropertyInput(): SetPropertyInput {
        return {
            type: { id: this.#typeSelector.selectedItem?.id },
            classification: { id: this.#classifier.selectedClassification?.id },
            value: getPropertyValueFromInput(this.#valueInput),
            unitOfMeasure: this.#unitsSelector.value || "None"
        };
    }

    #_onInput(_evt: Event) {
    }
}


customElements.define("property-editor", PropertyEditorElement);