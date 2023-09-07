import { identity } from "@juniper-lib/tslib/identity";
import { stringRandom } from "@juniper-lib/tslib/strings/stringRandom";
import { isBoolean, isFunction, isNullOrUndefined, isString } from "@juniper-lib/tslib/typeChecks";
const warnings = new Map();
/**
 * A setter functor for HTML attributes.
 **/
export class HtmlAttr {
    /**
     * Creates a new setter functor for HTML Attributes
     * @param key - the attribute name.
     * @param value - the value to set for the attribute.
     * @param bySetAttribute - whether the attribute should be set via the setAttribute method.
     * @param tags - the HTML tags that support this attribute.
     */
    constructor(key, value, bySetAttribute, ...tags) {
        this.key = key;
        this.value = value;
        this.bySetAttribute = bySetAttribute;
        this.tags = tags.map((t) => t.toLocaleUpperCase());
        Object.freeze(this);
    }
    /**
     * Set the attribute value on an HTMLElement
     * @param elem - the element on which to set the attribute.
     */
    applyToElement(elem) {
        if (this.tags.length > 0
            && this.tags.indexOf(elem.tagName) === -1) {
            let set = warnings.get(elem.tagName);
            if (!set) {
                warnings.set(elem.tagName, set = new Set());
            }
            if (!set.has(this.key)) {
                set.add(this.key);
                console.warn(`Element ${elem.tagName} does not support Attribute ${this.key}`);
            }
        }
        if (this.bySetAttribute) {
            elem.setAttribute(this.key, this.value.toString());
        }
        else if (this.key in elem) {
            elem[this.key] = this.value;
        }
        else if (this.value === false) {
            elem.removeAttribute(this.key);
        }
        else if (this.value === true) {
            elem.setAttribute(this.key, "");
        }
        else if (isFunction(this.value)) {
            this.value(elem);
        }
        else {
            elem.setAttribute(this.key, this.value.toString());
        }
    }
}
function attr(key, value, bySetAttribute, ...tags) {
    return new HtmlAttr(key, value, bySetAttribute, ...tags);
}
export function isAttr(obj) {
    return obj instanceof HtmlAttr;
}
/**
 * a list of types the server accepts, typically a file type.
 * @param value - the value to set on the attribute.
 **/
export function Accept(value) { return attr("accept", value, false, "form", "input"); }
/**
 * The accessKey attribute
 **/
export function AccessKey(value) { return attr("accessKey", value, false, "input", "button"); }
/**
 * specifying the horizontal alignment of the element.
 **/
export function Align(value) { return attr("align", value, false, "applet", "caption", "col", "colgroup", "hr", "iframe", "img", "table", "tbody", "td", "tfoot", "th", "thead", "tr"); }
/**
 * Specifies a feature-policy for the iframe.
 **/
export function Allow(value) { return attr("allow", value, false, "iframe"); }
/**
 * Whether or not to allow an IFrame to open full screen elements.
 */
export function AllowFullscreen(value) { return attr("allowfullscreen", value, false, "iframe"); }
/**
 * Alternative text in case an image can't be displayed.
 **/
export function Alt(value) { return attr("alt", value, false, "applet", "area", "img", "input"); }
/**
 * Identifies the currently active element when DOM focus is on a composite widget, textbox, group, or application.
 **/
export function AriaActiveDescendant(value) { return attr("ariaActiveDescendant", value, false); }
/**
 * Indicates whether assistive technologies will present all, or only parts of, the changed region based on the change notifications defined by the aria-relevant attribute.
 **/
export function AriaAtomic(value) { return attr("ariaAtomic", value, false); }
/**
 * Indicates whether inputting text could trigger display of one or more predictions of the user's intended value for an input and specifies how predictions would be presented if they are made.
 **/
export function AriaAutoComplete(value) { return attr("ariaAutoComplete", value, false); }
/**
 * Indicates an element is being modified and that assistive technologies MAY want to wait until the modifications are complete before exposing them to the user.
 **/
export function AriaBusy(value) { return attr("ariaBusy", value, false); }
/**
 * Indicates the current "checked" state of checkboxes, radio buttons, and other widgets. See related aria-pressed and aria-selected.
 **/
