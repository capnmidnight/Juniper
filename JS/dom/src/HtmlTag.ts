import { arrayRemove, Exception } from "@juniper-lib/util";
import { AbstractAppliable } from "./AbstractAppliable";
import { HtmlIDAttr, HtmlIsAttr } from "./attrs";
import { AbstractKeyValueAppliable } from "./attrs/AbstractKeyValueAppliable";
import { HtmlQueryAttr, HtmlRender } from "./util";
import { ErsatzNode } from "./ErsatzNode";

export type RawElementChild =
    | Node
    | string
    | number
    | Date;

export type ElementChild<T extends Node = Node> =
    | RawElementChild
    | ErsatzNode<Node>
    | AbstractAppliable<T>;

export type ElementFactory<ElementT extends Node = Node> = (...rest: ElementChild<ElementT>[]) => ElementT;

type FinderAttr = HtmlIDAttr | HtmlQueryAttr;
type HtmlFinderAttr = FinderAttr | HtmlIsAttr;

export function isFinderAttr(v: ElementChild): v is FinderAttr {
    return v instanceof AbstractKeyValueAppliable
        && (v.name === "id"
            || v.name === "querySelector");
}

export function isHtmlFinderAttr(v: ElementChild): v is HtmlFinderAttr {
    return isFinderAttr(v)
        || v instanceof AbstractKeyValueAppliable && v.name === "is";
}

/**
 * Creates an DOM element.
 */
export function HtmlTag<T extends HTMLElement>(name: string, ...sub: ElementChild<T>[]): T {
    sub = sub.flat(1);

    let tag: T;

    const finders = sub.filter(isHtmlFinderAttr);

    let is: string = null;

    for (const finder of finders) {
        if (finder instanceof HtmlIDAttr) {
            tag = document.getElementById(finder.value) as T; // Cast to string since it has to be a string. 
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
            tag = finder.value as T;
        }

        if (tag) {
            break;
        }
    }

    if (!tag) {
        if (is) {
            tag = document.createElement(name, { is }) as T;
        }
        else {
            tag = document.createElement(name) as T;
        }
    }

    return HtmlRender(tag, ...sub);
}



/**
 * Creates an SVG element.
 * @param name the name of the element to create
 * @param sub attribute assigner functions and sub element children objects.
 */
export function SvgTag<T extends keyof SVGElementTagNameMap>(name: T, ...sub: ElementChild[]): SVGElementTagNameMap[T] {
    sub = sub.flat(1);

    let tag: SVGElementTagNameMap[T];

    const finders = sub.filter(isFinderAttr);

    for (const finder of finders) {
        if (finder.name === "id") {
            const id = finder.value?.toString();
            if (id) {
                tag = document.getElementById(id) as unknown as SVGElementTagNameMap[T];
                if (tag) {
                    arrayRemove(sub, finder);
                }
            }
        }
        else {
            arrayRemove(sub, finder);
            tag = finder.value as unknown as SVGElementTagNameMap[T];
        }

        if (tag) {
            break;
        }
    }

    if (!tag) {
        tag = document.createElementNS<T>("http://www.w3.org/2000/svg", name);
    }

    return HtmlRender(tag, ...sub);
}