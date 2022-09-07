import type { MediaType } from "@juniper-lib/mediatypes";
import { arrayRemove, arrayScan } from "@juniper-lib/tslib/collections/arrays";
import { isDefined, isString } from "@juniper-lib/tslib/typeChecks";

/**
 * A setter functor for HTML attributes.
 **/
export class Attr {
    readonly tags: readonly string[];
    /**
     * Creates a new setter functor for HTML Attributes
     * @param key - the attribute name.
     * @param value - the value to set for the attribute.
     * @param tags - the HTML tags that support this attribute.
     */
    constructor(
        public readonly key: string,
        public readonly value: any,
        private readonly bySetAttribute: boolean,
        ...tags: string[]) {
        this.tags = tags.map((t) => t.toLocaleUpperCase());
        Object.freeze(this);
    }

    /**
     * Set the attribute value on an HTMLElement
     * @param elem - the element on which to set the attribute.
     */
    applyToElement(elem: HTMLElement) {
        const isDataSet = this.key.startsWith("data-");
        const isValid = this.tags.length === 0
            || this.tags.indexOf(elem.tagName) > -1
            || isDataSet;

        if (!isValid) {
            console.warn(`Element ${elem.tagName} does not support Attribute ${this.key}`);
        }
        else if (isDataSet) {
            const subkey = this.key.substring(5);
            elem.dataset[subkey] = this.value;
        }
        else if (this.key === "style") {
            Object.assign(elem.style, this.value);
        }
        else if (this.key === "classList") {
            (this.value as string[])
                .forEach((v) => elem.classList.add(v));
        }
        else if (this.bySetAttribute) {
            elem.setAttribute(this.key, this.value);
        }
        else if (this.key in elem) {
            (elem as any)[this.key] = this.value;
        }
        else if (this.value === false) {
            elem.removeAttribute(this.key);
        }
        else if (this.value === true) {
            elem.setAttribute(this.key, "");
        }
        else {
            elem.setAttribute(this.key, this.value);
        }
    }
}

export function isAttr(obj: any): obj is Attr {
    return obj instanceof Attr;
}

export function coallesceClassLists(attrs: Attr[], ...rest: string[]): string[] {
    const classes = [...rest];

    const classListAttr = arrayScan(attrs, attr => attr instanceof Attr && attr.key === "classList");
    if (isDefined(classListAttr)) {
        arrayRemove(attrs, classListAttr);
        classes.push(...classListAttr.value as string[]);
    }

    const classNameAttr = arrayScan(attrs, attr => attr instanceof Attr && attr.key === "className");
    if (isDefined(classNameAttr)) {
        arrayRemove(attrs, classNameAttr)
        classes.push(...(classNameAttr.value as string).split(" "));
    }

    return classes;
}

/**
 * a list of types the server accepts, typically a file type.
 * @param value - the value to set on the attribute.
 **/
export function accept(value: string) { return new Attr("accept", value, false, "form", "input"); }

/**
 * The accessKey attribute
 **/
export function accessKey(value: string) { return new Attr("accessKey", value, false, "input", "button"); }

/**
 * specifying the horizontal alignment of the element.
 **/
export function align(value: string) { return new Attr("align", value, false, "applet", "caption", "col", "colgroup", "hr", "iframe", "img", "table", "tbody", "td", "tfoot", "th", "thead", "tr"); }

/**
 * Specifies a feature-policy for the iframe.
 **/
export function allow(value: string) { return new Attr("allow", value, false, "iframe"); }

/**
 * Whether or not to allow an IFrame to open full screen elements.
 */
export function allowFullscreen(value: boolean) { return new Attr("allowfullscreen", value, false, "iframe"); }

/**
 * Alternative text in case an image can't be displayed.
 **/
export function alt(value: string) { return new Attr("alt", value, false, "applet", "area", "img", "input"); }

/**
 * Identifies the currently active element when DOM focus is on a composite widget, textbox, group, or application.
 **/
export function ariaActiveDescendant(value: string) { return new Attr("ariaActiveDescendant", value, false); }

/**
 * Indicates whether assistive technologies will present all, or only parts of, the changed region based on the change notifications defined by the aria-relevant attribute.
 **/
