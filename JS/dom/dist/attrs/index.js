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
export function Accept(value) {
    return new HtmlProp("accept", value);
}
/**
 * The accessKey attribute
 **/
export function AccessKey(value) {
    return new HtmlProp("accessKey", value);
}
/**
 * specifying the horizontal alignment of the element.
 **/
export function Align(value) {
    return new HtmlProp("align", value);
}
/**
 * Specifies a feature-policy for the iframe.
 **/
export function Allow(value) {
    return new HtmlProp("allow", value);
}
/**
 * Whether or not to allow an IFrame to open full screen elements.
 **/
export function AllowFullscreen(value) {
    return new HtmlProp("allowFullscreen", value);
}
/**
 * Alternative text in case an image can't be displayed.
 **/
export function Alt(value) {
    return new HtmlProp("alt", value);
}
export function Anchor(value) {
    return new HtmlAttr("anchor", value);
}
/**
 * Executes the script asynchronously.
 **/
export function Async(value) {
    return new HtmlProp("async", value);
}
/**
 * Sets whether input is automatically capitalized when entered by user
 **/
export function AutoCapitalize(value) {
    if (isBoolean(value)) {
        value = value ? "on" : "off";
    }
    return new HtmlProp("autocapitalize", value);
}
/**
 * Indicates whether controls in this form can by default have their values automatically completed by the browser.
 **/
export function AutoComplete(value) {
    if (isBoolean(value)) {
        value = value ? "on" : "off";
    }
    return new HtmlProp("autocomplete", value);
}
/**
 * The element should be automatically focused after the page loaded.
 **/
export function AutoFocus(value) {
    return new HtmlProp("autofocus", value);
}
/**
 * The audio or video should play as soon as possible.
 **/
export function AutoPlay(value) {
    return new HtmlProp("autoplay", value);
}
/**
 * Contains the time range of already buffered media.
 **/
export function Buffered(value) {
    return new HtmlProp("buffered", value);
}
/**
 * From the HTML Media Capture
 **/
export function Capture(value) {
    return new HtmlProp("capture", value);
}
/**
 * Declares the character encoding of the page or script.
 **/
export function CharSet(value) {
    return new HtmlProp("charset", value);
}
/**
 * Indicates whether the element should be checked on page load.
 **/
export function Checked(value) {
    return new HtmlProp("checked", value);
}
/**
 * Contains a URI which points to the source of the quote or change.
 **/
export function CiteURI(value) {
    return new HtmlProp("cite", value);
}
/**
 * Often used with CSS to style elements with common properties.
 **/
export function ClassName(value) {
    return new HtmlProp("className", value);
}
class HtmlClassListAttr extends HtmlProp {
    constructor(classes) {
        super("classList", classes.filter(identity));
    }
    apply(tag) {
        const classes = this.value;
        if (tag instanceof Element) {
            tag.classList.add(...classes.filter(c => c && !tag.classList.contains(c)));
        }
    }
}
/**
 * Assign a list of CSS classes to an element
 * @param classes the type to assign
 */
export function ClassList(...classes) {
    return new HtmlClassListAttr(classes);
}
/**
 * Specifies the URL of the applet's class file to be loaded and executed.
 **/
export function CodeURI(value) {
    return new HtmlProp("code", value);
}
/**
 * This attribute gives the absolute or relative URL of the directory where applets' .class files referenced by the code attribute are stored.
 **/
export function CodeBase(value) {
    return new HtmlProp("codebase", value);
}
/**
 * Defines the number of columns in a textarea.
 **/
export function Cols(value) {
    return new HtmlProp("cols", value);
}
/**
 * The colspan attribute defines the number of columns a cell should span.
 **/
export function ColSpan(value) {
    return new HtmlProp("colSpan", value);
}
/**
 * A value associated with http-equiv or name depending on the context.
 **/
export function Content(value) {
    return new HtmlProp("content", value);
}
/**
 * Indicates whether the element's content is editable.
 **/
export function ContentEditable(value) {
    return new HtmlProp("contenteditable", value);
}
/**
 * Defines the ID of a <menu> element which will serve as the element's context menu.
 **/
export function ContextMenu(value) {
    return new HtmlProp("contextmenu", value);
}
/**
 * Indicates whether the browser should show playback controls to the user.
 **/
export function Controls(value) {
    return new HtmlProp("controls", value);
}
/**
 * A set of values specifying the coordinates of the hot-spot region.
 **/
export function Coords(value) {
    return new HtmlProp("coords", value);
}
/**
 * How the element handles cross-origin requests
 * @param {} value
 **/
