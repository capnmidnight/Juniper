import { AbstractKeyValueAppliable } from "./AbstractKeyValueAppliable";
import { HtmlAttr } from "./HtmlAttr";
import { HtmlProp } from "./HtmlProp";
export * from "./AbstractKeyValueAppliable";
export * from "./Aria";
export * from "./HtmlAttr";
export * from "./HtmlProp";
/**********************************
 * ATTRIBUTES
 *********************************/
/**
 * a list of types the server accepts, typically a file type.
 * @param value - the value to set on the attribute.
 **/
export declare function Accept(value: string): HtmlProp<"accept", string, Node & Record<"accept", string>>;
/**
 * The accessKey attribute
 **/
export declare function AccessKey(value: string): HtmlProp<"accessKey", string, Node & Record<"accessKey", string>>;
/**
 * specifying the horizontal alignment of the element.
 **/
export declare function Align(value: string): HtmlProp<"align", string, Node & Record<"align", string>>;
/**
 * Specifies a feature-policy for the iframe.
 **/
export declare function Allow(value: string): HtmlProp<"allow", string, Node & Record<"allow", string>>;
/**
 * Whether or not to allow an IFrame to open full screen elements.
 **/
export declare function AllowFullscreen(value: boolean): HtmlProp<"allowFullscreen", boolean, Node & Record<"allowFullscreen", boolean>>;
/**
 * Alternative text in case an image can't be displayed.
 **/
export declare function Alt(value: string): HtmlProp<"alt", string, Node & Record<"alt", string>>;
export declare function Anchor(value: string): HtmlAttr<string, Node>;
/**
 * Executes the script asynchronously.
 **/
export declare function Async(value: boolean): HtmlProp<"async", boolean, Node & Record<"async", boolean>>;
type HTMLAutoCapitalizeValue = "none" | "off" | "sentences" | "on" | "words" | "characters";
/**
 * Sets whether input is automatically capitalized when entered by user
 **/
export declare function AutoCapitalize(value: HTMLAutoCapitalizeValue | boolean): HtmlProp<"autocapitalize", HTMLAutoCapitalizeValue, Node & Record<"autocapitalize", HTMLAutoCapitalizeValue>>;
/**
 * Indicates whether controls in this form can by default have their values automatically completed by the browser.
 **/
export declare function AutoComplete(value: AutoFill | boolean): HtmlProp<"autocomplete", AutoFill, Node & Record<"autocomplete", AutoFill>>;
/**
 * The element should be automatically focused after the page loaded.
 **/
export declare function AutoFocus(value: boolean): HtmlProp<"autofocus", boolean, Node & Record<"autofocus", boolean>>;
/**
 * The audio or video should play as soon as possible.
 **/
export declare function AutoPlay(value: boolean): HtmlProp<"autoplay", boolean, Node & Record<"autoplay", boolean>>;
/**
 * Contains the time range of already buffered media.
 **/
export declare function Buffered(value: boolean): HtmlProp<"buffered", boolean, Node & Record<"buffered", boolean>>;
/**
 * From the HTML Media Capture
 **/
export declare function Capture(value: boolean): HtmlProp<"capture", boolean, Node & Record<"capture", boolean>>;
/**
 * Declares the character encoding of the page or script.
 **/
export declare function CharSet(value: string): HtmlProp<"charset", string, Node & Record<"charset", string>>;
/**
 * Indicates whether the element should be checked on page load.
 **/
export declare function Checked(value: boolean): HtmlProp<"checked", boolean, Node & Record<"checked", boolean>>;
/**
 * Contains a URI which points to the source of the quote or change.
 **/
export declare function CiteURI(value: string): HtmlProp<"cite", string, Node & Record<"cite", string>>;
/**
 * Often used with CSS to style elements with common properties.
 **/
export declare function ClassName(value: string): HtmlProp<"className", string, Node & Record<"className", string>>;
declare class HtmlClassListAttr extends HtmlProp<"classList", string[] | DOMTokenList> {
    constructor(classes: string[]);
    apply(tag: Node): void;
}
/**
 * Assign a list of CSS classes to an element
 * @param classes the type to assign
 */