export function ariaAtomic(value: boolean) { return new Attr("ariaAtomic", value, false); }

/**
 * Indicates whether inputting text could trigger display of one or more predictions of the user's intended value for an input and specifies how predictions would be presented if they are made.
 **/
export function ariaAutoComplete(value: string) { return new Attr("ariaAutoComplete", value, false); }

/**
 * Indicates an element is being modified and that assistive technologies MAY want to wait until the modifications are complete before exposing them to the user.
 **/
export function ariaBusy(value: boolean) { return new Attr("ariaBusy", value, false); }

/**
 * Indicates the current "checked" state of checkboxes, radio buttons, and other widgets. See related aria-pressed and aria-selected.
 **/
export function ariaChecked(value: boolean) { return new Attr("ariaChecked", value, false); }

/**
 * Defines the total number of columns in a table, grid, or treegrid. See related aria-colindex.
  **/
export function ariaColCount(value: number) { return new Attr("ariaColCount", value, false); }

/**
 * Defines an element's column index or position with respect to the total number of columns within a table, grid, or treegrid. See related aria-colcount and aria-colspan.
  **/
export function ariaColIndex(value: number) { return new Attr("ariaColIndex", value, false); }

/**
 * Defines the number of columns spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-colindex and aria-rowspan.
  **/
export function ariaColSpan(value: number) { return new Attr("ariaColSpan", value, false); }

/**
 * Identifies the element (or elements) whose contents or presence are controlled by the current element. See related aria-owns.
  **/
export function ariaControls(value: string) { return new Attr("ariaControls", value, false); }

/**
 * Indicates the element that represents the current item within a container or set of related elements.
  **/
export function ariaCurrent(value: string) { return new Attr("ariaCurrent", value, false); }

/**
 * Identifies the element (or elements) that describes the object. See related aria-labelledby.
  **/
export function ariaDescribedBy(value: string) { return new Attr("ariaDescribedBy", value, false); }

/**
 * Identifies the element that provides a detailed, extended description for the object. See related aria-describedby.
  **/
export function ariaDetails(value: string) { return new Attr("ariaDetails", value, false); }

/**
 * Indicates that the element is perceivable but disabled, so it is not editable or otherwise operable. See related aria-hidden and aria-readonly.
  **/
export function ariaDisabled(value: boolean) { return new Attr("ariaDisabled", value, false); }

/**
 * Identifies the element that provides an error message for the object. See related aria-invalid and aria-describedby.
  **/
export function ariaErrorMessage(value: string) { return new Attr("ariaErrorMessage", value, false); }

/**
 * Indicates whether the element, or another grouping element it controls, is currently expanded or collapsed.
 **/
export function ariaExpanded(value: boolean) { return new Attr("ariaExpanded", value, false); }

/**
 * Identifies the next element (or elements) in an alternate reading order of content which, at the user's discretion, allows assistive technology to override the general default of reading in document source order.
  **/
export function ariaFlowTo(value: string) { return new Attr("ariaFlowTo", value, false); }

/**
 * Indicates the availability and type of interactive popup element, such as menu or dialog, that can be triggered by an element.
  **/
export function ariaHasPopup(value: string) { return new Attr("ariaHasPopup", value, false); }

/**
 * Indicates whether the element is exposed to an accessibility API. See related aria-disabled.
 **/
export function ariaHidden(value: boolean) { return new Attr("ariaHidden", value, false); }

/**
 * Indicates the entered value does not conform to the format expected by the application. See related aria-errormessage.
  **/
export function ariaInvalid(value: string) { return new Attr("ariaInvalid", value, false); }

/**
 * Indicates keyboard shortcuts that an author has implemented to activate or give focus to an element.
  **/
export function ariaKeyShortcuts(value: string) { return new Attr("ariaKeyShortcuts", value, false); }

/**
 * Defines a string value that labels the current element. See related aria-labelledby.
  **/
export function ariaLabel(value: string) { return new Attr("ariaLabel", value, false); }

/**
 * Identifies the element (or elements) that labels the current element. See related aria-describedby.
  **/
export function ariaLabelledBy(value: string) { return new Attr("ariaLabelledBy", value, false); }

/**
 * Defines the hierarchical level of an element within a structure.
  **/
