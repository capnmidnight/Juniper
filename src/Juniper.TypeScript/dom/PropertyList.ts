import { identity, isArray, isBoolean, isDate, isNumber, isString, stringRandom } from "juniper-tslib";
import { display, gridAutoFlow, gridColumn, gridTemplateColumns, margin, marginInlineStart, paddingRight, rule, textAlign, width } from "./css";
import { DD, Div, DL, DT, elementApply, ElementChild, elementSetDisplay, ErsatzElement, H2, isErsatzElement, isErsatzElements, Label, Style } from "./tags";

type PropertyElement = [string, ...ElementChild[]] | string | ElementChild;

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
type RowElement = Exclude<ElementChild, string | number | boolean | Date>;
type Row = Array<RowElement>;

Style(
    rule("dl",
        display("grid"),
        gridAutoFlow("row"),
        gridTemplateColumns("auto 1fr"),
        margin("1em")
    ),

    rule("dt",
        gridColumn(1),
        textAlign("right"),
        paddingRight("1em")
    ),

    rule("dd",
        textAlign("left"),
        gridColumn(2),
        marginInlineStart("0")
    ),

    rule("dl > span, dl > div",
        gridColumn(1, 3)
    ),

    rule("dl .alert",
        width("20em"))
);

const DEFAULT_PROPERTY_GROUP = "DefaultPropertyGroup" + stringRandom(16);

export class PropertyList
    implements ErsatzElement {

    public readonly element: HTMLElement;
    private readonly rowGroups = new Map<string, Row[]>();

    constructor(...rest: Property[]) {
        this.element = DL(...this.createElements(rest));
    }

    append(...rest: Property[]): void {
        elementApply(this.element, ...this.createElements(rest));
    }

    private createElements(rest: Property[]) {
        return rest.flatMap((entry) =>
            this.createGroups(entry)
                .flatMap(identity));
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

export function PropList(...rest: Property[]) {
    return new PropertyList(...rest);
}