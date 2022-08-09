import { className } from "@juniper-lib/dom/attrs";
import { color, display, gridAutoFlow, gridColumn, gridColumnGap, gridTemplateColumns, rule, textAlign, width } from "@juniper-lib/dom/css";
import { DD, Div, DL, DT, elementApply, ElementChild, elementSetClass, elementSetDisplay, ErsatzElement, H2, IDisableable, IElementAppliable, isDisableable, isErsatzElement, isErsatzElements, Label, resolveElement, Style } from "@juniper-lib/dom/tags";
import { identity, isArray, isBoolean, isDate, isNumber, isString, stringRandom } from "@juniper-lib/tslib";

type PropertyChild = Exclude<ElementChild, IElementAppliable>;
type PropertyElement = [string, ...PropertyChild[]] | string | PropertyChild;

class PropertyGroup {
    readonly properties: PropertyElement[];

    constructor(public readonly name: string, ...properties: PropertyElement[]) {
        this.properties = properties;
    }
}

export function group(name: string, ...properties: PropertyElement[]) {
    return new PropertyGroup(name, ...properties);
}

type Property = PropertyElement | PropertyGroup;
type RowElement = Exclude<PropertyChild, string | number | boolean | Date>;
type Row = Array<RowElement>;

Style(
    rule("dl.properties",
        display("grid"),
        gridAutoFlow("row"),
        gridTemplateColumns("auto 1fr"),
        gridColumnGap("1em")
    ),

    rule("dl.properties.disabled",
        color("#ccc")
    ),

    rule("dl.properties > span, dl.properties > div",
        gridColumn(1, 3),
        width("100%")
    ),

    rule("dl.properties > dt",
        gridColumn(1),
        textAlign("right")
    ),

    rule("dl.properties > dd",
        gridColumn(2),
        display("grid"),
        gridAutoFlow("row")
    ),

    rule("dl.properties input[type=number]",
        textAlign("right")
    )
);

const DEFAULT_PROPERTY_GROUP = "DefaultPropertyGroup" + stringRandom(16);

export class PropertyList
    implements ErsatzElement {

    public readonly element: HTMLElement;
    private readonly rowGroups = new Map<string, Row[]>();
    private readonly controls = new Array<IDisableable>();
    private _disabled = false;

    constructor(...rest: Property[]) {
        this.element = DL(
            className("properties"),
            ...this.createElements(rest));
    }

    append(...rest: Property[]): void {
        elementApply(this.element, ...this.createElements(rest));
    }

    get disabled() {
        return this._disabled;
    }

    set disabled(v) {
        if (v !== this.disabled) {
            this._disabled = v;
            elementSetClass(this, v, "disabled");
            for (const control of this.controls) {
                control.disabled = v;
            }
        }
    }

    get enabled() {
        return !this.disabled;
    }

    set enabled(v) {
        this.disabled = !v;
    }

    private createElements(rest: Property[]) {
        return rest.flatMap((entry) =>
            this.createGroups(entry)
                .flatMap(identity)
                .map((elem) => this.registerControl(elem)));
    }

    private registerControl(rowElem: RowElement): RowElement {
        const elems = new Array<HTMLElement>();
        if (isErsatzElements(rowElem)) {
            elems.push(...rowElem.elements.map(resolveElement));
        }
        else {
            elems.push(resolveElement(rowElem));
        }

        for (const elem of elems) {
            if (isDisableable(elem)) {
                this.controls.push(elem);
            }
        }
        return rowElem;
    }

    private createGroups(entry: Property): Row[] {
        let name: string = DEFAULT_PROPERTY_GROUP;
        const group = new Array<Row>();

        if (entry instanceof PropertyGroup) {
            name = entry.name;
            group.push(...entry.properties.map((e) => this.createRow(e)));
        }
        else {
            group.push(this.createRow(entry));
        }

        if (!this.rowGroups.has(name)) {
            this.rowGroups.set(name, []);
        }

        this.rowGroups.get(name).push(...group);
        return group;
    }

    private createRow(entry: PropertyElement): Row {
        if (isArray(entry)) {
            const [labelText, ...fields] = entry;
            const label = Label(labelText);
            for (const field of fields) {
                if (field instanceof HTMLInputElement
                    || field instanceof HTMLTextAreaElement
                    || field instanceof HTMLSelectElement) {
                    if (field.id.length === 0) {
                        field.id = stringRandom(10);
                    }
                    label.htmlFor = field.id;
                    break;
                }
            }

            return [
                DT(label),
                DD(...fields)
            ];
        }
        else if (isString(entry)
            || isNumber(entry)
            || isBoolean(entry)
            || isDate(entry)) {
            return [
                Div(H2(entry))
            ];
        }
        else {
            return [
                entry
            ];
        }
    }

    setGroupVisible(id: string, v: boolean): void {
        const rows = this.rowGroups.get(id);
        if (rows) {
            for (const row of rows) {
                for (const elem of row) {
                    if (isErsatzElements(elem)) {
                        for (const e of elem.elements) {
                            elementSetDisplay(e, v);
                        }
                    }
                    else if (isErsatzElement(elem) || elem instanceof HTMLElement) {
                        elementSetDisplay(elem, v);
                    }
                }
            }
        }
    }
}