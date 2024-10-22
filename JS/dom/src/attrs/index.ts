import { identity, isBoolean, stringRandom, unpackURL } from "@juniper-lib/util";
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
export function Accept(value: string) {
    return new HtmlProp("accept", value);
}

/**
 * The accessKey attribute
 **/
export function AccessKey(value: string) {
    return new HtmlProp("accessKey", value);
}

/**
 * specifying the horizontal alignment of the element.
 **/
export function Align(value: string) {
    return new HtmlProp("align", value);
}

/**
 * Specifies a feature-policy for the iframe.
 **/
export function Allow(value: string) {
    return new HtmlProp("allow", value);
}

/**
 * Whether or not to allow an IFrame to open full screen elements.
 **/
export function AllowFullscreen(value: boolean) {
    return new HtmlProp("allowFullscreen", value);
}

/**
 * Alternative text in case an image can't be displayed.
 **/
export function Alt(value: string) {
    return new HtmlProp("alt", value);
}

export function Anchor(value: string) {
    return new HtmlAttr("anchor", value);
}

/**
 * Executes the script asynchronously.
 **/
export function Async(value: boolean) {
    return new HtmlProp("async", value);
}

type HTMLAutoCapitalizeValue =
    | "none"//Do not automatically capitalize any text.
    | "off" //Do not automatically capitalize any text.
    | "sentences"//Automatically capitalize the first character of each sentence.
    | "on" //Automatically capitalize the first character of each sentence.
    | "words" // Automatically capitalize the first character of each word.
    | "characters"; // Automatically capitalize every character.

/**
 * Sets whether input is automatically capitalized when entered by user
 **/
export function AutoCapitalize(value: HTMLAutoCapitalizeValue | boolean) {
    if (isBoolean(value)) {
        value = value ? "on" : "off";
    }

    return new HtmlProp(
        "autocapitalize",
        value
    );
}

/**
 * Indicates whether controls in this form can by default have their values automatically completed by the browser.
 **/
export function AutoComplete(value: AutoFill | boolean) {
    if (isBoolean(value)) {
        value = value ? "on" : "off";
    }

    return new HtmlProp("autocomplete", value);
}

/**
 * The element should be automatically focused after the page loaded.
 **/
export function AutoFocus(value: boolean) {
    return new HtmlProp("autofocus", value);
}

/**
 * The audio or video should play as soon as possible.
 **/
export function AutoPlay(value: boolean) {
    return new HtmlProp("autoplay", value);
}

/**
 * Contains the time range of already buffered media.
 **/
export function Buffered(value: boolean) {
    return new HtmlProp("buffered", value);
}

/**
 * From the HTML Media Capture
 **/
export function Capture(value: boolean) {
    return new HtmlProp("capture", value);
}

/**
 * Declares the character encoding of the page or script.
 **/
export function CharSet(value: string) {
    return new HtmlProp("charset", value);
}

/**
 * Indicates whether the element should be checked on page load.
 **/
export function Checked(value: boolean) {
    return new HtmlProp("checked", value);
}

/**
 * Contains a URI which points to the source of the quote or change.
 **/
export function CiteURI(value: string) {
    return new HtmlProp("cite", value);
}

/**
 * Often used with CSS to style elements with common properties.
 **/
export function ClassName(value: string) {
    return new HtmlProp("className", value);
}

class HtmlClassListAttr extends HtmlProp<"classList", string[] | DOMTokenList> {
    constructor(classes: string[]) {
        super("classList", classes.filter(identity));
    }

    override apply(tag: Node): void {
        const classes = this.value as string[];
        if (tag instanceof Element) {
            tag.classList.add(...classes.filter(c =>
                c && !tag.classList.contains(c)));
        }
    }
}

/**
 * Assign a list of CSS classes to an element
 * @param classes the type to assign
 */
export function ClassList(...classes: string[]) {
    return new HtmlClassListAttr(classes);
}

/**
 * Specifies the URL of the applet's class file to be loaded and executed.
 **/
export function CodeURI(value: string) {
    return new HtmlProp("code", value);
}

/**
 * This attribute gives the absolute or relative URL of the directory where applets' .class files referenced by the code attribute are stored.
 **/
export function CodeBase(value: string) {
    return new HtmlProp("codebase", value);
}

