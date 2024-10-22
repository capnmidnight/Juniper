import { ElementChild } from "./HtmlTag";
import { PropSet } from "./css";
/**********************************
 * TAGS
 *********************************/
/**
 * Creates an Anchor element.
 */
export declare function A(...rest: ElementChild<HTMLAnchorElement>[]): HTMLAnchorElement;
/**
 * Creates a Abbr element.
 */
export declare function Abbr(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Address element.
 */
export declare function Address(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Area element.
 */
export declare function Area(...rest: ElementChild<HTMLAreaElement>[]): HTMLAreaElement;
/**
 * Creates a Article element.
 */
export declare function Article(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Aside element.
 */
export declare function Aside(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Audio element.
 */
export declare function Audio(...rest: ElementChild<HTMLAudioElement>[]): HTMLAudioElement;
/**
 * Creates a B element.
 */
export declare function B(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Base element.
 */
export declare function Base(...rest: ElementChild<HTMLBaseElement>[]): HTMLBaseElement;
/**
 * Creates a BDI element.
 * @param  {...ElementChild} rest Children of the BDI element, or attribute assigners.
 * @returns {HTMLElement}
 */
export declare function BDI(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a BDO element.
 */
export declare function BDO(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a BlockQuote element.
 */
export declare function BlockQuote(...rest: ElementChild<HTMLQuoteElement>[]): HTMLQuoteElement;
/**
 * Creates a Body element.
 */
export declare function Body(...rest: ElementChild<HTMLBodyElement>[]): HTMLBodyElement;
/**
 * Creates a BR element.
 */
export declare function BR(): HTMLBRElement;
/**
 * Creates a raw Button element.
 */
export declare function ButtonRaw(...rest: ElementChild<HTMLButtonElement>[]): HTMLButtonElement;
/**
 * Creates a Button element with the type already set to "button".
 */
export declare function Button(...rest: ElementChild<HTMLButtonElement>[]): HTMLButtonElement;
/**
 * Creates a Button element with the type already set to "submit" element.
 */
export declare function ButtonSubmit(...rest: ElementChild<HTMLButtonElement>[]): HTMLButtonElement;
/**
 * Creates a Button element with the type already set to "reset".
 */
export declare function ButtonReset(...rest: ElementChild<HTMLButtonElement>[]): HTMLButtonElement;
/**
 * Creates a Canvas element.
 */
export declare function Canvas(...rest: ElementChild<HTMLCanvasElement>[]): HTMLCanvasElement;
/**
 * Creates a Caption element.
 */
export declare function Caption(...rest: ElementChild<HTMLTableCaptionElement>[]): HTMLTableCaptionElement;
/**
 * Creates a Cite element.
 */
export declare function Cite(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Code element.
 */
export declare function Code(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Col element.
 */
export declare function Col(...rest: ElementChild<HTMLTableColElement>[]): HTMLTableColElement;
/**
 * Creates a ColGroup element.
 */
export declare function ColGroup(...rest: ElementChild<HTMLTableColElement>[]): HTMLTableColElement;
/**
 * Creates a Data element.
 */
export declare function Data(...rest: ElementChild<HTMLDataElement>[]): HTMLDataElement;
/**
 * Creates a DataList element.
 */
export declare function DataList(...rest: ElementChild<HTMLDataListElement>[]): HTMLDataListElement;
/**
 * Creates a DD element.
 */
export declare function DD(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Del element.
 */
export declare function Del(...rest: ElementChild<HTMLModElement>[]): HTMLModElement;
/**
 * Creates a Details element.
 */
export declare function Details(...rest: ElementChild<HTMLDetailsElement>[]): HTMLDetailsElement;
/**
 * Creates a DFN element.
 */
export declare function DFN(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Dialog element.
 */
export declare function Dialog(...rest: ElementChild<HTMLDialogElement>[]): HTMLDialogElement;
/**
 * Creates a Div element.
 */
export declare function Div(...rest: ElementChild<HTMLDivElement>[]): HTMLDivElement;
/**
 * Creates a DL element.
 */
export declare function DL(...rest: ElementChild<HTMLDListElement>[]): HTMLDListElement;
/**
 * Creates a DT element.
 */
export declare function DT(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Em element.
 */
export declare function Em(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Embed element.
 */
export declare function Embed(...rest: ElementChild<HTMLEmbedElement>[]): HTMLEmbedElement;
/**
 * Creates a FieldSet element.
 */
export declare function FieldSet(...rest: ElementChild<HTMLFieldSetElement>[]): HTMLFieldSetElement;
/**
 * Creates a FigCaption element.
 */
export declare function FigCaption(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Figure element.
 */
export declare function Figure(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Footer element.
 */
export declare function Footer(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Form element.
 */
export declare function FormTag(...rest: ElementChild<HTMLFormElement>[]): HTMLFormElement;
/**
 * Creates a H1 element.
 */
export declare function H1(...rest: ElementChild<HTMLHeadingElement>[]): HTMLHeadingElement;
/**
 * Creates a H2 element.
 */
export declare function H2(...rest: ElementChild<HTMLHeadingElement>[]): HTMLHeadingElement;
/**
 * Creates a H3 element.
 */
export declare function H3(...rest: ElementChild<HTMLHeadingElement>[]): HTMLHeadingElement;
/**
 * Creates a H4 element.
 */
export declare function H4(...rest: ElementChild<HTMLHeadingElement>[]): HTMLHeadingElement;
/**
 * Creates a H5 element.
 */
export declare function H5(...rest: ElementChild<HTMLHeadingElement>[]): HTMLHeadingElement;
/**
 * Creates a H6 element.
 */
export declare function H6(...rest: ElementChild<HTMLHeadingElement>[]): HTMLHeadingElement;
/**
 * Creates a HR element.
 */
export declare function HR(...rest: ElementChild<HTMLHRElement>[]): HTMLHRElement;
/**
 * Creates a Head element.
 */
export declare function Head(...rest: ElementChild<HTMLHeadElement>[]): HTMLHeadElement;
/**
 * Creates a Header element.
 */
export declare function Header(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a HGroup element.
 */
export declare function HGroup(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a HTML element.
 */
export declare function HTML(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a I element.
 */
export declare function I(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a IFrame element.
 */
export declare function IFrame(...rest: ElementChild<HTMLIFrameElement>[]): HTMLIFrameElement;
/**
 * Creates a Img element.
 */
export declare function Img(...rest: ElementChild<HTMLImageElement>[]): HTMLImageElement;
/**
 * Creates a Input element.
 */
export declare function Input(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * Creates a Ins element.
 */
export declare function Ins(...rest: ElementChild<HTMLModElement>[]): HTMLModElement;
/**
 * Creates a KBD element.
 */
export declare function KBD(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Label element.
 */
export declare function Label(...rest: ElementChild<HTMLLabelElement>[]): HTMLLabelElement;
/**
 * Creates a Legend element.
 */
export declare function Legend(...rest: ElementChild<HTMLLegendElement>[]): HTMLLegendElement;
/**
 * Creates a LI element.
 */
export declare function LI(...rest: ElementChild<HTMLLIElement>[]): HTMLLIElement;
/**
 * Creates a Link element.
 */
export declare function Link(...rest: ElementChild<HTMLLinkElement>[]): HTMLLinkElement;
/**
 * Creates a Link element for stylesheets.
 */
export declare function LinkStyleSheet(src: string | URL, ...rest: ElementChild<HTMLLinkElement>[]): HTMLLinkElement;
/**
 * Creates a Main element.
 */
export declare function Main(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Map element. "Map" is a container type in JavaScript, so this function has a postfix of "_tag" to help distinguish it.
 */
export declare function MapTag(...rest: ElementChild<HTMLMapElement>[]): HTMLMapElement;
/**
 * Creates a Mark element.
 */
export declare function Mark(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Menu element.
 */
export declare function Menu(...rest: ElementChild<HTMLMenuElement>[]): HTMLMenuElement;
/**
 * Creates a Meta element.
 */
export declare function Meta(...rest: ElementChild<HTMLMetaElement>[]): HTMLMetaElement;
/**
 * Creates a Meter element.
 */
export declare function Meter(...rest: ElementChild<HTMLMeterElement>[]): HTMLMeterElement;
/**
 * Creates a Nav element.
 */
export declare function Nav(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a NoScript element.
 */
export declare function NoScript(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates an Object element. "Object" is a type in JavaScript already, so  this function has a postfix of "_tag" to help distinguish it.
 */
export declare function ObjectTag(...rest: ElementChild<HTMLObjectElement>[]): HTMLObjectElement;
/**
 * Creates a OL element.
 */
export declare function OL(...rest: ElementChild<HTMLOListElement>[]): HTMLOListElement;
/**
 * Creates a OptGroup element.
 */
export declare function OptGroup(...rest: ElementChild<HTMLOptGroupElement>[]): HTMLOptGroupElement;
/**
 * Creates a Option element.
 */
export declare function Option(...rest: ElementChild<HTMLOptionElement>[]): HTMLOptionElement;
/**
 * Creates a Output element.
 */
export declare function Output(...rest: ElementChild<HTMLOutputElement>[]): HTMLOutputElement;
/**
 * Creates a P element.
 */
export declare function P(...rest: ElementChild<HTMLParagraphElement>[]): HTMLParagraphElement;
/**
 * Creates a Picture element.

 */
export declare function Picture(...rest: ElementChild<HTMLParagraphElement>[]): HTMLParagraphElement;
/**
 * Creates a Pre element.
 */
export declare function Pre(...rest: ElementChild<HTMLPreElement>[]): HTMLPreElement;
/**
 * Creates a Progress element.
 */
export declare function Progress(...rest: ElementChild<HTMLProgressElement>[]): HTMLProgressElement;
/**
 * Creates a Q element.
 */
export declare function Q(...rest: ElementChild<HTMLQuoteElement>[]): HTMLQuoteElement;
/**
 * Creates a RP element.
 */
export declare function RP(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a RT element.
 */
export declare function RT(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Ruby element.
 */
export declare function Ruby(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a S element.
 */
export declare function S(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Samp element.
 */
export declare function Samp(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Script element.
 */
export declare function Script(...rest: ElementChild<HTMLScriptElement>[]): HTMLScriptElement;
/**
 * Creates a Section element.
 */
export declare function Section(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Select element.
 */
export declare function Select(...rest: ElementChild<HTMLSelectElement>[]): HTMLSelectElement;
/**
 * Creates a Slot element.
 */
export declare function SlotTag(...rest: ElementChild<HTMLSlotElement>[]): HTMLSlotElement;
/**
 * Creates a Small element.
 */
export declare function Small(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Source element.
 */
export declare function Source(...rest: ElementChild<HTMLSourceElement>[]): HTMLSourceElement;
/**
 * Creates a Span element.
 */
export declare function SpanTag(...rest: ElementChild<HTMLSpanElement>[]): HTMLSpanElement;
/**
 * Creates a Strong element.
 */
export declare function Strong(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Sub element.
 */
export declare function Sub(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Link element that references a CSS StyleSheet in a Blob URL.
 */
export declare function StyleBlob(content: string): HTMLLinkElement;
export declare function StyleBlob(...content: PropSet[]): HTMLLinkElement;
export declare function SingletonStyleBlob(name: string, makeContent: () => (PropSet | PropSet[])): HTMLLinkElement;
/**
 * Creates a CSS Style element.
 */
export declare function StyleTag(...rest: PropSet[]): HTMLStyleElement;
/**
 * Creates a Summary element.
 */
export declare function SummaryTag(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Sup element.
 */
export declare function Sup(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Table element.
 */
export declare function Table(...rest: ElementChild<HTMLTableElement>[]): HTMLTableElement;
/**
 * Creates a TBody element.
 */
export declare function TBody(...rest: ElementChild<HTMLTableSectionElement>[]): HTMLTableSectionElement;
/**
 * Creates a TD element.
 */
export declare function TD(...rest: ElementChild<HTMLTableCellElement>[]): HTMLTableCellElement;
/**
 * Creates a Template element.
 */
export declare function Template(...rest: ElementChild<HTMLTemplateElement>[]): HTMLTemplateElement;
/**
 * Streamlines creating instances of templates
 * @param name
 * @param factory
 * @returns
 */
export declare function TemplateInstance(name: string, factory: () => (ElementChild | ElementChild[])): DocumentFragment;
/**
 * Creates a TextArea element.
 */
export declare function TextArea(...rest: ElementChild<HTMLTextAreaElement>[]): HTMLTextAreaElement;
/**
 * Creates a TFoot element.
 */
export declare function TFoot(...rest: ElementChild<HTMLTableSectionElement>[]): HTMLTableSectionElement;
/**
 * Creates a TH element.
 */
export declare function TH(...rest: ElementChild<HTMLTableCellElement>[]): HTMLTableCellElement;
/**
 * Creates a THead element.
 */
export declare function THead(...rest: ElementChild<HTMLTableSectionElement>[]): HTMLTableSectionElement;
/**
 * Creates a Time element.
 */
export declare function Time(date: Date, formattedDate: string, ...rest: ElementChild<HTMLTimeElement>[]): HTMLTimeElement;
/**
 * Creates a Title element.
 */
export declare function TitleTag(...rest: ElementChild<HTMLTitleElement>[]): HTMLTitleElement;
/**
 * Creates a TR element.
 */
export declare function TR(...rest: ElementChild<HTMLTableRowElement>[]): HTMLTableRowElement;
/**
 * Creates a Track element.
 */
export declare function Track(...rest: ElementChild<HTMLTrackElement>[]): HTMLTrackElement;
/**
 * Creates a U element.
 */
export declare function U(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a UL element.
 */
export declare function UL(...rest: ElementChild<HTMLUListElement>[]): HTMLUListElement;
/**
 * Creates a Var element.
 */
export declare function Var(...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * Creates a Video element.
 */
export declare function Video(...rest: ElementChild<HTMLVideoElement>[]): HTMLVideoElement;
/**
 * Creates a WBR element.
 */
export declare function WBR(): HTMLElement;
/**
 * Creates a FontAwesome icon
 */
export declare function FAIcon(iconName: string, ...rest: ElementChild<HTMLElement>[]): HTMLElement;
/**
 * creates an HTML Input tag that is a button.
 */
export declare function InputButton(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a checkbox.
 */
export declare function InputCheckbox(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a color picker.
 */
export declare function InputColor(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a date picker.
 */
export declare function InputDate(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a local date-time picker.
 */
export declare function InputDateTime(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is an email entry field.
 */
export declare function InputEmail(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a file picker.
 */
export declare function InputFile(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a hidden field.
 */
export declare function InputHidden(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a graphical submit button.
 */
export declare function InputImage(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a month picker.
 */
export declare function InputMonth(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a month picker.
 */
export declare function InputNumber(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a password entry field.
 */
export declare function InputPassword(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a radio button.
 */
export declare function InputRadio(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a range selector.
 */
export declare function InputRange(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a form reset button.
 */
export declare function InputReset(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a search entry field.
 */
export declare function InputSearch(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a submit button.
 */
export declare function InputSubmit(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a telephone number entry field.
 */
export declare function InputTelephone(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a text entry field.
 */
export declare function InputText(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a time picker.
 */
export declare function InputTime(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a URL entry field.
 */
export declare function InputURL(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a week picker.
 */
export declare function InputWeek(...rest: ElementChild<HTMLInputElement>[]): HTMLInputElement;
/**
 * Creates a text node out of the given input.
 */
export declare function TextNode(txt: {
    toString: () => string;
}): Text;
//# sourceMappingURL=tags.d.ts.map