export function ariaLevel(value: number) { return new Attr("ariaLevel", value, false); }

/**
 * Indicates that an element will be updated, and describes the types of updates the user agents, assistive technologies, and user can expect from the live region.
  **/
export function ariaLive(value: string) { return new Attr("ariaLive", value, false); }

/**
 * Indicates whether an element is modal when displayed
  **/
export function ariaModal(value: boolean) { return new Attr("ariaModal", value, false); }

/**
 * Indicates whether a text box accepts multiple lines of input or only a single line.
  **/
export function ariaMultiline(value: boolean) { return new Attr("ariaMultiline", value, false); }

/**
 * Indicates that the user may select more than one item from the current selectable descendants.
  **/
export function ariaMultiSelectable(value: boolean) { return new Attr("ariaMultiSelectable", value, false); }

/**
 * Indicates that the user may select more than one item from the current selectable descendants.
  **/
export function ariaOrientation(value: string) { return new Attr("ariaOrientation", value, false); }

/**
 * Identifies an element (or elements) in order to define a visual, functional, or contextual parent/child relationship between DOM elements where the DOM hierarchy cannot be used to represent the relationship. See related aria-controls.
  **/
export function ariaOwns(value: string) { return new Attr("ariaOwns", value, false); }

/**
 * Defines a short hint (a word or short phrase) intended to aid the user with data entry when the control has no value. A hint could be a sample value or a brief description of the expected format.
  **/
export function ariaPlaceholder(value: string) { return new Attr("ariaPlaceholder", value, false); }

/**
 * Defines an element's number or position in the current set of listitems or treeitems. Not required if all elements in the set are present in the DOM. See related aria-setsize.
  **/
export function ariaPosInSet(value: number) { return new Attr("ariaPosInSet", value, false); }

/**
 * Indicates the current "pressed" state of toggle buttons. See related aria-checked and aria-selected.
 **/
export function ariaPressed(value: boolean) { return new Attr("ariaPressed", value, false); }

/**
 * Indicates that the element is not editable, but is otherwise operable. See related aria-disabled.
  **/
export function ariaReadOnly(value: boolean) { return new Attr("ariaReadOnly", value, false); }

/**
 * Indicates what notifications the user agent will trigger when the accessibility tree within a live region is modified. See related aria-atomic.
  **/
export function ariaRelevant(value: string) { return new Attr("ariaRelevant", value, false); }

/**
 * Indicates that user input is required on the element before a form may be submitted.
  **/
export function ariaRequired(value: boolean) { return new Attr("ariaRequired", value, false); }

/**
 * Defines a human-readable, author-localized description for the role of an element.
  **/
export function ariaRoleDescription(value: string) { return new Attr("ariaRoleDescription", value, false); }

/**
 * Defines the total number of rows in a table, grid, or treegrid. See related aria-rowindex.
  **/
export function ariaRowCount(value: number) { return new Attr("ariaRowCount", value, false); }

/**
 * Defines an element's row index or position with respect to the total number of rows within a table, grid, or treegrid. See related aria-rowcount and aria-rowspan.
  **/
export function ariaRowIndex(value: number) { return new Attr("ariaRowIndex", value, false); }

/**
 Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-rowindex and aria-colspan.
  **/
export function ariaRowSpan(value: number) { return new Attr("ariaRowSpan", value, false); }

/**
 * Indicates the current "selected" state of various widgets. See related aria-checked and aria-pressed.
 **/
export function ariaSelected(value: boolean) { return new Attr("ariaSelected", value, false); }

/**
 * Defines the number of items in the current set of listitems or treeitems. Not required if all elements in the set are present in the DOM. See related aria-posinset.
  **/
export function ariaSetSize(value: number) { return new Attr("ariaSetsize", value, false); }

/**
 * Indicates if items in a table or grid are sorted in ascending or descending order.
  **/
export function ariaSort(value: string) { return new Attr("ariaSort", value, false); }

/**
 * Defines the maximum allowed value for a range widget.
  **/
export function ariaValueMax(value: number) { return new Attr("ariaValueMax", value, false); }

/**
 * Defines the minimum allowed value for a range widget.
  **/
export function ariaValueMin(value: number) { return new Attr("ariaValueMin", value, false); }

