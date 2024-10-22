import { first, isBoolean, isDefined, isObject } from "@juniper-lib/util";
import { AbstractAppliable } from "./AbstractAppliable";
import { Height, InnerHTML, StyleAttr, Width } from "./attrs";
import { AbstractKeyValueAppliable } from "./attrs/AbstractKeyValueAppliable";
import { px } from "./css";
import { isErsatzNode, resolveNode } from './ErsatzNode';
import { Canvas, Div } from "./tags";
/**********************************
 * UTILITIES
 *********************************/
/**
 * Set the document element and the body's scrollTop to 0
 */
export function scrollWindowToTop() {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}
class HtmlClearAttr extends AbstractAppliable {
    apply(tag) {
        tag.innerHTML = "";
    }
}
/**
 * Clears all children out of an element.
 */
export function Clear() { return new HtmlClearAttr(); }
class CssClearAttr extends AbstractAppliable {
    apply(tag) {
        tag.removeAttribute("style");
    }
}
export function ClearCss() { return new CssClearAttr(); }
export function Optional(test, ...rest) {
    if (!test) {
        return [];
    }
    return rest;
}
/**
 * Set the style.display property for a tag
 * @param selectorOrElement
 * @param type the display type to set. Defaults to "block"
 */
export function show(selectorOrElement, inType) {
    let element;
    if (typeof selectorOrElement === "string") {
        element = document.querySelector(selectorOrElement);
    }
    else {
        element = resolveNode(selectorOrElement);
    }
    if (!element) {
        console.warn(`Could not find element at ${selectorOrElement}.`);
        return;
    }
    const style = getComputedStyle(element);
    let type = inType;
    if (type === undefined
        && style.display === "none") {
        type = "block";
    }
    if (type !== style.display) {
        if (type) {
            element.style.display = type;
        }
        else {
            element.style.removeProperty("display");
        }
    }
}
/**
 * Set the style.display property for a tag to "none"
 */
export function hide(selectorOrElement) {
    show(selectorOrElement, "none");
}
export function elementSetDisplay(elem, visible, visibleDisplayType = "") {
    if (isDefined(elem)) {
        if (visible) {
            show(elem, visibleDisplayType);
        }
        else {
            hide(elem);
        }
    }
}
export function isDisplayed(elem) {
    elem = resolveNode(elem);
    const style = getComputedStyle(elem);
    return style.display !== "none";
}
export function elementToggleDisplay(elem, visibleDisplayType = "") {
    elementSetDisplay(elem, !isDisplayed(elem), visibleDisplayType);
}
async function ImportHTML(path) {
    const response = await fetch(path);
    let type = response.headers.get("content-type");
    if (type) {
        const commaIndex = type.indexOf(";");
        if (commaIndex >= 0) {
            type = type.substring(0, commaIndex);
        }
        if (type !== "text/html") {
            throw new Error(`Document at path "${path} was not HTML.`);
        }
    }
    const html = await response.text();
    return Div(InnerHTML(html));
}
export async function ImportTemplate(path) {
    const parser = await ImportHTML(path);
    return parser.querySelector("template");
}
export async function ImportDOM(path) {
    const parser = await ImportHTML(path);
    return Array.from(parser.children);
}
/**
 * Parses the given HTML text and returns the elements it defines.
 */
export function RawHtmlElements(html) {
    const parser = Div(InnerHTML(html));
    return Array.from(parser.children);
}
/**
 * Parses the given HTML text and returns the first element it defines.
 */
export function RawHtmlElement(html) {
    return first(RawHtmlElements(html));
}
/**
 * Builds an HTML structure in a tag.
 * @param node
 * @param  rest attribute assigner functions and sub element children objects.
 * @returns the same tag back out again
 */
