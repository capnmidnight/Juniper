import { arrayRemove } from "@juniper-lib/collections/arrays";
import { once } from "@juniper-lib/events/once";
import { Text_Css } from "@juniper-lib/mediatypes";
import { isBoolean, isDate, isDefined, isFunction, isNumber, isObject, isString } from "@juniper-lib/tslib/typeChecks";
import { ClassList, Href, HtmlFor, Rel, Type, isAttr } from "./attrs";
import { margin } from "./css";
export function isErsatzElement(obj) {
    if (!isObject(obj)) {
        return false;
    }
    const elem = obj;
    return elem.element instanceof Element;
}
export function resolveElement(elem) {
    if (isErsatzElement(elem)) {
        return elem.element;
    }
    else if (isString(elem)) {
        return getElement(elem);
    }
    return elem;
}
export function isIElementAppliable(obj) {
    return isObject(obj)
        && "applyToElement" in obj
        && isFunction(obj.applyToElement);
}
export function isElementChild(obj) {
    return obj instanceof Element
        || isErsatzElement(obj)
        || isIElementAppliable(obj)
        || isString(obj)
        || isNumber(obj)
        || isBoolean(obj)
        || isDate(obj);
}
export function isElements(child) {
    return isErsatzElement(child)
        || child instanceof Element;
}
export function isFocusable(elem) {
    return "focus" in elem && isFunction(elem.focus);
}
export function elementSetDisplay(elem, visible, visibleDisplayType = "") {
    elem = resolveElement(elem);
    if (visible) {
        elem.style.removeProperty("display");
        const style = getComputedStyle(elem);
        if (style.display === "none") {
            elem.style.display = visibleDisplayType || "block";
        }
    }
    else {
        elem.style.display = "none";
    }
}
export function elementIsDisplayed(elem) {
    elem = resolveElement(elem);
    return elem.style.display !== "none";
}
export function elementToggleDisplay(elem, visibleDisplayType = "block") {
    elementSetDisplay(elem, !elementIsDisplayed(elem), visibleDisplayType);
}
export function elementInsertBefore(parent, newElem, refElem) {
    parent = resolveElement(parent);
    newElem = resolveElement(newElem);
    refElem = resolveElement(refElem);
    if (parent && newElem) {
        parent.insertBefore(newElem, refElem);
    }
}
export function elementGetIndexInParent(elem) {
    elem = resolveElement(elem);
    if (elem.parentElement) {
        for (let i = 0; i < elem.parentElement.childElementCount; ++i) {
            if (elem.parentElement.children[i] === elem) {
                return i;
            }
        }
    }
    return null;
}
export function Clear() {
    return {
        applyToElement(elem) { elem.innerHTML = ""; }
    };
}
export function elementGetCustomData(elem, name) {
    elem = resolveElement(elem);
    return elem.dataset[name.toLowerCase()];
}
export function HtmlRender(element, ...children) {
    const elem = element instanceof Element
        ? element
        : element instanceof ShadowRoot
            ? element
            : isString(element)
                ? document.querySelector(element)
                : element.element;
    const target = elem instanceof HTMLTemplateElement
        ? elem.content
        : elem;
    for (const child of children) {
        if (isDefined(child)) {
            if (child instanceof Node) {
                target.appendChild(child);
            }
            else if (isErsatzElement(child)) {
                target.appendChild(resolveElement(child));
            }
            else if (isIElementAppliable(child)) {
                if (!(elem instanceof ShadowRoot)) {
                    child.applyToElement(elem);
                }
            }
            else {
                target.appendChild(document.createTextNode(child.toLocaleString()));
            }
        }
    }
    return elem;
}
export function elementRemoveFromParent(elem) {
    elem = resolveElement(elem);
    if (isDefined(elem)) {
        elem.remove();
    }
}
export function elementReplace(elem, ...elems) {
    elem = resolveElement(elem);
    elem.replaceWith(...elems.map(resolveElement));
    return elem;
}
export function elementSwap(elem, withPlaceholder) {
    const placeholder = Div();
    const e = withPlaceholder(placeholder);
    elementReplace(placeholder, elementReplace(elem, e));
    return e;
}
export function getElement(selector) {
    return document.querySelector(selector);
}
export function getElements(selector) {
    return Array.from(document.querySelectorAll(selector));
}
export function getButton(selector) {
    return getElement(selector);
}
export function getButtons(selector) {
    return getElements(selector);
}
export function getInput(selector) {
    return getElement(selector);
}
export function getDataList(selector) {
    return getElement(selector);
}
export function getInputs(selector) {
    return getElements(selector);
}
export function getSelect(selector) {
    return getElement(selector);
}
export function getCanvas(selector) {
    return getElement(selector);
}
/**
 * Creates an HTML element for a given tag name.
 *
 * Boolean attributes that you want to default to true can be passed
 * as just the attribute creating function,
 *   e.g. `Audio(autoPlay)` vs `Audio(autoPlay(true))`
 * @param name - the name of the tag
 * @param rest - optional attributes, child elements, and text
 * @returns
 */
