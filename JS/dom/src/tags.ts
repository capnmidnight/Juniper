import { arrayInsert, isArray, isDefined, isObject, isString, singleton, toString } from "@juniper-lib/util";
import { ElementChild, HtmlTag } from "./HtmlTag";
import { ClassList, DateTime, HRef, InnerHTML, Rel, TitleAttr, Type } from "./attrs";
import { PropSet } from "./css";

/**********************************
 * TAGS
 *********************************/

/**
 * Creates an Anchor element.
 */
export function A(...rest: ElementChild<HTMLAnchorElement>[]) { return HtmlTag("a", ...rest); }

/**
 * Creates a Abbr element.
 */
export function Abbr(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("abbr", ...rest); }

/**
 * Creates a Address element.
 */
export function Address(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("address", ...rest); }

/**
 * Creates a Area element.
 */
export function Area(...rest: ElementChild<HTMLAreaElement>[]) { return HtmlTag("area", ...rest); }

/**
 * Creates a Article element.
 */
export function Article(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("article", ...rest); }

/**
 * Creates a Aside element.
 */
export function Aside(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("aside", ...rest); }

/**
 * Creates a Audio element.
 */
export function Audio(...rest: ElementChild<HTMLAudioElement>[]) { return HtmlTag("audio", ...rest); }

/**
 * Creates a B element.
 */
export function B(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("b", ...rest); }

/**
 * Creates a Base element.
 */
export function Base(...rest: ElementChild<HTMLBaseElement>[]) { return HtmlTag("base", ...rest); }

/**
 * Creates a BDI element.
 * @param  {...ElementChild} rest Children of the BDI element, or attribute assigners.
 * @returns {HTMLElement}
 */
export function BDI(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("bdi", ...rest); }

/**
 * Creates a BDO element.
 */
export function BDO(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("bdo", ...rest); }

/**
 * Creates a BlockQuote element.
 */
export function BlockQuote(...rest: ElementChild<HTMLQuoteElement>[]) { return HtmlTag("blockquote", ...rest); }

/**
 * Creates a Body element.
 */
export function Body(...rest: ElementChild<HTMLBodyElement>[]) { return HtmlTag("body", ...rest); }

/**
 * Creates a BR element.
 */
export function BR(): HTMLBRElement { return HtmlTag("br"); }

/**
 * Creates a raw Button element.
 */
export function ButtonRaw(...rest: ElementChild<HTMLButtonElement>[]) { return HtmlTag("button", ...rest); }

/**
 * Creates a Button element with the type already set to "button".
 */
export function Button(...rest: ElementChild<HTMLButtonElement>[]) { return ButtonRaw(Type("button"), ...rest); }

/**
 * Creates a Button element with the type already set to "submit" element.
 */
export function ButtonSubmit(...rest: ElementChild<HTMLButtonElement>[]) { return ButtonRaw(Type("submit"), ...rest); }

/**
 * Creates a Button element with the type already set to "reset".
 */
export function ButtonReset(...rest: ElementChild<HTMLButtonElement>[]) { return ButtonRaw(Type("reset"), ...rest); }

/**
 * Creates a Canvas element.
 */
export function Canvas(...rest: ElementChild<HTMLCanvasElement>[]) { return HtmlTag("canvas", ...rest); }

/**
 * Creates a Caption element.
 */
export function Caption(...rest: ElementChild<HTMLTableCaptionElement>[]) { return HtmlTag("caption", ...rest); }

/**
 * Creates a Cite element.
 */
export function Cite(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("cite", ...rest); }

/**
 * Creates a Code element.
 */
export function Code(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("code", ...rest); }

/**
 * Creates a Col element.
 */
export function Col(...rest: ElementChild<HTMLTableColElement>[]) { return HtmlTag("col", ...rest); }

/**
 * Creates a ColGroup element.
 */
export function ColGroup(...rest: ElementChild<HTMLTableColElement>[]) { return HtmlTag("colgroup", ...rest); }

/**
 * Creates a Data element.
 */
export function Data(...rest: ElementChild<HTMLDataElement>[]) { return HtmlTag("data", ...rest); }

/**
 * Creates a DataList element.
 */
export function DataList(...rest: ElementChild<HTMLDataListElement>[]) { return HtmlTag("datalist", ...rest); }

/**
 * Creates a DD element.
 */
export function DD(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("dd", ...rest); }

/**
 * Creates a Del element.
 */