export function AriaChecked(value) { return attr("ariaChecked", value, false); }
/**
 * Defines the total number of columns in a table, grid, or treegrid. See related aria-colindex.
  **/
export function AriaColCount(value) { return attr("ariaColCount", value, false); }
/**
 * Defines an element's column index or position with respect to the total number of columns within a table, grid, or treegrid. See related aria-colcount and aria-colspan.
  **/
export function AriaColIndex(value) { return attr("ariaColIndex", value, false); }
/**
 * Defines the number of columns spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-colindex and aria-rowspan.
  **/
export function AriaColSpan(value) { return attr("ariaColSpan", value, false); }
/**
 * Identifies the element (or elements) whose contents or presence are controlled by the current element. See related aria-owns.
  **/
export function AriaControls(value) { return attr("ariaControls", value, false); }
/**
 * Indicates the element that represents the current item within a container or set of related elements.
  **/
export function AriaCurrent(value) { return attr("ariaCurrent", value, false); }
/**
 * Identifies the element (or elements) that describes the object. See related aria-labelledby.
  **/
export function AriaDescribedBy(value) { return attr("ariaDescribedBy", value, false); }
/**
 * Identifies the element that provides a detailed, extended description for the object. See related aria-describedby.
  **/
export function AriaDetails(value) { return attr("ariaDetails", value, false); }
/**
 * Indicates that the element is perceivable but disabled, so it is not editable or otherwise operable. See related aria-hidden and aria-readonly.
  **/
export function AriaDisabled(value) { return attr("ariaDisabled", value, false); }
/**
 * Identifies the element that provides an error message for the object. See related aria-invalid and aria-describedby.
  **/
export function AriaErrorMessage(value) { return attr("ariaErrorMessage", value, false); }
/**
 * Indicates whether the element, or another grouping element it controls, is currently expanded or collapsed.
 **/
export function AriaExpanded(value) { return attr("ariaExpanded", value, false); }
/**
 * Identifies the next element (or elements) in an alternate reading order of content which, at the user's discretion, allows assistive technology to override the general default of reading in document source order.
  **/
export function AriaFlowTo(value) { return attr("ariaFlowTo", value, false); }
/**
 * Indicates the availability and type of interactive popup element, such as menu or dialog, that can be triggered by an element.
  **/
export function AriaHasPopup(value) { return attr("ariaHasPopup", value, false); }
/**
 * Indicates whether the element is exposed to an accessibility API. See related aria-disabled.
 **/
export function AriaHidden(value) { return attr("ariaHidden", value, false); }
/**
 * Indicates the entered value does not conform to the format expected by the application. See related aria-errormessage.
  **/
export function AriaInvalid(value) { return attr("ariaInvalid", value, false); }
/**
 * Indicates keyboard shortcuts that an author has implemented to activate or give focus to an element.
  **/
export function AriaKeyShortcuts(value) { return attr("ariaKeyShortcuts", value, false); }
/**
 * Defines a string value that labels the current element. See related aria-labelledby.
  **/
export function AriaLabel(value) { return attr("ariaLabel", value, false); }
/**
 * Identifies the element (or elements) that labels the current element. See related aria-describedby.
  **/
export function AriaLabelledBy(value) { return attr("ariaLabelledBy", value, false); }
/**
 * Defines the hierarchical level of an element within a structure.
  **/
export function AriaLevel(value) { return attr("ariaLevel", value, false); }
/**
 * Indicates that an element will be updated, and describes the types of updates the user agents, assistive technologies, and user can expect from the live region.
  **/
export function AriaLive(value) { return attr("ariaLive", value, false); }
/**
 * Indicates whether an element is modal when displayed
  **/
export function AriaModal(value) { return attr("ariaModal", value, false); }
/**
 * Indicates whether a text box accepts multiple lines of input or only a single line.
  **/
export function AriaMultiline(value) { return attr("ariaMultiline", value, false); }
/**
 * Indicates that the user may select more than one item from the current selectable descendants.
  **/
