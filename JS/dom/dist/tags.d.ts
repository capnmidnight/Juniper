import { IProgress } from "@juniper-lib/progress/dist/IProgress";
import { PropSet } from "./css";
export interface ErsatzElement<T extends Element = Element> {
    element: T;
}
export declare function isErsatzElement<T extends Element = Element>(obj: any): obj is ErsatzElement<T>;
export type Elements<T extends Element = Element> = T | ErsatzElement<T>;
export declare function resolveElement<T extends Element = HTMLElement>(elem: Elements<T> | string): T;
export interface IElementAppliable<T extends Element = Element> {
    applyToElement(x: Elements<T>): void;
}
export declare function isIElementAppliable<T extends Element = Element>(obj: any): obj is IElementAppliable<T>;
export type ElementChild<T extends Element = Element> = Elements<T> | IElementAppliable | string | number | boolean | Date;
export declare function isElementChild(obj: any): obj is ElementChild;
export declare function isElements(child: ElementChild): child is Elements;
export interface IFocusable {
    focus(): void;
}
export declare function isFocusable(elem: any): elem is IFocusable;
export declare function elementSetDisplay<T extends HTMLElement = HTMLElement>(elem: Elements<T>, visible: boolean, visibleDisplayType?: CssGlobalValue | CssDisplayValue): void;
export declare function elementIsDisplayed<T extends HTMLElement = HTMLElement>(elem: Elements<T>): boolean;
export declare function elementToggleDisplay<T extends HTMLElement = HTMLElement>(elem: Elements<T>, visibleDisplayType?: CssGlobalValue | CssDisplayValue): void;
export declare function elementInsertBefore(parent: Elements, newElem: Elements, refElem: Elements): void;
export declare function elementGetIndexInParent(elem: Elements): number;
export declare function Clear(): {
    applyToElement(elem: HTMLElement): void;
};
export declare function elementGetCustomData(elem: Elements<HTMLElement>, name: Lowercase<string>): string;
type ReturnElementType<T> = T extends Element ? T : T extends ShadowRoot ? T : T extends Elements<infer ElementT> ? ElementT : never;
export declare function HtmlRender<T extends Elements | ShadowRoot>(element: T | string, ...children: ElementChild[]): ReturnElementType<T>;
export declare function elementRemoveFromParent(elem: Elements | string): void;
export declare function elementReplace(elem: Elements, ...elems: Elements[]): Elements;
export declare function elementSwap<T extends Elements>(elem: Elements, withPlaceholder: (placeholder: Elements) => T): T;
export declare function getElement<T extends Element = HTMLElement>(selector: string): T;
export declare function getElements<T extends Element = HTMLElement>(selector: string): T[];
export declare function getButton(selector: string): HTMLButtonElement;
export declare function getButtons(selector: string): HTMLButtonElement[];
export declare function getInput(selector: string): HTMLInputElement;
export declare function getDataList(selector: string): HTMLDataListElement;
export declare function getInputs(selector: string): HTMLInputElement[];
export declare function getSelect(selector: string): HTMLSelectElement;
export declare function getCanvas(selector: string): HTMLCanvasElement;
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
export declare function HtmlTag<MapT extends Record<keyof MapT, HTMLElement> = HTMLElementTagNameMap, K extends keyof MapT & string = keyof MapT & string>(name: K, ...rest: ElementChild[]): MapT[K];
export interface IDisableable {
    disabled: boolean;
}
export declare function isDisableable(obj: any): obj is IDisableable;
/**
 * Empty an element of all children. This is faster than setting `innerHTML = ""`.
 */