export declare function ClassList(...classes: string[]): HtmlClassListAttr;
/**
 * Specifies the URL of the applet's class file to be loaded and executed.
 **/
export declare function CodeURI(value: string): HtmlProp<"code", string, Node & Record<"code", string>>;
/**
 * This attribute gives the absolute or relative URL of the directory where applets' .class files referenced by the code attribute are stored.
 **/
export declare function CodeBase(value: string): HtmlProp<"codebase", string, Node & Record<"codebase", string>>;
/**
 * Defines the number of columns in a textarea.
 **/
export declare function Cols(value: number): HtmlProp<"cols", number, Node & Record<"cols", number>>;
/**
 * The colspan attribute defines the number of columns a cell should span.
 **/
export declare function ColSpan(value: number): HtmlProp<"colSpan", number, Node & Record<"colSpan", number>>;
/**
 * A value associated with http-equiv or name depending on the context.
 **/
export declare function Content(value: string): HtmlProp<"content", string, Node & Record<"content", string>>;
/**
 * Indicates whether the element's content is editable.
 **/
export declare function ContentEditable(value: boolean): HtmlProp<"contenteditable", boolean, Node & Record<"contenteditable", boolean>>;
/**
 * Defines the ID of a <menu> element which will serve as the element's context menu.
 **/
export declare function ContextMenu(value: string): HtmlProp<"contextmenu", string, Node & Record<"contextmenu", string>>;
/**
 * Indicates whether the browser should show playback controls to the user.
 **/
export declare function Controls(value: boolean): HtmlProp<"controls", boolean, Node & Record<"controls", boolean>>;
/**
 * A set of values specifying the coordinates of the hot-spot region.
 **/
export declare function Coords(value: string): HtmlProp<"coords", string, Node & Record<"coords", string>>;
type HTMLCrossOriginValue = "anonymous" | "use-credentials";
/**
 * How the element handles cross-origin requests
 * @param {} value
 **/
export declare function CrossOrigin(value: HTMLCrossOriginValue): HtmlProp<"crossorigin", HTMLCrossOriginValue, Node & Record<"crossorigin", HTMLCrossOriginValue>>;
/**
 * Specifies the Content Security Policy that an embedded document must agree to enforce upon itself.
 **/
export declare function CSP(value: string): HtmlProp<"csp", string, Node & Record<"csp", string>>;
/**
 * Lets you attach custom attributes to an HTML element.
 **/
export declare function CustomData(name: string, value: {
    toString(): string;
}): HtmlAttr<{
    toString(): string;
}, Node>;
/**
 * Indicates the date and time associated with the element.
 **/
export declare function DateTime(value: Date): HtmlProp<"dateTime", string, Node & Record<"dateTime", string>>;
/**
 * Indicates the preferred method to decode the image.
 **/
export declare function Decoding(value: string): HtmlProp<"decoding", string, Node & Record<"decoding", string>>;
/**
 * Indicates that the track should be enabled unless the user's preferences indicate something different.
 **/
export declare function Default(value: boolean | string): HtmlProp<"default", string | boolean, Node & Record<"default", string | boolean>>;
/**
 * Indicates that the script should be executed after the page has been parsed.
 **/
export declare function Defer(value: boolean): HtmlProp<"defer", boolean, Node & Record<"defer", boolean>>;
/**
 * Defines the text direction. Allowed values are ltr (Left-To-Right) or rtl (Right-To-Left)
 **/
export declare function Dir(value: string): HtmlProp<"dir", string, Node & Record<"dir", string>>;
/**
 * Indicates whether the user can interact with the element.
 **/
export declare function Disabled(value: boolean): HtmlProp<"disabled", boolean, Node & Record<"disabled", boolean>>;
/**
 * The name to provide a POSTed FormData field when submitting an Input or TextArea's input direction.
 **/
export declare function DirName(value: string): HtmlProp<"dirname", string, Node & Record<"dirname", string>>;
/**
 * Indicates that the hyperlink is to be used for downloading a resource by giving the file a name.
 * @param name the name of the file to download
 */
