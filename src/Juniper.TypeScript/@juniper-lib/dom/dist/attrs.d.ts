import type { MediaType } from "@juniper-lib/mediatypes/dist";
/**
 * A setter functor for HTML attributes.
 **/
export declare class HtmlAttr<T extends string = string, V = number | object | ((elem: HTMLElement) => unknown)> {
    readonly key: T;
    readonly value: V;
    private readonly bySetAttribute;
    readonly tags: readonly string[];
    /**
     * Creates a new setter functor for HTML Attributes
     * @param key - the attribute name.
     * @param value - the value to set for the attribute.
     * @param bySetAttribute - whether the attribute should be set via the setAttribute method.
     * @param tags - the HTML tags that support this attribute.
     */
    constructor(key: T, value: V, bySetAttribute: boolean, ...tags: string[]);
    /**
     * Set the attribute value on an HTMLElement
     * @param elem - the element on which to set the attribute.
     */
    applyToElement(elem: HTMLElement): void;
}
export declare function isAttr(obj: unknown): obj is HtmlAttr;
/**
 * a list of types the server accepts, typically a file type.
 * @param value - the value to set on the attribute.
 **/
export declare function Accept(value: string): HtmlAttr<"accept", string>;
/**
 * The accessKey attribute
 **/
export declare function AccessKey(value: string): HtmlAttr<"accessKey", string>;
/**
 * specifying the horizontal alignment of the element.
 **/
export declare function Align(value: string): HtmlAttr<"align", string>;
/**
 * Specifies a feature-policy for the iframe.
 **/
export declare function Allow(value: string): HtmlAttr<"allow", string>;
/**
 * Whether or not to allow an IFrame to open full screen elements.
 */
export declare function AllowFullscreen(value: boolean): HtmlAttr<"allowfullscreen", boolean>;
/**
 * Alternative text in case an image can't be displayed.
 **/
export declare function Alt(value: string): HtmlAttr<"alt", string>;
/**
 * Identifies the currently active element when DOM focus is on a composite widget, textbox, group, or application.
 **/
export declare function AriaActiveDescendant(value: string): HtmlAttr<"ariaActiveDescendant", string>;
/**
 * Indicates whether assistive technologies will present all, or only parts of, the changed region based on the change notifications defined by the aria-relevant attribute.
 **/
export declare function AriaAtomic(value: boolean): HtmlAttr<"ariaAtomic", boolean>;
/**
 * Indicates whether inputting text could trigger display of one or more predictions of the user's intended value for an input and specifies how predictions would be presented if they are made.
 **/
export declare function AriaAutoComplete(value: string): HtmlAttr<"ariaAutoComplete", string>;
/**
 * Indicates an element is being modified and that assistive technologies MAY want to wait until the modifications are complete before exposing them to the user.
 **/
export declare function AriaBusy(value: boolean): HtmlAttr<"ariaBusy", boolean>;
/**
 * Indicates the current "checked" state of checkboxes, radio buttons, and other widgets. See related aria-pressed and aria-selected.
 **/
export declare function AriaChecked(value: boolean): HtmlAttr<"ariaChecked", boolean>;
/**
 * Defines the total number of columns in a table, grid, or treegrid. See related aria-colindex.
  **/
export declare function AriaColCount(value: number): HtmlAttr<"ariaColCount", number>;
/**
 * Defines an element's column index or position with respect to the total number of columns within a table, grid, or treegrid. See related aria-colcount and aria-colspan.
  **/
export declare function AriaColIndex(value: number): HtmlAttr<"ariaColIndex", number>;
/**
 * Defines the number of columns spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-colindex and aria-rowspan.
  **/
export declare function AriaColSpan(value: number): HtmlAttr<"ariaColSpan", number>;
/**
 * Identifies the element (or elements) whose contents or presence are controlled by the current element. See related aria-owns.
  **/
export declare function AriaControls(value: string): HtmlAttr<"ariaControls", string>;
/**
 * Indicates the element that represents the current item within a container or set of related elements.
  **/