export function CrossOrigin(value) {
    return new HtmlProp("crossorigin", value);
}
/**
 * Specifies the Content Security Policy that an embedded document must agree to enforce upon itself.
 **/
export function CSP(value) {
    return new HtmlProp("csp", value);
}
/**
 * Lets you attach custom attributes to an HTML element.
 **/
export function CustomData(name, value) {
    name = "data-" + name.toLowerCase();
    return new HtmlAttr(name, value);
}
/**
 * Indicates the date and time associated with the element.
 **/
export function DateTime(value) {
    return new HtmlProp("dateTime", value?.toISOString());
}
/**
 * Indicates the preferred method to decode the image.
 **/
export function Decoding(value) {
    return new HtmlProp("decoding", value);
}
/**
 * Indicates that the track should be enabled unless the user's preferences indicate something different.
 **/
export function Default(value) {
    return new HtmlProp("default", value);
}
/**
 * Indicates that the script should be executed after the page has been parsed.
 **/
export function Defer(value) {
    return new HtmlProp("defer", value);
}
/**
 * Defines the text direction. Allowed values are ltr (Left-To-Right) or rtl (Right-To-Left)
 **/
export function Dir(value) {
    return new HtmlProp("dir", value);
}
/**
 * Indicates whether the user can interact with the element.
 **/
export function Disabled(value) {
    return new HtmlProp("disabled", value);
}
/**
 * The name to provide a POSTed FormData field when submitting an Input or TextArea's input direction.
 **/
export function DirName(value) {
    return new HtmlProp("dirname", value);
}
/**
 * Indicates that the hyperlink is to be used for downloading a resource by giving the file a name.
 * @param name the name of the file to download
 */
export function Download(name) {
    return new HtmlProp("download", name);
}
/**
 * Defines whether the element can be dragged.
 **/
export function Draggable(value) {
    return new HtmlProp("draggable", value);
}
/**
 * Indicates that the element accepts the dropping of content onto it.
 **/
export function DropZone(value) {
    return new HtmlProp("dropzone", value);
}
/**
 * Defines the content type of the form data when the method is POST.
 **/
export function EncType(value) {
    return new HtmlProp("enctype", value);
}
/**
 * The enterkeyhint specifies what action label (or icon) to present for the enter key on virtual keyboards. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
 **/
export function EnterKeyHint(value) {
    return new HtmlProp("enterkeyhint", value);
}
/**
 * Describes elements which belongs to this one.
 **/
export function For(value) {
    return new HtmlProp("htmlFor", value);
}
/**
 * Indicates the form that is the owner of the element.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function FormAttr(form) {
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
export function FormAction(value) {
    return new HtmlProp("formaction", value);
}
/**
 * If the button/input is a submit button (type="submit"
), this attribute sets the encoding type to use during form submission. If this attribute is specified, it overrides the enctype attribute of the button's form owner.
 **/
export function FormEncType(value) {
    return new HtmlProp("formenctype", value);
}
/**
 * If the button/input is a submit button (type="submit"
), this attribute sets the submission method to use during form submission (GET, POST, etc.). If this attribute is specified, it overrides the method attribute of the button's form owner.
 **/
export function FormMethod(value) {
    return new HtmlProp("formmethod", value);
}
/**
 * If the button/input is a submit button (type="submit"
), this boolean attribute specifies that the form is not to be validated when it is submitted. If this attribute is specified, it overrides the novalidate attribute of the button's form owner.
 **/
export function FormNoValidate(value) {
    return new HtmlProp("formnovalidate", value);
}
/**
 * If the button/input is a submit button (type="submit"
), this attribute specifies the browsing context (for example, tab, window, or inline frame) in which to display the response that is received after submitting the form. If this attribute is specified, it overrides the target attribute of the button's form owner.
 **/
export function FormTarget(value) {
    return new HtmlProp("formtarget", value);
}
/**
 * Width of the border to put around an `iframe` tag.
 * @param value
 **/
export function FrameBorder(value) {
    if (typeof value === "boolean") {
        value = value ? "yes" : "no";
    }
    return new HtmlProp("frameborder", value);
}
/**
 * IDs of the <th> elements which applies to this element.
 **/
export function Headers(value) {
    return new HtmlProp("headers", value);
}
/**
 * Specifies the height of elements listed here. For all other elements, use the CSS height property.
 * @param value
 **/
export function Height(value) {
    return new HtmlProp("height", value);
}
/**
 * Prevents rendering of given element, while keeping child elements, e.g. script elements, active.
 **/