export declare function elementClearChildren(elem: Elements): void;
export declare function elementSetText(elem: Elements<HTMLElement>, text: string): void;
export declare function elementGetText(elem: Elements<HTMLElement>): string;
export declare function elementSetTitle(elem: Elements<HTMLElement>, text: string): void;
export declare function elementSetClass(elem: Elements, enabled: boolean, className: string): void;
export type ButtonStyleType = "primary" | "secondary" | "success" | "info" | "warning" | "danger" | "light" | "dark";
export declare function buttonSetEnabled(button: Elements<HTMLButtonElement>, enabled: boolean): void;
export declare function buttonSetEnabled(button: Elements<HTMLButtonElement>, enabled: boolean, label: string): void;
export declare function buttonSetEnabled(button: Elements<HTMLButtonElement>, enabled: boolean, label: string, title: string): void;
export declare function buttonSetEnabled(button: Elements<HTMLButtonElement>, style: ButtonStyleType, enabled: boolean): void;
export declare function buttonSetEnabled(button: Elements<HTMLButtonElement>, style: ButtonStyleType, enabled: boolean, label: string): void;
export declare function buttonSetEnabled(button: Elements<HTMLButtonElement>, style: ButtonStyleType, enabled: boolean, label: string, title: string): void;
export declare function mediaElementCanPlay(elem: HTMLMediaElement, prog?: IProgress): Promise<boolean>;
export declare function mediaElementCanPlayThrough(elem: HTMLMediaElement, prog?: IProgress): Promise<boolean>;
export declare function A(...rest: ElementChild[]): HTMLAnchorElement;
export declare function Abbr(...rest: ElementChild[]): HTMLElement;
export declare function Address(...rest: ElementChild[]): HTMLElement;
export declare function Area(...rest: ElementChild[]): HTMLAreaElement;
export declare function Article(...rest: ElementChild[]): HTMLElement;
export declare function Aside(...rest: ElementChild[]): HTMLElement;
export declare function Audio(...rest: ElementChild[]): HTMLAudioElement;
export declare function B(...rest: ElementChild[]): HTMLElement;
export declare function Base(...rest: ElementChild[]): HTMLBaseElement;
export declare function BDI(...rest: ElementChild[]): HTMLElement;
export declare function BDO(...rest: ElementChild[]): HTMLElement;
export declare function BlockQuote(...rest: ElementChild[]): HTMLQuoteElement;
export declare function Body(...rest: ElementChild[]): HTMLBodyElement;
export declare function BR(): HTMLBRElement;
export declare function ButtonRaw(...rest: ElementChild[]): HTMLButtonElement;
export declare function Button(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonSmall(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonPrimary(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonPrimaryOutline(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonPrimarySmall(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonPrimaryOutlineSmall(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonSecondary(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonSecondaryOutline(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonSecondarySmall(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonSecondaryOutlineSmall(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonDanger(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonDangerOutline(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonDangerSmall(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonDangerOutlineSmalle(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonSubmit(...rest: ElementChild[]): HTMLButtonElement;
export declare function ButtonReset(...rest: ElementChild[]): HTMLButtonElement;
export declare function Canvas(...rest: ElementChild[]): HTMLCanvasElement;
export declare function Caption(...rest: ElementChild[]): HTMLTableCaptionElement;
export declare function Cite(...rest: ElementChild[]): HTMLElement;
export declare function Code(...rest: ElementChild[]): HTMLElement;
export declare function Col(...rest: ElementChild[]): HTMLTableColElement;
export declare function ColGroup(...rest: ElementChild[]): HTMLTableColElement;
export declare function DataTag(...rest: ElementChild[]): HTMLDataElement;
export declare function DataList(...rest: ElementChild[]): HTMLDataListElement;
export declare function DD(...rest: ElementChild[]): HTMLElement;
export declare function Del(...rest: ElementChild[]): HTMLModElement;
export declare function Details(...rest: ElementChild[]): HTMLDetailsElement;
export declare function DFN(...rest: ElementChild[]): HTMLElement;
export declare function Dialog(...rest: ElementChild[]): HTMLDialogElement;
export declare function Div(...rest: ElementChild[]): HTMLDivElement;
export declare function DL(...rest: ElementChild[]): HTMLDListElement;
export declare function DT(...rest: ElementChild[]): HTMLElement;
export declare function Em(...rest: ElementChild[]): HTMLElement;
export declare function Embed(...rest: ElementChild[]): HTMLEmbedElement;
export declare function FieldSet(...rest: ElementChild[]): HTMLFieldSetElement;
export declare function FigCaption(...rest: ElementChild[]): HTMLElement;
export declare function Figure(...rest: ElementChild[]): HTMLElement;
export declare function Footer(...rest: ElementChild[]): HTMLElement;
export declare function Form(...rest: ElementChild[]): HTMLFormElement;
export declare function H1(...rest: ElementChild[]): HTMLHeadingElement;
export declare function H2(...rest: ElementChild[]): HTMLHeadingElement;
export declare function H3(...rest: ElementChild[]): HTMLHeadingElement;
export declare function H4(...rest: ElementChild[]): HTMLHeadingElement;
export declare function H5(...rest: ElementChild[]): HTMLHeadingElement;
export declare function H6(...rest: ElementChild[]): HTMLHeadingElement;
export declare function HR(...rest: ElementChild[]): HTMLHRElement;
export declare function Head(...rest: ElementChild[]): HTMLHeadElement;
export declare function Header(...rest: ElementChild[]): HTMLElement;
export declare function HGroup(...rest: ElementChild[]): HTMLElement;
export declare function HTML(...rest: ElementChild[]): HTMLElement;
export declare function I(...rest: ElementChild[]): HTMLElement;
export declare function FAIcon(iconName: string, ...rest: ElementChild[]): HTMLElement;
export declare function IFrame(...rest: ElementChild[]): HTMLIFrameElement;
export declare function Img(...rest: ElementChild[]): HTMLImageElement;
export declare function Input(...rest: ElementChild[]): HTMLInputElement;
export declare function Ins(...rest: ElementChild[]): HTMLModElement;
export declare function KBD(...rest: ElementChild[]): HTMLElement;
export declare function Label(...rest: ElementChild[]): HTMLLabelElement;
export declare function PreLabeled<T extends Elements>(id: string, label: ElementChild, input: T): [HTMLLabelElement, T];
export declare function PostLabeled<T extends Elements>(id: string, label: ElementChild, input: T): [T, HTMLLabelElement];
export declare function Legend(...rest: ElementChild[]): HTMLLegendElement;
export declare function LI(...rest: ElementChild[]): HTMLLIElement;
export declare function Link(...rest: ElementChild[]): HTMLLinkElement;
export declare function Main(...rest: ElementChild[]): HTMLElement;
export declare function Map_tag(...rest: ElementChild[]): HTMLMapElement;
export declare function Mark(...rest: ElementChild[]): HTMLElement;
export declare function Menu(...rest: ElementChild[]): HTMLMenuElement;
export declare function Meta(...rest: ElementChild[]): HTMLMetaElement;
export declare function Meter(...rest: ElementChild[]): HTMLMeterElement;
export declare function Nav(...rest: ElementChild[]): HTMLElement;
export declare function NoScript(...rest: ElementChild[]): HTMLElement;
export declare function Object_tag(...rest: ElementChild[]): HTMLObjectElement;
export declare function OL(...rest: ElementChild[]): HTMLOListElement;
export declare function OptGroup(...rest: ElementChild[]): HTMLOptGroupElement;
export declare function Option(...rest: ElementChild[]): HTMLOptionElement;
export declare function Output(...rest: ElementChild[]): HTMLOutputElement;
export declare function P(...rest: ElementChild[]): HTMLParagraphElement;
export declare function Picture(...rest: ElementChild[]): HTMLPictureElement;
export declare function Pre(...rest: ElementChild[]): HTMLPreElement;
export declare function Progress(...rest: ElementChild[]): HTMLProgressElement;
export declare function Q(...rest: ElementChild[]): HTMLQuoteElement;
export declare function RP(...rest: ElementChild[]): HTMLElement;
export declare function RT(...rest: ElementChild[]): HTMLElement;
export declare function Ruby(...rest: ElementChild[]): HTMLElement;
export declare function S(...rest: ElementChild[]): HTMLElement;
export declare function Samp(...rest: ElementChild[]): HTMLElement;
export declare function Script(...rest: ElementChild[]): HTMLScriptElement;
export declare function Section(...rest: ElementChild[]): HTMLElement;
export declare function Select(...rest: ElementChild[]): HTMLSelectElement;
export declare function Slot(...rest: ElementChild[]): HTMLSlotElement;
export declare function Small(...rest: ElementChild[]): HTMLElement;
export declare function Source(...rest: ElementChild[]): HTMLSourceElement;
export declare function Span(...rest: ElementChild[]): HTMLSpanElement;
export declare function Strong(...rest: ElementChild[]): HTMLElement;
export declare function Sub(...rest: ElementChild[]): HTMLElement;
export declare function Summary(...rest: ElementChild[]): HTMLElement;
export declare function Sup(...rest: ElementChild[]): HTMLElement;
export declare function Table(...rest: ElementChild[]): HTMLTableElement;
export declare function TBody(...rest: ElementChild[]): HTMLTableSectionElement;
export declare function TD(...rest: ElementChild[]): HTMLTableCellElement;
export declare function Template(...rest: ElementChild[]): HTMLTemplateElement;
export declare function TextArea(...rest: ElementChild[]): HTMLTextAreaElement;
export declare function TFoot(...rest: ElementChild[]): HTMLTableSectionElement;
export declare function TH(...rest: ElementChild[]): HTMLTableCellElement;
export declare function THead(...rest: ElementChild[]): HTMLTableSectionElement;
export declare function Time(...rest: ElementChild[]): HTMLTimeElement;
export declare function Title(...rest: ElementChild[]): HTMLTitleElement;
export declare function TR(...rest: ElementChild[]): HTMLTableRowElement;
export declare function Track(...rest: ElementChild[]): HTMLTrackElement;
export declare function U(...rest: ElementChild[]): HTMLElement;
export declare function UL(...rest: ElementChild[]): HTMLUListElement;
export declare function Var(...rest: ElementChild[]): HTMLElement;
export declare function Video(...rest: ElementChild[]): HTMLVideoElement;
export declare function WBR(): HTMLElement;
/**
 * creates an HTML Input tag that is a button.
 */
export declare function InputButton(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a checkbox.
 */
export declare function InputCheckbox(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a color picker.
 */
export declare function InputColor(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a date picker.
 */
export declare function InputDate(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a local date-time picker.
 */
export declare function InputDateTime(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is an email entry field.
 */
export declare function InputEmail(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a file picker.
 */
export declare function InputFile(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a hidden field.
 */
export declare function InputHidden(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a graphical submit button.
 */
export declare function InputImage(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a month picker.
 */
export declare function InputMonth(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a month picker.
 */
export declare function InputNumber(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a password entry field.
 */
export declare function InputPassword(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a radio button.
 */
export declare function InputRadio(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a range selector.
 */
export declare function InputRange(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a form reset button.
 */
export declare function InputReset(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a search entry field.
 */
export declare function InputSearch(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a submit button.
 */
export declare function InputSubmit(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a telephone number entry field.
 */
export declare function InputTelephone(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a text entry field.
 */
export declare function InputText(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a time picker.
 */
export declare function InputTime(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a URL entry field.
 */
export declare function InputURL(...rest: ElementChild[]): HTMLInputElement;
/**
 * creates an HTML Input tag that is a week picker.
 */
export declare function InputWeek(...rest: ElementChild[]): HTMLInputElement;
/**
 * Creates a text node out of the give input.
 */
export declare function TextNode(txt: any): Text;
/**
 * Creates a Div element with margin: auto.
 */
export declare function Run(...rest: ElementChild[]): HTMLDivElement;
export declare function Style(...props: PropSet[]): HTMLStyleElement;
export declare function StyleBlob(...props: (string | PropSet)[]): HTMLLinkElement;
export {};
//# sourceMappingURL=tags.d.ts.map