export function AriaMultiSelectable(value) { return attr("ariaMultiSelectable", value, false); }
/**
 * Indicates that the user may select more than one item from the current selectable descendants.
  **/
export function AriaOrientation(value) { return attr("ariaOrientation", value, false); }
/**
 * Identifies an element (or elements) in order to define a visual, functional, or contextual parent/child relationship between DOM elements where the DOM hierarchy cannot be used to represent the relationship. See related aria-controls.
  **/
export function AriaOwns(value) { return attr("ariaOwns", value, false); }
/**
 * Defines a short hint (a word or short phrase) intended to aid the user with data entry when the control has no value. A hint could be a sample value or a brief description of the expected format.
  **/
export function AriaPlaceholder(value) { return attr("ariaPlaceholder", value, false); }
/**
 * Defines an element's number or position in the current set of listitems or treeitems. Not required if all elements in the set are present in the DOM. See related aria-setsize.
  **/
export function AriaPosInSet(value) { return attr("ariaPosInSet", value, false); }
/**
 * Indicates the current "pressed" state of toggle buttons. See related aria-checked and aria-selected.
 **/
export function AriaPressed(value) { return attr("ariaPressed", value, false); }
/**
 * Indicates that the element is not editable, but is otherwise operable. See related aria-disabled.
  **/
export function AriaReadOnly(value) { return attr("ariaReadOnly", value, false); }
/**
 * Indicates what notifications the user agent will trigger when the accessibility tree within a live region is modified. See related aria-atomic.
  **/
export function AriaRelevant(value) { return attr("ariaRelevant", value, false); }
/**
 * Indicates that user input is required on the element before a form may be submitted.
  **/
export function AriaRequired(value) { return attr("ariaRequired", value, false); }
/**
 * Defines a human-readable, author-localized description for the role of an element.
  **/
export function AriaRoleDescription(value) { return attr("ariaRoleDescription", value, false); }
/**
 * Defines the total number of rows in a table, grid, or treegrid. See related aria-rowindex.
  **/
export function AriaRowCount(value) { return attr("ariaRowCount", value, false); }
/**
 * Defines an element's row index or position with respect to the total number of rows within a table, grid, or treegrid. See related aria-rowcount and aria-rowspan.
  **/
export function AriaRowIndex(value) { return attr("ariaRowIndex", value, false); }
/**
 Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-rowindex and aria-colspan.
  **/
export function AriaRowSpan(value) { return attr("ariaRowSpan", value, false); }
/**
 * Indicates the current "selected" state of various widgets. See related aria-checked and aria-pressed.
 **/
export function AriaSelected(value) { return attr("ariaSelected", value, false); }
/**
 * Defines the number of items in the current set of listitems or treeitems. Not required if all elements in the set are present in the DOM. See related aria-posinset.
  **/
export function AriaSetSize(value) { return attr("ariaSetsize", value, false); }
/**
 * Indicates if items in a table or grid are sorted in ascending or descending order.
  **/
export function AriaSort(value) { return attr("ariaSort", value, false); }
/**
 * Defines the maximum allowed value for a range widget.
  **/
export function AriaValueMax(value) { return attr("ariaValueMax", value, false); }
/**
 * Defines the minimum allowed value for a range widget.
  **/
export function AriaValueMin(value) { return attr("ariaValueMin", value, false); }
/**
 * Defines the current value for a range widget. See related aria-valuetext.
  **/
export function AriaValueNow(value) { return attr("ariaValueNow", value, false); }
/**
 * Defines the human readable text alternative of aria-valuenow for a range widget.
  **/
export function AriaValueText(value) { return attr("ariaValueText", value, false); }
/**
 * Executes the script asynchronously.
  **/
export function Async(value) { return attr("async", value, false, "script"); }
/**
 * Sets whether input is automatically capitalized when entered by user
  **/
export function AutoCapitalize(value) { return attr("autocapitalize", value, false); }
/**
 * Indicates whether controls in this form can by default have their values automatically completed by the browser.
  **/
export function AutoComplete(value) { return attr("autocomplete", value, false, "form", "input", "select", "textarea"); }
/**
 * The element should be automatically focused after the page loaded.
  **/