export function Del(...rest: ElementChild<HTMLModElement>[]) { return HtmlTag("del", ...rest); }

/**
 * Creates a Details element.
 */
export function Details(...rest: ElementChild<HTMLDetailsElement>[]) { return HtmlTag("details", ...rest); }

/**
 * Creates a DFN element.
 */
export function DFN(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("dfn", ...rest); }

/**
 * Creates a Dialog element.
 */
export function Dialog(...rest: ElementChild<HTMLDialogElement>[]) { return HtmlTag("dialog", ...rest); }

/**
 * Creates a Div element.
 */
export function Div(...rest: ElementChild<HTMLDivElement>[]) { return HtmlTag("div", ...rest); }

/**
 * Creates a DL element.
 */
export function DL(...rest: ElementChild<HTMLDListElement>[]) { return HtmlTag("dl", ...rest); }

/**
 * Creates a DT element.
 */
export function DT(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("dt", ...rest); }

/**
 * Creates a Em element.
 */
export function Em(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("em", ...rest); }

/**
 * Creates a Embed element.
 */
export function Embed(...rest: ElementChild<HTMLEmbedElement>[]) { return HtmlTag("embed", ...rest); }

/**
 * Creates a FieldSet element.
 */
export function FieldSet(...rest: ElementChild<HTMLFieldSetElement>[]) { return HtmlTag("fieldset", ...rest); }

/**
 * Creates a FigCaption element.
 */
export function FigCaption(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("figcaption", ...rest); }

/**
 * Creates a Figure element.
 */
export function Figure(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("figure", ...rest); }

/**
 * Creates a Footer element.
 */
export function Footer(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("footer", ...rest); }

/**
 * Creates a Form element.
 */
export function FormTag(...rest: ElementChild<HTMLFormElement>[]) { return HtmlTag("form", ...rest); }

/**
 * Creates a H1 element.
 */
export function H1(...rest: ElementChild<HTMLHeadingElement>[]) { return HtmlTag("h1", ...rest); }

/**
 * Creates a H2 element.
 */
export function H2(...rest: ElementChild<HTMLHeadingElement>[]) { return HtmlTag("h2", ...rest); }

/**
 * Creates a H3 element.
 */
export function H3(...rest: ElementChild<HTMLHeadingElement>[]) { return HtmlTag("h3", ...rest); }

/**
 * Creates a H4 element.
 */
export function H4(...rest: ElementChild<HTMLHeadingElement>[]) { return HtmlTag("h4", ...rest); }

/**
 * Creates a H5 element.
 */
export function H5(...rest: ElementChild<HTMLHeadingElement>[]) { return HtmlTag("h5", ...rest); }

/**
 * Creates a H6 element.
 */
export function H6(...rest: ElementChild<HTMLHeadingElement>[]) { return HtmlTag("h6", ...rest); }

/**
 * Creates a HR element.
 */
export function HR(...rest: ElementChild<HTMLHRElement>[]) { return HtmlTag("hr", ...rest); }

/**
 * Creates a Head element.
 */
export function Head(...rest: ElementChild<HTMLHeadElement>[]) { return HtmlTag("head", ...rest); }

/**
 * Creates a Header element.
 */
export function Header(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("header", ...rest); }

/**
 * Creates a HGroup element.
 */
export function HGroup(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("hgroup", ...rest); }

/**
 * Creates a HTML element.
 */
export function HTML(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("html", ...rest); }

/**
 * Creates a I element.
 */
export function I(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("i", ...rest); }

/**
 * Creates a IFrame element.
 */
export function IFrame(...rest: ElementChild<HTMLIFrameElement>[]) { return HtmlTag("iframe", ...rest); }

/**
 * Creates a Img element.
 */
export function Img(...rest: ElementChild<HTMLImageElement>[]) { return HtmlTag("img", ...rest); }

/**
 * Creates a Input element.
 */
export function Input(...rest: ElementChild<HTMLInputElement>[]) { return HtmlTag("input", ...rest); }

/**
 * Creates a Ins element.
 */
export function Ins(...rest: ElementChild<HTMLModElement>[]) { return HtmlTag("ins", ...rest); }

/**
 * Creates a KBD element.
 */
export function KBD(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("kbd", ...rest); }

/**
 * Creates a Label element.
 */
export function Label(...rest: ElementChild<HTMLLabelElement>[]) { return HtmlTag("label", ...rest); }

/**
 * Creates a Legend element.
 */
