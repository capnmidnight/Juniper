import { arrayRemove, arrayScan } from "@juniper-lib/collections/arrays";
import { once } from "@juniper-lib/events/once";
import { IProgress } from "@juniper-lib/progress/IProgress";
import { isBoolean, isDate, isDefined, isFunction, isNumber, isObject, isString } from "@juniper-lib/tslib/typeChecks";
import { ClassList, HtmlFor, Type, isAttr } from "./attrs";
import { PropSet, margin } from "./css";

export interface ErsatzElement<T extends Element = Element> {
    element: T;
}

export function isErsatzElement<T extends Element = Element>(obj: any): obj is ErsatzElement<T> {
    if (!isObject(obj)) {
        return false;
    }

    const elem = obj as ErsatzElement;
    return elem.element instanceof Element;
}

export type Elements<T extends Element = Element> = T | ErsatzElement<T>;

export function resolveElement<T extends Element = HTMLElement>(elem: Elements<T> | string): T {
    if (isErsatzElement(elem)) {
        return elem.element;
    }
    else if (isString(elem)) {
        return getElement(elem);
    }

    return elem;
}

export interface IElementAppliable<T extends Element = Element> {
    applyToElement(x: Elements<T>): void;
}

export function isIElementAppliable<T extends Element = Element>(obj: any): obj is IElementAppliable<T> {
    return isObject(obj)
        && "applyToElement" in obj
        && isFunction((obj as any).applyToElement);
}

export type ElementChild<T extends Element = Element> = Elements<T>
    | IElementAppliable
    | string
    | number
    | boolean
    | Date;

export function isElementChild(obj: any): obj is ElementChild {
    return obj instanceof Element
        || isErsatzElement(obj)
        || isIElementAppliable(obj)
        || isString(obj)
        || isNumber(obj)
        || isBoolean(obj)
        || isDate(obj);
}

export function isElements(child: ElementChild): child is Elements {
    return isErsatzElement(child)
        || child instanceof Element;
}

export interface IFocusable {
    focus(): void;
}

export function isFocusable(elem: any): elem is IFocusable {
    return "focus" in elem && isFunction((elem as IFocusable).focus);
}

export function elementSetDisplay<T extends HTMLElement = HTMLElement>(elem: Elements<T>, visible: boolean, visibleDisplayType: CssGlobalValue | CssDisplayValue = ""): void {
    elem = resolveElement(elem);
    elem.style.display = visible ? visibleDisplayType : "none";
}

export function elementIsDisplayed<T extends HTMLElement = HTMLElement>(elem: Elements<T>): boolean {
    elem = resolveElement(elem);
    return elem.style.display !== "none";
}

export function elementToggleDisplay<T extends HTMLElement = HTMLElement>(elem: Elements<T>, visibleDisplayType: CssGlobalValue | CssDisplayValue = "block"): void {
    elementSetDisplay(elem, !elementIsDisplayed(elem), visibleDisplayType);
}

export function elementInsertBefore(parent: Elements, newElem: Elements, refElem: Elements): void {
    parent = resolveElement(parent);
    newElem = resolveElement(newElem);
    refElem = resolveElement(refElem);
    if (parent && newElem) {
        parent.insertBefore(newElem, refElem);
    }
}