export declare function Download(name: string): HtmlProp<"download", string, Node & Record<"download", string>>;
/**
 * Defines whether the element can be dragged.
 **/
export declare function Draggable(value: boolean): HtmlProp<"draggable", boolean, Node & Record<"draggable", boolean>>;
/**
 * Indicates that the element accepts the dropping of content onto it.
 **/
export declare function DropZone(value: string): HtmlProp<"dropzone", string, Node & Record<"dropzone", string>>;
/**
 * Defines the content type of the form data when the method is POST.
 **/
export declare function EncType(value: string): HtmlProp<"enctype", string, Node & Record<"enctype", string>>;
/**
 * The enterkeyhint specifies what action label (or icon) to present for the enter key on virtual keyboards. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
 **/
export declare function EnterKeyHint(value: string): HtmlProp<"enterkeyhint", string, Node & Record<"enterkeyhint", string>>;
/**
 * Describes elements which belongs to this one.
 **/
export declare function For(value: string): HtmlProp<"htmlFor", string, Node & Record<"htmlFor", string>>;
/**
 * Indicates the form that is the owner of the element.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export declare function FormAttr(form: HTMLFormElement | string): HtmlAttr<string, Node>;
/**
 * Indicates the action of the element, overriding the action defined in the <form>.
 **/
export declare function FormAction(value: string): HtmlProp<"formaction", string, Node & Record<"formaction", string>>;
/**
 * If the button/input is a submit button (type="submit"
), this attribute sets the encoding type to use during form submission. If this attribute is specified, it overrides the enctype attribute of the button's form owner.
 **/
export declare function FormEncType(value: string): HtmlProp<"formenctype", string, Node & Record<"formenctype", string>>;
/**
 * If the button/input is a submit button (type="submit"
), this attribute sets the submission method to use during form submission (GET, POST, etc.). If this attribute is specified, it overrides the method attribute of the button's form owner.
 **/
export declare function FormMethod(value: string): HtmlProp<"formmethod", string, Node & Record<"formmethod", string>>;
/**
 * If the button/input is a submit button (type="submit"
), this boolean attribute specifies that the form is not to be validated when it is submitted. If this attribute is specified, it overrides the novalidate attribute of the button's form owner.
 **/
export declare function FormNoValidate(value: boolean): HtmlProp<"formnovalidate", boolean, Node & Record<"formnovalidate", boolean>>;
/**
 * If the button/input is a submit button (type="submit"
), this attribute specifies the browsing context (for example, tab, window, or inline frame) in which to display the response that is received after submitting the form. If this attribute is specified, it overrides the target attribute of the button's form owner.
 **/
export declare function FormTarget(value: string): HtmlProp<"formtarget", string, Node & Record<"formtarget", string>>;
/**
 * Width of the border to put around an `iframe` tag.
 * @param value
 **/
export declare function FrameBorder(value: string | number | boolean): HtmlProp<"frameborder", string | number, Node & Record<"frameborder", string | number>>;
/**
 * IDs of the <th> elements which applies to this element.
 **/
export declare function Headers(value: string): HtmlProp<"headers", string, Node & Record<"headers", string>>;
/**
 * Specifies the height of elements listed here. For all other elements, use the CSS height property.
 * @param value
 **/
export declare function Height(value: number | string): HtmlProp<"height", string | number, Node & Record<"height", string | number>>;
/**
 * Prevents rendering of given element, while keeping child elements, e.g. script elements, active.
 **/
export declare function Hidden(value: boolean): HtmlProp<"hidden", boolean, Node & Record<"hidden", boolean>>;
/**
 * Indicates the lower bound of the upper range.
 **/
export declare function High(value: number): HtmlProp<"high", number, Node & Record<"high", number>>;
/**
 * The URL of a linked resource.
 * @param href the hyper-reference to assign
 */
export declare function HRef(href: string | URL): HtmlProp<"href", string, Node & Record<"href", string>>;
/**
 * Specifies the language of the linked resource.
 **/