export function Legend(...rest: ElementChild<HTMLLegendElement>[]) { return HtmlTag("legend", ...rest); }

/**
 * Creates a LI element.
 */
export function LI(...rest: ElementChild<HTMLLIElement>[]) { return HtmlTag("li", ...rest); }

/**
 * Creates a Link element.
 */
export function Link(...rest: ElementChild<HTMLLinkElement>[]) { return HtmlTag("link", ...rest); }

/**
 * Creates a Link element for stylesheets.
 */
export function LinkStyleSheet(src: string | URL, ...rest: ElementChild<HTMLLinkElement>[]) {
    return Link(
        Rel("stylesheet"),
        HRef(src),
        ...rest
    );
}

/**
 * Creates a Main element.
 */
export function Main(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("main", ...rest); }

/**
 * Creates a Map element. "Map" is a container type in JavaScript, so this function has a postfix of "_tag" to help distinguish it.
 */
export function MapTag(...rest: ElementChild<HTMLMapElement>[]) { return HtmlTag("map", ...rest); }

/**
 * Creates a Mark element.
 */
export function Mark(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("mark", ...rest); }

/**
 * Creates a Menu element.
 */
export function Menu(...rest: ElementChild<HTMLMenuElement>[]) { return HtmlTag("menu", ...rest); }

/**
 * Creates a Meta element.
 */
export function Meta(...rest: ElementChild<HTMLMetaElement>[]) { return HtmlTag("meta", ...rest); }

/**
 * Creates a Meter element.
 */
export function Meter(...rest: ElementChild<HTMLMeterElement>[]) { return HtmlTag("meter", ...rest); }

/**
 * Creates a Nav element.
 */
export function Nav(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("nav", ...rest); }

/**
 * Creates a NoScript element.
 */
export function NoScript(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("noscript", ...rest); }

/**
 * Creates an Object element. "Object" is a type in JavaScript already, so  this function has a postfix of "_tag" to help distinguish it.
 */
export function ObjectTag(...rest: ElementChild<HTMLObjectElement>[]) { return HtmlTag("object", ...rest); }

/**
 * Creates a OL element.
 */
export function OL(...rest: ElementChild<HTMLOListElement>[]) { return HtmlTag("ol", ...rest); }

/**
 * Creates a OptGroup element.
 */
export function OptGroup(...rest: ElementChild<HTMLOptGroupElement>[]) { return HtmlTag("optgroup", ...rest); }

/**
 * Creates a Option element.
 */
export function Option(...rest: ElementChild<HTMLOptionElement>[]) { return HtmlTag("option", ...rest); }

/**
 * Creates a Output element.
 */
export function Output(...rest: ElementChild<HTMLOutputElement>[]) { return HtmlTag("output", ...rest); }

/**
 * Creates a P element.
 */
export function P(...rest: ElementChild<HTMLParagraphElement>[]) { return HtmlTag("p", ...rest); }

/**
 * Creates a Picture element.

 */
export function Picture(...rest: ElementChild<HTMLParagraphElement>[]) { return HtmlTag("picture", ...rest); }

/**
 * Creates a Pre element.
 */
export function Pre(...rest: ElementChild<HTMLPreElement>[]) { return HtmlTag("pre", ...rest); }

/**
 * Creates a Progress element.
 */
export function Progress(...rest: ElementChild<HTMLProgressElement>[]) { return HtmlTag("progress", ...rest); }

/**
 * Creates a Q element.
 */
export function Q(...rest: ElementChild<HTMLQuoteElement>[]) { return HtmlTag("q", ...rest); }

/**
 * Creates a RP element.
 */
export function RP(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("rp", ...rest); }

/**
 * Creates a RT element.
 */
export function RT(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("rt", ...rest); }

/**
 * Creates a Ruby element.
 */
export function Ruby(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("ruby", ...rest); }

/**
 * Creates a S element.
 */
export function S(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("s", ...rest); }

/**
 * Creates a Samp element.
 */
export function Samp(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("samp", ...rest); }

/**
 * Creates a Script element.
 */
export function Script(...rest: ElementChild<HTMLScriptElement>[]) { return HtmlTag("script", ...rest); }

/**
 * Creates a Section element.
 */
export function Section(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("section", ...rest); }

/**
 * Creates a Select element.
 */
export function Select(...rest: ElementChild<HTMLSelectElement>[]) { return HtmlTag("select", ...rest); }

/**
 * Creates a Slot element.
 */