export declare function AriaCurrent(value: string): HtmlAttr<"ariaCurrent", string>;
/**
 * Identifies the element (or elements) that describes the object. See related aria-labelledby.
  **/
export declare function AriaDescribedBy(value: string): HtmlAttr<"ariaDescribedBy", string>;
/**
 * Identifies the element that provides a detailed, extended description for the object. See related aria-describedby.
  **/
export declare function AriaDetails(value: string): HtmlAttr<"ariaDetails", string>;
/**
 * Indicates that the element is perceivable but disabled, so it is not editable or otherwise operable. See related aria-hidden and aria-readonly.
  **/
export declare function AriaDisabled(value: boolean): HtmlAttr<"ariaDisabled", boolean>;
/**
 * Identifies the element that provides an error message for the object. See related aria-invalid and aria-describedby.
  **/
export declare function AriaErrorMessage(value: string): HtmlAttr<"ariaErrorMessage", string>;
/**
 * Indicates whether the element, or another grouping element it controls, is currently expanded or collapsed.
 **/
export declare function AriaExpanded(value: boolean): HtmlAttr<"ariaExpanded", boolean>;
/**
 * Identifies the next element (or elements) in an alternate reading order of content which, at the user's discretion, allows assistive technology to override the general default of reading in document source order.
  **/
export declare function AriaFlowTo(value: string): HtmlAttr<"ariaFlowTo", string>;
/**
 * Indicates the availability and type of interactive popup element, such as menu or dialog, that can be triggered by an element.
  **/
export declare function AriaHasPopup(value: string): HtmlAttr<"ariaHasPopup", string>;
/**
 * Indicates whether the element is exposed to an accessibility API. See related aria-disabled.
 **/
export declare function AriaHidden(value: boolean): HtmlAttr<"ariaHidden", boolean>;
/**
 * Indicates the entered value does not conform to the format expected by the application. See related aria-errormessage.
  **/
export declare function AriaInvalid(value: string): HtmlAttr<"ariaInvalid", string>;
/**
 * Indicates keyboard shortcuts that an author has implemented to activate or give focus to an element.
  **/
export declare function AriaKeyShortcuts(value: string): HtmlAttr<"ariaKeyShortcuts", string>;
/**
 * Defines a string value that labels the current element. See related aria-labelledby.
  **/
export declare function AriaLabel(value: string): HtmlAttr<"ariaLabel", string>;
/**
 * Identifies the element (or elements) that labels the current element. See related aria-describedby.
  **/
export declare function AriaLabelledBy(value: string): HtmlAttr<"ariaLabelledBy", string>;
/**
 * Defines the hierarchical level of an element within a structure.
  **/
export declare function AriaLevel(value: number): HtmlAttr<"ariaLevel", number>;
/**
 * Indicates that an element will be updated, and describes the types of updates the user agents, assistive technologies, and user can expect from the live region.
  **/
export declare function AriaLive(value: string): HtmlAttr<"ariaLive", string>;
/**
 * Indicates whether an element is modal when displayed
  **/
export declare function AriaModal(value: boolean): HtmlAttr<"ariaModal", boolean>;
/**
 * Indicates whether a text box accepts multiple lines of input or only a single line.
  **/
export declare function AriaMultiline(value: boolean): HtmlAttr<"ariaMultiline", boolean>;
/**
 * Indicates that the user may select more than one item from the current selectable descendants.
  **/
export declare function AriaMultiSelectable(value: boolean): HtmlAttr<"ariaMultiSelectable", boolean>;
/**
 * Indicates that the user may select more than one item from the current selectable descendants.
  **/
export declare function AriaOrientation(value: string): HtmlAttr<"ariaOrientation", string>;
/**
 * Identifies an element (or elements) in order to define a visual, functional, or contextual parent/child relationship between DOM elements where the DOM hierarchy cannot be used to represent the relationship. See related aria-controls.
  **/