export function AutoFocus(value) { return attr("autofocus", value, false, "button", "input", "keygen", "select", "textarea"); }
/**
 * The audio or video should play as soon as possible.
  **/
export function AutoPlay(value) { return attr("autoplay", value, false, "audio", "video"); }
/**
 * Contains the time range of already buffered media.
  **/
export function Buffered(value) { return attr("buffered", value, false, "audio", "video"); }
/**
 * From the HTML Media Capture
  **/
export function Capture(value) { return attr("capture", value, false, "input"); }
/**
 * Declares the character encoding of the page or script.
  **/
export function CharSet(value) { return attr("charset", value, false, "meta", "script"); }
/**
 * Indicates whether the element should be checked on page load.
  **/
export function Checked(value) { return attr("checked", value, false, "command", "input"); }
/**
 * Contains a URI which points to the source of the quote or change.
  **/
export function Cite_attr(value) { return attr("cite", value, false, "blockquote", "del", "ins", "q"); }
/**
 * Often used with CSS to style elements with common properties.
  **/
export function ClassName(value) { return attr("className", value, false); }
/**
 * Often used with CSS to style elements with common properties.
  **/
export function ClassList(...values) {
    values = values.filter(identity);
    return attr("CLASS_LIST", (element) => element.classList.add(...values), false);
}
/**
 * Specifies the URL of the applet's class file to be loaded and executed.
  **/
export function Code_attr(value) { return attr("code", value, false, "applet"); }
/**
 * This attribute gives the absolute or relative URL of the directory where applets' .class files referenced by the code attribute are stored.
  **/
export function CodeBase(value) { return attr("codebase", value, false, "applet"); }
/**
 * Defines the number of columns in a textarea.
  **/
export function Cols(value) { return attr("cols", value, false, "textarea"); }
/**
 * The colspan attribute defines the number of columns a cell should span.
  **/
export function ColSpan(value) { return attr("colspan", value, false, "td", "th"); }
/**
 * A value associated with http-equiv or name depending on the context.
  **/
export function Content(value) { return attr("content", value, false, "meta"); }
/**
 * Indicates whether the element's content is editable.
  **/
export function ContentEditable(value) { return attr("contenteditable", value, false); }
/**
 * Defines the ID of a <menu> element which will serve as the element's context menu.
  **/
export function ContextMenu(value) { return attr("contextmenu", value, false); }
/**
 * Indicates whether the browser should show playback controls to the user.
  **/
export function Controls(value) { return attr("controls", value, false, "audio", "video"); }
/**
 * A set of values specifying the coordinates of the hot-spot region.
  **/
export function Coords(value) { return attr("coords", value, false, "area"); }
/**
 * How the element handles cross-origin requests
  **/
export function CrossOrigin(value) { return attr("crossorigin", value, false, "audio", "img", "link", "script", "video"); }
/**
 * Specifies the Content Security Policy that an embedded document must agree to enforce upon itself.
  **/
export function CSP(value) { return attr("csp", value, false, "iframe"); }
/**
 * Specifies the URL of the resource.
  **/
export function Data_attr(value) { return attr("data", value, false, "object"); }
/**
 * Lets you attach custom attributes to an HTML element.
 */
export function CustomData(name, value) { return attr("data-" + name.toLowerCase(), value, true); }
/**
 * Indicates the date and time associated with the element.
  **/
export function DateTime(value) { return attr("datetime", value, false, "del", "ins", "time"); }
/**
 * Indicates the preferred method to decode the image.
  **/
export function Decoding(value) { return attr("decoding", value, false, "img"); }
/**
 * Indicates that the track should be enabled unless the user's preferences indicate something different.
  **/
export function Default(value) { return attr("default", value, false, "track"); }
/**
 * Indicates that the script should be executed after the page has been parsed.
  **/
export function Defer(value) { return attr("defer", value, false, "script"); }
/**
 * Defines the text direction. Allowed values are ltr (Left-To-Right) or rtl (Right-To-Left)
  **/