export function HtmlTag(name, ...rest) {
    let elem = null;
    const finders = rest.filter(isAttr).filter(v => v.key === "id" || v.key === "query");
    for (const finder of finders) {
        if (finder.key === "query") {
            elem = finder.value;
            arrayRemove(rest, finder);
        }
        else if (finder.key === "id") {
            elem = document.getElementById(finder.value);
            if (elem) {
                arrayRemove(rest, finder);
            }
        }
    }
    if (elem && elem.tagName !== name.toUpperCase()) {
        console.warn(`Expected a "${name.toUpperCase()}" element but found a "${elem.tagName}".`);
    }
    if (!elem) {
        elem = document.createElement(name);
    }
    HtmlRender(elem, ...rest);
    return elem;
}
export function isDisableable(obj) {
    return isObject(obj)
        && "disabled" in obj
        && isBoolean(obj.disabled);
}
/**
 * Empty an element of all children. This is faster than setting `innerHTML = ""`.
 */
export function elementClearChildren(elem) {
    elem = resolveElement(elem);
    while (elem.lastChild) {
        elem.lastChild.remove();
    }
}
export function elementSetText(elem, text) {
    elem = resolveElement(elem);
    elementClearChildren(elem);
    elem.append(TextNode(text));
}
export function elementGetText(elem) {
    elem = resolveElement(elem);
    return elem.innerText;
}
export function elementSetTitle(elem, text) {
    elem = resolveElement(elem);
    elem.title = text;
}
export function elementSetClass(elem, enabled, className) {
    elem = resolveElement(elem);
    const canEnable = isDefined(className);
    const hasEnabled = canEnable && elem.classList.contains(className);
    if (canEnable && hasEnabled !== enabled) {
        elem.classList.toggle(className);
    }
}
export function buttonSetEnabled(button, styleOrEnabled, enabledOrlabel, labelOrTitle, title) {
    button = resolveElement(button);
    let style = null;
    let enabled = null;
    let label = null;
    if (isBoolean(styleOrEnabled)) {
        enabled = styleOrEnabled;
        label = enabledOrlabel;
        title = labelOrTitle;
    }
    else {
        style = styleOrEnabled;
        enabled = enabledOrlabel;
        label = labelOrTitle;
    }
    button.disabled = !enabled;
    if (label) {
        elementSetText(button, label);
    }
    if (title) {
        elementSetTitle(button, title);
    }
    if (style) {
        button.classList.toggle("btn-" + style, enabled);
        button.classList.toggle("btn-outline-" + style, !enabled);
    }
}
async function mediaElementCan(type, elem, prog) {
    if (isDefined(prog)) {
        prog.start();
    }
    const expectedState = type === "canplay"
        ? elem.HAVE_CURRENT_DATA
        : elem.HAVE_ENOUGH_DATA;
    if (elem.readyState >= expectedState) {
        return true;
    }
    try {
        await once(elem, type, "error");
        return true;
    }
    catch (err) {
        console.warn(elem.error, err);
        return false;
    }
    finally {
        if (isDefined(prog)) {
            prog.end();
        }
    }
}
export function mediaElementCanPlay(elem, prog) {
    return mediaElementCan("canplay", elem, prog);
}
export function mediaElementCanPlayThrough(elem, prog) {
    return mediaElementCan("canplaythrough", elem, prog);
}
export function A(...rest) { return HtmlTag("a", ...rest); }
export function Abbr(...rest) { return HtmlTag("abbr", ...rest); }
export function Address(...rest) { return HtmlTag("address", ...rest); }
export function Area(...rest) { return HtmlTag("area", ...rest); }
export function Article(...rest) { return HtmlTag("article", ...rest); }
export function Aside(...rest) { return HtmlTag("aside", ...rest); }
export function Audio(...rest) { return HtmlTag("audio", ...rest); }
export function B(...rest) { return HtmlTag("b", ...rest); }
export function Base(...rest) { return HtmlTag("base", ...rest); }
export function BDI(...rest) { return HtmlTag("bdi", ...rest); }
export function BDO(...rest) { return HtmlTag("bdo", ...rest); }
export function BlockQuote(...rest) { return HtmlTag("blockquote", ...rest); }
export function Body(...rest) { return HtmlTag("body", ...rest); }
export function BR() { return HtmlTag("br"); }
export function ButtonRaw(...rest) { return HtmlTag("button", ...rest); }
export function Button(...rest) { return ButtonRaw(...rest, Type("button")); }
export function ButtonSmall(...rest) { return Button(...rest, ClassList("btn", "btn-sm")); }
export function ButtonPrimary(...rest) { return Button(...rest, ClassList("btn", "btn-primary")); }
export function ButtonPrimaryOutline(...rest) { return Button(...rest, ClassList("btn", "btn-outline-primary")); }
export function ButtonPrimarySmall(...rest) { return Button(...rest, ClassList("btn", "btn-sm", "btn-primary")); }
export function ButtonPrimaryOutlineSmall(...rest) { return Button(...rest, ClassList("btn", "btn-sm", "btn-outline-primary")); }
export function ButtonSecondary(...rest) { return Button(...rest, ClassList("btn", "btn-secondary")); }
export function ButtonSecondaryOutline(...rest) { return Button(...rest, ClassList("btn", "btn-outline-secondary")); }
export function ButtonSecondarySmall(...rest) { return Button(...rest, ClassList("btn", "btn-sm", "btn-secondary")); }
export function ButtonSecondaryOutlineSmall(...rest) { return Button(...rest, ClassList("btn", "btn-sm", "btn-outline-secondary")); }
export function ButtonDanger(...rest) { return Button(...rest, ClassList("btn", "btn-danger")); }
export function ButtonDangerOutline(...rest) { return Button(...rest, ClassList("btn", "btn-outline-danger")); }
export function ButtonDangerSmall(...rest) { return Button(...rest, ClassList("btn", "btn-sm", "btn-danger")); }
export function ButtonDangerOutlineSmalle(...rest) { return Button(...rest, ClassList("btn", "btn-sm", "btn-outline-danger")); }
export function ButtonSubmit(...rest) { return ButtonRaw(...rest, Type("submit")); }
export function ButtonReset(...rest) { return ButtonRaw(...rest, Type("reset")); }
export function Canvas(...rest) { return HtmlTag("canvas", ...rest); }
export function Caption(...rest) { return HtmlTag("caption", ...rest); }
export function Cite(...rest) { return HtmlTag("cite", ...rest); }
export function Code(...rest) { return HtmlTag("code", ...rest); }
export function Col(...rest) { return HtmlTag("col", ...rest); }
export function ColGroup(...rest) { return HtmlTag("colgroup", ...rest); }
export function DataTag(...rest) { return HtmlTag("data", ...rest); }
export function DataList(...rest) { return HtmlTag("datalist", ...rest); }
export function DD(...rest) { return HtmlTag("dd", ...rest); }
export function Del(...rest) { return HtmlTag("del", ...rest); }
export function Details(...rest) { return HtmlTag("details", ...rest); }
export function DFN(...rest) { return HtmlTag("dfn", ...rest); }
export function Dialog(...rest) { return HtmlTag("dialog", ...rest); }
export function Div(...rest) { return HtmlTag("div", ...rest); }
export function DL(...rest) { return HtmlTag("dl", ...rest); }
export function DT(...rest) { return HtmlTag("dt", ...rest); }
export function Em(...rest) { return HtmlTag("em", ...rest); }
export function Embed(...rest) { return HtmlTag("embed", ...rest); }
export function FieldSet(...rest) { return HtmlTag("fieldset", ...rest); }
export function FigCaption(...rest) { return HtmlTag("figcaption", ...rest); }
export function Figure(...rest) { return HtmlTag("figure", ...rest); }
export function Footer(...rest) { return HtmlTag("footer", ...rest); }
export function Form(...rest) { return HtmlTag("form", ...rest); }
export function H1(...rest) { return HtmlTag("h1", ...rest); }
export function H2(...rest) { return HtmlTag("h2", ...rest); }
export function H3(...rest) { return HtmlTag("h3", ...rest); }
export function H4(...rest) { return HtmlTag("h4", ...rest); }
export function H5(...rest) { return HtmlTag("h5", ...rest); }
export function H6(...rest) { return HtmlTag("h6", ...rest); }
export function HR(...rest) { return HtmlTag("hr", ...rest); }
export function Head(...rest) { return HtmlTag("head", ...rest); }
export function Header(...rest) { return HtmlTag("header", ...rest); }
export function HGroup(...rest) { return HtmlTag("hgroup", ...rest); }
export function HTML(...rest) { return HtmlTag("html", ...rest); }
export function I(...rest) { return HtmlTag("i", ...rest); }
export function FAIcon(iconName, ...rest) { return I(ClassList(`fa fa-${iconName}`), ...rest); }
export function IFrame(...rest) { return HtmlTag("iframe", ...rest); }
export function Img(...rest) { return HtmlTag("img", ...rest); }
export function Input(...rest) { return HtmlTag("input", ...rest); }
export function Ins(...rest) { return HtmlTag("ins", ...rest); }
export function KBD(...rest) { return HtmlTag("kbd", ...rest); }
export function Label(...rest) { return HtmlTag("label", ...rest); }
export function PreLabeled(id, label, input) {
    resolveElement(input).id = id;
    return [
        Label(HtmlFor(id), label),
        input
    ];
}
export function PostLabeled(id, label, input) {
    resolveElement(input).id = id;
    return [
        input,
        Label(HtmlFor(id), label)
    ];
}
export function Legend(...rest) { return HtmlTag("legend", ...rest); }
export function LI(...rest) { return HtmlTag("li", ...rest); }
export function Link(...rest) { return HtmlTag("link", ...rest); }
export function Main(...rest) { return HtmlTag("main", ...rest); }
export function Map_tag(...rest) { return HtmlTag("map", ...rest); }
export function Mark(...rest) { return HtmlTag("mark", ...rest); }
export function Menu(...rest) { return HtmlTag("menu", ...rest); }
export function Meta(...rest) { return HtmlTag("meta", ...rest); }
export function Meter(...rest) { return HtmlTag("meter", ...rest); }
export function Nav(...rest) { return HtmlTag("nav", ...rest); }
export function NoScript(...rest) { return HtmlTag("noscript", ...rest); }
export function Object_tag(...rest) { return HtmlTag("object", ...rest); }
export function OL(...rest) { return HtmlTag("ol", ...rest); }
export function OptGroup(...rest) { return HtmlTag("optgroup", ...rest); }
export function Option(...rest) { return HtmlTag("option", ...rest); }
export function Output(...rest) { return HtmlTag("output", ...rest); }
export function P(...rest) { return HtmlTag("p", ...rest); }
export function Picture(...rest) { return HtmlTag("picture", ...rest); }
export function Pre(...rest) { return HtmlTag("pre", ...rest); }
export function Progress(...rest) { return HtmlTag("progress", ...rest); }
export function Q(...rest) { return HtmlTag("q", ...rest); }
export function RP(...rest) { return HtmlTag("rp", ...rest); }
export function RT(...rest) { return HtmlTag("rt", ...rest); }
export function Ruby(...rest) { return HtmlTag("ruby", ...rest); }
export function S(...rest) { return HtmlTag("s", ...rest); }
export function Samp(...rest) { return HtmlTag("samp", ...rest); }
export function Script(...rest) { return HtmlTag("script", ...rest); }
export function Section(...rest) { return HtmlTag("section", ...rest); }
export function Select(...rest) { return HtmlTag("select", ...rest); }
export function Slot(...rest) { return HtmlTag("slot", ...rest); }
export function Small(...rest) { return HtmlTag("small", ...rest); }
export function Source(...rest) { return HtmlTag("source", ...rest); }
export function Span(...rest) { return HtmlTag("span", ...rest); }
export function Strong(...rest) { return HtmlTag("strong", ...rest); }
export function Sub(...rest) { return HtmlTag("sub", ...rest); }
export function Summary(...rest) { return HtmlTag("summary", ...rest); }
export function Sup(...rest) { return HtmlTag("sup", ...rest); }
export function Table(...rest) { return HtmlTag("table", ...rest); }
export function TBody(...rest) { return HtmlTag("tbody", ...rest); }
export function TD(...rest) { return HtmlTag("td", ...rest); }
export function Template(...rest) { return HtmlTag("template", ...rest); }
export function TextArea(...rest) { return HtmlTag("textarea", ...rest); }
export function TFoot(...rest) { return HtmlTag("tfoot", ...rest); }
export function TH(...rest) { return HtmlTag("th", ...rest); }
export function THead(...rest) { return HtmlTag("thead", ...rest); }
export function Time(...rest) { return HtmlTag("time", ...rest); }
export function Title(...rest) { return HtmlTag("title", ...rest); }
export function TR(...rest) { return HtmlTag("tr", ...rest); }
export function Track(...rest) { return HtmlTag("track", ...rest); }
export function U(...rest) { return HtmlTag("u", ...rest); }
export function UL(...rest) { return HtmlTag("ul", ...rest); }
export function Var(...rest) { return HtmlTag("var", ...rest); }
export function Video(...rest) { return HtmlTag("video", ...rest); }
export function WBR() { return HtmlTag("wbr"); }
/**
 * creates an HTML Input tag that is a button.
 */