export declare function AriaOwns(value: string): HtmlAttr<"ariaOwns", string>;
/**
 * Defines a short hint (a word or short phrase) intended to aid the user with data entry when the control has no value. A hint could be a sample value or a brief description of the expected format.
  **/
export declare function AriaPlaceholder(value: string): HtmlAttr<"ariaPlaceholder", string>;
/**
 * Defines an element's number or position in the current set of listitems or treeitems. Not required if all elements in the set are present in the DOM. See related aria-setsize.
  **/
export declare function AriaPosInSet(value: number): HtmlAttr<"ariaPosInSet", number>;
/**
 * Indicates the current "pressed" state of toggle buttons. See related aria-checked and aria-selected.
 **/
export declare function AriaPressed(value: boolean): HtmlAttr<"ariaPressed", boolean>;
/**
 * Indicates that the element is not editable, but is otherwise operable. See related aria-disabled.
  **/
export declare function AriaReadOnly(value: boolean): HtmlAttr<"ariaReadOnly", boolean>;
/**
 * Indicates what notifications the user agent will trigger when the accessibility tree within a live region is modified. See related aria-atomic.
  **/
export declare function AriaRelevant(value: string): HtmlAttr<"ariaRelevant", string>;
/**
 * Indicates that user input is required on the element before a form may be submitted.
  **/
export declare function AriaRequired(value: boolean): HtmlAttr<"ariaRequired", boolean>;
/**
 * Defines a human-readable, author-localized description for the role of an element.
  **/
export declare function AriaRoleDescription(value: string): HtmlAttr<"ariaRoleDescription", string>;
/**
 * Defines the total number of rows in a table, grid, or treegrid. See related aria-rowindex.
  **/
export declare function AriaRowCount(value: number): HtmlAttr<"ariaRowCount", number>;
/**
 * Defines an element's row index or position with respect to the total number of rows within a table, grid, or treegrid. See related aria-rowcount and aria-rowspan.
  **/
export declare function AriaRowIndex(value: number): HtmlAttr<"ariaRowIndex", number>;
/**
 Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-rowindex and aria-colspan.
  **/
export declare function AriaRowSpan(value: number): HtmlAttr<"ariaRowSpan", number>;
/**
 * Indicates the current "selected" state of various widgets. See related aria-checked and aria-pressed.
 **/
export declare function AriaSelected(value: boolean): HtmlAttr<"ariaSelected", boolean>;
/**
 * Defines the number of items in the current set of listitems or treeitems. Not required if all elements in the set are present in the DOM. See related aria-posinset.
  **/
export declare function AriaSetSize(value: number): HtmlAttr<"ariaSetsize", number>;
/**
 * Indicates if items in a table or grid are sorted in ascending or descending order.
  **/
export declare function AriaSort(value: string): HtmlAttr<"ariaSort", string>;
/**
 * Defines the maximum allowed value for a range widget.
  **/
export declare function AriaValueMax(value: number): HtmlAttr<"ariaValueMax", number>;
/**
 * Defines the minimum allowed value for a range widget.
  **/
export declare function AriaValueMin(value: number): HtmlAttr<"ariaValueMin", number>;
/**
 * Defines the current value for a range widget. See related aria-valuetext.
  **/
export declare function AriaValueNow(value: number): HtmlAttr<"ariaValueNow", number>;
/**
 * Defines the human readable text alternative of aria-valuenow for a range widget.
  **/
export declare function AriaValueText(value: string): HtmlAttr<"ariaValueText", string>;
/**
 * Executes the script asynchronously.
  **/
export declare function Async(value: string): HtmlAttr<"async", string>;
/**
 * Sets whether input is automatically capitalized when entered by user
  **/
export declare function AutoCapitalize(value: boolean): HtmlAttr<"autocapitalize", boolean>;
/**
 * Indicates whether controls in this form can by default have their values automatically completed by the browser.
  **/