export declare function HRefLang(value: string): HtmlProp<"hreflang", string, Node & Record<"hreflang", string>>;
/**
 * Defines a pragma directive.
 **/
export declare function HttpEquiv(value: string): HtmlProp<"httpEquiv", string, Node & Record<"httpEquiv", string>>;
/**
 * Specifies a picture which represents the command.
 **/
export declare function Icon(value: string): HtmlProp<"icon", string, Node & Record<"icon", string>>;
export declare function IconAttr(value: string): HtmlAttr<string, Node>;
export declare class HtmlIDAttr extends HtmlProp<"id", string> {
    readonly required: boolean;
    constructor(value: string, required: boolean);
}
/**
 * Often used with CSS to style a specific element. The value of this attribute must be unique.
 **/
export declare function ID(value: string, required?: boolean): HtmlIDAttr;
/**
 * Indicates the relative fetch priority for the resource.
 **/
export declare function Importance(value: string): HtmlProp<"importance", string, Node & Record<"importance", string>>;
/**
 * Sets the innerHTML property of an element.
 **/
export declare function InnerHTML(html: string): HtmlProp<"innerHTML", string, Node & Record<"innerHTML", string>>;
/**
 * Provides a hint as to the type of data that might be entered by the user while editing the element or its contents. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
 **/
export declare function InputMode(value: string): HtmlProp<"inputmode", string, Node & Record<"inputmode", string>>;
/**
 * Specifies a sub-resource Integrity value that allows browsers to verify what they fetch.
 **/
export declare function Integrity(value: string): HtmlProp<"integrity", string, Node & Record<"integrity", string>>;
/**
 * This attribute tells the browser to ignore the actual intrinsic size of the image and pretend it’s the size specified in the attribute.
 **/
export declare function IntrinsicSize(value: string): HtmlProp<"intrinsicsize", string, Node & Record<"intrinsicsize", string>>;
export declare class HtmlIsAttr extends HtmlAttr<string, HTMLElement> {
    constructor(value: string);
}
/**
 * This attribute indicates that a base HTML element should be rendered as a given custom element name.
 */
export declare function Is(value: string): HtmlIsAttr;
/**
 * Indicates that the image is part of a server-side image map.
 **/
export declare function IsMap(value: boolean): HtmlProp<"ismap", boolean, Node & Record<"ismap", boolean>>;
/**
 * The itemprop attribute
 **/
export declare function ItemProp(value: string): HtmlProp<"itemprop", string, Node & Record<"itemprop", string>>;
/**
 * Specifies the type of key generated.
 **/
export declare function KeyType(value: string): HtmlProp<"keytype", string, Node & Record<"keytype", string>>;
/**
 * Specifies the kind of text track.
 **/
export declare function Kind(value: string): HtmlProp<"kind", string, Node & Record<"kind", string>>;
/**
 * Specifies a user-readable title of the element.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export declare function LabelAttr(value: string): HtmlProp<"label", string, Node & Record<"label", string>>;
/**
 * Defines the language used in the element.
 **/
export declare function Lang(value: string): HtmlProp<"lang", string, Node & Record<"lang", string>>;
/**
 * Defines the script language used in the element.
 **/
export declare function Language(value: string): HtmlProp<"language", string, Node & Record<"language", string>>;
/**
 * Identifies a list of pre-defined options to suggest to the user.
 **/
export declare function ListAttr(value: string | HTMLDataListElement): HtmlAttr<string, Node>;
/**
 * Indicates whether the media should start playing from the start when it's finished.
 **/
export declare function Loop(value: boolean): HtmlProp<"loop", boolean, Node & Record<"loop", boolean>>;
/**
 * Indicates the upper bound of the lower range.
 **/
export declare function Low(value: number): HtmlProp<"low", number, Node & Record<"low", number>>;
/**
 * Indicates the maximum value allowed.
 **/
export declare function Max(value: number): HtmlProp<"max", string, Node & Record<"max", string>>;
/**
 * Defines the maximum number of characters allowed in the element.
 **/
export declare function MaxLength(value: number): HtmlProp<"maxlength", number, Node & Record<"maxlength", number>>;
/**
 * Defines the minimum number of characters allowed in the element.
 **/