export function Dir(value) { return attr("dir", value, false); }
/**
 * Indicates whether the user can interact with the element.
  **/
export function Disabled(value) { return attr("disabled", value, false, "button", "command", "fieldset", "input", "keygen", "optgroup", "option", "select", "textarea"); }
/**
 * ???
  **/
export function DirName(value) { return attr("dirname", value, false, "input", "textarea"); }
/**
 * Indicates that the hyperlink is to be used for downloading a resource by giving the file a name.
  **/
export function Download(value) { return attr("download", value, false, "a", "area"); }
/**
 * Defines whether the element can be dragged.
  **/
export function Draggable(value) { return attr("draggable", value, false); }
/**
 * Indicates that the element accepts the dropping of content onto it.
  **/
export function DropZone(value) { return attr("dropzone", value, false); }
/**
 * Defines the content type of the form data when the method is POST.
  **/
export function EncType(value) { return attr("enctype", value, false, "form"); }
/**
 * The enterkeyhint specifies what action label (or icon) to present for the enter key on virtual keyboards. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
  **/
export function EnterKeyHint(value) { return attr("enterkeyhint", value, false, "textarea"); }
/**
 * Describes elements which belongs to this one.
  **/
export function HtmlFor(value) { return attr("htmlFor", value, false, "label", "output"); }
/**
 * Indicates the form that is the owner of the element.
  **/
export function Form_attr(value) { return attr("form", value, false, "button", "fieldset", "input", "keygen", "label", "meter", "object", "output", "progress", "select", "textarea"); }
/**
 * Indicates the action of the element, overriding the action defined in the <form>.
  **/
export function FormAction(value) { return attr("formaction", value, false, "input", "button"); }
/**
 * If the button/input is a submit button (type="submit"), this attribute sets the encoding type to use during form submission. If this attribute is specified, it overrides the enctype attribute of the button's form owner.
  **/
export function FormEncType(value) { return attr("formenctype", value, false, "button", "input"); }
/**
 * If the button/input is a submit button (type="submit"), this attribute sets the submission method to use during form submission (GET, POST, etc.). If this attribute is specified, it overrides the method attribute of the button's form owner.
  **/
export function FormMethod(value) { return attr("formmethod", value, false, "button", "input"); }
/**
 * If the button/input is a submit button (type="submit"), this boolean attribute specifies that the form is not to be validated when it is submitted. If this attribute is specified, it overrides the novalidate attribute of the button's form owner.
  **/
export function FormNoValidate(value) { return attr("formnovalidate", value, false, "button", "input"); }
/**
 * If the button/input is a submit button (type="submit"), this attribute specifies the browsing context (for example, tab, window, or inline frame) in which to display the response that is received after submitting the form. If this attribute is specified, it overrides the target attribute of the button's form owner.
  **/
export function FormTarget(value) { return attr("formtarget", value, false, "button", "input"); }
/**
 * Width of the border to put around an `iframe` tag.
 */
export function FrameBorder(value) {
    if (isBoolean(value)) {
        value = value ? "yes" : "no";
    }
    return attr("frameborder", value, false, "iframe");
}
/**
 * IDs of the <th> elements which applies to this element.
  **/
export function Headers(value) { return attr("headers", value, false, "td", "th"); }
/**
 * Specifies the height of elements listed here. For all other elements, use the CSS height property.
  **/
export function Height(value) { return attr("height", value, false, "canvas", "embed", "iframe", "img", "input", "object", "video"); }
/**
 * Prevents rendering of given element, while keeping child elements, e.g. script elements, active.
  **/
export function Hidden(value) { return attr("hidden", value, false); }
/**
 * Indicates the lower bound of the upper range.
  **/
export function High(value) { return attr("high", value, false, "meter"); }
/**
 * The URL of a linked resource.
  **/
export function Href(value) {
    if (value instanceof Blob) {
        value = URL.createObjectURL(value);
    }
    return attr("href", unpackURL(value), false, "a", "area", "base", "link");
}
/**
 * Specifies the language of the linked resource.
  **/
export function HrefLang(value) { return attr("hreflang", value, false, "a", "area", "link"); }
/**
 * Defines a pragma directive.
  **/