export declare function AutoComplete(value: HTMLAutoCompleteAttributeValue): HtmlAttr<"autocomplete", HTMLAutoCompleteAttributeValue>;
/**
 * The element should be automatically focused after the page loaded.
  **/
export declare function AutoFocus(value: boolean): HtmlAttr<"autofocus", boolean>;
/**
 * The audio or video should play as soon as possible.
  **/
export declare function AutoPlay(value: boolean): HtmlAttr<"autoplay", boolean>;
/**
 * Contains the time range of already buffered media.
  **/
export declare function Buffered(value: boolean): HtmlAttr<"buffered", boolean>;
/**
 * From the HTML Media Capture
  **/
export declare function Capture(value: boolean): HtmlAttr<"capture", boolean>;
/**
 * Declares the character encoding of the page or script.
  **/
export declare function CharSet(value: string): HtmlAttr<"charset", string>;
/**
 * Indicates whether the element should be checked on page load.
  **/
export declare function Checked(value: boolean): HtmlAttr<"checked", boolean>;
/**
 * Contains a URI which points to the source of the quote or change.
  **/
export declare function Cite_attr(value: string): HtmlAttr<"cite", string>;
/**
 * Often used with CSS to style elements with common properties.
  **/
export declare function ClassName(value: string): HtmlAttr<"className", string>;
/**
 * Often used with CSS to style elements with common properties.
  **/
export declare function ClassList(...values: string[]): HtmlAttr<"CLASS_LIST", (element: HTMLElement) => void>;
/**
 * Specifies the URL of the applet's class file to be loaded and executed.
  **/
export declare function Code_attr(value: string): HtmlAttr<"code", string>;
/**
 * This attribute gives the absolute or relative URL of the directory where applets' .class files referenced by the code attribute are stored.
  **/
export declare function CodeBase(value: string): HtmlAttr<"codebase", string>;
/**
 * Defines the number of columns in a textarea.
  **/
export declare function Cols(value: number): HtmlAttr<"cols", number>;
/**
 * The colspan attribute defines the number of columns a cell should span.
  **/
export declare function ColSpan(value: number): HtmlAttr<"colspan", number>;
/**
 * A value associated with http-equiv or name depending on the context.
  **/
export declare function Content(value: string): HtmlAttr<"content", string>;
/**
 * Indicates whether the element's content is editable.
  **/
export declare function ContentEditable(value: string): HtmlAttr<"contenteditable", string>;
/**
 * Defines the ID of a <menu> element which will serve as the element's context menu.
  **/
export declare function ContextMenu(value: string): HtmlAttr<"contextmenu", string>;
/**
 * Indicates whether the browser should show playback controls to the user.
  **/
export declare function Controls(value: boolean): HtmlAttr<"controls", boolean>;
/**
 * A set of values specifying the coordinates of the hot-spot region.
  **/
export declare function Coords(value: string): HtmlAttr<"coords", string>;
/**
 * How the element handles cross-origin requests
  **/
export declare function CrossOrigin(value: string): HtmlAttr<"crossorigin", string>;
/**
 * Specifies the Content Security Policy that an embedded document must agree to enforce upon itself.
  **/
export declare function CSP(value: string): HtmlAttr<"csp", string>;
/**
 * Specifies the URL of the resource.
  **/
export declare function Data_attr(value: string): HtmlAttr<"data", string>;
/**
 * Lets you attach custom attributes to an HTML element.
 */
export declare function CustomData<V = number | object>(name: string, value: V): HtmlAttr<string, V>;
/**
 * Indicates the date and time associated with the element.
  **/
export declare function DateTime(value: Date): HtmlAttr<"datetime", Date>;
/**
 * Indicates the preferred method to decode the image.
  **/
export declare function Decoding(value: string): HtmlAttr<"decoding", string>;
/**
 * Indicates that the track should be enabled unless the user's preferences indicate something different.
  **/
export declare function Default(value: boolean | string): HtmlAttr<"default", string | boolean>;
/**
 * Indicates that the script should be executed after the page has been parsed.
  **/