export declare function MinLength(value: number): HtmlProp<"minlength", number, Node & Record<"minlength", number>>;
/**
 * Specifies a hint of the media for which the linked resource was designed.
 **/
export declare function Media(value: string): HtmlProp<"media", string, Node & Record<"media", string>>;
/**
 * Defines which HTTP method to use when submitting the form. Can be GET (default) or POST.
 **/
export declare function Method(value: string): HtmlProp<"method", string, Node & Record<"method", string>>;
/**
 * Indicates the minimum value allowed.
 **/
export declare function Min(value: number): HtmlProp<"min", string, Node & Record<"min", string>>;
/**
 * Indicates whether multiple values can be entered in an input of the type email or file.
 **/
export declare function Multiple(value: boolean): HtmlProp<"multiple", boolean, Node & Record<"multiple", boolean>>;
/**
 * Indicates whether the audio will be initially silenced on page load.
 **/
export declare function Muted(value: boolean): HtmlProp<"muted", boolean, Node & Record<"muted", boolean>>;
/**
 * Name of the element. For example used by the server to identify the fields in form submits.
 **/
export declare function Name(value: string): HtmlProp<"name", string, Node & Record<"name", string>>;
/**
 * This attribute indicates that the form shouldn't be validated when submitted.
 **/
export declare function NoValidate(value: boolean): HtmlProp<"novalidate", boolean, Node & Record<"novalidate", boolean>>;
/**
 * Indicates whether the details will be shown on page load.
 **/
export declare function Open(value: boolean): HtmlProp<"open", boolean, Node & Record<"open", boolean>>;
/**
 * Indicates the optimal numeric value.
 **/
export declare function Optimum(value: number): HtmlProp<"optimum", number, Node & Record<"optimum", number>>;
/**
 * Defines a regular expression which the element's value will be validated against.
 **/
export declare function Pattern(value: string): HtmlProp<"pattern", string, Node & Record<"pattern", string>>;
/**
 * The ping attribute specifies a space-separated list of URLs to be notified if a user follows the hyperlink.
 **/
export declare function Ping(value: string): HtmlProp<"ping", string, Node & Record<"ping", string>>;
/**
 * Provides a hint to the user of what can be entered in the field.
 **/
export declare function PlaceHolder(value: string): HtmlProp<"placeholder", string, Node & Record<"placeholder", string>>;
export type PopoverValue = "auto" | "manual";
/**
 * Used to designate an element as a popover element.
 **/
export declare function Popover(value: PopoverValue): HtmlProp<"popover", string, Node & Record<"popover", string>>;
/**
 * Turns a <button> element into a popover control button; takes the ID of the popover element to control as its value.
 **/
export declare function PopoverTargetAttr(value: string): HtmlAttr<string, Node>;
/**
 * Turns a <button> element into a popover control button; takes the ID of the popover element to control as its value.
 **/
export declare function PopoverTargetElement(value: Element): HtmlProp<"popoverTargetElement", Element, Node & Record<"popoverTargetElement", Element>>;
export type PopoverTargetActionValue = "hide" | "show" | "toggle";
/**
 * Specifies the action to be performed on a popover element being controlled by a control <button>. Possible values are:
 **/
export declare function PopoverTargetAction(value: PopoverTargetActionValue): HtmlProp<"popoverTargetAction", string, Node & Record<"popoverTargetAction", string>>;
/**
 * A URL indicating a poster frame to show until the user plays or seeks.
 **/
export declare function Poster(value: string): HtmlProp<"poster", string, Node & Record<"poster", string>>;
/**
 * Indicates whether the whole resource, parts of it or nothing should be preloaded.
 **/
export declare function Preload(value: boolean | string): HtmlProp<"preload", string | boolean, Node & Record<"preload", string | boolean>>;
/**
 * Indicates whether the element can be edited.
 **/
export declare function ReadOnly(value: boolean): HtmlProp<"readOnly", boolean, Node & Record<"readOnly", boolean>>;
/**
 * The radiogroup attribute
 **/
