import { arrayReplace, compareBy, compareCallback, identity, singleton } from "@juniper-lib/util";
import { Button, ClassList, Div, ElementChild, H1, H4, HtmlTag, ID, InputText, Label, OnClick, P, PlaceHolder, SlotAttr, StyleBlob, TypedHTMLElement, borderRadius, content, display, gap, marginLeft, rule } from "@juniper-lib/dom";
import { CompareBy, InclusionList, InclusionListElement, LabelField, SortKeyField, StandardDialog, StandardDialogElement, TypedInput, TypedInputElement, TypedSelect, TypedSelectElement, UpdatedEvent, ValueField } from "@juniper-lib/widgets";
import { CedrusDataAPI } from "../adapters";
import { ClassificationCaveatModel, ClassificationLevelModel, ClassificationModel, SetClassificationCaveatInput, SetClassificationInput } from "../models";

document.head.append(
    StyleBlob(
        rule("classification-editor button",
            borderRadius("5px"),
            marginLeft("2px")
        ),
        rule(".modal-section-container",
            display("grid"),
            gap("11px")
        ),
        rule("#newCaveatBlock",
            display("flex"),
            gap("20px")
        ),
        rule("#newCaveatBlock label::after",
            content(":")
        ),
        rule(".new-caveat-field",
            marginLeft("5px")
        ),
        rule("#newCaveatBlock button",
            borderRadius("5px")
        )
    )
);


export function ClassificationEditor(...rest: ElementChild[]): ClassificationEditorElement {
    return HtmlTag("classification-editor", ...rest);
}

const defaultCompare = compareBy<string>(identity);
const compareClassificationName: compareCallback<string> = (a: string, b: string) => {
    if (a === "U" && b === "U") {
        return 0;
    }
    else if (a === "U") {
        return -1;
    }
    else if (b === "U") {
        return 1;
    }
    else {
        return defaultCompare(a, b);
    }
}

class ClassificationDataManager extends EventTarget {