/**
 * Defines the current value for a range widget. See related aria-valuetext.
  **/
export function ariaValueNow(value: number) { return new Attr("ariaValueNow", value, false); }

/**
 * Defines the human readable text alternative of aria-valuenow for a range widget.
  **/
export function ariaValueText(value: string) { return new Attr("ariaValueText", value, false); }

/**
 * Executes the script asynchronously.
  **/
export function async(value: string) { return new Attr("async", value, false, "script"); }

/**
 * Sets whether input is automatically capitalized when entered by user
  **/
export function autoCapitalize(value: boolean) { return new Attr("autocapitalize", value, false); }

/**
 * Indicates whether controls in this form can by default have their values automatically completed by the browser.
  **/
export function autoComplete(value: boolean) { return new Attr("autocomplete", value ? "on" : "off", false, "form", "input", "select", "textarea"); }

/**
 * The element should be automatically focused after the page loaded.
  **/
export function autoFocus(value: boolean) { return new Attr("autofocus", value, false, "button", "input", "keygen", "select", "textarea"); }

/**
 * The audio or video should play as soon as possible.
  **/
export function autoPlay(value: boolean) { return new Attr("autoplay", value, false, "audio", "video"); }

/**
 * Contains the time range of already buffered media.
  **/
export function buffered(value: boolean) { return new Attr("buffered", value, false, "audio", "video"); }

/**
 * From the HTML Media Capture
  **/
export function capture(value: boolean) { return new Attr("capture", value, false, "input"); }

/**
 * Declares the character encoding of the page or script.
  **/
export function charSet(value: string) { return new Attr("charset", value, false, "meta", "script"); }

/**
 * Indicates whether the element should be checked on page load.
  **/
export function checked(value: boolean) { return new Attr("checked", value, false, "command", "input"); }

/**
 * Contains a URI which points to the source of the quote or change.
  **/
export function cite(value: string) { return new Attr("cite", value, false, "blockquote", "del", "ins", "q"); }

/**
 * Often used with CSS to style elements with common properties.
  **/
export function className(value: string) { return new Attr("className", value, false); }

/**
 * Often used with CSS to style elements with common properties.
  **/
export function classList(...values: string[]) { return new Attr("classList", values, false); }

/**
 * Specifies the URL of the applet's class file to be loaded and executed.
  **/
export function code(value: string) { return new Attr("code", value, false, "applet"); }

/**
 * This attribute gives the absolute or relative URL of the directory where applets' .class files referenced by the code attribute are stored.
  **/
export function codeBase(value: string) { return new Attr("codebase", value, false, "applet"); }

/**
 * Defines the number of columns in a textarea.
  **/
export function cols(value: number) { return new Attr("cols", value, false, "textarea"); }

/**
 * The colspan attribute defines the number of columns a cell should span.
  **/
export function colSpan(value: number) { return new Attr("colspan", value, false, "td", "th"); }

/**
 * A value associated with http-equiv or name depending on the context.
  **/
export function content(value: string) { return new Attr("content", value, false, "meta"); }

/**
 * Indicates whether the element's content is editable.
  **/
export function contentEditable(value: string) { return new Attr("contenteditable", value, false); }

/**
 * Defines the ID of a <menu> element which will serve as the element's context menu.
  **/
export function contextMenu(value: string) { return new Attr("contextmenu", value, false); }

/**
 * Indicates whether the browser should show playback controls to the user.
  **/
export function controls(value: boolean) { return new Attr("controls", value, false, "audio", "video"); }

/**
 * A set of values specifying the coordinates of the hot-spot region.
  **/
export function coords(value: string) { return new Attr("coords", value, false, "area"); }

/**
 * How the element handles cross-origin requests
  **/
export function crossOrigin(value: string) { return new Attr("crossorigin", value, false, "audio", "img", "link", "script", "video"); }

/**
 * Specifies the Content Security Policy that an embedded document must agree to enforce upon itself.
  **/
export function csp(value: string) { return new Attr("csp", value, false, "iframe"); }

/**
 * Specifies the URL of the resource.
  **/
export function data(value: string) { return new Attr("data", value, false, "object"); }

/**
 * Lets you attach custom attributes to an HTML element.
 */