export function HtmlRender(node, ...rest) {
    node = resolveNode(node);
    const target = node instanceof HTMLTemplateElement
        ? node.content
        : node;
    for (const v of rest) {
        if (isDefined(v)) {
            if (isErsatzNode(v)) {
                target.append(v.content);
            }
            else if (v instanceof Node) {
                target.append(v);
            }
            else if (v instanceof AbstractAppliable) {
                v.apply(target);
            }
            else {
                node.append(document.createTextNode(v.toString()));
            }
        }
    }
    return node;
}
function querySelect(rootOrSelector, selector) {
    let root;
    let pattern;
    if (typeof rootOrSelector === "string") {
        root = document;
        pattern = rootOrSelector;
    }
    else {
        root = resolveNode(rootOrSelector);
        pattern = selector;
    }
    if (!pattern) {
        throw new Error("No selector provided");
    }
    const tag = root.querySelector(pattern);
    return { root, pattern, tag };
}
export class HtmlQueryAttr extends AbstractKeyValueAppliable {
    constructor(errorOnNotFound, rootOrSelector, selector) {
        const { root, pattern, tag } = querySelect(rootOrSelector, selector);
        if (!tag) {
            console.warn(root, pattern);
            if (errorOnNotFound) {
                throw new Error(`Tag not found at "${pattern}".`);
            }
        }
        super("querySelector", tag);
    }
    apply(_node) {
        throw new Error("Not implemented");
    }
}
/**
 * A fake attribute that allows you to recover elements already in the document
 * via an document.querySelector(selector) call.
 * @param rootOrSelector If a string is provided, this parameter will be used as a query path on the document.
 * If an HTMLElement is provided, this parameter will be used as the root for the query.
 * @param selector If `rootOrSelector` is an HTMLElement, this parameter must be a string and will be used as the query path.
 */
export function MaybeQuery(rootOrSelector, selector) {
    return new HtmlQueryAttr(false, rootOrSelector, selector);
}
/**
 * A fake attribute that allows you to recover elements already in the document
 * via an document.querySelector(selector) call.
 * @param rootOrSelector If a string is provided, this parameter will be used as a query path on the document.
 * If an HTMLElement is provided, this parameter will be used as the root for the query.
 * @param selector If `rootOrSelector` is an HTMLElement, this parameter must be a string and will be used as the query path.
 */
export function Query(rootOrSelector, selector) {
    return new HtmlQueryAttr(true, rootOrSelector, selector);
}
/**
 * Retrieve elements already in the document
 * via an document.querySelectorAll(selector) call.
 * @param rootOrSelector If a string is provided, this parameter will be used as a query path on the document.
 * If an HTMLElement is provided, this parameter will be used as the root for the query.
 * @param selector * If `rootOrSelector` is an HTMLElement, this parameter must be a string and will be used as the query path.
 */
export function QueryAll(rootOrSelector, selector) {
    let root;
    if (typeof rootOrSelector === "string") {
        root = document;
        selector = rootOrSelector;
    }
    else {
        root = resolveNode(rootOrSelector);
    }
    if (!selector) {
        throw new Error("No selector provided");
    }
    return Array.from(root.querySelectorAll(selector));
}
/**
 * Insert an item into an array, making sure to handle the return value of binarySearch correctly
 * @returns the index at which the item was inserted.
 */