export function SlotTag(...rest: ElementChild<HTMLSlotElement>[]) { return HtmlTag("slot", ...rest); }

/**
 * Creates a Small element.
 */
export function Small(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("small", ...rest); }

/**
 * Creates a Source element.
 */
export function Source(...rest: ElementChild<HTMLSourceElement>[]) { return HtmlTag("source", ...rest); }

/**
 * Creates a Span element.
 */
export function SpanTag(...rest: ElementChild<HTMLSpanElement>[]) { return HtmlTag("span", ...rest); }

/**
 * Creates a Strong element.
 */
export function Strong(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("strong", ...rest); }

/**
 * Creates a Sub element.
 */
export function Sub(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("sub", ...rest); }

/**
 * Creates a Link element that references a CSS StyleSheet in a Blob URL.
 */
export function StyleBlob(content: string): HTMLLinkElement;
export function StyleBlob(...content: PropSet[]): HTMLLinkElement;
export function StyleBlob(...content: (PropSet|string)[]): HTMLLinkElement {
    for (let i = 0; i < content.length; ++i) {
        const prop = content[i];
        if (prop instanceof PropSet) {
            const subProps = prop._subProps;
            const insertionPoint = content.length;
            for (let j = subProps.length - 1; j >= 0; --j) {
                const subProp = subProps[j];
                if (subProp instanceof PropSet) {
                    arrayInsert(content, subProp, insertionPoint);
                    subProps.splice(j, 1);
                }
            }
        }
    }

    const rules = content
        .filter(p => isString(p) || isObject(p) && (!p._subProps || p._subProps.length > 0))
        .map(toString);
    const blob = new Blob(rules, { type: "text/css" });
    return LinkStyleSheet(URL.createObjectURL(blob));
}

export function SingletonStyleBlob(name: string, makeContent: () => (PropSet | PropSet[])) {
    return singleton(name + "::StyleSheet", () => {
        let content = makeContent();
        if (!isArray(content)) {
            content = [content];
        }
        const styleLink = StyleBlob(...content);
        document.head.append(styleLink);
        return styleLink;
    });
}

/**
 * Creates a CSS Style element.
 */
export function StyleTag(...rest: PropSet[]): HTMLStyleElement {
    return HtmlTag<HTMLStyleElement>("style",
        InnerHTML(rest
            .map(toString)
            .join("\n")
        )
    );
}

/**
 * Creates a Summary element.
 */
export function SummaryTag(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("summary", ...rest); }

/**
 * Creates a Sup element.
 */
export function Sup(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("sup", ...rest); }

/**
 * Creates a Table element.
 */
export function Table(...rest: ElementChild<HTMLTableElement>[]) { return HtmlTag("table", ...rest); }

/**
 * Creates a TBody element.
 */
export function TBody(...rest: ElementChild<HTMLTableSectionElement>[]) { return HtmlTag("tbody", ...rest); }

/**
 * Creates a TD element.
 */
export function TD(...rest: ElementChild<HTMLTableCellElement>[]) { return HtmlTag("td", ...rest); }

/**
 * Creates a Template element.
 */
export function Template(...rest: ElementChild<HTMLTemplateElement>[]) { return HtmlTag("template", ...rest); }

/**
 * Streamlines creating instances of templates
 * @param name
 * @param factory
 * @returns
 */
export function TemplateInstance(name: string, factory: () => (ElementChild | ElementChild[])): DocumentFragment {
    const template = singleton(name + "::Template", () => {
        let children = factory();
        if (!isArray(children)) {
            children = [children];
        };
        return Template(...children);
    });
    return template.content.cloneNode(true) as DocumentFragment;
}

/**
 * Creates a TextArea element.
 */
export function TextArea(...rest: ElementChild<HTMLTextAreaElement>[]) { return HtmlTag("textarea", ...rest); }

/**
 * Creates a TFoot element.
 */
export function TFoot(...rest: ElementChild<HTMLTableSectionElement>[]) { return HtmlTag("tfoot", ...rest); }

/**
 * Creates a TH element.
 */
export function TH(...rest: ElementChild<HTMLTableCellElement>[]) { return HtmlTag("th", ...rest); }

/**
 * Creates a THead element.
 */
export function THead(...rest: ElementChild<HTMLTableSectionElement>[]) { return HtmlTag("thead", ...rest); }

/**
 * Creates a Time element.
 */