/**
 * Defines the number of columns in a textarea.
 **/
export function Cols(value: number) {
    return new HtmlProp("cols", value);
}

/**
 * The colspan attribute defines the number of columns a cell should span.
 **/
export function ColSpan(value: number) {
    return new HtmlProp("colSpan", value);
}

/**
 * A value associated with http-equiv or name depending on the context.
 **/
export function Content(value: string) {
    return new HtmlProp("content", value);
}

/**
 * Indicates whether the element's content is editable.
 **/
export function ContentEditable(value: boolean) {
    return new HtmlProp(
        "contenteditable",
        value
    );
}

/**
 * Defines the ID of a <menu> element which will serve as the element's context menu.
 **/
export function ContextMenu(value: string) {
    return new HtmlProp(
        "contextmenu",
        value
    );
}

/**
 * Indicates whether the browser should show playback controls to the user.
 **/
export function Controls(value: boolean) {
    return new HtmlProp("controls", value);
}

/**
 * A set of values specifying the coordinates of the hot-spot region.
 **/
export function Coords(value: string) {
    return new HtmlProp("coords", value);
}

type HTMLCrossOriginValue = "anonymous" | "use-credentials";

/**
 * How the element handles cross-origin requests
 * @param {} value
 **/
export function CrossOrigin(value: HTMLCrossOriginValue) {
    return new HtmlProp("crossorigin", value);
}

/**
 * Specifies the Content Security Policy that an embedded document must agree to enforce upon itself.
 **/
export function CSP(value: string) {
    return new HtmlProp("csp", value);
}

/**
 * Lets you attach custom attributes to an HTML element.
 **/
export function CustomData(name: string, value: { toString(): string }) {
    name = "data-" + name.toLowerCase();
    return new HtmlAttr(name, value);
}

/**
 * Indicates the date and time associated with the element.
 **/
export function DateTime(value: Date) {
    return new HtmlProp("dateTime", value?.toISOString());
}

/**
 * Indicates the preferred method to decode the image.
 **/
export function Decoding(value: string) {
    return new HtmlProp("decoding", value);
}

/**
 * Indicates that the track should be enabled unless the user's preferences indicate something different.
 **/
export function Default(value: boolean | string) {
    return new HtmlProp("default", value);
}

/**
 * Indicates that the script should be executed after the page has been parsed.
 **/
export function Defer(value: boolean) {
    return new HtmlProp("defer", value);
}

/**
 * Defines the text direction. Allowed values are ltr (Left-To-Right) or rtl (Right-To-Left)
 **/
export function Dir(value: string) {
    return new HtmlProp(
        "dir",
        value
    );
}

/**
 * Indicates whether the user can interact with the element.
 **/
export function Disabled(value: boolean) {
    return new HtmlProp("disabled", value);
}

/**
 * The name to provide a POSTed FormData field when submitting an Input or TextArea's input direction.
 **/
export function DirName(value: string) {
    return new HtmlProp("dirname", value);
}

/**
 * Indicates that the hyperlink is to be used for downloading a resource by giving the file a name.
 * @param name the name of the file to download 
 */
export function Download(name: string) {
    return new HtmlProp("download", name);
}

/**
 * Defines whether the element can be dragged.
 **/
export function Draggable(value: boolean) {
    return new HtmlProp(
        "draggable",
        value
    );
}

/**
 * Indicates that the element accepts the dropping of content onto it.
 **/
export function DropZone(value: string) {
    return new HtmlProp(
        "dropzone",
        value
    );
}

/**
 * Defines the content type of the form data when the method is POST.
 **/
export function EncType(value: string) {
    return new HtmlProp("enctype", value);
}

/**
 * The enterkeyhint specifies what action label (or icon) to present for the enter key on virtual keyboards. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
 **/
export function EnterKeyHint(value: string) {
    return new HtmlProp("enterkeyhint", value);
}

/**
 * Describes elements which belongs to this one.
 **/
export function For(value: string) {
    return new HtmlProp("htmlFor", value);
}

/**
 * Indicates the form that is the owner of the element. 
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function FormAttr(form: HTMLFormElement | string) {
    if (form instanceof HTMLFormElement) {
        if (!form.id) {
            form.id = stringRandom(12);
        }
        form = form.id;
    }

    return new HtmlAttr("form", form);
}

/**
 * Indicates the action of the element, overriding the action defined in the <form>.
 **/