export function optionsInsert(arr, item, index) {
    if (index < 0) {
        index = -index - 1;
    }
    arr.add(item, index);
    return index;
}
export function* nodeWalkBreadthFirst(node) {
    node = resolveNode(node);
    const queue = [node];
    while (queue.length > 0) {
        const here = queue.shift();
        queue.push(...Array.from(here.children));
        yield here;
    }
}
export function* nodeWalkDepthFirst(node) {
    node = resolveNode(node);
    const stack = [node];
    while (stack.length > 0) {
        const here = stack.pop();
        stack.push(...Array.from(here.children));
        yield here;
    }
}
export function elementComputeBounds(element, cache) {
    element = resolveNode(element);
    if (cache && cache.has(element)) {
        return cache.get(element);
    }
    const boundingRect = element.getBoundingClientRect();
    const styles = getComputedStyle(element);
    if (boundingRect.width * boundingRect.height === 0 || styles.display === "none") {
        return null;
    }
    const sMarginTop = parseFloat(styles.marginTop);
    const sMarginRight = parseFloat(styles.marginRight);
    const sMarginBottom = parseFloat(styles.marginBottom);
    const sMarginLeft = parseFloat(styles.marginLeft);
    const sPaddingTop = parseFloat(styles.paddingTop);
    const sPaddingRight = parseFloat(styles.paddingRight);
    const sPaddingBottom = parseFloat(styles.paddingBottom);
    const sPaddingLeft = parseFloat(styles.paddingLeft);
    const sBorderTop = parseFloat(styles.borderTopWidth);
    const sBorderRight = parseFloat(styles.borderRightWidth);
    const sBorderBottom = parseFloat(styles.borderBottomWidth);
    const sBorderLeft = parseFloat(styles.borderLeftWidth);
    const borderLeft = boundingRect.x;
    const borderTop = boundingRect.y;
    const borderWidth = boundingRect.width;
    const borderHeight = boundingRect.height;
    const borderRight = borderLeft + borderWidth;
    const borderBottom = borderTop + borderHeight;
    const marginLeft = borderLeft - sMarginLeft;
    const marginTop = borderTop - sMarginTop;
    const paddingLeft = borderLeft + sBorderLeft;
    const paddingTop = borderTop + sBorderTop;
    const interiorLeft = paddingLeft + sPaddingLeft;
    const interiorTop = paddingTop + sPaddingTop;
    const marginRight = borderRight + sMarginRight;
    const marginBottom = borderBottom + sMarginBottom;
    const paddingRight = borderRight - sBorderRight;
    const paddingBottom = borderBottom - sBorderBottom;
    const interiorRight = paddingRight - sPaddingRight;
    const interiorBottom = paddingBottom - sPaddingBottom;
    const marginWidth = marginRight - marginLeft;
    const marginHeight = marginBottom - marginTop;
    const paddingWidth = paddingRight - paddingLeft;
    const paddingHeight = paddingBottom - paddingTop;
    const interiorWidth = interiorRight - interiorLeft;
    const interiorHeight = interiorBottom - interiorTop;
    const bounds = {
        styles,
        margin: {
            left: marginLeft,
            right: marginRight,
            top: marginTop,
            bottom: marginBottom,
            width: marginWidth,
            height: marginHeight
        },
        border: {
            left: borderLeft,
            top: borderTop,
            right: borderRight,
            bottom: borderBottom,
            width: borderWidth,
            height: borderHeight
        },
        padding: {
            left: paddingLeft,
            top: paddingTop,
            right: paddingRight,
            bottom: paddingBottom,
            width: paddingWidth,
            height: paddingHeight
        },
        interior: {
            left: interiorLeft,
            top: interiorTop,
            right: interiorRight,
            bottom: interiorBottom,
            width: interiorWidth,
            height: interiorHeight
        }
    };
    if (cache) {
        cache.set(element, bounds);
    }
    return bounds;
}
function mergeBounds(a, b) {
    const left = Math.min(a.left, b.left);
    const right = Math.max(a.right, b.right);
    const top = Math.min(a.top, b.top);
    const bottom = Math.max(a.bottom, b.bottom);
    return {
        left,
        right,
        top,
        bottom,
        width: right - left,
        height: bottom - top
    };
}
export function elementMaxBounds(element, cache) {
    let maxBounds = {
        left: Number.MAX_VALUE,
        right: -Number.MAX_VALUE,
        top: Number.MAX_VALUE,
        bottom: -Number.MAX_VALUE,
        width: null,
        height: null
    };
    for (const here of nodeWalkDepthFirst(element)) {
        const bounds = elementComputeBounds(here, cache);
        if (!bounds) {
            continue;
        }
        maxBounds = mergeBounds(maxBounds, bounds.margin);
        maxBounds = mergeBounds(maxBounds, bounds.border);
        maxBounds = mergeBounds(maxBounds, bounds.padding);
        maxBounds = mergeBounds(maxBounds, bounds.interior);
    }
    if (!maxBounds.width || !maxBounds.height) {
        return null;
    }
    return maxBounds;
}
const DEFAULT_RESOLUTION = 300;
/**
 * Render an element to a canvas
 * @param element the element to render
 * @param resolution the number of pixels to render per inch. Defaults to 300 (a typical resolution for printing)
 * @returns
 */