export function customData(name: string, value: any) { return new Attr("data-" + name.toLowerCase(), value, false); }

/**
 * Indicates the date and time associated with the element.
  **/
export function dateTime(value: Date) { return new Attr("datetime", value, false, "del", "ins", "time"); }

/**
 * Indicates the preferred method to decode the image.
  **/
export function decoding(value: string) { return new Attr("decoding", value, false, "img"); }

/**
 * Indicates that the track should be enabled unless the user's preferences indicate something different.
  **/
export function htmlDefault(value: boolean | string) { return new Attr("default", value, false, "track"); }

/**
 * Indicates that the script should be executed after the page has been parsed.
  **/
export function defer(value: boolean) { return new Attr("defer", value, false, "script"); }

/**
 * Defines the text direction. Allowed values are ltr (Left-To-Right) or rtl (Right-To-Left)
  **/
export function dir(value: string) { return new Attr("dir", value, false); }

/**
 * Indicates whether the user can interact with the element.
  **/
export function disabled(value: boolean) { return new Attr("disabled", value, false, "button", "command", "fieldset", "input", "keygen", "optgroup", "option", "select", "textarea"); }

/**
 * ??? 
  **/
export function dirName(value: string) { return new Attr("dirname", value, false, "input", "textarea"); }

/**
 * Indicates that the hyperlink is to be used for downloading a resource.
  **/
export function download(value: boolean) { return new Attr("download", value, false, "a", "area"); }

/**
 * Defines whether the element can be dragged.
  **/
export function draggable(value: boolean) { return new Attr("draggable", value, false); }

/**
 * Indicates that the element accepts the dropping of content onto it.
  **/
export function dropZone(value: string) { return new Attr("dropzone", value, false); }

/**
 * Defines the content type of the form data when the method is POST.
  **/
export function encType(value: string) { return new Attr("enctype", value, false, "form"); }

/**
 * The enterkeyhint specifies what action label (or icon) to present for the enter key on virtual keyboards. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
  **/
export function enterKeyHint(value: string) { return new Attr("enterkeyhint", value, false, "textarea"); }

/**
 * Describes elements which belongs to this one.
  **/
export function htmlFor(value: string) { return new Attr("htmlFor", value, false, "label", "output"); }

/**
 * Indicates the form that is the owner of the element.
  **/
export function form(value: string) { return new Attr("form", value, false, "button", "fieldset", "input", "keygen", "label", "meter", "object", "output", "progress", "select", "textarea"); }

/**
 * Indicates the action of the element, overriding the action defined in the <form>.
  **/
export function formAction(value: string) { return new Attr("formaction", value, false, "input", "button"); }

/**
 * If the button/input is a submit button (type="submit"), this attribute sets the encoding type to use during form submission. If this attribute is specified, it overrides the enctype attribute of the button's form owner.
  **/
export function formEncType(value: string) { return new Attr("formenctype", value, false, "button", "input"); }

/**
 * If the button/input is a submit button (type="submit"), this attribute sets the submission method to use during form submission (GET, POST, etc.). If this attribute is specified, it overrides the method attribute of the button's form owner.
  **/
export function formMethod(value: string) { return new Attr("formmethod", value, false, "button", "input"); }

/**
 * If the button/input is a submit button (type="submit"), this boolean attribute specifies that the form is not to be validated when it is submitted. If this attribute is specified, it overrides the novalidate attribute of the button's form owner.
  **/
export function formNoValidate(value: boolean) { return new Attr("formnovalidate", value, false, "button", "input"); }

/**
 * If the button/input is a submit button (type="submit"), this attribute specifies the browsing context (for example, tab, window, or inline frame) in which to display the response that is received after submitting the form. If this attribute is specified, it overrides the target attribute of the button's form owner.
  **/
export function formTarget(value: string) { return new Attr("formtarget", value, false, "button", "input"); }

/**
 * Width of the border to put around an IFrame.
 */
export function frameBorder(value: string | number) { return new Attr("frameborder", value, false, "iframe"); }

/**
 * IDs of the <th> elements which applies to this element.
  **/
export function headers(value: string) { return new Attr("headers", value, false, "td", "th"); }

/**
 * Specifies the height of elements listed here. For all other elements, use the CSS height property.
  **/