export function elementGetIndexInParent(elem: Elements): number {
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


export function elementGetCustomData(elem: Elements<HTMLElement>, name: Lowercase<string>): string {
    elem = resolveElement(elem);
    return elem.dataset[name.toLowerCase()];
}

export function elementApply(element: Elements | string, ...children: ElementChild[]): Elements {
    const elem = resolveElement(element);

    for (const child of children) {
        if (isDefined(child)) {
            if (child instanceof Node) {
                elem.append(child);
            }
            else if (isErsatzElement(child)) {
                elem.append(resolveElement(child));
            }
            else if (isIElementAppliable(child)) {
                child.applyToElement(elem);
            }
            else {
                elem.append(document.createTextNode(child.toLocaleString()));
            }
        }
    }

    return elem;
}

export function elementRemoveFromParent(elem: Elements | string): void {
    elem = resolveElement(elem);
    if (isDefined(elem)) {
        elem.remove();
    }
}

export function elementReplace(elem: Elements, ...elems: Elements[]): Elements {
    elem = resolveElement(elem);
    elem.replaceWith(...elems.map(resolveElement));
    return elem;
}

export function elementSwap<T extends Elements>(elem: Elements, withPlaceholder: (placeholder: Elements) => T): T {
    const placeholder = Div();
    const e = withPlaceholder(placeholder);
    elementReplace(placeholder, elementReplace(elem, e));
    return e;
}

export function getElement<T extends Element = HTMLElement>(selector: string): T {
    return document.querySelector<T>(selector);
}

export function getElements<T extends Element = HTMLElement>(selector: string): T[] {
    return Array.from(document.querySelectorAll<T>(selector));
}

export function getButton(selector: string) {
    return getElement<HTMLButtonElement>(selector);
}

export function getButtons(selector: string) {
    return getElements<HTMLButtonElement>(selector);
}

export function getInput(selector: string) {
    return getElement<HTMLInputElement>(selector);
}

export function getDataList(selector: string) {
    return getElement<HTMLDataListElement>(selector);
}

export function getInputs(selector: string) {
    return getElements<HTMLInputElement>(selector);
}

export function getSelect(selector: string) {
    return getElement<HTMLSelectElement>(selector);
}

export function getCanvas(selector: string) {
    return getElement<HTMLCanvasElement>(selector);
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
export function HtmlTag<K extends keyof MapT & string, MapT extends Record<keyof MapT, HTMLElement> = HTMLElementTagNameMap>(name: K, ...rest: ElementChild[]): MapT[K] {
    const attrs = rest.filter(isAttr);
    const idAttr = arrayScan(attrs, (v => v.key === "id"));
    const queryAttr = arrayScan(attrs, (v => v.key === "query"));

    let elem: MapT[K] & HTMLElement = null;

    if (queryAttr) {
        elem = queryAttr.value as any;
        arrayRemove(rest, queryAttr);
    }

    if (!elem && idAttr) {
        elem = document.querySelector(idAttr.value as any) as any;
        if (elem) {
            arrayRemove(rest, idAttr);
            if (elem.tagName !== name.toUpperCase()) {
                console.warn(`Expected a "${name.toUpperCase()}" element but found a "${elem.tagName}".`);
            }
        }
    }

    if (!elem) {
        elem = document.createElement(name) as MapT[K];
    }

    elementApply(elem, ...rest);

    return elem;
}

export interface IDisableable {
    disabled: boolean;
}

export function isDisableable(obj: any): obj is IDisableable {
    return isObject(obj)
        && "disabled" in obj
        && isBoolean(obj.disabled);
}

/**
 * Empty an element of all children. This is faster than setting `innerHTML = ""`.
 */
export function elementClearChildren(elem: Elements) {
    elem = resolveElement(elem);
    while (elem.lastChild) {
        elem.lastChild.remove();
    }
}

export function elementSetText(elem: Elements<HTMLElement>, text: string): void {
    elem = resolveElement(elem);
    elementClearChildren(elem);
    elem.append(TextNode(text));
}

export function elementGetText(elem: Elements<HTMLElement>): string {
    elem = resolveElement(elem);
    return elem.innerText;
}

export function elementSetTitle(elem: Elements<HTMLElement>, text: string): void {
    elem = resolveElement(elem);
    elem.title = text;
}

export function elementSetClass(elem: Elements, enabled: boolean, className: string) {
    elem = resolveElement(elem);
    const canEnable = isDefined(className);
    const hasEnabled = canEnable && elem.classList.contains(className);

    if (canEnable && hasEnabled !== enabled) {
        elem.classList.toggle(className);
    }
}

export function buttonSetEnabled(button: Elements<HTMLButtonElement>, enabled: boolean, label: string, title: string) {
    button = resolveElement(button);
    button.disabled = !enabled;
    elementSetText(button, label);
    elementSetTitle(button, title);
}


async function mediaElementCan(type: "canplay" | "canplaythrough", elem: HTMLMediaElement, prog?: IProgress): Promise<boolean> {
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

export function mediaElementCanPlay(elem: HTMLMediaElement, prog?: IProgress): Promise<boolean> {
    return mediaElementCan("canplay", elem, prog);
}

export function mediaElementCanPlayThrough(elem: HTMLMediaElement, prog?: IProgress): Promise<boolean> {
    return mediaElementCan("canplaythrough", elem, prog);
}

export function A(...rest: ElementChild[]): HTMLAnchorElement { return HtmlTag("a", ...rest); }
export function Abbr(...rest: ElementChild[]): HTMLElement { return HtmlTag("abbr", ...rest); }
export function Address(...rest: ElementChild[]): HTMLElement { return HtmlTag("address", ...rest); }
export function Area(...rest: ElementChild[]): HTMLAreaElement { return HtmlTag("area", ...rest); }
export function Article(...rest: ElementChild[]): HTMLElement { return HtmlTag("article", ...rest); }
export function Aside(...rest: ElementChild[]): HTMLElement { return HtmlTag("aside", ...rest); }
export function Audio(...rest: ElementChild[]): HTMLAudioElement { return HtmlTag("audio", ...rest); }
export function B(...rest: ElementChild[]): HTMLElement { return HtmlTag("b", ...rest); }
export function Base(...rest: ElementChild[]): HTMLBaseElement { return HtmlTag("base", ...rest); }
export function BDI(...rest: ElementChild[]): HTMLElement { return HtmlTag("bdi", ...rest); }
export function BDO(...rest: ElementChild[]): HTMLElement { return HtmlTag("bdo", ...rest); }
export function BlockQuote(...rest: ElementChild[]): HTMLQuoteElement { return HtmlTag("blockquote", ...rest); }
export function Body(...rest: ElementChild[]): HTMLBodyElement { return HtmlTag("body", ...rest); }
export function BR(): HTMLBRElement { return HtmlTag("br"); }
export function ButtonRaw(...rest: ElementChild[]): HTMLButtonElement { return HtmlTag("button", ...rest); }
export function Button(...rest: ElementChild[]): HTMLButtonElement { return ButtonRaw(...rest, Type("button")); }
export function ButtonSmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-sm")); }
export function ButtonPrimary(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-primary")); }
export function ButtonPrimaryOutline(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-outline-primary")); }
export function ButtonPrimarySmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-sm", "btn-primary")); }
export function ButtonPrimaryOutlineSmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-sm", "btn-outline-primary")); }
export function ButtonSecondary(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-secondary")); }
export function ButtonSecondaryOutline(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-outline-secondary")); }
export function ButtonSecondarySmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-sm", "btn-secondary")); }
export function ButtonSecondaryOutlineSmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-sm", "btn-outline-secondary")); }
export function ButtonDanger(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-danger")); }
export function ButtonDangerOutline(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-outline-danger")); }
export function ButtonDangerSmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-sm", "btn-danger")); }
export function ButtonDangerOutlineSmalle(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, ClassList("btn", "btn-sm", "btn-outline-danger")); }
export function ButtonSubmit(...rest: ElementChild[]): HTMLButtonElement { return ButtonRaw(...rest, Type("submit")); }
export function ButtonReset(...rest: ElementChild[]): HTMLButtonElement { return ButtonRaw(...rest, Type("reset")); }
export function Canvas(...rest: ElementChild[]): HTMLCanvasElement { return HtmlTag("canvas", ...rest); }
export function Caption(...rest: ElementChild[]): HTMLTableCaptionElement { return HtmlTag("caption", ...rest); }
export function Cite(...rest: ElementChild[]): HTMLElement { return HtmlTag("cite", ...rest); }
export function Code(...rest: ElementChild[]): HTMLElement { return HtmlTag("code", ...rest); }
export function Col(...rest: ElementChild[]): HTMLTableColElement { return HtmlTag("col", ...rest); }
export function ColGroup(...rest: ElementChild[]): HTMLTableColElement { return HtmlTag("colgroup", ...rest); }
export function Data(...rest: ElementChild[]): HTMLDataElement { return HtmlTag("data", ...rest); }
export function DataList(...rest: ElementChild[]): HTMLDataListElement { return HtmlTag("datalist", ...rest); }
export function DD(...rest: ElementChild[]): HTMLElement { return HtmlTag("dd", ...rest); }
export function Del(...rest: ElementChild[]): HTMLModElement { return HtmlTag("del", ...rest); }
export function Details(...rest: ElementChild[]): HTMLDetailsElement { return HtmlTag("details", ...rest); }
export function DFN(...rest: ElementChild[]): HTMLElement { return HtmlTag("dfn", ...rest); }
export function Dialog(...rest: ElementChild[]): HTMLDialogElement { return HtmlTag("dialog", ...rest); }
export function Div(...rest: ElementChild[]): HTMLDivElement { return HtmlTag("div", ...rest); }
export function DL(...rest: ElementChild[]): HTMLDListElement { return HtmlTag("dl", ...rest); }
export function DT(...rest: ElementChild[]): HTMLElement { return HtmlTag("dt", ...rest); }
export function Em(...rest: ElementChild[]): HTMLElement { return HtmlTag("em", ...rest); }
export function Embed(...rest: ElementChild[]): HTMLEmbedElement { return HtmlTag("embed", ...rest); }
export function FieldSet(...rest: ElementChild[]): HTMLFieldSetElement { return HtmlTag("fieldset", ...rest); }
export function FigCaption(...rest: ElementChild[]): HTMLElement { return HtmlTag("figcaption", ...rest); }
export function Figure(...rest: ElementChild[]): HTMLElement { return HtmlTag("figure", ...rest); }
export function Footer(...rest: ElementChild[]): HTMLElement { return HtmlTag("footer", ...rest); }
export function Form(...rest: ElementChild[]): HTMLFormElement { return HtmlTag("form", ...rest); }
export function H1(...rest: ElementChild[]): HTMLHeadingElement { return HtmlTag("h1", ...rest); }
export function H2(...rest: ElementChild[]): HTMLHeadingElement { return HtmlTag("h2", ...rest); }
export function H3(...rest: ElementChild[]): HTMLHeadingElement { return HtmlTag("h3", ...rest); }
export function H4(...rest: ElementChild[]): HTMLHeadingElement { return HtmlTag("h4", ...rest); }
export function H5(...rest: ElementChild[]): HTMLHeadingElement { return HtmlTag("h5", ...rest); }
export function H6(...rest: ElementChild[]): HTMLHeadingElement { return HtmlTag("h6", ...rest); }
export function HR(...rest: ElementChild[]): HTMLHRElement { return HtmlTag("hr", ...rest); }
export function Head(...rest: ElementChild[]): HTMLHeadElement { return HtmlTag("head", ...rest); }
export function Header(...rest: ElementChild[]): HTMLElement { return HtmlTag("header", ...rest); }
export function HGroup(...rest: ElementChild[]): HTMLElement { return HtmlTag("hgroup", ...rest); }
export function HTML(...rest: ElementChild[]): HTMLElement { return HtmlTag("html", ...rest); }
export function I(...rest: ElementChild[]): HTMLElement { return HtmlTag("i", ...rest); }
export function FAIcon(iconName: string, ...rest: ElementChild[]): HTMLElement { return I(ClassList(`fa fa-${iconName}`), ...rest); }
export function IFrame(...rest: ElementChild[]): HTMLIFrameElement { return HtmlTag("iframe", ...rest); }
export function Img(...rest: ElementChild[]): HTMLImageElement { return HtmlTag("img", ...rest); }
export function Input(...rest: ElementChild[]): HTMLInputElement { return HtmlTag("input", ...rest); }
export function Ins(...rest: ElementChild[]): HTMLModElement { return HtmlTag("ins", ...rest); }
export function KBD(...rest: ElementChild[]): HTMLElement { return HtmlTag("kbd", ...rest); }
export function Label(...rest: ElementChild[]): HTMLLabelElement { return HtmlTag("label", ...rest); }
export function PreLabeled<T extends Elements>(id: string, label: ElementChild, input: T): [HTMLLabelElement, T] {
    resolveElement(input).id = id;
    return [
        Label(HtmlFor(id), label),
        input
    ];
}
export function PostLabeled<T extends Elements>(id: string, label: ElementChild, input: T): [T, HTMLLabelElement] {
    resolveElement(input).id = id;
    return [
        input,
        Label(HtmlFor(id), label)
    ];
}
export function Legend(...rest: ElementChild[]) { return HtmlTag("legend", ...rest); }
export function LI(...rest: ElementChild[]) { return HtmlTag("li", ...rest); }
export function Link(...rest: ElementChild[]) { return HtmlTag("link", ...rest); }
export function Main(...rest: ElementChild[]) { return HtmlTag("main", ...rest); }
export function Map_tag(...rest: ElementChild[]) { return HtmlTag("map", ...rest); }
export function Mark(...rest: ElementChild[]) { return HtmlTag("mark", ...rest); }
export function Menu(...rest: ElementChild[]) { return HtmlTag("menu", ...rest); }
export function Meta(...rest: ElementChild[]) { return HtmlTag("meta", ...rest); }
export function Meter(...rest: ElementChild[]) { return HtmlTag("meter", ...rest); }
export function Nav(...rest: ElementChild[]) { return HtmlTag("nav", ...rest); }
export function NoScript(...rest: ElementChild[]) { return HtmlTag("noscript", ...rest); }
export function Object_tag(...rest: ElementChild[]) { return HtmlTag("object", ...rest); }
export function OL(...rest: ElementChild[]) { return HtmlTag("ol", ...rest); }
export function OptGroup(...rest: ElementChild[]) { return HtmlTag("optgroup", ...rest); }
export function Option(...rest: ElementChild[]) { return HtmlTag("option", ...rest); }
export function Output(...rest: ElementChild[]) { return HtmlTag("output", ...rest); }
export function P(...rest: ElementChild[]) { return HtmlTag("p", ...rest); }
export function Picture(...rest: ElementChild[]) { return HtmlTag("picture", ...rest); }
export function Pre(...rest: ElementChild[]) { return HtmlTag("pre", ...rest); }
export function Progress(...rest: ElementChild[]) { return HtmlTag("progress", ...rest); }
export function Q(...rest: ElementChild[]) { return HtmlTag("q", ...rest); }
export function RP(...rest: ElementChild[]) { return HtmlTag("rp", ...rest); }
export function RT(...rest: ElementChild[]) { return HtmlTag("rt", ...rest); }
export function Ruby(...rest: ElementChild[]) { return HtmlTag("ruby", ...rest); }
export function S(...rest: ElementChild[]) { return HtmlTag("s", ...rest); }
export function Samp(...rest: ElementChild[]) { return HtmlTag("samp", ...rest); }
export function Script(...rest: ElementChild[]) { return HtmlTag("script", ...rest); }
export function Section(...rest: ElementChild[]) { return HtmlTag("section", ...rest); }
export function Select(...rest: ElementChild[]) { return HtmlTag("select", ...rest); }
export function Slot(...rest: ElementChild[]) { return HtmlTag("slot", ...rest); }
export function Small(...rest: ElementChild[]) { return HtmlTag("small", ...rest); }
export function Source(...rest: ElementChild[]) { return HtmlTag("source", ...rest); }
export function Span(...rest: ElementChild[]) { return HtmlTag("span", ...rest); }
export function Strong(...rest: ElementChild[]) { return HtmlTag("strong", ...rest); }
export function Sub(...rest: ElementChild[]) { return HtmlTag("sub", ...rest); }
export function Summary(...rest: ElementChild[]) { return HtmlTag("summary", ...rest); }
export function Sup(...rest: ElementChild[]) { return HtmlTag("sup", ...rest); }
export function Table(...rest: ElementChild[]) { return HtmlTag("table", ...rest); }
export function TBody(...rest: ElementChild[]) { return HtmlTag("tbody", ...rest); }
export function TD(...rest: ElementChild[]) { return HtmlTag("td", ...rest); }
export function Template(...rest: ElementChild[]) { return HtmlTag("template", ...rest); }
export function TextArea(...rest: ElementChild[]) { return HtmlTag("textarea", ...rest); }
export function TFoot(...rest: ElementChild[]) { return HtmlTag("tfoot", ...rest); }
export function TH(...rest: ElementChild[]) { return HtmlTag("th", ...rest); }
export function THead(...rest: ElementChild[]) { return HtmlTag("thead", ...rest); }
export function Time(...rest: ElementChild[]) { return HtmlTag("time", ...rest); }
export function Title(...rest: ElementChild[]) { return HtmlTag("title", ...rest); }
export function TR(...rest: ElementChild[]) { return HtmlTag("tr", ...rest); }
export function Track(...rest: ElementChild[]) { return HtmlTag("track", ...rest); }
export function U(...rest: ElementChild[]) { return HtmlTag("u", ...rest); }
export function UL(...rest: ElementChild[]) { return HtmlTag("ul", ...rest); }
export function Var(...rest: ElementChild[]) { return HtmlTag("var", ...rest); }
export function Video(...rest: ElementChild[]) { return HtmlTag("video", ...rest); }
export function WBR() { return HtmlTag("wbr"); }

/**
 * creates an HTML Input tag that is a button.
 */
export function InputButton(...rest: ElementChild[]) { return Input(Type("button"), ...rest); }

/**
 * creates an HTML Input tag that is a checkbox.
 */
export function InputCheckbox(...rest: ElementChild[]) { return Input(Type("checkbox"), ...rest); }

/**
 * creates an HTML Input tag that is a color picker.
 */
export function InputColor(...rest: ElementChild[]) { return Input(Type("color"), ...rest); }

/**
 * creates an HTML Input tag that is a date picker.
 */
export function InputDate(...rest: ElementChild[]) { return Input(Type("date"), ...rest); }

/**
 * creates an HTML Input tag that is a local date-time picker.
 */
export function InputDateTime(...rest: ElementChild[]) { return Input(Type("datetime-local"), ...rest); }

/**
 * creates an HTML Input tag that is an email entry field.
 */
export function InputEmail(...rest: ElementChild[]) { return Input(Type("email"), ...rest); }

/**
 * creates an HTML Input tag that is a file picker.
 */
export function InputFile(...rest: ElementChild[]) { return Input(Type("file"), ...rest); }

/**
 * creates an HTML Input tag that is a hidden field.
 */
export function InputHidden(...rest: ElementChild[]) { return Input(Type("hidden"), ...rest); }

/**
 * creates an HTML Input tag that is a graphical submit button.
 */
export function InputImage(...rest: ElementChild[]) { return Input(Type("image"), ...rest); }

/**
 * creates an HTML Input tag that is a month picker.
 */
export function InputMonth(...rest: ElementChild[]) { return Input(Type("month"), ...rest); }

/**
 * creates an HTML Input tag that is a month picker.
 */
export function InputNumber(...rest: ElementChild[]) { return Input(Type("number"), ...rest); }

/**
 * creates an HTML Input tag that is a password entry field.
 */
export function InputPassword(...rest: ElementChild[]) { return Input(Type("password"), ...rest); }

/**
 * creates an HTML Input tag that is a radio button.
 */
export function InputRadio(...rest: ElementChild[]) { return Input(Type("radio"), ...rest); }

/**
 * creates an HTML Input tag that is a range selector.
 */
export function InputRange(...rest: ElementChild[]) { return Input(Type("range"), ...rest); }

/**
 * creates an HTML Input tag that is a form reset button.
 */
export function InputReset(...rest: ElementChild[]) { return Input(Type("reset"), ...rest); }

/**
 * creates an HTML Input tag that is a search entry field.
 */
export function InputSearch(...rest: ElementChild[]) { return Input(Type("search"), ...rest); }

/**
 * creates an HTML Input tag that is a submit button.
 */
export function InputSubmit(...rest: ElementChild[]) { return Input(Type("submit"), ...rest); }

/**
 * creates an HTML Input tag that is a telephone number entry field.
 */
export function InputTelephone(...rest: ElementChild[]) { return Input(Type("tel"), ...rest); }

/**
 * creates an HTML Input tag that is a text entry field.
 */
export function InputText(...rest: ElementChild[]) { return Input(Type("text"), ...rest); }

/**
 * creates an HTML Input tag that is a time picker.
 */
export function InputTime(...rest: ElementChild[]) { return Input(Type("time"), ...rest); }

/**
 * creates an HTML Input tag that is a URL entry field.
 */
export function InputURL(...rest: ElementChild[]) { return Input(Type("url"), ...rest); }

/**
 * creates an HTML Input tag that is a week picker.
 */
export function InputWeek(...rest: ElementChild[]) { return Input(Type("week"), ...rest); }

/**
 * Creates a text node out of the give input.
 */
export function TextNode(txt: any) {
    return document.createTextNode(txt);
}

/**
 * Creates a Div element with margin: auto.
 */
export function Run(...rest: ElementChild[]) {
    return Div(
        margin("auto"),
        ...rest);
}

export function Style(parent: ParentNode, ...props: PropSet[]) {
    const elem = document.createElement("style");

    parent.append(elem);

    for (const prop of props) {
        prop.applyToSheet(elem.sheet);
    }

    return elem;
}

export function addStyle(...props: PropSet[]) {
    Style(document.head, ...props);
}