export function HttpEquiv(value) { return attr("httpEquiv", value, false, "meta"); }
/**
 * Specifies a picture which represents the command.
  **/
export function Icon(value) { return attr("icon", value, false, "command"); }
/**
 * Often used with CSS to style a specific element. The value of this attribute must be unique.
  **/
export function ID(value) { return attr("id", value, false); }
/**
 * Indicates the relative fetch priority for the resource.
  **/
export function Importance(value) { return attr("importance", value, false, "iframe", "img", "link", "script"); }
/**
 * Provides a hint as to the type of data that might be entered by the user while editing the element or its contents. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
  **/
export function InputMode(value) { return attr("inputmode", value, false, "textarea"); }
/**
 * Specifies a Subresource Integrity value that allows browsers to verify what they fetch.
  **/
export function Integrity(value) { return attr("integrity", value, false, "link", "script"); }
/**
 * This attribute tells the browser to ignore the actual intrinsic size of the image and pretend it’s the size specified in the attribute.
  **/
export function IntrinsicSize(value) { return attr("intrinsicsize", value, false, "img"); }
export function Is(value) { return attr("is", value, false); }
/**
 * Indicates that the image is part of a server-side image map.
  **/
export function IsMap(value) { return attr("ismap", value, false, "img"); }
/**
 * The itemprop attribute
  **/
export function ItemProp(value) { return attr("itemprop", value, false); }
/**
 * Specifies the type of key generated.
  **/
export function KeyType(value) { return attr("keytype", value, false, "keygen"); }
/**
 * Specifies the kind of text track.
  **/
export function Kind(value) { return attr("kind", value, false, "track"); }
/**
 * Specifies a user-readable title of the element.
  **/
export function Label_attr(value) { return attr("label", value, false, "optgroup", "option", "track"); }
/**
 * Defines the language used in the element.
  **/
export function Lang(value) { return attr("lang", value, false); }
/**
 * Defines the script language used in the element.
  **/
export function Language(value) { return attr("language", value, false, "script"); }
/**
 * Identifies a list of pre-defined options to suggest to the user.
  **/
export function List(value) {
    if (value instanceof HTMLDataListElement) {
        if (isNullOrUndefined(value.id)) {
            value.id = stringRandom(12);
        }
        if (!value.isConnected) {
            document.body.append(value);
        }
        value = value.id;
    }
    return attr("list", value, true, "input");
}
/**
 * Indicates whether the media should start playing from the start when it's finished.
  **/
export function Loop(value) { return attr("loop", value, false, "audio", "bgsound", "marquee", "video"); }
/**
 * Indicates the upper bound of the lower range.
  **/
export function Low(value) { return attr("low", value, false, "meter"); }
/**
 * Indicates the maximum value allowed.
  **/
export function Max(value) { return attr("max", value, false, "input", "meter", "progress"); }
/**
 * Defines the maximum number of characters allowed in the element.
  **/
export function MaxLength(value) { return attr("maxlength", value, false, "input", "textarea"); }
/**
 * Defines the minimum number of characters allowed in the element.
  **/
export function MinLength(value) { return attr("minlength", value, false, "input", "textarea"); }
/**
 * Specifies a hint of the media for which the linked resource was designed.
  **/
export function Media(value) { return attr("media", value, false, "a", "area", "link", "source", "style"); }
/**
 * Defines which HTTP method to use when submitting the form. Can be GET (default) or POST.
  **/
export function Method(value) { return attr("method", value, false, "form"); }
/**
 * Indicates the minimum value allowed.
  **/
export function Min(value) { return attr("min", value, false, "input", "meter"); }
/**
 * Indicates whether multiple values can be entered in an input of the type email or file.
  **/
export function Multiple(value) { return attr("multiple", value, false, "input", "select"); }
/**
 * Indicates whether the audio will be initially silenced on page load.
  **/
export function Muted(value) { return attr("muted", value, false, "audio", "video"); }
/**
 * Name of the element. For example used by the server to identify the fields in form submits.
  **/
