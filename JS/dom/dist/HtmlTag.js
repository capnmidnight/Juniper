import { arrayRemove, Exception } from "@juniper-lib/util";
import { HtmlIDAttr, HtmlIsAttr } from "./attrs";
import { AbstractKeyValueAppliable } from "./attrs/AbstractKeyValueAppliable";
import { HtmlRender } from "./util";
export function isFinderAttr(v) {
    return v instanceof AbstractKeyValueAppliable
        && (v.name === "id"
            || v.name === "querySelector");
}
export function isHtmlFinderAttr(v) {
    return isFinderAttr(v)
        || v instanceof AbstractKeyValueAppliable && v.name === "is";
}
/**
 * Creates an DOM element.
 */
export function HtmlTag(name, ...sub) {
    sub = sub.flat(1);
    let tag;
    const finders = sub.filter(isHtmlFinderAttr);
    let is = null;
    for (const finder of finders) {
        if (finder instanceof HtmlIDAttr) {
            tag = document.getElementById(finder.value); // Cast to string since it has to be a string. 
            if (tag) {
                if (tag.tagName !== name.toUpperCase()) {
                    throw new Exception(`The element retrieved by ID did not match the expected tag type. Given: ${tag.tagName}. Expected ${name}.`);
                }
                arrayRemove(sub, finder);
            }
            else if (finder.required) {
                throw new Exception(`Element ${name}#${finder.value} not found.`);
            }
        }
        else if (finder instanceof HtmlIsAttr) {
            is = finder.value;
        }
        else {
            arrayRemove(sub, finder);
            tag = finder.value;
        }
        if (tag) {
            break;
        }
    }
    if (!tag) {
        if (is) {
            tag = document.createElement(name, { is });
        }
        else {
            tag = document.createElement(name);
        }
    }
    return HtmlRender(tag, ...sub);
}
/**
 * Creates an SVG element.
 * @param name the name of the element to create
 * @param sub attribute assigner functions and sub element children objects.
 */
export function SvgTag(name, ...sub) {
    sub = sub.flat(1);
    let tag;
    const finders = sub.filter(isFinderAttr);
    for (const finder of finders) {
        if (finder.name === "id") {
            const id = finder.value?.toString();
            if (id) {
                tag = document.getElementById(id);
                if (tag) {
                    arrayRemove(sub, finder);
                }
            }
        }
        else {
            arrayRemove(sub, finder);
            tag = finder.value;
        }
        if (tag) {
            break;
        }
    }
    if (!tag) {
        tag = document.createElementNS("http://www.w3.org/2000/svg", name);
    }
    return HtmlRender(tag, ...sub);
}
//# sourceMappingURL=HtmlTag.js.map