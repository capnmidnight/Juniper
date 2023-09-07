import { PriorityList } from "@juniper-lib/collections/PriorityList";
import { ClassList, CustomData, isAttr } from "@juniper-lib/dom/attrs";
import { isCssElementStyleProp } from "@juniper-lib/dom/css";
import { DD, DL, DT, H2, Label, HtmlRender, elementIsDisplayed, elementSetClass, elementSetDisplay, getElements, isDisableable, resolveElement } from "@juniper-lib/dom/tags";
import { identity } from "@juniper-lib/tslib/identity";
import { stringRandom } from "@juniper-lib/tslib/strings/stringRandom";
import { isArray, isBoolean, isDate, isDefined, isNumber, isString } from "@juniper-lib/tslib/typeChecks";
import "./styles.css";
class PropertyGroup {
    constructor(name, ...properties) {
        this.name = name;
        this.properties = properties;
    }
}
export function group(name, ...properties) {
    return new PropertyGroup(name, ...properties);
}
const DEFAULT_PROPERTY_GROUP = "DefaultPropertyGroup" + stringRandom(16);
const singleItem = ClassList("single-item");
function createElements(rest) {
    return rest.flatMap((entry) => createRows(entry)
        .flatMap(identity));
}
function createRows(entry) {
    let groupName = DEFAULT_PROPERTY_GROUP;
    const rows = new Array();
    if (entry instanceof PropertyGroup) {
        groupName = entry.name;
        rows.push(...entry.properties.map((e) => createRow(groupName, e)));
    }
    else {
        rows.push(createRow(groupName, entry));
    }
    return rows;
}
function createRow(groupName, entry) {
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
function isPropertyDef(obj) {
    return isDefined(obj)
        && !isCssElementStyleProp(obj)
        && !isAttr(obj);
}
export class PropertyList {
    static find() {
        return Array.from(PropertyList._find());
    }
    static *_find() {
        for (const elem of getElements(".properties")) {
            yield new PropertyList(elem);
        }
    }
    static create(...rest) {
        const props = rest.filter(isPropertyDef);
        const styles = rest.filter(isCssElementStyleProp);
        const attrs = rest.filter(isAttr);
        const rows = createElements(props);
        return new PropertyList(DL(ClassList("properties"), ...styles, ...attrs, ...rows));
    }
    constructor(element) {
        this.element = element;
        this.groups = new PriorityList();
        this.controls = new Array();
        this._disabled = false;
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
    append(...props) {
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
    checkGroup(row) {
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
    setGroupVisible(id, v) {
        const elems = this.groups.get(id);
        if (elems) {
            for (const elem of elems) {
                elementSetDisplay(elem, v);
            }
        }
    }
    getGroupVisible(id) {
        const elems = this.groups.get(id);
        if (elems) {
            for (const elem of elems) {
                return elementIsDisplayed(elem);
            }
        }
        return false;
    }
}
//# sourceMappingURL=index.js.map