export declare function Defer(value: boolean): HtmlAttr<"defer", boolean>;
/**
 * Defines the text direction. Allowed values are ltr (Left-To-Right) or rtl (Right-To-Left)
  **/
export declare function Dir(value: string): HtmlAttr<"dir", string>;
/**
 * Indicates whether the user can interact with the element.
  **/
export declare function Disabled(value: boolean): HtmlAttr<"disabled", boolean>;
/**
 * ???
  **/
export declare function DirName(value: string): HtmlAttr<"dirname", string>;
/**
 * Indicates that the hyperlink is to be used for downloading a resource by giving the file a name.
  **/
export declare function Download(value: string): HtmlAttr<"download", string>;
/**
 * Defines whether the element can be dragged.
  **/
export declare function Draggable(value: boolean): HtmlAttr<"draggable", boolean>;
/**
 * Indicates that the element accepts the dropping of content onto it.
  **/
export declare function DropZone(value: string): HtmlAttr<"dropzone", string>;
/**
 * Defines the content type of the form data when the method is POST.
  **/
export declare function EncType(value: string): HtmlAttr<"enctype", string>;
/**
 * The enterkeyhint specifies what action label (or icon) to present for the enter key on virtual keyboards. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
  **/
export declare function EnterKeyHint(value: string): HtmlAttr<"enterkeyhint", string>;
/**
 * Describes elements which belongs to this one.
  **/
export declare function HtmlFor(value: string): HtmlAttr<"htmlFor", string>;
/**
 * Indicates the form that is the owner of the element.
  **/
export declare function Form_attr(value: string): HtmlAttr<"form", string>;
/**
 * Indicates the action of the element, overriding the action defined in the <form>.
  **/
export declare function FormAction(value: string): HtmlAttr<"formaction", string>;
/**
 * If the button/input is a submit button (type="submit"), this attribute sets the encoding type to use during form submission. If this attribute is specified, it overrides the enctype attribute of the button's form owner.
  **/
export declare function FormEncType(value: string): HtmlAttr<"formenctype", string>;
/**
 * If the button/input is a submit button (type="submit"), this attribute sets the submission method to use during form submission (GET, POST, etc.). If this attribute is specified, it overrides the method attribute of the button's form owner.
  **/
export declare function FormMethod(value: string): HtmlAttr<"formmethod", string>;
/**
 * If the button/input is a submit button (type="submit"), this boolean attribute specifies that the form is not to be validated when it is submitted. If this attribute is specified, it overrides the novalidate attribute of the button's form owner.
  **/
export declare function FormNoValidate(value: boolean): HtmlAttr<"formnovalidate", boolean>;
/**
 * If the button/input is a submit button (type="submit"), this attribute specifies the browsing context (for example, tab, window, or inline frame) in which to display the response that is received after submitting the form. If this attribute is specified, it overrides the target attribute of the button's form owner.
  **/
export declare function FormTarget(value: string): HtmlAttr<"formtarget", string>;
/**
 * Width of the border to put around an `iframe` tag.
 */
export declare function FrameBorder(value: string | number | boolean): HtmlAttr<"frameborder", string | number>;
/**
 * IDs of the <th> elements which applies to this element.
  **/
export declare function Headers(value: string): HtmlAttr<"headers", string>;
/**
 * Specifies the height of elements listed here. For all other elements, use the CSS height property.
  **/
export declare function Height(value: number | string): HtmlAttr<"height", string | number>;
/**
 * Prevents rendering of given element, while keeping child elements, e.g. script elements, active.
  **/
export declare function Hidden(value: boolean): HtmlAttr<"hidden", boolean>;
/**
 * Indicates the lower bound of the upper range.
  **/
export declare function High(value: number): HtmlAttr<"high", number>;
/**
 * The URL of a linked resource.
  **/
export declare function Href(value: string | URL | Blob): HtmlAttr<"href", string>;
/**
 * Specifies the language of the linked resource.
  **/