export function InputButton(...rest) { return Input(Type("button"), ...rest); }
/**
 * creates an HTML Input tag that is a checkbox.
 */
export function InputCheckbox(...rest) { return Input(Type("checkbox"), ...rest); }
/**
 * creates an HTML Input tag that is a color picker.
 */
export function InputColor(...rest) { return Input(Type("color"), ...rest); }
/**
 * creates an HTML Input tag that is a date picker.
 */
export function InputDate(...rest) { return Input(Type("date"), ...rest); }
/**
 * creates an HTML Input tag that is a local date-time picker.
 */
export function InputDateTime(...rest) { return Input(Type("datetime-local"), ...rest); }
/**
 * creates an HTML Input tag that is an email entry field.
 */
export function InputEmail(...rest) { return Input(Type("email"), ...rest); }
/**
 * creates an HTML Input tag that is a file picker.
 */
export function InputFile(...rest) { return Input(Type("file"), ...rest); }
/**
 * creates an HTML Input tag that is a hidden field.
 */
export function InputHidden(...rest) { return Input(Type("hidden"), ...rest); }
/**
 * creates an HTML Input tag that is a graphical submit button.
 */
export function InputImage(...rest) { return Input(Type("image"), ...rest); }
/**
 * creates an HTML Input tag that is a month picker.
 */
