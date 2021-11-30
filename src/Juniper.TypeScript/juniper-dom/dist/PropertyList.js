import { isArray, isBoolean, isDate, isNumber, isString, stringRandom } from "juniper-tslib";
import { display, gridAutoFlow, gridColumn, gridTemplateColumns, margin, marginInlineStart, paddingRight, rule, textAlign, width } from "./css";
import { DD, Div, DL, DT, elementApply, elementSetDisplay, H2, isErsatzElement, isErsatzElements, Label, Style } from "./tags";
class PropertyGroup {
    name;
    properties;
    constructor(name, ...properties) {
        this.name = name;
        this.properties = properties;
    }
}
export function group(name, ...properties) {
    return new PropertyGroup(name, ...properties);
}
Style(rule("dl", display("grid"), gridAutoFlow("row"), gridTemplateColumns("auto 1fr"), margin("1em")), rule("dt", gridColumn("1/2"), textAlign("right"), paddingRight("1em")), rule("dd", textAlign("left"), gridColumn("2/3"), marginInlineStart("0")), rule("dl > span, dl > div", gridColumn("1/3")), rule("dl .alert", width("20em")));
const DEFAULT_PROPERTY_GROUP = "DefaultPropertyGroup" + stringRandom(16);
export class PropertyList {
    element;
    rowGroups = new Map();
    constructor(...rest) {
        this.element = DL(...this.createElements(rest));
    }
    append(...rest) {
        elementApply(this.element, ...this.createElements(rest));
    }
    createElements(rest) {
        return rest.flatMap((entry) => this.createGroups(entry)
            .flatMap(e => e));
    }
    createGroups(entry) {
        let name = DEFAULT_PROPERTY_GROUP;
        const group = new Array();
        if (entry instanceof PropertyGroup) {
            name = entry.name;
            group.push(...entry.properties.map(e => this.createRow(e)));
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
    createRow(entry) {
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
    setGroupVisible(id, v) {
        const rows = this.rowGroups.get(id);
        if (rows) {
            for (const row of rows) {
                for (const elem of row) {
                    if (isErsatzElements(elem)) {
                        for (const e of elem.elements) {
                            elementSetDisplay(e, v, "block");
                        }
                    }
                    else if (isErsatzElement(elem) || elem instanceof HTMLElement) {
                        elementSetDisplay(elem, v, "block");
                    }
                }
            }
        }
    }
}
export function PropList(...rest) {
    return new PropertyList(...rest);
}