export function htmlHeight(value: number | string) { return new Attr("height", value, false, "canvas", "embed", "iframe", "img", "input", "object", "video"); }

/**
 * Prevents rendering of given element, while keeping child elements, e.g. script elements, active.
  **/
export function hidden(value: boolean) { return new Attr("hidden", value, false); }

/**
 * Indicates the lower bound of the upper range.
  **/
export function high(value: number) { return new Attr("high", value, false, "meter"); }

/**
 * The URL of a linked resource.
  **/
export function href(value: string | URL) { return new Attr("href", unpackURL(value), false, "a", "area", "base", "link"); }

/**
 * Specifies the language of the linked resource.
  **/
export function hrefLang(value: string) { return new Attr("hreflang", value, false, "a", "area", "link"); }

/**
 * Defines a pragma directive.
  **/
export function httpEquiv(value: string) { return new Attr("httpEquiv", value, false, "meta"); }

/**
 * Specifies a picture which represents the command.
  **/
export function icon(value: string) { return new Attr("icon", value, false, "command"); }

/**
 * Often used with CSS to style a specific element. The value of this attribute must be unique.
  **/
export function id(value: string) { return new Attr("id", value, false); }

/**
 * Indicates the relative fetch priority for the resource.
  **/
export function importance(value: string) { return new Attr("importance", value, false, "iframe", "img", "link", "script"); }

/**
 * Provides a hint as to the type of data that might be entered by the user while editing the element or its contents. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
  **/
export function inputMode(value: string) { return new Attr("inputmode", value, false, "textarea"); }

/**
 * Specifies a Subresource Integrity value that allows browsers to verify what they fetch.
  **/
export function integrity(value: string) { return new Attr("integrity", value, false, "link", "script"); }

/**
 * This attribute tells the browser to ignore the actual intrinsic size of the image and pretend it’s the size specified in the attribute.
  **/
export function intrinsicSize(value: string) { return new Attr("intrinsicsize", value, false, "img"); }

/**
 * Indicates that the image is part of a server-side image map.
  **/
export function isMap(value: boolean) { return new Attr("ismap", value, false, "img"); }

/**
 * Specifies the type of key generated.
  **/
export function keyType(value: string) { return new Attr("keytype", value, false, "keygen"); }

/**
 * The itemprop attribute
  **/
export function itemProp(value: string) { return new Attr("itemprop", value, false); }

/**
 * Specifies the kind of text track.
  **/
export function kind(value: string) { return new Attr("kind", value, false, "track"); }

/**
 * Specifies a user-readable title of the element.
  **/
export function label(value: string) { return new Attr("label", value, false, "optgroup", "option", "track"); }

/**
 * Defines the language used in the element.
  **/
export function lang(value: string) { return new Attr("lang", value, false); }

/**
 * Defines the script language used in the element.
  **/
export function language(value: string) { return new Attr("language", value, false, "script"); }

/**
 * Identifies a list of pre-defined options to suggest to the user.
  **/
export function list(value: string) { return new Attr("list", value, true, "input"); }

/**
 * Indicates whether the media should start playing from the start when it's finished.
  **/
export function loop(value: boolean) { return new Attr("loop", value, false, "audio", "bgsound", "marquee", "video"); }

/**
 * Indicates the upper bound of the lower range.
  **/
export function low(value: number) { return new Attr("low", value, false, "meter"); }

/**
 * Indicates the maximum value allowed.
  **/
export function max(value: number) { return new Attr("max", value, false, "input", "meter", "progress"); }

/**
 * Defines the maximum number of characters allowed in the element.
  **/
export function maxLength(value: number) { return new Attr("maxlength", value, false, "input", "textarea"); }

/**
 * Defines the minimum number of characters allowed in the element.
  **/
export function minLength(value: number) { return new Attr("minlength", value, false, "input", "textarea"); }

/**
 * Specifies a hint of the media for which the linked resource was designed.
  **/
export function media(value: string) { return new Attr("media", value, false, "a", "area", "link", "source", "style"); }

/**
 * Defines which HTTP method to use when submitting the form. Can be GET (default) or POST.
  **/
export function method(value: string) { return new Attr("method", value, false, "form"); }