export function FormAction(value: string) {
    return new HtmlProp("formaction", value);
}

/**
 * If the button/input is a submit button (type="submit"
), this attribute sets the encoding type to use during form submission. If this attribute is specified, it overrides the enctype attribute of the button's form owner.
 **/
export function FormEncType(value: string) {
    return new HtmlProp("formenctype", value);
}

/**
 * If the button/input is a submit button (type="submit"
), this attribute sets the submission method to use during form submission (GET, POST, etc.). If this attribute is specified, it overrides the method attribute of the button's form owner.
 **/
export function FormMethod(value: string) {
    return new HtmlProp("formmethod", value);
}

/**
 * If the button/input is a submit button (type="submit"
), this boolean attribute specifies that the form is not to be validated when it is submitted. If this attribute is specified, it overrides the novalidate attribute of the button's form owner.
 **/
export function FormNoValidate(value: boolean) {
    return new HtmlProp("formnovalidate", value);
}

/**
 * If the button/input is a submit button (type="submit"
), this attribute specifies the browsing context (for example, tab, window, or inline frame) in which to display the response that is received after submitting the form. If this attribute is specified, it overrides the target attribute of the button's form owner.
 **/
export function FormTarget(value: string) {
    return new HtmlProp("formtarget", value);
}

/**
 * Width of the border to put around an `iframe` tag.
 * @param value
 **/
export function FrameBorder(value: string | number | boolean) {
    if (typeof value === "boolean") {
        value = value ? "yes" : "no";
    }

    return new HtmlProp("frameborder", value);
}

/**
 * IDs of the <th> elements which applies to this element.
 **/
export function Headers(value: string) {
    return new HtmlProp("headers", value);
}

/**
 * Specifies the height of elements listed here. For all other elements, use the CSS height property.
 * @param value
 **/
export function Height(value: number | string) {
    return new HtmlProp("height", value);
}

/**
 * Prevents rendering of given element, while keeping child elements, e.g. script elements, active.
 **/
export function Hidden(value: boolean) {
    return new HtmlProp(
        "hidden",
        value
    );
}

/**
 * Indicates the lower bound of the upper range.
 **/
export function High(value: number) {
    return new HtmlProp("high", value);
}

/**
 * The URL of a linked resource.
 * @param href the hyper-reference to assign 
 */
export function HRef(href: string | URL) {
    return new HtmlProp("href", unpackURL(href));
}

/**
 * Specifies the language of the linked resource.
 **/
export function HRefLang(value: string) {
    return new HtmlProp("hreflang", value);
}

/**
 * Defines a pragma directive.
 **/
export function HttpEquiv(value: string) {
    return new HtmlProp("httpEquiv", value);
}

/**
 * Specifies a picture which represents the command.
 **/
export function Icon(value: string) {
    return new HtmlProp("icon", value);
}

export function IconAttr(value: string) {
    return new HtmlAttr("icon", value);
}

export class HtmlIDAttr extends HtmlProp<"id", string> {
    constructor(value: string, public readonly required: boolean) {
        super("id", value);
    }
}

/**
 * Often used with CSS to style a specific element. The value of this attribute must be unique.
 **/
export function ID(value: string, required = false) {
    return new HtmlIDAttr(
        value,
        required
    );
}

/**
 * Indicates the relative fetch priority for the resource.
 **/
export function Importance(value: string) {
    return new HtmlProp("importance", value);
}

/**
 * Sets the innerHTML property of an element.
 **/
export function InnerHTML(html: string) {
    return new HtmlProp(
        "innerHTML",
        html
    );
}

/**
 * Provides a hint as to the type of data that might be entered by the user while editing the element or its contents. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
 **/
export function InputMode(value: string) {
    return new HtmlProp("inputmode", value);
}

/**
 * Specifies a sub-resource Integrity value that allows browsers to verify what they fetch.
 **/
export function Integrity(value: string) {
    return new HtmlProp("integrity", value);
}

/**
 * This attribute tells the browser to ignore the actual intrinsic size of the image and pretend it’s the size specified in the attribute.
 **/
export function IntrinsicSize(value: string) {
    return new HtmlProp("intrinsicsize", value);
}

