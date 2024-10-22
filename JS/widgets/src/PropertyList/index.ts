import { PriorityList } from "@juniper-lib/collections/dist/PriorityList";
import { HtmlAttr, ClassList, CustomData, isAttr } from "@juniper-lib/dom/dist/attrs";
import { CssElementStyleProp, isCssElementStyleProp } from "@juniper-lib/dom/dist/css";
import {
    DD,
    DL,
    DT,
    ElementChild,
    Elements,
    ErsatzElement,
    H2,
    IDisableable,
    IElementAppliable,
    Label,
    HtmlRender,
    elementIsDisplayed,
    elementSetClass,
    elementSetDisplay,
    getElements,
    isDisableable,
    resolveElement
} from "@juniper-lib/dom/dist/tags";
import { identity } from "@juniper-lib/tslib/dist/identity";
import { stringRandom } from "@juniper-lib/tslib/dist/strings/stringRandom";
import {
    isArray,
    isBoolean,
    isDate,
    isDefined,
    isNumber,
    isString
} from "@juniper-lib/tslib/dist/typeChecks";

import "./styles.css";


type PropertyChild = Exclude<ElementChild<HTMLElement>, IElementAppliable>;
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

export type Property = PropertyElement | PropertyGroup;
export type PropertyDef = Property | HtmlAttr| CssElementStyleProp;
type Row = Elements<HTMLElement>[];

const DEFAULT_PROPERTY_GROUP = "DefaultPropertyGroup" + stringRandom(16);
const singleItem = ClassList("single-item");

function createElements(rest: Property[]) {
    return rest.flatMap((entry) =>
        createRows(entry)
            .flatMap(identity));
}

function createRows(entry: Property): Row[] {
    let groupName: string = DEFAULT_PROPERTY_GROUP;
    const rows = new Array<Row>();

    if (entry instanceof PropertyGroup) {
        groupName = entry.name;
        rows.push(...entry.properties.map((e) => createRow(groupName, e)));
    }
    else {
        rows.push(createRow(groupName, entry));
    }

    return rows;
}

function createRow(groupName: string, entry: PropertyElement): Elements<HTMLElement>[] {
    const group = groupName === DEFAULT_PROPERTY_GROUP
        ? null
        : CustomData("groupname", groupName);

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
            DT(group, label),
            DD(group, ...fields)
        ];
    }
    else {

        if (isString(entry)
            || isNumber(entry)
            || isBoolean(entry)
            || isDate(entry)) {
            entry = H2(entry);
        }

        return [
            DD(group, entry)
        ];
    }
}


function isPropertyDef(obj: PropertyDef): obj is Property {
    return isDefined(obj)
        && !isCssElementStyleProp(obj)
        && !isAttr(obj);
}

export class PropertyList
    implements ErsatzElement {

    private readonly groups = new PriorityList<string, Elements<HTMLElement>>();
    private readonly controls = new Array<IDisableable>();
    private _disabled = false;

    public static find() {
        return Array.from(PropertyList._find());
    }

    private static *_find() {
        for (const elem of getElements(".properties")) {
            yield new PropertyList(elem);
        }
    }

    public static create(...rest: PropertyDef[]) {
        const props = rest.filter(isPropertyDef);
        const styles = rest.filter(isCssElementStyleProp);
        const attrs = rest.filter(isAttr);
        const rows = createElements(props);

        return new PropertyList(DL(
            ClassList("properties"),
            ...styles,
            ...attrs,
            ...rows));
    }

    constructor(public readonly element: HTMLElement) {
        const queue = [...element.children];
        while (queue.length > 0) {
            const child = queue.shift();

            if (isDisableable(child)) {
                this.controls.push(child);
            }

            if (child instanceof HTMLElement) {
                this.checkGroup(child);
                queue.push(...child.children);
            }
        }
    }

    append(...props: Property[]): void {
        const rows = createElements(props);
        HtmlRender(this.element, ...rows);

        for (const propDef of props) {
            const props = propDef instanceof PropertyGroup
                ? propDef.properties
                : [propDef];
            for (const prop of props) {
                if (!isString(prop)) {
                    const [_, ...elems] = isArray(prop)
                        ? prop
                        : [null, prop];
                    for (const elem of elems) {
                        if (isDisableable(elem)) {
                            this.controls.push(elem);
                        }
                    }
                }
            }
        }

        for (const row of rows) {
            this.checkGroup(row);
        }
    }

    private checkGroup(row: Elements<HTMLElement>) {
        const elem = resolveElement(row);
        const groupName = elem.dataset["groupname"];
        if (groupName !== DEFAULT_PROPERTY_GROUP) {
            this.groups.add(groupName, row);
        }

        if (elem.parentElement === this.element
            && elem.tagName === "DD"
            && (!elem.previousElementSibling
                || elem.previousElementSibling.tagName !== "DT")) {
            singleItem.applyToElement(elem);
        }
    }

    get disabled() { return this._disabled; }
    set disabled(v) {
        if (v !== this.disabled) {
            this._disabled = v;
            elementSetClass(this, v, "disabled");
            for (const control of this.controls) {
                control.disabled = v;
            }
        }
    }

    get enabled() { return !this.disabled; }
    set enabled(v) { this.disabled = !v; }

    setGroupVisible(id: string, v: boolean): void {
        const elems = this.groups.get(id);
        if (elems) {
            for (const elem of elems) {
                elementSetDisplay(elem, v);
            }
        }
    }

    getGroupVisible(id: string): boolean {
        const elems = this.groups.get(id);
        if (elems) {
            for (const elem of elems) {
                return elementIsDisplayed(elem);
            }
        }

        return false;
    }
}