export function Name(value) { return attr("name", value, false, "button", "form", "fieldset", "iframe", "input", "keygen", "map", "meta", "object", "output", "param", "select", "slot", "textarea"); }
/**
 * This attribute indicates that the form shouldn't be validated when submitted.
  **/
export function NoValidate(value) { return attr("novalidate", value, false, "form"); }
/**
 * Indicates whether the details will be shown on page load.
  **/
export function Open(value) { return attr("open", value, false, "details"); }
/**
 * Indicates the optimal numeric value.
  **/
export function Optimum(value) { return attr("optimum", value, false, "meter"); }
/**
 * Defines a regular expression which the element's value will be validated against.
  **/
export function Pattern(value) { return attr("pattern", value, false, "input"); }
/**
 * The ping attribute specifies a space-separated list of URLs to be notified if a user follows the hyperlink.
  **/
export function Ping(value) { return attr("ping", value, false, "a", "area"); }
/**
 * Provides a hint to the user of what can be entered in the field.
  **/
export function PlaceHolder(value) { return attr("placeholder", value, false, "input", "textarea"); }
/**
 * A URL indicating a poster frame to show until the user plays or seeks.
  **/
export function Poster(value) { return attr("poster", value, false, "video"); }
/**
 * Indicates whether the whole resource, parts of it or nothing should be preloaded.
  **/
export function Preload(value) { return attr("preload", value, false, "audio", "video"); }
/**
 * Attempts to find an element in the document.
 * @param rootOrSelector
 * @param selector
 */
export function Query(rootOrSelector, selector) {
    let root = null;
    if (isString(rootOrSelector)) {
        root = document;
        selector = rootOrSelector;
    }
    else {
        root = rootOrSelector;
    }
    const elem = root.querySelector(selector);
    return attr("query", elem, false);
}
/**
 * Attempts to find an element in the document.
 * @param rootOrSelector
 * @param selector
 */
export function QueryAll(rootOrSelector, selector) {
    let root = null;
    if (isString(rootOrSelector)) {
        root = document;
        selector = rootOrSelector;
    }
    else {
        root = rootOrSelector;
    }
    const elems = root.querySelectorAll(selector);
    return Array.from(elems);
}
/**
 * Indicates whether the element can be edited.
  **/
export function ReadOnly(value) { return attr("readonly", value, false, "input", "textarea"); }
/**
 * The radiogroup attribute
  **/
export function RadioGroup(value) { return attr("radiogroup", value, false, "command"); }
/**
 * Specifies which referrer is sent when fetching the resource.
  **/
export function ReferrerPolicy(value) { return attr("referrerpolicy", value, false, "a", "area", "iframe", "img", "link", "script"); }
/**
 * Specifies the relationship of the target object to the link object.
  **/
export function Rel(value) { return attr("rel", value, false, "a", "area", "link"); }
/**
 * Indicates whether this element is required to fill out or not.
  **/
export function Required(value) { return attr("required", value, false, "input", "select", "textarea"); }
/**
 * Indicates whether the list should be displayed in a descending order instead of a ascending.
  **/
export function Reversed(value) { return attr("reversed", value, false, "ol"); }
/**
 * Defines the number of rows in a text area.
  **/
export function Role(value) { return attr("role", value, false); }
/**
 * The rows attribute
  **/
export function Rows(value) { return attr("rows", value, false, "textarea"); }
/**
 * Defines the number of rows a table cell should span over.
  **/
export function RowSpan(value) { return attr("rowspan", value, false, "td", "th"); }
/**
 * Stops a document loaded in an iframe from using certain features (such as submitting forms or opening new windows).
  **/
export function Sandbox(value) { return attr("sandbox", value, false, "iframe"); }
/**
 * Defines the cells that the header test (defined in the th element) relates to.
  **/
export function Scope(value) { return attr("scope", value, false, "th"); }
/**
 * The scoped attribute for `style` tags.
  **/
export function Scoped(value) { return attr("scoped", value, false, "style"); }
/**
 * The scrolling attribute for `iframe` tags.
  **/
export function Scrolling(value) { return attr("scrolling", value ? "yes" : "no", false, "iframe"); }
/**
 * Defines an `option` tag which will be selected on page load.
  **/