export class HtmlIsAttr extends HtmlAttr<string, HTMLElement> {
    constructor(value: string) {
        super("is", value);
    }
}

/**
 * This attribute indicates that a base HTML element should be rendered as a given custom element name.
 */
export function Is(value: string) {
    return new HtmlIsAttr(value);
}

/**
 * Indicates that the image is part of a server-side image map.
 **/
export function IsMap(value: boolean) {
    return new HtmlProp("ismap", value);
}

/**
 * The itemprop attribute
 **/
export function ItemProp(value: string) {
    return new HtmlProp(
        "itemprop",
        value
    );
}

/**
 * Specifies the type of key generated.
 **/
export function KeyType(value: string) {
    return new HtmlProp("keytype", value);
}

/**
 * Specifies the kind of text track.
 **/
export function Kind(value: string) {
    return new HtmlProp("kind", value);
}

/**
 * Specifies a user-readable title of the element.
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function LabelAttr(value: string) {
    return new HtmlProp("label", value);
}

/**
 * Defines the language used in the element.
 **/
export function Lang(value: string) {
    return new HtmlProp(
        "lang",
        value
    );
}

/**
 * Defines the script language used in the element.
 **/
export function Language(value: string) {
    return new HtmlProp("language", value);
}

/**
 * Identifies a list of pre-defined options to suggest to the user.
 **/
export function ListAttr(value: string | HTMLDataListElement) {
    if (value instanceof HTMLDataListElement) {
        if (!value.id) {
            value.id = stringRandom(12);
        }

        if (!value.isConnected) {
            document.body.append(value);
        }

        value = value.id;
    }

    return new HtmlAttr("list", value);
}

/**
 * Indicates whether the media should start playing from the start when it's finished.
 **/
export function Loop(value: boolean) {
    return new HtmlProp("loop", value);
}

/**
 * Indicates the upper bound of the lower range.
 **/
export function Low(value: number) {
    return new HtmlProp("low", value);
}

/**
 * Indicates the maximum value allowed.
 **/
export function Max(value: number) {
    return new HtmlProp("max", value.toString());
}

/**
 * Defines the maximum number of characters allowed in the element.
 **/
export function MaxLength(value: number) {
    return new HtmlProp("maxlength", value);
}

/**
 * Defines the minimum number of characters allowed in the element.
 **/
export function MinLength(value: number) {
    return new HtmlProp("minlength", value);
}

/**
 * Specifies a hint of the media for which the linked resource was designed.
 **/
export function Media(value: string) {
    return new HtmlProp("media", value);
}

/**
 * Defines which HTTP method to use when submitting the form. Can be GET (default) or POST.
 **/
export function Method(value: string) {
    return new HtmlProp("method", value);
}

/**
 * Indicates the minimum value allowed.
 **/
export function Min(value: number) {
    return new HtmlProp("min", value.toString());
}

/**
 * Indicates whether multiple values can be entered in an input of the type email or file.
 **/
export function Multiple(value: boolean) {
    return new HtmlProp("multiple", value);
}

/**
 * Indicates whether the audio will be initially silenced on page load.
 **/
export function Muted(value: boolean) {
    return new HtmlProp("muted", value);
}

/**
 * Name of the element. For example used by the server to identify the fields in form submits.
 **/
export function Name(value: string) {
    return new HtmlProp("name", value);
}

/**
 * This attribute indicates that the form shouldn't be validated when submitted.
 **/
export function NoValidate(value: boolean) {
    return new HtmlProp("novalidate", value);
}

/**
 * Indicates whether the details will be shown on page load.
 **/
export function Open(value: boolean) {
    return new HtmlProp("open", value);
}

/**
 * Indicates the optimal numeric value.
 **/
export function Optimum(value: number) {
    return new HtmlProp("optimum", value);
}

/**
 * Defines a regular expression which the element's value will be validated against.
 **/
export function Pattern(value: string) {
    return new HtmlProp("pattern", value);
}

/**
 * The ping attribute specifies a space-separated list of URLs to be notified if a user follows the hyperlink.
 **/
export function Ping(value: string) {
    return new HtmlProp("ping", value);
}

/**
 * Provides a hint to the user of what can be entered in the field.
 **/
export function PlaceHolder(value: string) {
    return new HtmlProp("placeholder", value);
}