export declare function HrefLang(value: string): HtmlAttr<"hreflang", string>;
/**
 * Defines a pragma directive.
  **/
export declare function HttpEquiv(value: string): HtmlAttr<"httpEquiv", string>;
/**
 * Specifies a picture which represents the command.
  **/
export declare function Icon(value: string): HtmlAttr<"icon", string>;
/**
 * Often used with CSS to style a specific element. The value of this attribute must be unique.
  **/
export declare function ID(value: string): HtmlAttr<"id", string>;
/**
 * Indicates the relative fetch priority for the resource.
  **/
export declare function Importance(value: string): HtmlAttr<"importance", string>;
/**
 * Provides a hint as to the type of data that might be entered by the user while editing the element or its contents. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
  **/
export declare function InputMode(value: string): HtmlAttr<"inputmode", string>;
/**
 * Specifies a Subresource Integrity value that allows browsers to verify what they fetch.
  **/
export declare function Integrity(value: string): HtmlAttr<"integrity", string>;
/**
 * This attribute tells the browser to ignore the actual intrinsic size of the image and pretend it’s the size specified in the attribute.
  **/
export declare function IntrinsicSize(value: string): HtmlAttr<"intrinsicsize", string>;
export declare function Is(value: string): HtmlAttr<"is", string>;
/**
 * Indicates that the image is part of a server-side image map.
  **/
export declare function IsMap(value: boolean): HtmlAttr<"ismap", boolean>;
/**
 * The itemprop attribute
  **/
export declare function ItemProp(value: string): HtmlAttr<"itemprop", string>;
/**
 * Specifies the type of key generated.
  **/
export declare function KeyType(value: string): HtmlAttr<"keytype", string>;
/**
 * Specifies the kind of text track.
  **/
export declare function Kind(value: string): HtmlAttr<"kind", string>;
/**
 * Specifies a user-readable title of the element.
  **/
export declare function Label_attr(value: string): HtmlAttr<"label", string>;
/**
 * Defines the language used in the element.
  **/
export declare function Lang(value: string): HtmlAttr<"lang", string>;
/**
 * Defines the script language used in the element.
  **/
export declare function Language(value: string): HtmlAttr<"language", string>;
/**
 * Identifies a list of pre-defined options to suggest to the user.
  **/
export declare function List(value: string | HTMLDataListElement): HtmlAttr<"list", string>;
/**
 * Indicates whether the media should start playing from the start when it's finished.
  **/
export declare function Loop(value: boolean): HtmlAttr<"loop", boolean>;
/**
 * Indicates the upper bound of the lower range.
  **/
export declare function Low(value: number): HtmlAttr<"low", number>;
/**
 * Indicates the maximum value allowed.
  **/
export declare function Max(value: number): HtmlAttr<"max", number>;
/**
 * Defines the maximum number of characters allowed in the element.
  **/
export declare function MaxLength(value: number): HtmlAttr<"maxlength", number>;
/**
 * Defines the minimum number of characters allowed in the element.
  **/
export declare function MinLength(value: number): HtmlAttr<"minlength", number>;
/**
 * Specifies a hint of the media for which the linked resource was designed.
  **/
export declare function Media(value: string): HtmlAttr<"media", string>;
/**
 * Defines which HTTP method to use when submitting the form. Can be GET (default) or POST.
  **/
export declare function Method(value: string): HtmlAttr<"method", string>;
/**
 * Indicates the minimum value allowed.
  **/
export declare function Min(value: number): HtmlAttr<"min", number>;
/**
 * Indicates whether multiple values can be entered in an input of the type email or file.
  **/
export declare function Multiple(value: boolean): HtmlAttr<"multiple", boolean>;
/**
 * Indicates whether the audio will be initially silenced on page load.
  **/
export declare function Muted(value: boolean): HtmlAttr<"muted", boolean>;
/**
 * Name of the element. For example used by the server to identify the fields in form submits.
  **/