/**
 * Indicates the minimum value allowed.
  **/
export function min(value: number) { return new Attr("min", value, false, "input", "meter"); }

/**
 * Indicates whether multiple values can be entered in an input of the type email or file.
  **/
export function multiple(value: boolean) { return new Attr("multiple", value, false, "input", "select"); }

/**
 * Indicates whether the audio will be initially silenced on page load.
  **/
export function muted(value: boolean) { return new Attr("muted", value, false, "audio", "video"); }

/**
 * Name of the element. For example used by the server to identify the fields in form submits.
  **/
export function name(value: string) { return new Attr("name", value, false, "button", "form", "fieldset", "iframe", "input", "keygen", "object", "output", "select", "textarea", "map", "meta", "param"); }

/**
 * This attribute indicates that the form shouldn't be validated when submitted.
  **/
export function noValidate(value: boolean) { return new Attr("novalidate", value, false, "form"); }

/**
 * Indicates whether the details will be shown on page load.
  **/
export function open(value: boolean) { return new Attr("open", value, false, "details"); }

/**
 * Indicates the optimal numeric value.
  **/
export function optimum(value: number) { return new Attr("optimum", value, false, "meter"); }

/**
 * Defines a regular expression which the element's value will be validated against.
  **/
export function pattern(value: string) { return new Attr("pattern", value, false, "input"); }

/**
 * The ping attribute specifies a space-separated list of URLs to be notified if a user follows the hyperlink.
  **/
export function ping(value: string) { return new Attr("ping", value, false, "a", "area"); }

/**
 * Provides a hint to the user of what can be entered in the field.
  **/
export function placeHolder(value: string) { return new Attr("placeholder", value, false, "input", "textarea"); }

/**
 * Indicates that the media element should play automatically on iOS.
  **/
export function playsInline(value: boolean) { return new Attr("playsInline", value, false, "audio", "video"); }

/**
 * A URL indicating a poster frame to show until the user plays or seeks.
  **/
export function poster(value: string) { return new Attr("poster", value, false, "video"); }

/**
 * Indicates whether the whole resource, parts of it or nothing should be preloaded.
  **/
export function preload(value: boolean | string) { return new Attr("preload", value, false, "audio", "video"); }

/**
 * Indicates whether the element can be edited.
  **/
export function readOnly(value: boolean) { return new Attr("readonly", value, false, "input", "textarea"); }

/**
 * The radiogroup attribute
  **/
export function radioGroup(value: string) { return new Attr("radiogroup", value, false, "command"); }

/**
 * Specifies which referrer is sent when fetching the resource.
  **/
export function referrerPolicy(value: string) { return new Attr("referrerpolicy", value, false, "a", "area", "iframe", "img", "link", "script"); }

/**
 * Specifies the relationship of the target object to the link object.
  **/
export function rel(value: string) { return new Attr("rel", value, false, "a", "area", "link"); }

/**
 * Indicates whether this element is required to fill out or not.
  **/
export function required(value: boolean) { return new Attr("required", value, false, "input", "select", "textarea"); }

/**
 * Indicates whether the list should be displayed in a descending order instead of a ascending.
  **/
export function reversed(value: boolean) { return new Attr("reversed", value, false, "ol"); }

/**
 * Defines the number of rows in a text area.
  **/
export function role(value: string) { return new Attr("role", value, false); }

/**
 * The rows attribute
  **/
export function rows(value: number) { return new Attr("rows", value, false, "textarea"); }

/**
 * Defines the number of rows a table cell should span over.
  **/
export function rowSpan(value: number) { return new Attr("rowspan", value, false, "td", "th"); }

/**
 * Stops a document loaded in an iframe from using certain features (such as submitting forms or opening new windows).
  **/
export function sandbox(value: string) { return new Attr("sandbox", value, false, "iframe"); }

/**
 * Defines the cells that the header test (defined in the th element) relates to.
  **/
export function scope(value: string) { return new Attr("scope", value, false, "th"); }

/**
 * The scoped attribute
  **/
export function scoped(value: boolean) { return new Attr("scoped", value, false, "style"); }

/**
 * Defines a value which will be selected on page load.
  **/
export function selected(value: boolean) { return new Attr("selected", value, false, "option"); }