export type PopoverValue =
    | "auto"
    | "manual";

/**
 * Used to designate an element as a popover element.
 **/
export function Popover(value: PopoverValue) {
    return new HtmlProp(
        "popover",
        value as string
    );
}

/**
 * Turns a <button> element into a popover control button; takes the ID of the popover element to control as its value.
 **/
export function PopoverTargetAttr(value: string) {
    return new HtmlAttr("popovertarget", value);
}

/**
 * Turns a <button> element into a popover control button; takes the ID of the popover element to control as its value.
 **/
export function PopoverTargetElement(value: Element) {
    return new HtmlProp("popoverTargetElement", value);
}

export type PopoverTargetActionValue =
    | "hide"
    | "show"
    | "toggle";

/**
 * Specifies the action to be performed on a popover element being controlled by a control <button>. Possible values are:
 **/
export function PopoverTargetAction(value: PopoverTargetActionValue) {
    return new HtmlProp("popoverTargetAction", value as string);
}

/**
 * A URL indicating a poster frame to show until the user plays or seeks.
 **/
export function Poster(value: string) {
    return new HtmlProp("poster", value);
}

/**
 * Indicates whether the whole resource, parts of it or nothing should be preloaded.
 **/
export function Preload(value: boolean | string) {
    return new HtmlProp("preload", value);
}

/**
 * Indicates whether the element can be edited.
 **/
export function ReadOnly(value: boolean) {
    return new HtmlProp("readOnly", value);
}

/**
 * The radiogroup attribute
 **/
export function RadioGroup(value: string) {
    return new HtmlProp("radiogroup", value);
}

/**
 * Specifies which referrer is sent when fetching the resource.
 **/
export function ReferrerPolicy(value: string) {
    return new HtmlProp("referrerpolicy", value);
}

/**
 * Specifies the relationship of the target object to the link object.
 **/
export function Rel(value: string) {
    return new HtmlProp("rel", value);
}

/**
 * Indicates whether this element is required to fill out or not.
 **/
export function Required(value: boolean) {
    return new HtmlProp("required", value);
}

/**
 * Indicates whether the list should be displayed in a descending order instead of a ascending.
 **/
export function Reversed(value: boolean) {
    return new HtmlProp("reversed", value);
}

/**
 * Defines the number of rows in a text area.
 **/
export function Role(value: string) {
    return new HtmlProp(
        "role",
        value
    );
}

/**
 * The rows attribute
 **/
export function Rows(value: number) {
    return new HtmlProp("rows", value);
}

/**
 * Defines the number of rows a table cell should span over.
 **/
export function RowSpan(value: number) {
    return new HtmlProp("rowSpan", value);
}

/**
 * Stops a document loaded in an iframe from using certain features (such as submitting forms or opening new windows).
 **/
export function Sandbox(value: string) {
    return new HtmlProp("sandbox", value);
}

/**
 * Defines the cells that the header test (defined in the th element) relates to.
 **/
export function Scope(value: string) {
    return new HtmlProp("scope", value);
}

/**
 * The scoped attribute for `style` tags.
 **/
export function Scoped(value: boolean) {
    return new HtmlProp("scoped", value);
}

/**
 * The scrolling attribute for `iframe` tags.
 **/
export function Scrolling(value: boolean) {
    return new HtmlProp("scrolling", value ? "yes" : "no");
}

/**
 * Defines an `option` tag which will be selected on page load.
 **/
export function Selected(value: boolean) {
    return new HtmlProp("selected", value);
}

/**
 * Sets the `selectedIndex` property on a Select element.
 */
export function SelectedIndex(value: number) {
    return new HtmlProp("selectedIndex", value);
}

/**
 * The shape attribute for `a` and `area` tags.
 **/
export function Shape(value: string) {
    return new HtmlProp("shape", value);
}

/**
 * Defines the width of the element (in pixels). If the element's type attribute is text or password then it's the number of characters.
 **/
export function Size(value: number) {
    return new HtmlProp("size", value);
}

/**
 * Assigns a slot in a shadow DOM shadow tree to an element.
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function SlotAttr(value: string) {
    return new HtmlProp(
        "slot",
        value
    );
}

/**
 * The sizes attribute
 **/
export function Sizes(value: string) {
    return new HtmlProp("sizes", value);
}