export function Time(date: Date, formattedDate: string, ...rest: ElementChild<HTMLTimeElement>[]) {
    return HtmlTag<HTMLTimeElement>(
        "time",
        ...rest,
        DateTime(date),
        date && TitleAttr(date.toLocaleString()) || null,
        formattedDate
    );
}

/**
 * Creates a Title element.
 */
export function TitleTag(...rest: ElementChild<HTMLTitleElement>[]) { return HtmlTag("title", ...rest); }

/**
 * Creates a TR element.
 */
export function TR(...rest: ElementChild<HTMLTableRowElement>[]) { return HtmlTag("tr", ...rest); }

/**
 * Creates a Track element.
 */
export function Track(...rest: ElementChild<HTMLTrackElement>[]) { return HtmlTag("track", ...rest); }

/**
 * Creates a U element.
 */
export function U(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("u", ...rest); }

/**
 * Creates a UL element.
 */
export function UL(...rest: ElementChild<HTMLUListElement>[]) { return HtmlTag("ul", ...rest); }

/**
 * Creates a Var element.
 */
export function Var(...rest: ElementChild<HTMLElement>[]) { return HtmlTag("var", ...rest); }

/**
 * Creates a Video element.
 */
export function Video(...rest: ElementChild<HTMLVideoElement>[]) { return HtmlTag("video", ...rest); }

/**
 * Creates a WBR element.
 */
export function WBR(): HTMLElement { return HtmlTag("wbr"); }

/**
 * Creates a FontAwesome icon
 */
export function FAIcon(iconName: string, ...rest: ElementChild<HTMLElement>[]) { return I(ClassList("fa", "fa-solid", `fa-${iconName}`), ...rest); }

/**
 * creates an HTML Input tag that is a button.
 */
export function InputButton(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("button"), ...rest); }

/**
 * creates an HTML Input tag that is a checkbox.
 */
export function InputCheckbox(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("checkbox"), ...rest); }

/**
 * creates an HTML Input tag that is a color picker.
 */
export function InputColor(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("color"), ...rest); }

/**
 * creates an HTML Input tag that is a date picker.
 */
export function InputDate(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("date"), ...rest); }

/**
 * creates an HTML Input tag that is a local date-time picker.
 */
export function InputDateTime(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("datetime-local"), ...rest); }

/**
 * creates an HTML Input tag that is an email entry field.
 */
export function InputEmail(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("email"), ...rest); }

/**
 * creates an HTML Input tag that is a file picker.
 */
export function InputFile(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("file"), ...rest); }

/**
 * creates an HTML Input tag that is a hidden field.
 */
export function InputHidden(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("hidden"), ...rest); }

/**
 * creates an HTML Input tag that is a graphical submit button.
 */
export function InputImage(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("image"), ...rest); }

/**
 * creates an HTML Input tag that is a month picker.
 */
export function InputMonth(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("month"), ...rest); }

/**
 * creates an HTML Input tag that is a month picker.
 */
export function InputNumber(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("number"), ...rest); }

/**
 * creates an HTML Input tag that is a password entry field.
 */
export function InputPassword(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("password"), ...rest); }

/**
 * creates an HTML Input tag that is a radio button.
 */
export function InputRadio(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("radio"), ...rest); }

/**
 * creates an HTML Input tag that is a range selector.
 */
export function InputRange(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("range"), ...rest); }

/**
 * creates an HTML Input tag that is a form reset button.
 */
export function InputReset(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("reset"), ...rest); }

/**
 * creates an HTML Input tag that is a search entry field.
 */
export function InputSearch(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("search"), ...rest); }

/**
 * creates an HTML Input tag that is a submit button.
 */
export function InputSubmit(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("submit"), ...rest); }

/**
 * creates an HTML Input tag that is a telephone number entry field.
 */
export function InputTelephone(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("tel"), ...rest); }

/**
 * creates an HTML Input tag that is a text entry field.
 */
export function InputText(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("text"), ...rest); }

/**
 * creates an HTML Input tag that is a time picker.
 */
export function InputTime(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("time"), ...rest); }

/**
 * creates an HTML Input tag that is a URL entry field.
 */
export function InputURL(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("url"), ...rest); }

/**
 * creates an HTML Input tag that is a week picker.
 */
export function InputWeek(...rest: ElementChild<HTMLInputElement>[]) { return Input(Type("week"), ...rest); }

/**
 * Creates a text node out of the given input.
 */
export function TextNode(txt: { toString: () => string; }) {
    return isDefined(txt)
        && document.createTextNode(txt.toString())
        || null;
}