    readonly #levels: ClassificationLevelModel[] = [];
    get levels() { return this.#levels; }

    readonly #caveats: ClassificationCaveatModel[] = [];
    get caveats() { return this.#caveats; }

    readonly #classifications: ClassificationModel[] = [];
    get classifications() { return this.#classifications; }

    readonly #updated = new UpdatedEvent();

    constructor() {
        super();
        this.#getData();
    }

    async #getData() {
        const ds = await CedrusDataAPI.dataSourceTask;
        const [
            classifications,
            levels,
            caveats
        ] = await Promise.all([
            ds.getClassifications(),
            ds.getClassificationLevels(),
            ds.getClassificationCaveats()
        ]);

        arrayReplace(this.#levels, levels);
        arrayReplace(this.#classifications, classifications);
        arrayReplace(this.#caveats, caveats);

        this.#onUpdate();
    }

    #onUpdate() {
        this.dispatchEvent(this.#updated);
    }

    async addCaveat(input: SetClassificationCaveatInput) {
        const ds = await CedrusDataAPI.dataSourceTask;
        const caveat = await ds.setClassificationCaveat(input);
        this.#caveats.push(caveat);
        this.#onUpdate();
        return caveat;
    }

    async addClassification(input: SetClassificationInput) {
        const ds = await CedrusDataAPI.dataSourceTask;
        const classification = await ds.setClassification(input);
        this.#classifications.push(classification);
        this.#onUpdate();
        return classification;
    }
}


type ClassificationEditorEventMap = {
    "updated": UpdatedEvent<ClassificationEditorElement>
}

export class ClassificationEditorElement extends TypedHTMLElement<ClassificationEditorEventMap> {

    readonly #selector: TypedInputElement<ClassificationModel>;
    readonly #addButton: HTMLButtonElement;
    readonly #dialog: StandardDialogElement;
    readonly #levelSelector: TypedSelectElement<ClassificationLevelModel>;
    readonly #caveatSelector: InclusionListElement<ClassificationCaveatModel>;
    readonly #newCaveatName: HTMLInputElement;
    readonly #newCaveatDescription: HTMLInputElement;
    readonly #newCaveatLevelSelector: TypedSelectElement<ClassificationLevelModel>;
    readonly #newCaveatButton: HTMLButtonElement;

    #dataManager: ClassificationDataManager;

    get selectedClassification() { return this.#selector.selectedItem; }
    set selectedClassification(v) { this.#selector.selectedItem = v; }


    constructor() {
        super();

        this.#selector = TypedInput(
            LabelField<ClassificationModel>(c => c.name),
            ValueField<ClassificationModel>(c => c.name),
            CompareBy<ClassificationModel>((a, b) => compareClassificationName(a.name, b.name)),
            PlaceHolder("Classification")
        );

        this.#addButton = Button("+", OnClick(async () => {
            this.#dialog.show();
            try {
                if (!(await this.#dialog.cancelled())) {
                    const classification = await this.#dataManager.addClassification({
                        caveats: this.#caveatSelector.selectedItems,
                        level: this.#levelSelector.selectedItem
                    });
                    this.#selector.selectedItem = classification;
                }
            }
            finally {
                this.#dialog.close();
            }
        }));

        this.#dialog = StandardDialog(
            H1(SlotAttr("modal-title"), "Add New Classification"),
            Div(
                SlotAttr("modal-body"),
                ClassList("modal-body"),
                Div(
                    ClassList("modal-section-container"),
                    Div(
                        ClassList("modal-section"),
                        H4("Classication Level"),
                        P("Select primary level of classification"),
                        this.#levelSelector = TypedSelect(
                            PlaceHolder("Level"),
                            LabelField<ClassificationLevelModel>(c => c.description),
                            ValueField<ClassificationLevelModel>(c => c.name),
                            CompareBy<ClassificationLevelModel>((a, b) => compareClassificationName(a.name, b.name))
                        )),
                    Div(
                        ClassList("modal-section"),
                        H4("Caveat"),
                        P("Select restrictive caveats from the list"),
                        this.#caveatSelector = InclusionList(
                            LabelField<ClassificationCaveatModel>(c => c.description),
                            ValueField<ClassificationCaveatModel>(c => c.name),
                            SortKeyField<ClassificationLevelModel>(c => c.name)
                        )),
                    Div(
                        ClassList("modal-section"),
                        H4("Create new Caveat"),
                        P("Create a new Caveat to add to the Caveat list"),
                        Div(
                            ID("newCaveatBlock"),
                            Label("Name", this.#newCaveatName = InputText(ClassList("new-caveat-field"))),
                            Label("Description", this.#newCaveatDescription = InputText(ClassList("new-caveat-field"))),
                            Label("Level", this.#newCaveatLevelSelector = TypedSelect(
                                ClassList("new-caveat-field"),
                                PlaceHolder("Caveat Classification Level"),
                                LabelField<ClassificationLevelModel>(c => c.name),
                                ValueField<ClassificationLevelModel>(c => c.name),
                                CompareBy<ClassificationLevelModel>((a, b) => compareClassificationName(a.name, b.name)))),

                            this.#newCaveatButton = Button("Add new Caveat", OnClick(async () => {
                                this.#newCaveatButton.disabled
                                    = this.#newCaveatDescription.disabled
                                    = this.#newCaveatLevelSelector.disabled
                                    = this.#newCaveatName.disabled
                                    = true;
                                try {
                                    const name = this.#newCaveatName.value;
                                    const description = this.#newCaveatDescription.value;
                                    const level = this.#newCaveatLevelSelector.selectedItem;
                                    const selected = this.#caveatSelector.selectedItems;
                                    const caveat = await this.#dataManager.addCaveat({
                                        name,
                                        description,
                                        classificationLevel: level
                                    });
                                    selected.push(caveat);
                                    this.#caveatSelector.selectedItems = selected;
                                }
                                finally {
                                    this.#newCaveatButton.disabled
                                        = this.#newCaveatDescription.disabled
                                        = this.#newCaveatLevelSelector.disabled
                                        = this.#newCaveatName.disabled
                                        = false;
                                }
                            }))
                        )
                    )
                )
            )
        );

        this.#dataManager = singleton("ClassificationEditorElement::ClassificationDataManager", () => new ClassificationDataManager());
        this.#dataManager.addEventListener("updated", () => this.#refresh());
        CedrusDataAPI.dataSourceTask.then(() => this.#refresh());
    }

    #refresh() {
        this.#selector.data = this.#dataManager.classifications;
        this.#levelSelector.data = this.#dataManager.levels;
        this.#newCaveatLevelSelector.data = this.#dataManager.levels;
        this.#caveatSelector.data = this.#dataManager.caveats;
    }

    connectedCallback() {
        if (!this.#selector.isConnected) {
            this.append(
                this.#selector,
                this.#addButton,
                this.#dialog
            );

            this.#dialog.saveButtonText = "OK";
        }
    }
}

customElements.define("classification-editor", ClassificationEditorElement);