/**
 * The span attribute.
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function SpanAttr(value: string) {
    return new HtmlProp("span", value);
}

/**
 * Indicates whether spell checking is allowed for the element.
 **/
export function SpellCheck(value: boolean) {
    return new HtmlProp(
        "spellcheck",
        value
    );
}

/**
 * The URL of the embeddable content.
 **/
export function Src(value: string | URL) {
    return new HtmlProp("src", unpackURL(value));
}

/**
 * The srcdoc attribute
 **/
export function SrcDoc(value: string) {
    return new HtmlProp("srcdoc", value);
}

/**
 * The srclang attribute
 **/
export function SrcLang(value: string) {
    return new HtmlProp("srclang", value);
}

/**
 * A MediaStream object to use as a source for an HTML video or audio element
 **/
export function SrcObject(value: MediaProvider) {
    return new HtmlProp("srcObject", value);
}

/**
 * One or more responsive image candidates.
 **/
export function SrcSet(value: string) {
    return new HtmlProp("srcset", value);
}

/**
 * Defines the first number if other than 1.
 **/
export function Start(value: number) {
    return new HtmlProp("start", value);
}

/**
 * The step attribute
 **/
export function Step(value: number) {
    return new HtmlProp("step", value.toString());
}

export class HtmlStyleAttr extends AbstractKeyValueAppliable<HTMLElement, "style", Record<string, string>> {
    constructor(dict: Record<string, string>) {
        super("style", dict);
    }

    override apply(tag: HTMLElement): void {
        for (const [key, value] of Object.entries(this.value)) {
            tag.style.setProperty(key, value);
        }
    }
}

/**
 * Creates an assigner function for objects with a style attribute.
 * @param dict the value to assign. Use the full, kebob-case CSS field names as keys, e.g. "grid-template-columns".
 */
export function StyleAttr(dict: Record<string, string>) {
    return new HtmlStyleAttr(dict);
}

/**
 * The summary attribute.
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function SummaryAttr(value: string) {
    return new HtmlProp("summary", value);
}

/**
 * Overrides the browser's default tab order and follows the one specified instead.
 **/
export function TabIndex(value: number) {
    return new HtmlProp(
        "tabIndex",
        value
    );
}

export function TextContent(text: { toString(): string }) {
    return new HtmlProp(
        "textContent",
        text
    );
}

/**
 * Text to be displayed in a tooltip when hovering over the element. 
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function TitleAttr(value: string) {
    return new HtmlProp(
        "title",
        value
    );
}

/**
 * The target attribute
 **/
export function Target(value: string) {
    return new HtmlProp("target", value);
}

/**
 * Specify whether an element’s attribute values and the values of its Text node children are to be translated when the page is localized, or whether to leave them unchanged.
 **/
export function Translate(value: boolean) {
    return new HtmlProp(
        "translate",
        value
    );
}

/**
 * Defines the type of the element.
 **/
export function Type(value: string | { value: string }) {
    if (typeof value !== "string") {
        value = value.value;
    }

    return new HtmlProp("type", value);
}

/**
 * Defines the type of the element.
 **/
export function TypeAttr(value: string) {
    return new HtmlAttr("type", value);
}

/**
 * Defines a default value which will be displayed in the element on page load.
 */
export function Value(value: { toString(): string }) {
    return new HtmlProp("value", value);
}

/**
 * Defines a default value which will be displayed in the element on page load.
 **/
export function ValueAsNumber(value: number) {
    return new HtmlProp("valueAsNumber", value);
}

/**
 * Defines a default value which will be displayed in the element on page load.
 **/
export function ValueAsDate(value: Date) {
    return new HtmlProp("valueAsDate", value);
}

/**
 * setting the volume at which a media element plays.
 **/
export function Volume(value: number) {
    return new HtmlProp("volume", value);
}

/**
 * The usemap attribute
 **/
export function UseMap(value: boolean) {
    return new HtmlProp("usemap", value);
}

/**
 * For the elements listed here, this establishes the element's width.
 **/
export function Width(value: number | string) {
    return new HtmlProp("width", value);
}

type HTMLWrapValue = "hard" | "soft" | "off";

/**
 * Indicates whether the text should be wrapped.
 **/
export function Wrap(value: HTMLWrapValue) {
    return new HtmlProp("wrap", value);
}