export declare function RadioGroup(value: string): HtmlProp<"radiogroup", string, Node & Record<"radiogroup", string>>;
/**
 * Specifies which referrer is sent when fetching the resource.
 **/
export declare function ReferrerPolicy(value: string): HtmlProp<"referrerpolicy", string, Node & Record<"referrerpolicy", string>>;
/**
 * Specifies the relationship of the target object to the link object.
 **/
export declare function Rel(value: string): HtmlProp<"rel", string, Node & Record<"rel", string>>;
/**
 * Indicates whether this element is required to fill out or not.
 **/
export declare function Required(value: boolean): HtmlProp<"required", boolean, Node & Record<"required", boolean>>;
/**
 * Indicates whether the list should be displayed in a descending order instead of a ascending.
 **/
export declare function Reversed(value: boolean): HtmlProp<"reversed", boolean, Node & Record<"reversed", boolean>>;
/**
 * Defines the number of rows in a text area.
 **/
export declare function Role(value: string): HtmlProp<"role", string, Node & Record<"role", string>>;
/**
 * The rows attribute
 **/
export declare function Rows(value: number): HtmlProp<"rows", number, Node & Record<"rows", number>>;
/**
 * Defines the number of rows a table cell should span over.
 **/
export declare function RowSpan(value: number): HtmlProp<"rowSpan", number, Node & Record<"rowSpan", number>>;
/**
 * Stops a document loaded in an iframe from using certain features (such as submitting forms or opening new windows).
 **/
export declare function Sandbox(value: string): HtmlProp<"sandbox", string, Node & Record<"sandbox", string>>;
/**
 * Defines the cells that the header test (defined in the th element) relates to.
 **/
export declare function Scope(value: string): HtmlProp<"scope", string, Node & Record<"scope", string>>;
/**
 * The scoped attribute for `style` tags.
 **/
export declare function Scoped(value: boolean): HtmlProp<"scoped", boolean, Node & Record<"scoped", boolean>>;
/**
 * The scrolling attribute for `iframe` tags.
 **/
export declare function Scrolling(value: boolean): HtmlProp<"scrolling", string, Node & Record<"scrolling", string>>;
/**
 * Defines an `option` tag which will be selected on page load.
 **/
export declare function Selected(value: boolean): HtmlProp<"selected", boolean, Node & Record<"selected", boolean>>;
/**
 * Sets the `selectedIndex` property on a Select element.
 */
export declare function SelectedIndex(value: number): HtmlProp<"selectedIndex", number, Node & Record<"selectedIndex", number>>;
/**
 * The shape attribute for `a` and `area` tags.
 **/
export declare function Shape(value: string): HtmlProp<"shape", string, Node & Record<"shape", string>>;
/**
 * Defines the width of the element (in pixels). If the element's type attribute is text or password then it's the number of characters.
 **/
export declare function Size(value: number): HtmlProp<"size", number, Node & Record<"size", number>>;
/**
 * Assigns a slot in a shadow DOM shadow tree to an element.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export declare function SlotAttr(value: string): HtmlProp<"slot", string, Node & Record<"slot", string>>;
/**
 * The sizes attribute
 **/
export declare function Sizes(value: string): HtmlProp<"sizes", string, Node & Record<"sizes", string>>;
/**
 * The span attribute.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export declare function SpanAttr(value: string): HtmlProp<"span", string, Node & Record<"span", string>>;
/**
 * Indicates whether spell checking is allowed for the element.
 **/
export declare function SpellCheck(value: boolean): HtmlProp<"spellcheck", boolean, Node & Record<"spellcheck", boolean>>;
/**
 * The URL of the embeddable content.
 **/
export declare function Src(value: string | URL): HtmlProp<"src", string, Node & Record<"src", string>>;
/**
 * The srcdoc attribute
 **/
export declare function SrcDoc(value: string): HtmlProp<"srcdoc", string, Node & Record<"srcdoc", string>>;
/**
 * The srclang attribute
 **/