export function Hidden(value) {
    return new HtmlProp("hidden", value);
}
/**
 * Indicates the lower bound of the upper range.
 **/
export function High(value) {
    return new HtmlProp("high", value);
}
/**
 * The URL of a linked resource.
 * @param href the hyper-reference to assign
 */
export function HRef(href) {
    return new HtmlProp("href", unpackURL(href));
}
/**
 * Specifies the language of the linked resource.
 **/
export function HRefLang(value) {
    return new HtmlProp("hreflang", value);
}
/**
 * Defines a pragma directive.
 **/
export function HttpEquiv(value) {
    return new HtmlProp("httpEquiv", value);
}
/**
 * Specifies a picture which represents the command.
 **/
export function Icon(value) {
    return new HtmlProp("icon", value);
}
export function IconAttr(value) {
    return new HtmlAttr("icon", value);
}
export class HtmlIDAttr extends HtmlProp {
    constructor(value, required) {
        super("id", value);
        this.required = required;
    }
}
/**
 * Often used with CSS to style a specific element. The value of this attribute must be unique.
 **/
export function ID(value, required = false) {
    return new HtmlIDAttr(value, required);
}
/**
 * Indicates the relative fetch priority for the resource.
 **/
export function Importance(value) {
    return new HtmlProp("importance", value);
}
/**
 * Sets the innerHTML property of an element.
 **/
export function InnerHTML(html) {
    return new HtmlProp("innerHTML", html);
}
/**
 * Provides a hint as to the type of data that might be entered by the user while editing the element or its contents. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
 **/
export function InputMode(value) {
    return new HtmlProp("inputmode", value);
}
/**
 * Specifies a sub-resource Integrity value that allows browsers to verify what they fetch.
 **/
export function Integrity(value) {
    return new HtmlProp("integrity", value);
}
/**
 * This attribute tells the browser to ignore the actual intrinsic size of the image and pretend it’s the size specified in the attribute.
 **/
export function IntrinsicSize(value) {
    return new HtmlProp("intrinsicsize", value);
}
export class HtmlIsAttr extends HtmlAttr {
    constructor(value) {
        super("is", value);
    }
}
/**
 * This attribute indicates that a base HTML element should be rendered as a given custom element name.
 */
export function Is(value) {
    return new HtmlIsAttr(value);
}
/**
 * Indicates that the image is part of a server-side image map.
 **/
export function IsMap(value) {
    return new HtmlProp("ismap", value);
}
/**
 * The itemprop attribute
 **/
export function ItemProp(value) {
    return new HtmlProp("itemprop", value);
}
/**
 * Specifies the type of key generated.
 **/
export function KeyType(value) {
    return new HtmlProp("keytype", value);
}
/**
 * Specifies the kind of text track.
 **/
export function Kind(value) {
    return new HtmlProp("kind", value);
}
/**
 * Specifies a user-readable title of the element.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function LabelAttr(value) {
    return new HtmlProp("label", value);
}
/**
 * Defines the language used in the element.
 **/
export function Lang(value) {
    return new HtmlProp("lang", value);
}
/**
 * Defines the script language used in the element.
 **/
export function Language(value) {
    return new HtmlProp("language", value);
}
/**
 * Identifies a list of pre-defined options to suggest to the user.
 **/
