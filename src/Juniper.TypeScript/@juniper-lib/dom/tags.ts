import { once } from "@juniper-lib/tslib/events/once";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { isBoolean, isDate, isDefined, isFunction, isNumber, isObject, isString } from "@juniper-lib/tslib/typeChecks";
import { Attr, autoPlay, classList, className, controls, htmlFor, loop, muted, playsInline, type } from "./attrs";
import { CSSInJSRule, display, margin, styles } from "./css";

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

export function resolveElement<T extends Element = HTMLElement>(elem: Elements<T>): T {
    if (isErsatzElement(elem)) {
        return elem.element;
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

export function elementSetDisplay<T extends HTMLElement = HTMLElement>(elem: Elements<T>, visible: boolean, visibleDisplayType: string = ""): void {
    elem = resolveElement(elem);
    elem.style.display = visible ? visibleDisplayType : "none";
}

export function elementIsDisplayed<T extends HTMLElement = HTMLElement>(elem: Elements<T>): boolean {
    elem = resolveElement(elem);
    return elem.style.display !== "none";
}

export function elementToggleDisplay<T extends HTMLElement = HTMLElement>(elem: Elements<T>, visibleDisplayType: string = "block"): void {
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

export function elementApply(elem: Elements, ...children: ElementChild[]): Elements {
    elem = resolveElement(elem);

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

export function elementReplace(elem: Elements, ...elems: Elements[]): Elements {
    elem = resolveElement(elem);
    elem.replaceWith(...elems.map(resolveElement));
    return elem;
}

export function elementSwap(elem: Elements, withPlaceholder: (placeholder: Elements) => Elements) {
    const placeholder = Div();
    elementReplace(placeholder, elementReplace(elem, withPlaceholder(placeholder)));
}

export function getElement<T extends Element = HTMLElement>(selector: string): T {
    return document.querySelector<T>(selector);
}

export function getButton(selector: string) {
    return getElement<HTMLButtonElement>(selector);
}

export function getInput(selector: string) {
    return getElement<HTMLInputElement>(selector);
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
export function tag<K extends keyof HTMLElementTagNameMap>(name: K, ...rest: ElementChild[]): HTMLElementTagNameMap[K] {
    let elem: HTMLElementTagNameMap[K] = null;

    for (const attr of rest) {
        if (attr instanceof Attr && attr.key === "id") {
            elem = document.getElementById(attr.value) as HTMLElementTagNameMap[K];
            break;
        }
    }

    if (elem == null) {
        elem = document.createElement(name);
    }

    elementApply(elem, ...rest);

    return elem;
}

export interface IDisableable {
    disabled: boolean;
}

export function isDisableable(obj: any): obj is IDisableable {
    return "disabled" in obj
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
        await once<HTMLMediaElementEventMap>(elem, type, "error");
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

export function A(...rest: ElementChild[]): HTMLAnchorElement { return tag("a", ...rest); }
export function Abbr(...rest: ElementChild[]): HTMLElement { return tag("abbr", ...rest); }
export function Address(...rest: ElementChild[]): HTMLElement { return tag("address", ...rest); }
export function Area(...rest: ElementChild[]): HTMLAreaElement { return tag("area", ...rest); }
export function Article(...rest: ElementChild[]): HTMLElement { return tag("article", ...rest); }
export function Aside(...rest: ElementChild[]): HTMLElement { return tag("aside", ...rest); }
export function Audio(...rest: ElementChild[]): HTMLAudioElement { return tag("audio", ...rest); }
export function B(...rest: ElementChild[]): HTMLElement { return tag("b", ...rest); }
export function Base(...rest: ElementChild[]): HTMLBaseElement { return tag("base", ...rest); }
export function BDI(...rest: ElementChild[]): HTMLElement { return tag("bdi", ...rest); }
export function BDO(...rest: ElementChild[]): HTMLElement { return tag("bdo", ...rest); }
export function BlockQuote(...rest: ElementChild[]): HTMLQuoteElement { return tag("blockquote", ...rest); }
export function Body(...rest: ElementChild[]): HTMLBodyElement { return tag("body", ...rest); }
export function BR(): HTMLBRElement { return tag("br"); }
export function ButtonRaw(...rest: ElementChild[]): HTMLButtonElement { return tag("button", ...rest); }
export function Button(...rest: ElementChild[]): HTMLButtonElement { return ButtonRaw(...rest, type("button")); }
export function ButtonSmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-sm")); }
export function ButtonPrimary(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-primary")); }
export function ButtonPrimaryOutline(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-outline-primary")); }
export function ButtonPrimarySmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-sm", "btn-primary")); }
export function ButtonPrimaryOutlineSmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-sm", "btn-outline-primary")); }
export function ButtonSecondary(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-secondary")); }
export function ButtonSecondaryOutline(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-outline-secondary")); }
export function ButtonSecondarySmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-sm", "btn-secondary")); }
export function ButtonSecondaryOutlineSmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-sm", "btn-outline-secondary")); }
export function ButtonDanger(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-danger")); }
export function ButtonDangerOutline(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-outline-danger")); }
export function ButtonDangerSmall(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-sm", "btn-danger")); }
export function ButtonDangerOutlineSmalle(...rest: ElementChild[]): HTMLButtonElement { return Button(...rest, classList("btn", "btn-sm", "btn-outline-danger")); }
export function ButtonSubmit(...rest: ElementChild[]): HTMLButtonElement { return ButtonRaw(...rest, type("submit")); }
export function ButtonReset(...rest: ElementChild[]): HTMLButtonElement { return ButtonRaw(...rest, type("reset")); }
export function Canvas(...rest: ElementChild[]): HTMLCanvasElement { return tag("canvas", ...rest); }
export function Caption(...rest: ElementChild[]): HTMLTableCaptionElement { return tag("caption", ...rest); }
export function Cite(...rest: ElementChild[]): HTMLElement { return tag("cite", ...rest); }
export function Code(...rest: ElementChild[]): HTMLElement { return tag("code", ...rest); }
export function Col(...rest: ElementChild[]): HTMLTableColElement { return tag("col", ...rest); }
export function ColGroup(...rest: ElementChild[]): HTMLTableColElement { return tag("colgroup", ...rest); }
export function Data(...rest: ElementChild[]): HTMLDataElement { return tag("data", ...rest); }
export function DataList(...rest: ElementChild[]): HTMLDataListElement { return tag("datalist", ...rest); }
export function DD(...rest: ElementChild[]): HTMLElement { return tag("dd", ...rest); }
export function Del(...rest: ElementChild[]): HTMLModElement { return tag("del", ...rest); }
export function Details(...rest: ElementChild[]): HTMLDetailsElement { return tag("details", ...rest); }
export function DFN(...rest: ElementChild[]): HTMLElement { return tag("dfn", ...rest); }
export function Dialog(...rest: ElementChild[]): HTMLDialogElement { return tag("dialog", ...rest); }
export function Dir(...rest: ElementChild[]): HTMLDirectoryElement { return tag("dir", ...rest); }
export function Div(...rest: ElementChild[]): HTMLDivElement { return tag("div", ...rest); }
export function DL(...rest: ElementChild[]): HTMLDListElement { return tag("dl", ...rest); }
export function DT(...rest: ElementChild[]): HTMLElement { return tag("dt", ...rest); }
export function Em(...rest: ElementChild[]): HTMLElement { return tag("em", ...rest); }
export function Embed(...rest: ElementChild[]): HTMLEmbedElement { return tag("embed", ...rest); }
export function FieldSet(...rest: ElementChild[]): HTMLFieldSetElement { return tag("fieldset", ...rest); }
export function FigCaption(...rest: ElementChild[]): HTMLElement { return tag("figcaption", ...rest); }
export function Figure(...rest: ElementChild[]): HTMLElement { return tag("figure", ...rest); }
export function Footer(...rest: ElementChild[]): HTMLElement { return tag("footer", ...rest); }
export function Form(...rest: ElementChild[]): HTMLFormElement { return tag("form", ...rest); }
export function H1(...rest: ElementChild[]): HTMLHeadingElement { return tag("h1", ...rest); }
export function H2(...rest: ElementChild[]): HTMLHeadingElement { return tag("h2", ...rest); }
export function H3(...rest: ElementChild[]): HTMLHeadingElement { return tag("h3", ...rest); }
export function H4(...rest: ElementChild[]): HTMLHeadingElement { return tag("h4", ...rest); }
export function H5(...rest: ElementChild[]): HTMLHeadingElement { return tag("h5", ...rest); }
export function H6(...rest: ElementChild[]): HTMLHeadingElement { return tag("h6", ...rest); }
export function HR(...rest: ElementChild[]): HTMLHRElement { return tag("hr", ...rest); }
export function Head(...rest: ElementChild[]): HTMLHeadElement { return tag("head", ...rest); }
export function Header(...rest: ElementChild[]): HTMLElement { return tag("header", ...rest); }
export function HGroup(...rest: ElementChild[]): HTMLElement { return tag("hgroup", ...rest); }
export function HTML(...rest: ElementChild[]): HTMLElement { return tag("html", ...rest); }
export function I(...rest: ElementChild[]): HTMLElement { return tag("i", ...rest); }
export function FAIcon(iconName: string, ...rest: ElementChild[]): HTMLElement { return I(className(`fa fa-${iconName}`), ...rest); }
export function IFrame(...rest: ElementChild[]): HTMLIFrameElement { return tag("iframe", ...rest); }
export function Img(...rest: ElementChild[]): HTMLImageElement { return tag("img", ...rest); }
export function Input(...rest: ElementChild[]): HTMLInputElement { return tag("input", ...rest); }
export function Ins(...rest: ElementChild[]): HTMLModElement { return tag("ins", ...rest); }
export function KBD(...rest: ElementChild[]): HTMLElement { return tag("kbd", ...rest); }
export function Label(...rest: ElementChild[]): HTMLLabelElement { return tag("label", ...rest); }
export function PreLabeled<T extends Elements>(id: string, label: ElementChild, input: T): [HTMLLabelElement, T] {
    resolveElement(input).id = id;
    return [
        Label(htmlFor(id), label),
        input
    ];
}
export function PostLabeled<T extends Elements>(id: string, label: ElementChild, input: T): [T, HTMLLabelElement] {
    resolveElement(input).id = id;
    return [
        input,
        Label(htmlFor(id), label)
    ];
}
export function Legend(...rest: ElementChild[]) { return tag("legend", ...rest); }
export function LI(...rest: ElementChild[]) { return tag("li", ...rest); }
export function Link(...rest: ElementChild[]) { return tag("link", ...rest); }
export function Main(...rest: ElementChild[]) { return tag("main", ...rest); }
export function HtmlMap(...rest: ElementChild[]) { return tag("map", ...rest); }
export function Mark(...rest: ElementChild[]) { return tag("mark", ...rest); }
export function Marquee(...rest: ElementChild[]) { return tag("marquee", ...rest); }
export function Menu(...rest: ElementChild[]) { return tag("menu", ...rest); }
export function Meta(...rest: ElementChild[]) { return tag("meta", ...rest); }
export function Meter(...rest: ElementChild[]) { return tag("meter", ...rest); }
export function Nav(...rest: ElementChild[]) { return tag("nav", ...rest); }
export function NoScript(...rest: ElementChild[]) { return tag("noscript", ...rest); }
export function HtmlObject(...rest: ElementChild[]) { return tag("object", ...rest); }
export function OL(...rest: ElementChild[]) { return tag("ol", ...rest); }
export function OptGroup(...rest: ElementChild[]) { return tag("optgroup", ...rest); }
export function Option(...rest: ElementChild[]) { return tag("option", ...rest); }
export function Output(...rest: ElementChild[]) { return tag("output", ...rest); }
export function P(...rest: ElementChild[]) { return tag("p", ...rest); }
export function Param(...rest: ElementChild[]) { return tag("param", ...rest); }
export function Picture(...rest: ElementChild[]) { return tag("picture", ...rest); }
export function Pre(...rest: ElementChild[]) { return tag("pre", ...rest); }
export function Progress(...rest: ElementChild[]) { return tag("progress", ...rest); }
export function Q(...rest: ElementChild[]) { return tag("q", ...rest); }
export function RP(...rest: ElementChild[]) { return tag("rp", ...rest); }
export function RT(...rest: ElementChild[]) { return tag("rt", ...rest); }
export function Ruby(...rest: ElementChild[]) { return tag("ruby", ...rest); }
export function S(...rest: ElementChild[]) { return tag("s", ...rest); }
export function Samp(...rest: ElementChild[]) { return tag("samp", ...rest); }
export function Script(...rest: ElementChild[]) { return tag("script", ...rest); }
export function Section(...rest: ElementChild[]) { return tag("section", ...rest); }
export function Select(...rest: ElementChild[]) { return tag("select", ...rest); }
export function Slot(...rest: ElementChild[]) { return tag("slot", ...rest); }
export function Small(...rest: ElementChild[]) { return tag("small", ...rest); }
export function Source(...rest: ElementChild[]) { return tag("source", ...rest); }
export function Span(...rest: ElementChild[]) { return tag("span", ...rest); }
export function Strong(...rest: ElementChild[]) { return tag("strong", ...rest); }
export function Sub(...rest: ElementChild[]) { return tag("sub", ...rest); }
export function Summary(...rest: ElementChild[]) { return tag("summary", ...rest); }
export function Sup(...rest: ElementChild[]) { return tag("sup", ...rest); }
export function Table(...rest: ElementChild[]) { return tag("table", ...rest); }
export function TBody(...rest: ElementChild[]) { return tag("tbody", ...rest); }
export function TD(...rest: ElementChild[]) { return tag("td", ...rest); }
export function Template(...rest: ElementChild[]) { return tag("template", ...rest); }
export function TextArea(...rest: ElementChild[]) { return tag("textarea", ...rest); }
export function TFoot(...rest: ElementChild[]) { return tag("tfoot", ...rest); }
export function TH(...rest: ElementChild[]) { return tag("th", ...rest); }
export function THead(...rest: ElementChild[]) { return tag("thead", ...rest); }
export function Time(...rest: ElementChild[]) { return tag("time", ...rest); }
export function Title(...rest: ElementChild[]) { return tag("title", ...rest); }
export function TR(...rest: ElementChild[]) { return tag("tr", ...rest); }
export function Track(...rest: ElementChild[]) { return tag("track", ...rest); }
export function U(...rest: ElementChild[]) { return tag("u", ...rest); }
export function UL(...rest: ElementChild[]) { return tag("ul", ...rest); }
export function Var(...rest: ElementChild[]) { return tag("var", ...rest); }
export function Video(...rest: ElementChild[]) { return tag("video", ...rest); }
export function WBR() { return tag("wbr"); }

/**
 * creates an HTML Input tag that is a button.
 */
export function InputButton(...rest: ElementChild[]) { return Input(type("button"), ...rest); }

/**
 * creates an HTML Input tag that is a checkbox.
 */
export function InputCheckbox(...rest: ElementChild[]) { return Input(type("checkbox"), ...rest); }

/**
 * creates an HTML Input tag that is a color picker.
 */
export function InputColor(...rest: ElementChild[]) { return Input(type("color"), ...rest); }

/**
 * creates an HTML Input tag that is a date picker.
 */
export function InputDate(...rest: ElementChild[]) { return Input(type("date"), ...rest); }

/**
 * creates an HTML Input tag that is a local date-time picker.
 */
export function InputDateTime(...rest: ElementChild[]) { return Input(type("datetime-local"), ...rest); }

/**
 * creates an HTML Input tag that is an email entry field.
 */
export function InputEmail(...rest: ElementChild[]) { return Input(type("email"), ...rest); }

/**
 * creates an HTML Input tag that is a file picker.
 */
export function InputFile(...rest: ElementChild[]) { return Input(type("file"), ...rest); }

/**
 * creates an HTML Input tag that is a hidden field.
 */
export function InputHidden(...rest: ElementChild[]) { return Input(type("hidden"), ...rest); }

/**
 * creates an HTML Input tag that is a graphical submit button.
 */
export function InputImage(...rest: ElementChild[]) { return Input(type("image"), ...rest); }

/**
 * creates an HTML Input tag that is a month picker.
 */
export function InputMonth(...rest: ElementChild[]) { return Input(type("month"), ...rest); }

/**
 * creates an HTML Input tag that is a month picker.
 */
export function InputNumber(...rest: ElementChild[]) { return Input(type("number"), ...rest); }

/**
 * creates an HTML Input tag that is a password entry field.
 */
export function InputPassword(...rest: ElementChild[]) { return Input(type("password"), ...rest); }

/**
 * creates an HTML Input tag that is a radio button.
 */
export function InputRadio(...rest: ElementChild[]) { return Input(type("radio"), ...rest); }

/**
 * creates an HTML Input tag that is a range selector.
 */
export function InputRange(...rest: ElementChild[]) { return Input(type("range"), ...rest); }

/**
 * creates an HTML Input tag that is a form reset button.
 */
export function InputReset(...rest: ElementChild[]) { return Input(type("reset"), ...rest); }

/**
 * creates an HTML Input tag that is a search entry field.
 */
export function InputSearch(...rest: ElementChild[]) { return Input(type("search"), ...rest); }

/**
 * creates an HTML Input tag that is a submit button.
 */
export function InputSubmit(...rest: ElementChild[]) { return Input(type("submit"), ...rest); }

/**
 * creates an HTML Input tag that is a telephone number entry field.
 */
export function InputTelephone(...rest: ElementChild[]) { return Input(type("tel"), ...rest); }

/**
 * creates an HTML Input tag that is a text entry field.
 */
export function InputText(...rest: ElementChild[]) { return Input(type("text"), ...rest); }

/**
 * creates an HTML Input tag that is a time picker.
 */
export function InputTime(...rest: ElementChild[]) { return Input(type("time"), ...rest); }

/**
 * creates an HTML Input tag that is a URL entry field.
 */
export function InputURL(...rest: ElementChild[]) { return Input(type("url"), ...rest); }

/**
 * creates an HTML Input tag that is a week picker.
 */
export function InputWeek(...rest: ElementChild[]) { return Input(type("week"), ...rest); }

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
        styles(
            margin("auto")),
        ...rest);
}

export function Style(...rest: CSSInJSRule[]) {
    let elem = document.createElement("style");
    document.head.append(elem);

    for (let x of rest) {
        x.apply(elem.sheet);
    }

    return elem;
}

export function BackgroundAudio(autoplay: boolean, mute: boolean, looping: boolean, ...rest: ElementChild[]) {
    return Audio(
        playsInline(true),
        controls(false),
        muted(mute),
        autoPlay(autoplay),
        loop(looping),
        styles(display("none")),
        ...rest);
}

export function BackgroundVideo(autoplay: boolean, mute: boolean, looping: boolean, ...rest: ElementChild[]) {
    return Video(
        playsInline(true),
        controls(false),
        muted(mute),
        autoPlay(autoplay),
        loop(looping),
        styles(display("none")),
        ...rest);
}