export declare function Name(value: string): HtmlAttr<"name", string>;
/**
 * This attribute indicates that the form shouldn't be validated when submitted.
  **/
export declare function NoValidate(value: boolean): HtmlAttr<"novalidate", boolean>;
/**
 * Indicates whether the details will be shown on page load.
  **/
export declare function Open(value: boolean): HtmlAttr<"open", boolean>;
/**
 * Indicates the optimal numeric value.
  **/
export declare function Optimum(value: number): HtmlAttr<"optimum", number>;
/**
 * Defines a regular expression which the element's value will be validated against.
  **/
export declare function Pattern(value: string): HtmlAttr<"pattern", string>;
/**
 * The ping attribute specifies a space-separated list of URLs to be notified if a user follows the hyperlink.
  **/
export declare function Ping(value: string): HtmlAttr<"ping", string>;
/**
 * Provides a hint to the user of what can be entered in the field.
  **/
export declare function PlaceHolder(value: string): HtmlAttr<"placeholder", string>;
/**
 * A URL indicating a poster frame to show until the user plays or seeks.
  **/
export declare function Poster(value: string): HtmlAttr<"poster", string>;
/**
 * Indicates whether the whole resource, parts of it or nothing should be preloaded.
  **/
export declare function Preload(value: boolean | string): HtmlAttr<"preload", string | boolean>;
/**
 * Attempts to find an element under a given element.
 * @param root
 * @param selector
 */
export declare function Query(root: ParentNode, selector: string): HtmlAttr<"query", HTMLElement>;
/**
 * Attempts to find an element in the document.
 * @param selector
 */
export declare function Query(selector: string): HtmlAttr<"query", HTMLElement>;
export declare function QueryAll<T extends Element>(root: ParentNode, selector: string): T[];
/**
 * Attempts to find an element in the document.
 * @param selector
 */
export declare function QueryAll<T extends Element = HTMLElement>(selector: string): T[];
/**
 * Indicates whether the element can be edited.
  **/
export declare function ReadOnly(value: boolean): HtmlAttr<"readonly", boolean>;
/**
 * The radiogroup attribute
  **/
export declare function RadioGroup(value: string): HtmlAttr<"radiogroup", string>;
/**
 * Specifies which referrer is sent when fetching the resource.
  **/
export declare function ReferrerPolicy(value: string): HtmlAttr<"referrerpolicy", string>;
/**
 * Specifies the relationship of the target object to the link object.
  **/
export declare function Rel(value: string): HtmlAttr<"rel", string>;
/**
 * Indicates whether this element is required to fill out or not.
  **/
export declare function Required(value: boolean): HtmlAttr<"required", boolean>;
/**
 * Indicates whether the list should be displayed in a descending order instead of a ascending.
  **/
export declare function Reversed(value: boolean): HtmlAttr<"reversed", boolean>;
/**
 * Defines the number of rows in a text area.
  **/
export declare function Role(value: string): HtmlAttr<"role", string>;
/**
 * The rows attribute
  **/
export declare function Rows(value: number): HtmlAttr<"rows", number>;
/**
 * Defines the number of rows a table cell should span over.
  **/
export declare function RowSpan(value: number): HtmlAttr<"rowspan", number>;
/**
 * Stops a document loaded in an iframe from using certain features (such as submitting forms or opening new windows).
  **/
export declare function Sandbox(value: string): HtmlAttr<"sandbox", string>;
/**
 * Defines the cells that the header test (defined in the th element) relates to.
  **/
export declare function Scope(value: string): HtmlAttr<"scope", string>;
/**
 * The scoped attribute for `style` tags.
  **/
export declare function Scoped(value: boolean): HtmlAttr<"scoped", boolean>;
/**
 * The scrolling attribute for `iframe` tags.
  **/
export declare function Scrolling(value: boolean): HtmlAttr<"scrolling", string>;
/**
 * Defines an `option` tag which will be selected on page load.
  **/