export function renderHTMLElement(element, resolution = DEFAULT_RESOLUTION) {
    const cache = new Map();
    const maxBounds = elementMaxBounds(element, cache);
    if (!maxBounds) {
        // this element isn't visible in the DOM
        return null;
    }
    // CSS pixels are 96 to an inch, so convert to our desired PPI
    resolution /= 96;
    const canvas = Canvas(Width(maxBounds.width * resolution), Height(maxBounds.height * resolution), StyleAttr({
        width: px(maxBounds.width),
        height: px(maxBounds.height),
        "image-rendering": "high-quality"
    }));
    const work = new OffscreenCanvas(canvas.width, canvas.height);
    const back = canvas.getContext("2d");
    back.imageSmoothingEnabled = true;
    back.imageSmoothingQuality = "high";
    const front = work.getContext("2d");
    front.imageSmoothingEnabled = true;
    front.imageSmoothingQuality = "high";
    front.scale(resolution, resolution);
    // Convert the coordinate space of the Element to the coordinate
    // space of the Canvas
    front.translate(-maxBounds.left, -maxBounds.top);
    // Used for measuring Text nodes
    const range = document.createRange();
    for (const here of nodeWalkBreadthFirst(element)) {
        const bounds = elementComputeBounds(here, cache);
        if (!bounds) {
            continue;
        }
        front.save();
        front.clearRect(0, 0, work.width, work.height);
        // Border
        if (bounds.styles.borderWidth !== "0px") {
            front.fillStyle = bounds.styles.borderColor;
            front.fillRect(bounds.border.left, bounds.border.top, bounds.border.width, bounds.border.height);
            front.clearRect(bounds.padding.left, bounds.padding.top, bounds.padding.width, bounds.padding.height);
        }
        // Background
        if (bounds.styles.backgroundClip !== "rgba(0, 0, 0, 0)") {
            front.fillStyle = bounds.styles.backgroundColor;
            front.fillRect(bounds.padding.left, bounds.padding.top, bounds.padding.width, bounds.padding.height);
        }
        // Content
        if (here instanceof HTMLImageElement) {
            front.drawImage(here, bounds.interior.left, bounds.interior.top, bounds.interior.width, bounds.interior.height);
        }
        else {
            const textNodes = Array.from(here.childNodes)
                .filter(v => v instanceof Text);
            for (const textNode of textNodes) {
                const text = textNode.textContent;
                if (text.length > 0) {
                    range.selectNode(textNode);
                    const textBounds = range.getBoundingClientRect();
                    let textX = textBounds.left;
                    let textY = textBounds.top;
                    front.textBaseline = "top";
                    front.fillStyle = bounds.styles.color;
                    front.font = bounds.styles.font;
                    if (bounds.styles.writingMode.startsWith("vertical")) {
                        const fontBounds = front.measureText(text);
                        const midX = 0.5 * (fontBounds.actualBoundingBoxLeft + fontBounds.actualBoundingBoxRight);
                        const midY = 0.5 * (fontBounds.fontBoundingBoxAscent + fontBounds.fontBoundingBoxDescent);
                        front.translate(textX + midY, textY + midX);
                        front.rotate(Math.PI / 2);
                        textX = -midX;
                        textY = -midY;
                    }
                    front.fillText(text, textX, textY);
                }
            }
        }
        front.restore();
        back.drawImage(work, 0, 0, work.width, work.height, 0, 0, canvas.width, canvas.height);
    }
    return canvas;
}
/**
 * Disables all of the disablable elements that are children of the given
 * parent element and returns the set of elements that were not disabled before
 * the function was called.
 * @param parent the element to search for disablable children
 */
export function disableChildren(parent) {
    parent = resolveNode(parent);
    const elements = parent.querySelectorAll("input,button,fieldset,select,textarea");
    const wasEnabled = new Set();
    for (const element of elements) {
        if (!element.disabled) {
            wasEnabled.add(element);
            element.disabled = true;
        }
    }
    return wasEnabled;
}
/**
 * Find the position of an element within it's parent element. Returns null
 * if the element is not currently a child of another element.
 * @param elem
 * @returns
 */
export function elementGetIndexInParent(elem) {
    elem = resolveNode(elem);
    if (elem.parentElement) {
        for (let i = 0; i < elem.parentElement.childElementCount; ++i) {
            if (elem.parentElement.children[i] === elem) {
                return i;
            }
        }
    }
    return null;
}
export function isDisableable(obj) {
    return isObject(obj)
        && "disabled" in obj
        && isBoolean(obj.disabled);
}
export function elementSetEnabled(node, enabled) {
    node = resolveNode(node);
    if (isDisableable(node)) {
        node.disabled = !enabled;
    }
}
export function elementRemoveFromParent(node) {
    node = resolveNode(node);
    node.remove();
}
export function elementClearChildren(node) {
    node = resolveNode(node);
    node.innerHTML = "";
}
//# sourceMappingURL=util.js.map