export function InputMonth(...rest) { return Input(Type("month"), ...rest); }
/**
 * creates an HTML Input tag that is a month picker.
 */
export function InputNumber(...rest) { return Input(Type("number"), ...rest); }
/**
 * creates an HTML Input tag that is a password entry field.
 */
export function InputPassword(...rest) { return Input(Type("password"), ...rest); }
/**
 * creates an HTML Input tag that is a radio button.
 */
export function InputRadio(...rest) { return Input(Type("radio"), ...rest); }
/**
 * creates an HTML Input tag that is a range selector.
 */
export function InputRange(...rest) { return Input(Type("range"), ...rest); }
/**
 * creates an HTML Input tag that is a form reset button.
 */
export function InputReset(...rest) { return Input(Type("reset"), ...rest); }
/**
 * creates an HTML Input tag that is a search entry field.
 */
export function InputSearch(...rest) { return Input(Type("search"), ...rest); }
/**
 * creates an HTML Input tag that is a submit button.
 */
export function InputSubmit(...rest) { return Input(Type("submit"), ...rest); }
/**
 * creates an HTML Input tag that is a telephone number entry field.
 */
export function InputTelephone(...rest) { return Input(Type("tel"), ...rest); }
/**
 * creates an HTML Input tag that is a text entry field.
 */
export function InputText(...rest) { return Input(Type("text"), ...rest); }
/**
 * creates an HTML Input tag that is a time picker.
 */
export function InputTime(...rest) { return Input(Type("time"), ...rest); }
/**
 * creates an HTML Input tag that is a URL entry field.
 */
export function InputURL(...rest) { return Input(Type("url"), ...rest); }
/**
 * creates an HTML Input tag that is a week picker.
 */
export function InputWeek(...rest) { return Input(Type("week"), ...rest); }
/**
 * Creates a text node out of the give input.
 */
export function TextNode(txt) {
    return document.createTextNode(txt);
}
/**
 * Creates a Div element with margin: auto.
 */
export function Run(...rest) {
    return Div(margin("auto"), ...rest);
}
export function Style(...props) {
    const elem = document.createElement("style");
    document.head.append(elem);
    for (const prop of props) {
        prop.applyToSheet(elem.sheet);
    }
    return elem;
}
export function StyleBlob(...props) {
    const blob = new Blob(props.map(p => p.toString()), {
        type: Text_Css.value
    });
    return Link(Rel("stylesheet"), Href(blob));
}
//# sourceMappingURL=tags.js.map