export declare function Selected(value: boolean): HtmlAttr<"selected", boolean>;
/**
 * The shape attribute for `a` and `area` tags.
  **/
export declare function Shape(value: string): HtmlAttr<"shape", string>;
/**
 * Defines the width of the element (in pixels). If the element's type attribute is text or password then it's the number of characters.
  **/
export declare function Size(value: number): HtmlAttr<"size", number>;
/**
 * Assigns a slot in a shadow DOM shadow tree to an element.
  **/
export declare function Slot_attr(value: string): HtmlAttr<"slot", string>;
/**
 * The sizes attribute
  **/
export declare function Sizes(value: string): HtmlAttr<"sizes", string>;
/**
 * The span attribute
  **/
export declare function Span_attr(value: string): HtmlAttr<"span", string>;
/**
 * Indicates whether spell checking is allowed for the element.
  **/
export declare function SpellCheck(value: boolean): HtmlAttr<"spellcheck", boolean>;
/**
 * The URL of the embeddable content.
  **/
export declare function Src(value: string | URL): HtmlAttr<"src", string>;
/**
 * The srcdoc attribute
  **/
export declare function SrcDoc(value: string): HtmlAttr<"srcdoc", string>;
/**
 * The srclang attribute
  **/
export declare function SrcLang(value: string): HtmlAttr<"srclang", string>;
/**
 * A MediaStream object to use as a source for an HTML video or audio element
  **/
export declare function SrcObject(value: MediaProvider): HtmlAttr<"srcObject", MediaProvider>;
/**
 * One or more responsive image candidates.
  **/
export declare function SrcSet(value: string): HtmlAttr<"srcset", string>;
/**
 * Defines the first number if other than 1.
  **/
export declare function Start(value: number): HtmlAttr<"start", number>;
/**
 * The step attribute
  **/
export declare function Step(value: number): HtmlAttr<"step", number>;
/**
 * The summary attribute
  **/
export declare function Summary_attr(value: string): HtmlAttr<"summary", string>;
/**
 * Overrides the browser's default tab order and follows the one specified instead.
  **/
export declare function TabIndex(value: number): HtmlAttr<"tabindex", number>;
/**
 * Text to be displayed in a tooltip when hovering over the element.
  **/
export declare function Title_attr(value: string): HtmlAttr<"title", string>;
/**
 * The target attribute
  **/
export declare function Target(value: string): HtmlAttr<"target", string>;
/**
 * Specify whether an element’s attribute values and the values of its Text node children are to be translated when the page is localized, or whether to leave them unchanged.
  **/
export declare function Translate(value: boolean): HtmlAttr<"translate", boolean>;
/**
 * Defines the type of the element.
  **/
export declare function Type(value: string | MediaType): HtmlAttr<"type", string>;
/**
 * Defines a default value which will be displayed in the element on page load.
  **/
export declare function Value(value: string | number): HtmlAttr<"value", string | number>;
/**
 * Defines a default value which will be displayed in the element on page load.
  **/
export declare function ValueAsNumber(value: number): HtmlAttr<"valueAsNumber", number>;
/**
 * Defines a default value which will be displayed in the element on page load.
  **/
export declare function ValueAsDate(value: Date): HtmlAttr<"valueAsDate", Date>;
/**
 * setting the volume at which a media element plays.
  **/
export declare function Volume(value: number): HtmlAttr<"volume", number>;
/**
 * The usemap attribute
  **/
export declare function UseMap(value: boolean): HtmlAttr<"usemap", boolean>;
/**
 * For the elements listed here, this establishes the element's width.
  **/
export declare function Width(value: number | string): HtmlAttr<"width", string | number>;
/**
 * Indicates whether the text should be wrapped.
  **/
export type HTMLTextAreaWrapValue = "hard" | "soft" | "off";
export declare function Wrap(value: HTMLTextAreaWrapValue): HtmlAttr<"wrap", HTMLTextAreaWrapValue>;
//# sourceMappingURL=attrs.d.ts.map