export function Selected(value) { return attr("selected", value, false, "option"); }
/**
 * The shape attribute for `a` and `area` tags.
  **/
export function Shape(value) { return attr("shape", value, false, "a", "area"); }
/**
 * Defines the width of the element (in pixels). If the element's type attribute is text or password then it's the number of characters.
  **/
export function Size(value) { return attr("size", value, false, "input", "select"); }
/**
 * Assigns a slot in a shadow DOM shadow tree to an element.
  **/
export function Slot_attr(value) { return attr("slot", value, false); }
/**
 * The sizes attribute
  **/
export function Sizes(value) { return attr("sizes", value, false, "link", "img", "source"); }
/**
 * The span attribute
  **/
export function Span_attr(value) { return attr("span", value, false, "col", "colgroup"); }
/**
 * Indicates whether spell checking is allowed for the element.
  **/
export function SpellCheck(value) { return attr("spellcheck", value, false); }
function unpackURL(value) {
    if (value instanceof URL) {
        value = value.href;
    }
    return value;
}
/**
 * The URL of the embeddable content.
  **/
export function Src(value) {
    return attr("src", unpackURL(value), false, "audio", "embed", "iframe", "img", "input", "script", "source", "track", "video");
}
/**
 * The srcdoc attribute
  **/
export function SrcDoc(value) { return attr("srcdoc", value, false, "iframe"); }
/**
 * The srclang attribute
  **/
export function SrcLang(value) { return attr("srclang", value, false, "track"); }
/**
 * A MediaStream object to use as a source for an HTML video or audio element
  **/
export function SrcObject(value) { return attr("srcObject", value, false, "audio", "video"); }
/**
 * One or more responsive image candidates.
  **/
export function SrcSet(value) { return attr("srcset", value, false, "img", "source"); }
/**
 * Defines the first number if other than 1.
  **/
export function Start(value) { return attr("start", value, false, "ol"); }
/**
 * The step attribute
  **/
export function Step(value) { return attr("step", value, false, "input"); }
/**
 * The summary attribute
  **/
export function Summary_attr(value) { return attr("summary", value, false, "table"); }
/**
 * Overrides the browser's default tab order and follows the one specified instead.
  **/
export function TabIndex(value) { return attr("tabindex", value, false); }
/**
 * Text to be displayed in a tooltip when hovering over the element.
  **/
export function Title_attr(value) { return attr("title", value, false); }
/**
 * The target attribute
  **/
export function Target(value) { return attr("target", value, false, "a", "area", "base", "form"); }
/**
 * Specify whether an element’s attribute values and the values of its Text node children are to be translated when the page is localized, or whether to leave them unchanged.
  **/
export function Translate(value) { return attr("translate", value, false); }
/**
 * Defines the type of the element.
  **/
export function Type(value) {
    if (!isString(value)) {
        value = value.value;
    }
    return attr("type", value, false, "button", "input", "command", "embed", "link", "object", "script", "source", "style", "menu");
}
/**
 * Defines a default value which will be displayed in the element on page load.
  **/
export function Value(value) { return attr("value", value, false, "button", "data", "input", "li", "meter", "option", "progress", "param"); }
/**
 * Defines a default value which will be displayed in the element on page load.
  **/
export function ValueAsNumber(value) { return attr("valueAsNumber", value, false, "input"); }
/**
 * Defines a default value which will be displayed in the element on page load.
  **/
export function ValueAsDate(value) { return attr("valueAsDate", value, false, "input"); }
/**
 * setting the volume at which a media element plays.
  **/
export function Volume(value) { return attr("volume", value, false, "audio", "video"); }
/**
 * The usemap attribute
  **/
export function UseMap(value) { return attr("usemap", value, false, "img", "input", "object"); }
/**
 * For the elements listed here, this establishes the element's width.
  **/
export function Width(value) { return attr("width", value, false, "canvas", "embed", "iframe", "img", "input", "object", "video"); }
export function Wrap(value) { return attr("wrap", value, false, "textarea"); }
//# sourceMappingURL=attrs.js.map