export declare function SrcLang(value: string): HtmlProp<"srclang", string, Node & Record<"srclang", string>>;
/**
 * A MediaStream object to use as a source for an HTML video or audio element
 **/
export declare function SrcObject(value: MediaProvider): HtmlProp<"srcObject", MediaProvider, Node & Record<"srcObject", MediaProvider>>;
/**
 * One or more responsive image candidates.
 **/
export declare function SrcSet(value: string): HtmlProp<"srcset", string, Node & Record<"srcset", string>>;
/**
 * Defines the first number if other than 1.
 **/
export declare function Start(value: number): HtmlProp<"start", number, Node & Record<"start", number>>;
/**
 * The step attribute
 **/
export declare function Step(value: number): HtmlProp<"step", string, Node & Record<"step", string>>;
export declare class HtmlStyleAttr extends AbstractKeyValueAppliable<HTMLElement, "style", Record<string, string>> {
    constructor(dict: Record<string, string>);
    apply(tag: HTMLElement): void;
}
/**
 * Creates an assigner function for objects with a style attribute.
 * @param dict the value to assign. Use the full, kebob-case CSS field names as keys, e.g. "grid-template-columns".
 */
export declare function StyleAttr(dict: Record<string, string>): HtmlStyleAttr;
/**
 * The summary attribute.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export declare function SummaryAttr(value: string): HtmlProp<"summary", string, Node & Record<"summary", string>>;
/**
 * Overrides the browser's default tab order and follows the one specified instead.
 **/
export declare function TabIndex(value: number): HtmlProp<"tabIndex", number, Node & Record<"tabIndex", number>>;
export declare function TextContent(text: {
    toString(): string;
}): HtmlProp<"textContent", {
    toString(): string;
}, Node & Record<"textContent", {
    toString(): string;
}>>;
/**
 * Text to be displayed in a tooltip when hovering over the element.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export declare function TitleAttr(value: string): HtmlProp<"title", string, Node & Record<"title", string>>;
/**
 * The target attribute
 **/
export declare function Target(value: string): HtmlProp<"target", string, Node & Record<"target", string>>;
/**
 * Specify whether an element’s attribute values and the values of its Text node children are to be translated when the page is localized, or whether to leave them unchanged.
 **/
export declare function Translate(value: boolean): HtmlProp<"translate", boolean, Node & Record<"translate", boolean>>;
/**
 * Defines the type of the element.
 **/
export declare function Type(value: string | {
    value: string;
}): HtmlProp<"type", string, Node & Record<"type", string>>;
/**
 * Defines the type of the element.
 **/
export declare function TypeAttr(value: string): HtmlAttr<string, Node>;
/**
 * Defines a default value which will be displayed in the element on page load.
 */
export declare function Value(value: {
    toString(): string;
}): HtmlProp<"value", {
    toString(): string;
}, Node & Record<"value", {
    toString(): string;
}>>;
/**
 * Defines a default value which will be displayed in the element on page load.
 **/
export declare function ValueAsNumber(value: number): HtmlProp<"valueAsNumber", number, Node & Record<"valueAsNumber", number>>;
/**
 * Defines a default value which will be displayed in the element on page load.
 **/
export declare function ValueAsDate(value: Date): HtmlProp<"valueAsDate", Date, Node & Record<"valueAsDate", Date>>;
/**
 * setting the volume at which a media element plays.
 **/
export declare function Volume(value: number): HtmlProp<"volume", number, Node & Record<"volume", number>>;
/**
 * The usemap attribute
 **/
export declare function UseMap(value: boolean): HtmlProp<"usemap", boolean, Node & Record<"usemap", boolean>>;
/**
 * For the elements listed here, this establishes the element's width.
 **/
export declare function Width(value: number | string): HtmlProp<"width", string | number, Node & Record<"width", string | number>>;
type HTMLWrapValue = "hard" | "soft" | "off";
/**
 * Indicates whether the text should be wrapped.
 **/
export declare function Wrap(value: HTMLWrapValue): HtmlProp<"wrap", HTMLWrapValue, Node & Record<"wrap", HTMLWrapValue>>;
//# sourceMappingURL=index.d.ts.map