export function ListAttr(value) {
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
export function Loop(value) {
    return new HtmlProp("loop", value);
}
/**
 * Indicates the upper bound of the lower range.
 **/
export function Low(value) {
    return new HtmlProp("low", value);
}
/**
 * Indicates the maximum value allowed.
 **/
export function Max(value) {
    return new HtmlProp("max", value.toString());
}
/**
 * Defines the maximum number of characters allowed in the element.
 **/
export function MaxLength(value) {
    return new HtmlProp("maxlength", value);
}
/**
 * Defines the minimum number of characters allowed in the element.
 **/
export function MinLength(value) {
    return new HtmlProp("minlength", value);
}
/**
 * Specifies a hint of the media for which the linked resource was designed.
 **/
export function Media(value) {
    return new HtmlProp("media", value);
}
/**
 * Defines which HTTP method to use when submitting the form. Can be GET (default) or POST.
 **/
export function Method(value) {
    return new HtmlProp("method", value);
}
/**
 * Indicates the minimum value allowed.
 **/
export function Min(value) {
    return new HtmlProp("min", value.toString());
}
/**
 * Indicates whether multiple values can be entered in an input of the type email or file.
 **/
export function Multiple(value) {
    return new HtmlProp("multiple", value);
}
/**
 * Indicates whether the audio will be initially silenced on page load.
 **/
export function Muted(value) {
    return new HtmlProp("muted", value);
}
/**
 * Name of the element. For example used by the server to identify the fields in form submits.
 **/
export function Name(value) {
    return new HtmlProp("name", value);
}
/**
 * This attribute indicates that the form shouldn't be validated when submitted.
 **/
export function NoValidate(value) {
    return new HtmlProp("novalidate", value);
}
/**
 * Indicates whether the details will be shown on page load.
 **/
export function Open(value) {
    return new HtmlProp("open", value);
}
/**
 * Indicates the optimal numeric value.
 **/
export function Optimum(value) {
    return new HtmlProp("optimum", value);
}
/**
 * Defines a regular expression which the element's value will be validated against.
 **/
export function Pattern(value) {
    return new HtmlProp("pattern", value);
}
/**
 * The ping attribute specifies a space-separated list of URLs to be notified if a user follows the hyperlink.
 **/
export function Ping(value) {
    return new HtmlProp("ping", value);
}
/**
 * Provides a hint to the user of what can be entered in the field.
 **/
export function PlaceHolder(value) {
    return new HtmlProp("placeholder", value);
}
/**
 * Used to designate an element as a popover element.
 **/
export function Popover(value) {
    return new HtmlProp("popover", value);
}
/**
 * Turns a <button> element into a popover control button; takes the ID of the popover element to control as its value.
 **/
export function PopoverTargetAttr(value) {
    return new HtmlAttr("popovertarget", value);
}
/**
 * Turns a <button> element into a popover control button; takes the ID of the popover element to control as its value.
 **/
export function PopoverTargetElement(value) {
    return new HtmlProp("popoverTargetElement", value);
}
/**
 * Specifies the action to be performed on a popover element being controlled by a control <button>. Possible values are:
 **/
export function PopoverTargetAction(value) {
    return new HtmlProp("popoverTargetAction", value);
}
/**
 * A URL indicating a poster frame to show until the user plays or seeks.
 **/
export function Poster(value) {
    return new HtmlProp("poster", value);
}
/**
 * Indicates whether the whole resource, parts of it or nothing should be preloaded.
 **/
export function Preload(value) {
    return new HtmlProp("preload", value);
}
/**
 * Indicates whether the element can be edited.
 **/
export function ReadOnly(value) {
    return new HtmlProp("readOnly", value);
}
/**
 * The radiogroup attribute
 **/
export function RadioGroup(value) {
    return new HtmlProp("radiogroup", value);
}
/**
 * Specifies which referrer is sent when fetching the resource.
 **/
export function ReferrerPolicy(value) {
    return new HtmlProp("referrerpolicy", value);
}
/**
 * Specifies the relationship of the target object to the link object.
 **/
export function Rel(value) {
    return new HtmlProp("rel", value);
}
/**
 * Indicates whether this element is required to fill out or not.
 **/
export function Required(value) {
    return new HtmlProp("required", value);
}
/**
 * Indicates whether the list should be displayed in a descending order instead of a ascending.
 **/
export function Reversed(value) {
    return new HtmlProp("reversed", value);
}
/**
 * Defines the number of rows in a text area.
 **/
export function Role(value) {
    return new HtmlProp("role", value);
}
/**
 * The rows attribute
 **/
export function Rows(value) {
    return new HtmlProp("rows", value);
}
/**
 * Defines the number of rows a table cell should span over.
 **/
export function RowSpan(value) {
    return new HtmlProp("rowSpan", value);
}
/**
 * Stops a document loaded in an iframe from using certain features (such as submitting forms or opening new windows).
 **/
export function Sandbox(value) {
    return new HtmlProp("sandbox", value);
}
/**
 * Defines the cells that the header test (defined in the th element) relates to.
 **/
export function Scope(value) {
    return new HtmlProp("scope", value);
}
/**
 * The scoped attribute for `style` tags.
 **/
export function Scoped(value) {
    return new HtmlProp("scoped", value);
}
/**
 * The scrolling attribute for `iframe` tags.
 **/
export function Scrolling(value) {
    return new HtmlProp("scrolling", value ? "yes" : "no");
}
/**
 * Defines an `option` tag which will be selected on page load.
 **/
export function Selected(value) {
    return new HtmlProp("selected", value);
}
/**
 * Sets the `selectedIndex` property on a Select element.
 */
export function SelectedIndex(value) {
    return new HtmlProp("selectedIndex", value);
}
/**
 * The shape attribute for `a` and `area` tags.
 **/
export function Shape(value) {
    return new HtmlProp("shape", value);
}
/**
 * Defines the width of the element (in pixels). If the element's type attribute is text or password then it's the number of characters.
 **/
export function Size(value) {
    return new HtmlProp("size", value);
}
/**
 * Assigns a slot in a shadow DOM shadow tree to an element.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function SlotAttr(value) {
    return new HtmlProp("slot", value);
}
/**
 * The sizes attribute
 **/
export function Sizes(value) {
    return new HtmlProp("sizes", value);
}
/**
 * The span attribute.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function SpanAttr(value) {
    return new HtmlProp("span", value);
}
/**
 * Indicates whether spell checking is allowed for the element.
 **/
export function SpellCheck(value) {
    return new HtmlProp("spellcheck", value);
}
/**
 * The URL of the embeddable content.
 **/
export function Src(value) {
    return new HtmlProp("src", unpackURL(value));
}
/**
 * The srcdoc attribute
 **/
export function SrcDoc(value) {
    return new HtmlProp("srcdoc", value);
}
/**
 * The srclang attribute
 **/
export function SrcLang(value) {
    return new HtmlProp("srclang", value);
}
/**
 * A MediaStream object to use as a source for an HTML video or audio element
 **/
export function SrcObject(value) {
    return new HtmlProp("srcObject", value);
}
/**
 * One or more responsive image candidates.
 **/
export function SrcSet(value) {
    return new HtmlProp("srcset", value);
}
/**
 * Defines the first number if other than 1.
 **/
export function Start(value) {
    return new HtmlProp("start", value);
}
/**
 * The step attribute
 **/
export function Step(value) {
    return new HtmlProp("step", value.toString());
}
export class HtmlStyleAttr extends AbstractKeyValueAppliable {
    constructor(dict) {
        super("style", dict);
    }
    apply(tag) {
        for (const [key, value] of Object.entries(this.value)) {
            tag.style.setProperty(key, value);
        }
    }
}
/**
 * Creates an assigner function for objects with a style attribute.
 * @param dict the value to assign. Use the full, kebob-case CSS field names as keys, e.g. "grid-template-columns".
 */
export function StyleAttr(dict) {
    return new HtmlStyleAttr(dict);
}
/**
 * The summary attribute.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function SummaryAttr(value) {
    return new HtmlProp("summary", value);
}
/**
 * Overrides the browser's default tab order and follows the one specified instead.
 **/
export function TabIndex(value) {
    return new HtmlProp("tabIndex", value);
}
export function TextContent(text) {
    return new HtmlProp("textContent", text);
}
/**
 * Text to be displayed in a tooltip when hovering over the element.
 *
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function TitleAttr(value) {
    return new HtmlProp("title", value);
}
/**
 * The target attribute
 **/
export function Target(value) {
    return new HtmlProp("target", value);
}
/**
 * Specify whether an element’s attribute values and the values of its Text node children are to be translated when the page is localized, or whether to leave them unchanged.
 **/
export function Translate(value) {
    return new HtmlProp("translate", value);
}
/**
 * Defines the type of the element.
 **/
export function Type(value) {
    if (typeof value !== "string") {
        value = value.value;
    }
    return new HtmlProp("type", value);
}
/**
 * Defines the type of the element.
 **/
export function TypeAttr(value) {
    return new HtmlAttr("type", value);
}
/**
 * Defines a default value which will be displayed in the element on page load.
 */
export function Value(value) {
    return new HtmlProp("value", value);
}
/**
 * Defines a default value which will be displayed in the element on page load.
 **/
export function ValueAsNumber(value) {
    return new HtmlProp("valueAsNumber", value);
}
/**
 * Defines a default value which will be displayed in the element on page load.
 **/
export function ValueAsDate(value) {
    return new HtmlProp("valueAsDate", value);
}
/**
 * setting the volume at which a media element plays.
 **/
export function Volume(value) {
    return new HtmlProp("volume", value);
}
/**
 * The usemap attribute
 **/
export function UseMap(value) {
    return new HtmlProp("usemap", value);
}
/**
 * For the elements listed here, this establishes the element's width.
 **/
export function Width(value) {
    return new HtmlProp("width", value);
}
/**
 * Indicates whether the text should be wrapped.
 **/
export function Wrap(value) {
    return new HtmlProp("wrap", value);
}
//# sourceMappingURL=index.js.map