/**
 * The shape attribute
  **/
export function shape(value: string) { return new Attr("shape", value, false, "a", "area"); }

/**
 * Defines the width of the element (in pixels). If the element's type attribute is text or password then it's the number of characters.
  **/
export function size(value: number) { return new Attr("size", value, false, "input", "select"); }

/**
 * Assigns a slot in a shadow DOM shadow tree to an element.
  **/
export function slot(value: string) { return new Attr("slot", value, false); }

/**
 * The sizes attribute
  **/
export function sizes(value: string) { return new Attr("sizes", value, false, "link", "img", "source"); }

/**
 * The span attribute
  **/
export function span(value: string) { return new Attr("span", value, false, "col", "colgroup"); }

/**
 * Indicates whether spell checking is allowed for the element.
  **/
export function spellCheck(value: boolean) { return new Attr("spellcheck", value, false); }

function unpackURL(value: string | URL) {
    if (value instanceof URL) {
        value = value.href;
    }

    return value;
}

/**
 * The URL of the embeddable content.
  **/
export function src(value: string | URL) {
    return new Attr("src", unpackURL(value), false, "audio", "embed", "iframe", "img", "input", "script", "source", "track", "video");
}

/**
 * The srcdoc attribute
  **/
export function srcDoc(value: string) { return new Attr("srcdoc", value, false, "iframe"); }

/**
 * The srclang attribute
  **/
export function srcLang(value: string) { return new Attr("srclang", value, false, "track"); }

/**
 * A MediaStream object to use as a source for an HTML video or audio element
  **/
export function srcObject(value: MediaProvider) { return new Attr("srcObject", value, false, "audio", "video"); }

/**
 * One or more responsive image candidates.
  **/
export function srcSet(value: string) { return new Attr("srcset", value, false, "img", "source"); }

/**
 * Defines the first number if other than 1.
  **/
export function start(value: number) { return new Attr("start", value, false, "ol"); }

/**
 * The step attribute
  **/
export function step(value: number) { return new Attr("step", value, false, "input"); }

/**
 * The summary attribute
  **/
export function summary(value: string) { return new Attr("summary", value, false, "table"); }

/**
 * Overrides the browser's default tab order and follows the one specified instead.
  **/
export function tabIndex(value: number) { return new Attr("tabindex", value, false); }

/**
 * Text to be displayed in a tooltip when hovering over the element.
  **/
export function title(value: string) { return new Attr("title", value, false); }

/**
 * The target attribute
  **/
export function target(value: string) { return new Attr("target", value, false, "a", "area", "base", "form"); }

/**
 * Specify whether an element’s attribute values and the values of its Text node children are to be translated when the page is localized, or whether to leave them unchanged.
  **/
export function translate(value: boolean) { return new Attr("translate", value, false); }

/**
 * Defines the type of the element.
  **/
export function type(value: string | MediaType) {
    if (!isString(value)) {
        value = value.value;
    }
    return new Attr("type", value, false, "button", "input", "command", "embed", "link", "object", "script", "source", "style", "menu");
}

/**
 * Defines a default value which will be displayed in the element on page load.
  **/
export function value(value: string | number) { return new Attr("value", value, false, "button", "data", "input", "li", "meter", "option", "progress", "param"); }

/**
 * Defines a default value which will be displayed in the element on page load.
  **/
export function valueAsNumber(value: number) { return new Attr("valueAsNumber", value, false, "input"); }

/**
 * Defines a default value which will be displayed in the element on page load.
  **/
export function valueAsDate(value: Date) { return new Attr("valueAsDate", value, false, "input"); }

/**
 * setting the volume at which a media element plays.
  **/
export function volume(value: number) { return new Attr("volume", value, false, "audio", "video"); }

/**
 * The usemap attribute
  **/
export function useMap(value: boolean) { return new Attr("usemap", value, false, "img", "input", "object"); }

/**
 * For the elements listed here, this establishes the element's width.
  **/
export function htmlWidth(value: number | string) { return new Attr("width", value, false, "canvas", "embed", "iframe", "img", "input", "object", "video"); }

/**
 * Indicates whether the text should be wrapped.
  **/
export function wrap(value: boolean) { return new Attr("wrap", value, false, "textarea"); }