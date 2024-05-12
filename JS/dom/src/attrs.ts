import { IToStringable, identity, stringRandom, toUpper, unpackURL } from "@juniper-lib/util";
import { AbstractAppliable } from "./AbstractAppliable";


export class HtmlAttr<AttributeT extends string = string, ValueT = IToStringable> extends AbstractAppliable<Node> {
    #name: AttributeT;
    get name() { return this.#name; }

    #value: ValueT;
    get value() { return this.#value; }

    #requiresSetAttribute;
    #allowableTags;

    /**
     * Validates the tag type before assigning an attribute.
     * @param name the name of the attribute, for debugging purposes.
     * @param value the value to assign to the attribute
     * @param requiresSetAttribute set to true to make the attribute value get set via the setAttribute method, rather than directly as a field
     * @param allowableTags
     */
    constructor(name: AttributeT, value: ValueT, requiresSetAttribute = false, ...allowableTags: string[]) {
        super();

        this.#name = name;
        this.#value = value;
        this.#requiresSetAttribute = requiresSetAttribute;
        this.#allowableTags = allowableTags.map(toUpper);
    }

    override apply(tag: Node) {
        if (tag instanceof Element) {
            if (this.#allowableTags.length > 0) {
                const tagName = (tag.hasAttribute("is")
                    ? tag.getAttribute("is")
                    : tag.tagName).toUpperCase();

                if (this.#allowableTags.indexOf(tagName) < 0) {
                    console.warn(`The "${this.name}" attribute is not assignable to "${tagName}" tags. It is valid for [${this.#allowableTags.join()}].`);
                }
            }

            if (this.value === true) {
                tag.setAttribute(this.name, "");
            }
            else if (this.value === false) {
                tag.removeAttribute(this.name);
            }
            else if (this.#requiresSetAttribute) {
                tag.setAttribute(this.name, this.value?.toString());
            }
            else {
                (tag as any)[this.name] = this.value;
            }
        }
    }
}



/**********************************
 * ATTRIBUTES
 *********************************/


/**
 * a list of types the server accepts, typically a file type.
 * @param value - the value to set on the attribute.
 **/
export function Accept(value: string) {
    return new HtmlAttr(
        "accept",
        value,
        false,
        "form",
        "input"
    );
}

/**
 * The accessKey attribute
 **/
export function AccessKey(value: string) {
    return new HtmlAttr(
        "accessKey",
        value,
        false,
        "button",
        "input"
    );
}

/**
 * specifying the horizontal alignment of the element.
 **/
export function Align(value: string) {
    return new HtmlAttr(
        "align",
        value,
        false,
        "applet",
        "caption",
        "col",
        "colgroup",
        "hr",
        "iframe",
        "img",
        "table",
        "tbody",
        "td",
        "tfoot",
        "th",
        "thead",
        "tr"
    );
}

/**
 * Specifies a feature-policy for the iframe.
 **/
export function Allow(value: string) {
    return new HtmlAttr(
        "allow",
        value,
        false,
        "iframe"
    );
}

/**
 * Whether or not to allow an IFrame to open full screen elements.
 **/
export function AllowFullscreen(value: boolean) {
    return new HtmlAttr(
        "allowfullscreen",
        value,
        false,
        "iframe"
    );
}

/**
 * Alternative text in case an image can't be displayed.
 **/
export function Alt(value: string) {
    return new HtmlAttr(
        "alt",
        value,
        false,
        "applet",
        "area",
        "img",
        "input"
    );
}

/**
 * Identifies the currently active element when DOM focus is on a composite widget, textbox, group, or application.
 **/
export function AriaActiveDescendant(value: string) {
    return new HtmlAttr(
        "ariaActiveDescendant",
        value,
        false
    );
}

/**
 * Indicates whether assistive technologies will present all, or only parts of, the changed region based on the change notifications defined by the aria-relevant attribute.
 **/
export function AriaAtomic(value: boolean) {
    return new HtmlAttr(
        "ariaAtomic",
        value,
        false
    );
}

/**
 * Indicates whether inputting text could trigger display of one or more predictions of the user's intended value for an input and specifies how predictions would be presented if they are made.
 **/
export function AriaAutoComplete(value: string) {
    return new HtmlAttr(
        "ariaAutoComplete",
        value,
        false
    );
}

/**
 * Indicates an element is being modified and that assistive technologies MAY want to wait until the modifications are complete before exposing them to the user.
 **/
export function AriaBusy(value: boolean) {
    return new HtmlAttr(
        "ariaBusy",
        value,
        false
    );
}

/**
 * Indicates the current "checked" state of checkboxes, radio buttons, and other widgets. See related aria-pressed and aria-selected.
 **/
export function AriaChecked(value: boolean) {
    return new HtmlAttr(
        "ariaChecked",
        value,
        false
    );
}

/**
 * Defines the total number of columns in a table, grid, or treegrid. See related aria-colindex.
 **/
export function AriaColCount(value: number) {
    return new HtmlAttr(
        "ariaColCount",
        value,
        false
    );
}

/**
 * Defines an element's column index or position with respect to the total number of columns within a table, grid, or treegrid. See related aria-colcount and aria-colspan.
 **/
export function AriaColIndex(value: number) {
    return new HtmlAttr(
        "ariaColIndex",
        value,
        false
    );
}

/**
 * Defines the number of columns spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-colindex and aria-rowspan.
 **/
export function AriaColSpan(value: number) {
    return new HtmlAttr(
        "ariaColSpan",
        value,
        false
    );
}

/**
 * Identifies the element (or elements) whose contents or presence are controlled by the current element. See related aria-owns.
 **/
export function AriaControls(value: string) {
    return new HtmlAttr(
        "aria-controls",
        value,
        true
    );
}

/**
 * Indicates the element that represents the current item within a container or set of related elements.
 **/
export function AriaCurrent(value: string) {
    return new HtmlAttr(
        "ariaCurrent",
        value,
        false
    );
}

/**
 * Identifies the element (or elements) that describes the object. See related aria-labelledby.
 **/
export function AriaDescribedBy(value: string) {
    return new HtmlAttr(
        "ariaDescribedBy",
        value,
        false
    );
}

/**
 * Identifies the element that provides a detailed, extended description for the object. See related aria-describedby.
 **/
export function AriaDetails(value: string) {
    return new HtmlAttr(
        "ariaDetails",
        value,
        false
    );
}

/**
 * Indicates that the element is perceivable but disabled, so it is not editable or otherwise operable. See related aria-hidden and aria-readonly.
 **/
export function AriaDisabled(value: boolean) {
    return new HtmlAttr(
        "ariaDisabled",
        value,
        false
    );
}

/**
 * Identifies the element that provides an error message for the object. See related aria-invalid and aria-describedby.
 **/
export function AriaErrorMessage(value: string) {
    return new HtmlAttr(
        "ariaErrorMessage",
        value,
        false
    );
}

/**
 * Indicates whether the element, or another grouping element it controls, is currently expanded or collapsed.
 **/
export function AriaExpanded(value: boolean) {
    return new HtmlAttr(
        "ariaExpanded",
        value,
        false
    );
}

/**
 * Identifies the next element (or elements) in an alternate reading order of content which, at the user's discretion, allows assistive technology to override the general default of reading in document source order.
 **/
export function AriaFlowTo(value: string) {
    return new HtmlAttr(
        "ariaFlowTo",
        value,
        false
    );
}

/**
 * Indicates the availability and type of interactive popup element, such as menu or dialog, that can be triggered by an element.
 **/
export function AriaHasPopup(value: string) {
    return new HtmlAttr(
        "ariaHasPopup",
        value,
        false
    );
}

/**
 * Indicates whether the element is exposed to an accessibility API. See related aria-disabled.
 **/
export function AriaHidden(value: boolean) {
    return new HtmlAttr(
        "ariaHidden",
        value,
        false
    );
}

/**
 * Indicates the entered value does not conform to the format expected by the application. See related aria-error_message.
 **/
export function AriaInvalid(value: string) {
    return new HtmlAttr(
        "ariaInvalid",
        value,
        false
    );
}

/**
 * Indicates keyboard shortcuts that an author has implemented to activate or give focus to an element.
 **/
export function AriaKeyShortcuts(value: string) {
    return new HtmlAttr(
        "ariaKeyShortcuts",
        value,
        false
    );
}

/**
 * Defines a string value that labels the current element. See related aria-labelledby.
 **/
export function AriaLabel(value: string) {
    return new HtmlAttr(
        "ariaLabel",
        value,
        false
    );
}

/**
 * Identifies the element (or elements) that labels the current element. See related aria-describedby.
 **/
export function AriaLabelledBy(value: string) {
    return new HtmlAttr(
        "ariaLabelledBy",
        value,
        false
    );
}

/**
 * Defines the hierarchical level of an element within a structure.
 **/
export function AriaLevel(value: number) {
    return new HtmlAttr(
        "ariaLevel",
        value,
        false
    );
}

/**
 * Indicates that an element will be updated, and describes the types of updates the user agents, assistive technologies, and user can expect from the live region.
 **/
export function AriaLive(value: string) {
    return new HtmlAttr(
        "ariaLive",
        value,
        false
    );
}

/**
 * Indicates whether an element is modal when displayed
 **/
export function AriaModal(value: boolean) {
    return new HtmlAttr(
        "ariaModal",
        value,
        false
    );
}

/**
 * Indicates whether a text box accepts multiple lines of input or only a single line.
 **/
export function AriaMultiline(value: boolean) {
    return new HtmlAttr(
        "ariaMultiline",
        value,
        false
    );
}

/**
 * Indicates that the user may select more than one item from the current selectable descendants.
 **/
export function AriaMultiSelectable(value: boolean) {
    return new HtmlAttr(
        "ariaMultiSelectable",
        value,
        false
    );
}

/**
 * Indicates that the user may select more than one item from the current selectable descendants.
 **/
export function AriaOrientation(value: string) {
    return new HtmlAttr(
        "ariaOrientation",
        value,
        false
    );
}

/**
 * Identifies an element (or elements) in order to define a visual, functional, or contextual parent/child relationship between DOM elements where the DOM hierarchy cannot be used to represent the relationship. See related aria-controls.
 **/
export function AriaOwns(value: string) {
    return new HtmlAttr(
        "ariaOwns",
        value,
        false
    );
}

/**
 * Defines a short hint (a word or short phrase) intended to aid the user with data entry when the control has no value. A hint could be a sample value or a brief description of the expected format.
 **/
export function AriaPlaceholder(value: string) {
    return new HtmlAttr(
        "ariaPlaceholder",
        value,
        false
    );
}

/**
 * Defines an element's number or position in the current set of list items or treeitems. Not required if all elements in the set are present in the DOM. See related aria-setsize.
 **/
export function AriaPosInSet(value: number) {
    return new HtmlAttr(
        "ariaPosInSet",
        value,
        false
    );
}

/**
 * Indicates the current "pressed" state of toggle buttons. See related aria-checked and aria-selected.
 **/
export function AriaPressed(value: boolean) {
    return new HtmlAttr(
        "ariaPressed",
        value,
        false
    );
}

/**
 * Indicates that the element is not editable, but is otherwise operable. See related aria-disabled.
 **/
export function AriaReadOnly(value: boolean) {
    return new HtmlAttr(
        "ariaReadOnly",
        value,
        false
    );
}

/**
 * Indicates what notifications the user agent will trigger when the accessibility tree within a live region is modified. See related aria-atomic.
 **/
export function AriaRelevant(value: string) {
    return new HtmlAttr(
        "ariaRelevant",
        value,
        false
    );
}

/**
 * Indicates that user input is required on the element before a form may be submitted.
 **/
export function AriaRequired(value: boolean) {
    return new HtmlAttr(
        "ariaRequired",
        value,
        false
    );
}

/**
 * Defines a human-readable, author-localized description for the role of an element.
 **/
export function AriaRoleDescription(value: string) {
    return new HtmlAttr(
        "ariaRoleDescription",
        value,
        false
    );
}

/**
 * Defines the total number of rows in a table, grid, or treegrid. See related aria-rowindex.
 **/
export function AriaRowCount(value: number) {
    return new HtmlAttr(
        "ariaRowCount",
        value,
        false
    );
}

/**
 * Defines an element's row index or position with respect to the total number of rows within a table, grid, or treegrid. See related aria-rowcount and aria-rowspan.
 **/
export function AriaRowIndex(value: number) {
    return new HtmlAttr(
        "ariaRowIndex",
        value,
        false
    );
}

/**
 Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-rowindex and aria-colspan.
 **/
export function AriaRowSpan(value: number) {
    return new HtmlAttr(
        "ariaRowSpan",
        value,
        false
    );
}

/**
 * Indicates the current "selected" state of various widgets. See related aria-checked and aria-pressed.
 **/
export function AriaSelected(value: boolean) {
    return new HtmlAttr(
        "ariaSelected",
        value,
        false
    );
}

/**
 * Defines the number of items in the current set of list items or tree items. Not required if all elements in the set are present in the DOM. See related aria-posinset.
 **/
export function AriaSetSize(value: number) {
    return new HtmlAttr(
        "ariaSetSize",
        value,
        false
    );
}

/**
 * Indicates if items in a table or grid are sorted in ascending or descending order.
 **/
export function AriaSort(value: string) {
    return new HtmlAttr(
        "ariaSort",
        value,
        false
    );
}

/**
 * Defines the maximum allowed value for a range widget.
 **/
export function AriaValueMax(value: number) {
    return new HtmlAttr(
        "ariaValueMax",
        value,
        false
    );
}

/**
 * Defines the minimum allowed value for a range widget.
 **/
export function AriaValueMin(value: number) {
    return new HtmlAttr(
        "ariaValueMin",
        value,
        false
    );
}

/**
 * Defines the current value for a range widget. See related aria-valuetext.
 **/
export function AriaValueNow(value: number) {
    return new HtmlAttr(
        "ariaValueNow",
        value,
        false
    );
}

/**
 * Defines the human readable text alternative of aria-valuenow for a range widget.
 **/
export function AriaValueText(value: string) {
    return new HtmlAttr(
        "ariaValueText",
        value,
        false
    );
}

/**
 * Executes the script asynchronously.
 **/
export function Async(value: boolean) {
    return new HtmlAttr(
        "async",
        value,
        false,
        "script"
    );
}

/**
 * Sets whether input is automatically capitalized when entered by user
 **/
export function AutoCapitalize(value: boolean) {
    return new HtmlAttr(
        "autocapitalize",
        value,
        false
    );
}

/**
 * Indicates whether controls in this form can by default have their values automatically completed by the browser.
 **/
export function AutoComplete(value: HTMLAutoCompleteAttributeValue) {
    return new HtmlAttr(
        "autocomplete",
        value ? "on" : "off",
        false,
        "form",
        "input",
        "select",
        "textarea"
    );
}

/**
 * The element should be automatically focused after the page loaded.
 **/
export function AutoFocus(value: boolean) {
    return new HtmlAttr(
        "autofocus",
        value,
        false,
        "button",
        "input",
        "keygen",
        "select",
        "textarea"
    );
}

/**
 * The audio or video should play as soon as possible.
 **/
export function AutoPlay(value: boolean) {
    return new HtmlAttr(
        "autoplay",
        value,
        false,
        "audio",
        "video"
    );
}

/**
 * Contains the time range of already buffered media.
 **/
export function Buffered(value: boolean) {
    return new HtmlAttr(
        "buffered",
        value,
        false,
        "audio",
        "video"
    );
}

/**
 * From the HTML Media Capture
 **/
export function Capture(value: boolean) {
    return new HtmlAttr(
        "capture",
        value,
        false,
        "input"
    );
}

/**
 * Declares the character encoding of the page or script.
 **/
export function CharSet(value: string) {
    return new HtmlAttr(
        "charset",
        value,
        false,
        "meta",
        "script"
    );
}

/**
 * Indicates whether the element should be checked on page load.
 **/
export function Checked(value: boolean) {
    return new HtmlAttr(
        "checked",
        value,
        false,
        "command",
        "input"
    );
}

/**
 * Contains a URI which points to the source of the quote or change.
 **/
export function CiteURI(value: string) {
    return new HtmlAttr(
        "cite",
        value,
        false,
        "blockquote",
        "del",
        "ins",
        "q"
    );
}

/**
 * Often used with CSS to style elements with common properties.
 **/
export function ClassName(value: string) {
    return new HtmlAttr(
        "className",
        value,
        false
    );
}

class HtmlClassListAttr extends HtmlAttr {
    constructor(classes: string[]) {
        super("classList", classes.filter(identity), false);
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
export function ClassList(...classes: string[]): HtmlAttr {
    return new HtmlClassListAttr(classes);
}

/**
 * Specifies the URL of the applet's class file to be loaded and executed.
 **/
export function CodeURI(value: string) {
    return new HtmlAttr(
        "code",
        value,
        false,
        "applet"
    );
}

/**
 * This attribute gives the absolute or relative URL of the directory where applets' .class files referenced by the code attribute are stored.
 **/
export function CodeBase(value: string) {
    return new HtmlAttr(
        "codebase",
        value,
        false,
        "applet"
    );
}

/**
 * Defines the number of columns in a textarea.
 **/
export function Cols(value: number) {
    return new HtmlAttr(
        "cols",
        value,
        false,
        "textarea"
    );
}

/**
 * The colspan attribute defines the number of columns a cell should span.
 **/
export function ColSpan(value: number) {
    return new HtmlAttr(
        "colSpan",
        value.toFixed(0),
        false,
        "td",
        "th"
    );
}

/**
 * A value associated with http-equiv or name depending on the context.
 **/
export function Content(value: string) {
    return new HtmlAttr(
        "content",
        value,
        false,
        "meta"
    );
}

/**
 * Indicates whether the element's content is editable.
 **/
export function ContentEditable(value: boolean) {
    return new HtmlAttr(
        "contenteditable",
        value,
        false
    );
}

/**
 * Defines the ID of a <menu> element which will serve as the element's context menu.
 **/
export function ContextMenu(value: string) {
    return new HtmlAttr(
        "contextmenu",
        value,
        false
    );
}

/**
 * Indicates whether the browser should show playback controls to the user.
 **/
export function Controls(value: boolean) {
    return new HtmlAttr(
        "controls",
        value,
        false,
        "audio",
        "video"
    );
}

/**
 * A set of values specifying the coordinates of the hot-spot region.
 **/
export function Coords(value: string) {
    return new HtmlAttr(
        "coords",
        value,
        false,
        "area"
    );
}

/**
 * How the element handles cross-origin requests
 * @param {} value
 **/
export function CrossOrigin(value: HTMLCrossOriginValue) {
    return new HtmlAttr(
        "crossorigin",
        value,
        false,
        "audio",
        "img",
        "link",
        "script",
        "video"
    );
}

/**
 * Specifies the Content Security Policy that an embedded document must agree to enforce upon itself.
 **/
export function CSP(value: string) {
    return new HtmlAttr(
        "csp",
        value,
        false,
        "iframe"
    );
}

/**
 * Lets you attach custom attributes to an HTML element.
 **/
export function CustomData(name: string, value: { toString(): string }) {
    name = "data-" + name.toLowerCase();
    return new HtmlAttr(
        name,
        value,
        true
    );
}

/**
 * Indicates the date and time associated with the element.
 **/
export function DateTime(value: Date) {
    return new HtmlAttr(
        "dateTime",
        value?.toISOString(),
        false,
        "del",
        "ins",
        "time"
    );
}

/**
 * Indicates the preferred method to decode the image.
 **/
export function Decoding(value: string) {
    return new HtmlAttr(
        "decoding",
        value,
        false,
        "img"
    );
}

/**
 * Indicates that the track should be enabled unless the user's preferences indicate something different.
 **/
export function Default(value: boolean | string) {
    return new HtmlAttr(
        "default",
        value,
        false,
        "track"
    );
}

/**
 * Indicates that the script should be executed after the page has been parsed.
 **/
export function Defer(value: boolean) {
    return new HtmlAttr(
        "defer",
        value,
        false,
        "script"
    );
}

/**
 * Defines the text direction. Allowed values are ltr (Left-To-Right) or rtl (Right-To-Left)
 **/
export function Dir(value: string) {
    return new HtmlAttr(
        "dir",
        value,
        false
    );
}

/**
 * Indicates whether the user can interact with the element.
 **/
export function Disabled(value: boolean) {
    return new HtmlAttr(
        "disabled",
        value,
        false,
        "button",
        "command",
        "fieldset",
        "input",
        "keygen",
        "optgroup",
        "option",
        "select",
        "textarea",
        "typed-select",
        "typed-input"
    );
}

/**
 * The name to provide a POSTed FormData field when submitting an Input or TextArea's input direction.
 **/
export function DirName(value: string) {
    return new HtmlAttr(
        "dirname",
        value,
        false,
        "input",
        "textarea"
    );
}

/**
 * Indicates that the hyperlink is to be used for downloading a resource by giving the file a name.
 * @param name the name of the file to download 
 */
export function Download(name: string) {
    return new HtmlAttr(
        "download",
        name,
        false,
        "a",
        "area"
    );
}

/**
 * Defines whether the element can be dragged.
 **/
export function Draggable(value: boolean) {
    return new HtmlAttr(
        "draggable",
        value,
        false
    );
}

/**
 * Indicates that the element accepts the dropping of content onto it.
 **/
export function DropZone(value: string) {
    return new HtmlAttr(
        "dropzone",
        value,
        false
    );
}

/**
 * Defines the content type of the form data when the method is POST.
 **/
export function EncType(value: string) {
    return new HtmlAttr(
        "enctype",
        value,
        false,
        "form"
    );
}

/**
 * The enterkeyhint specifies what action label (or icon) to present for the enter key on virtual keyboards. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
 **/
export function EnterKeyHint(value: string) {
    return new HtmlAttr(
        "enterkeyhint",
        value,
        false,
        "textarea"
    );
}

/**
 * Describes elements which belongs to this one.
 **/
export function For(value: string) {
    return new HtmlAttr(
        "for",
        value,
        true,
        "label",
        "output"
    );
}

/**
 * Indicates the form that is the owner of the element. 
 * 
 * This function needs the "Tag" postfix to differentiate it from the HTML attribute of the same name.
 **/
export function FormAttr(value: string) {
    return new HtmlAttr(
        "form",
        value,
        true,
        "button",
        "fieldset",
        "input",
        "input-date-range",
        "keygen",
        "label",
        "meter",
        "object",
        "output",
        "progress",
        "select",
        "textarea"
    );
}

/**
 * Indicates the action of the element, overriding the action defined in the <form>.
 **/
export function FormAction(value: string) {
    return new HtmlAttr(
        "formaction",
        value,
        false,
        "button",
        "input"
    );
}

/**
 * If the button/input is a submit button (type="submit"
), this attribute sets the encoding type to use during form submission. If this attribute is specified, it overrides the enctype attribute of the button's form owner.
 **/
export function FormEncType(value: string) {
    return new HtmlAttr(
        "formenctype",
        value,
        false,
        "button",
        "input"
    );
}

/**
 * If the button/input is a submit button (type="submit"
), this attribute sets the submission method to use during form submission (GET, POST, etc.). If this attribute is specified, it overrides the method attribute of the button's form owner.
 **/
export function FormMethod(value: string) {
    return new HtmlAttr(
        "formmethod",
        value,
        false,
        "button",
        "input"
    );
}

/**
 * If the button/input is a submit button (type="submit"
), this boolean attribute specifies that the form is not to be validated when it is submitted. If this attribute is specified, it overrides the novalidate attribute of the button's form owner.
 **/
export function FormNoValidate(value: boolean) {
    return new HtmlAttr(
        "formnovalidate",
        value,
        false,
        "button",
        "input"
    );
}

/**
 * If the button/input is a submit button (type="submit"
), this attribute specifies the browsing context (for example, tab, window, or inline frame) in which to display the response that is received after submitting the form. If this attribute is specified, it overrides the target attribute of the button's form owner.
 **/
export function FormTarget(value: string) {
    return new HtmlAttr(
        "formtarget",
        value,
        false,
        "button",
        "input"
    );
}

/**
 * Width of the border to put around an `iframe` tag.
 * @param value
 **/
export function FrameBorder(value: string | number | boolean) {
    if (typeof value === "boolean") {
        value = value ? "yes" : "no";
    }

    return new HtmlAttr(
        "frameborder",
        value,
        false,
        "iframe"
    );
}

/**
 * IDs of the <th> elements which applies to this element.
 **/
export function Headers(value: string) {
    return new HtmlAttr(
        "headers",
        value,
        false,
        "td",
        "th"
    );
}

/**
 * Specifies the height of elements listed here. For all other elements, use the CSS height property.
 * @param value
 **/
export function Height(value: number | string) {
    return new HtmlAttr(
        "height",
        value,
        false,
        "canvas",
        "embed",
        "iframe",
        "img",
        "input",
        "object",
        "video"
    );
}

/**
 * Prevents rendering of given element, while keeping child elements, e.g. script elements, active.
 **/
export function Hidden(value: boolean) {
    return new HtmlAttr(
        "hidden",
        value,
        false
    );
}

/**
 * Indicates the lower bound of the upper range.
 **/
export function High(value: number) {
    return new HtmlAttr(
        "high",
        value,
        false,
        "meter"
    );
}

/**
 * The URL of a linked resource.
 * @param href the hyper-reference to assign 
 */
export function HRef(href: string | URL) {
    return new HtmlAttr(
        "href",
        unpackURL(href),
        false,
        "a",
        "area",
        "base",
        "link"
    );
}

/**
 * Specifies the language of the linked resource.
 **/
export function HRefLang(value: string) {
    return new HtmlAttr(
        "hreflang",
        value,
        false,
        "a",
        "area",
        "link"
    );
}

/**
 * Defines a pragma directive.
 **/
export function HttpEquiv(value: string) {
    return new HtmlAttr(
        "httpEquiv",
        value,
        false,
        "meta"
    );
}

/**
 * Specifies a picture which represents the command.
 **/
export function Icon(value: string) {
    return new HtmlAttr(
        "icon",
        value,
        false,
        "command"
    );
}

export class HtmlIDAttr extends HtmlAttr {
    constructor(value: string, public readonly required: boolean) {
        super("id", value, false);
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
    return new HtmlAttr(
        "importance",
        value,
        false,
        "iframe",
        "img",
        "link",
        "script"
    );
}

/**
 * Sets the innerHTML property of an element.
 **/
export function InnerHTML(html: string) {
    return new HtmlAttr(
        "innerHTML",
        html,
        false
    );
}

/**
 * Provides a hint as to the type of data that might be entered by the user while editing the element or its contents. The attribute can be used with form controls (such as the value of textarea elements), or in elements in an editing host (e.g., using contenteditable attribute).
 **/
export function InputMode(value: string) {
    return new HtmlAttr(
        "inputmode",
        value,
        false,
        "textarea"
    );
}

/**
 * Specifies a sub-resource Integrity value that allows browsers to verify what they fetch.
 **/
export function Integrity(value: string) {
    return new HtmlAttr(
        "integrity",
        value,
        false,
        "link",
        "script"
    );
}

/**
 * This attribute tells the browser to ignore the actual intrinsic size of the image and pretend it’s the size specified in the attribute.
 **/
export function IntrinsicSize(value: string) {
    return new HtmlAttr(
        "intrinsicsize",
        value,
        false,
        "img"
    );
}

export class HtmlIsAttr extends HtmlAttr {
    constructor(value: string) {
        super("is", value, true);
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
    return new HtmlAttr(
        "ismap",
        value,
        false,
        "img"
    );
}

/**
 * The itemprop attribute
 **/
export function ItemProp(value: string) {
    return new HtmlAttr(
        "itemprop",
        value,
        false
    );
}

/**
 * Specifies the type of key generated.
 **/
export function KeyType(value: string) {
    return new HtmlAttr(
        "keytype",
        value,
        false,
        "keygen"
    );
}

/**
 * Specifies the kind of text track.
 **/
export function Kind(value: string) {
    return new HtmlAttr(
        "kind",
        value,
        false,
        "track"
    );
}

/**
 * Specifies a user-readable title of the element.
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function LabelAttr(value: string) {
    return new HtmlAttr(
        "label",
        value,
        false,
        "optgroup",
        "option",
        "track"
    );
}

/**
 * Defines the language used in the element.
 **/
export function Lang(value: string) {
    return new HtmlAttr(
        "lang",
        value,
        false
    );
}

/**
 * Defines the script language used in the element.
 **/
export function Language(value: string) {
    return new HtmlAttr(
        "language",
        value,
        false,
        "script"
    );
}

/**
 * Identifies a list of pre-defined options to suggest to the user.
 **/
export function List(value: string | HTMLDataListElement) {
    if (value instanceof HTMLDataListElement) {
        if (!value.id) {
            value.id = stringRandom(12);
        }

        if (!value.isConnected) {
            document.body.append(value);
        }

        value = value.id;
    }

    return new HtmlAttr(
        "list",
        value,
        true,
        "input",
        "typed-input"
    );
}

/**
 * Indicates whether the media should start playing from the start when it's finished.
 **/
export function Loop(value: boolean) {
    return new HtmlAttr(
        "loop",
        value,
        false,
        "audio",
        "bgsound",
        "marquee",
        "video"
    );
}

/**
 * Indicates the upper bound of the lower range.
 **/
export function Low(value: number) {
    return new HtmlAttr(
        "low",
        value,
        false,
        "meter"
    );
}

/**
 * Indicates the maximum value allowed.
 **/
export function Max(value: number) {
    return new HtmlAttr(
        "max",
        value,
        false,
        "input",
        "meter",
        "progress"
    );
}

/**
 * Defines the maximum number of characters allowed in the element.
 **/
export function MaxLength(value: number) {
    return new HtmlAttr(
        "maxlength",
        value,
        false,
        "input",
        "textarea"
    );
}

/**
 * Defines the minimum number of characters allowed in the element.
 **/
export function MinLength(value: number) {
    return new HtmlAttr(
        "minlength",
        value,
        false,
        "input",
        "textarea"
    );
}

/**
 * Specifies a hint of the media for which the linked resource was designed.
 **/
export function Media(value: string) {
    return new HtmlAttr(
        "media",
        value,
        false,
        "a",
        "area",
        "link",
        "source",
        "style"
    );
}

/**
 * Defines which HTTP method to use when submitting the form. Can be GET (default) or POST.
 **/
export function Method(value: string) {
    return new HtmlAttr(
        "method",
        value,
        false,
        "form"
    );
}

/**
 * Indicates the minimum value allowed.
 **/
export function Min(value: number) {
    return new HtmlAttr(
        "min",
        value,
        false,
        "input",
        "meter"
    );
}

/**
 * Indicates whether multiple values can be entered in an input of the type email or file.
 **/
export function Multiple(value: boolean) {
    return new HtmlAttr(
        "multiple",
        value,
        false,
        "input",
        "select"
    );
}

/**
 * Indicates whether the audio will be initially silenced on page load.
 **/
export function Muted(value: boolean) {
    return new HtmlAttr(
        "muted",
        value,
        false,
        "audio",
        "video"
    );
}

/**
 * Name of the element. For example used by the server to identify the fields in form submits.
 **/
export function Name(value: string) {
    return new HtmlAttr(
        "name",
        value,
        false,
        "button",
        "form",
        "fieldset",
        "iframe",
        "input",
        "keygen",
        "object",
        "output",
        "select",
        "slot",
        "textarea",
        "map",
        "meta",
        "param",
        "property-group"
    );
}

/**
 * This attribute indicates that the form shouldn't be validated when submitted.
 **/
export function NoValidate(value: boolean) {
    return new HtmlAttr(
        "novalidate",
        value,
        false,
        "form"
    );
}

/**
 * Indicates whether the details will be shown on page load.
 **/
export function Open(value: boolean) {
    return new HtmlAttr(
        "open",
        value,
        false,
        "details"
    );
}

/**
 * Indicates the optimal numeric value.
 **/
export function Optimum(value: number) {
    return new HtmlAttr(
        "optimum",
        value,
        false,
        "meter"
    );
}

/**
 * Defines a regular expression which the element's value will be validated against.
 **/
export function Pattern(value: string) {
    return new HtmlAttr(
        "pattern",
        value,
        false,
        "input"
    );
}

/**
 * The ping attribute specifies a space-separated list of URLs to be notified if a user follows the hyperlink.
 **/
export function Ping(value: string) {
    return new HtmlAttr(
        "ping",
        value,
        false,
        "a",
        "area"
    );
}

/**
 * Provides a hint to the user of what can be entered in the field.
 **/
export function PlaceHolder(value: string) {
    return new HtmlAttr(
        "placeholder",
        value,
        false,
        "input",
        "input-date-range",
        "select",
        "textarea",
        "typed-input",
        "typed-select"
    );
}

/**
 * Used to designate an element as a popover element.
 **/
export function Popover(value: boolean) {
    return new HtmlAttr(
        "popover",
        value,
        false
    );
}

/**
 * Turns a <button> element into a popover control button; takes the ID of the popover element to control as its value.
 **/
export function PopoverTarget(value: string) {
    return new HtmlAttr(
        "popovertarget",
        value,
        true,
        "button"
    );
}

/**
 * Turns a <button> element into a popover control button; takes the ID of the popover element to control as its value.
 **/
export function PopoverTargetElement(value: Element) {
    return new HtmlAttr(
        "popoverTargetElement",
        value,
        false,
        "button"
    );
}

export type PopoverTargetActionValue =
    | "hide"
    | "show"
    | "toggle";

/**
 * Specifies the action to be performed on a popover element being controlled by a control <button>. Possible values are:
 **/
export function PopoverTargetAction(value: PopoverTargetActionValue) {
    return new HtmlAttr(
        "popoverTargetAction",
        value,
        false,
        "button"
    );
}

/**
 * A URL indicating a poster frame to show until the user plays or seeks.
 **/
export function Poster(value: string) {
    return new HtmlAttr(
        "poster",
        value,
        false,
        "video"
    );
}

/**
 * Indicates whether the whole resource, parts of it or nothing should be preloaded.
 **/
export function Preload(value: boolean | string) {
    return new HtmlAttr(
        "preload",
        value,
        false,
        "audio",
        "video"
    );
}

/**
 * Indicates whether the element can be edited.
 **/
export function ReadOnly(value: boolean) {
    return new HtmlAttr(
        "readonly",
        value,
        false,
        "input",
        "textarea"
    );
}

/**
 * The radiogroup attribute
 **/
export function RadioGroup(value: string) {
    return new HtmlAttr(
        "radiogroup",
        value,
        false,
        "command"
    );
}

/**
 * Specifies which referrer is sent when fetching the resource.
 **/
export function ReferrerPolicy(value: string) {
    return new HtmlAttr(
        "referrerpolicy",
        value,
        false,
        "a",
        "area",
        "iframe",
        "img",
        "link",
        "script"
    );
}

/**
 * Specifies the relationship of the target object to the link object.
 **/
export function Rel(value: string) {
    return new HtmlAttr(
        "rel",
        value,
        false,
        "a",
        "area",
        "link"
    );
}

/**
 * Indicates whether this element is required to fill out or not.
 **/
export function Required(value: boolean) {
    return new HtmlAttr(
        "required",
        value,
        false,
        "input",
        "select",
        "textarea"
    );
}

/**
 * Indicates whether the list should be displayed in a descending order instead of a ascending.
 **/
export function Reversed(value: boolean) {
    return new HtmlAttr(
        "reversed",
        value,
        false,
        "ol"
    );
}

/**
 * Defines the number of rows in a text area.
 **/
export function Role(value: string) {
    return new HtmlAttr(
        "role",
        value,
        false
    );
}

/**
 * The rows attribute
 **/
export function Rows(value: number) {
    return new HtmlAttr(
        "rows",
        value,
        false,
        "textarea"
    );
}

/**
 * Defines the number of rows a table cell should span over.
 **/
export function RowSpan(value: number) {
    return new HtmlAttr(
        "rowspan",
        value,
        false,
        "td",
        "th"
    );
}

/**
 * Stops a document loaded in an iframe from using certain features (such as submitting forms or opening new windows).
 **/
export function Sandbox(value: string) {
    return new HtmlAttr(
        "sandbox",
        value,
        false,
        "iframe"
    );
}

/**
 * Defines the cells that the header test (defined in the th element) relates to.
 **/
export function Scope(value: string) {
    return new HtmlAttr(
        "scope",
        value,
        false,
        "th"
    );
}

/**
 * The scoped attribute for `style` tags.
 **/
export function Scoped(value: boolean) {
    return new HtmlAttr(
        "scoped",
        value,
        false,
        "style"
    );
}

/**
 * The scrolling attribute for `iframe` tags.
 **/
export function Scrolling(value: boolean) {
    return new HtmlAttr(
        "scrolling",
        value ? "yes" : "no",
        false,
        "iframe"
    );
}

/**
 * Defines an `option` tag which will be selected on page load.
 **/
export function Selected(value: boolean) {
    return new HtmlAttr(
        "selected",
        value,
        false,
        "option"
    );
}

/**
 * Sets the `selectedIndex` property on a Select element.
 */
export function SelectedIndex(value: number) {
    return new HtmlAttr(
        "selectedIndex",
        value,
        false,
        "select"
    );
}

/**
 * The shape attribute for `a` and `area` tags.
 **/
export function Shape(value: string) {
    return new HtmlAttr(
        "shape",
        value,
        false,
        "a",
        "area"
    );
}

/**
 * Defines the width of the element (in pixels). If the element's type attribute is text or password then it's the number of characters.
 **/
export function Size(value: number) {
    return new HtmlAttr(
        "size",
        value,
        false,
        "input",
        "select"
    );
}

/**
 * Assigns a slot in a shadow DOM shadow tree to an element.
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function SlotAttr(value: string) {
    return new HtmlAttr(
        "slot",
        value,
        false
    );
}

/**
 * The sizes attribute
 **/
export function Sizes(value: string) {
    return new HtmlAttr(
        "sizes",
        value,
        false,
        "link",
        "img",
        "source"
    );
}

/**
 * The span attribute.
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function SpanAttr(value: string) {
    return new HtmlAttr(
        "span",
        value,
        false,
        "col",
        "colgroup"
    );
}

/**
 * Indicates whether spell checking is allowed for the element.
 **/
export function SpellCheck(value: boolean) {
    return new HtmlAttr(
        "spellcheck",
        value,
        false
    );
}

/**
 * The URL of the embeddable content.
 **/
export function Src(value: string | URL) {
    return new HtmlAttr(
        "src",
        unpackURL(value), false, "audio",
        "async-doc-fragment",
        "embed",
        "iframe",
        "img",
        "input",
        "script",
        "source",
        "track",
        "video"
    );
}

/**
 * The srcdoc attribute
 **/
export function SrcDoc(value: string) {
    return new HtmlAttr(
        "srcdoc",
        value,
        false,
        "iframe"
    );
}

/**
 * The srclang attribute
 **/
export function SrcLang(value: string) {
    return new HtmlAttr(
        "srclang",
        value,
        false,
        "track"
    );
}

/**
 * A MediaStream object to use as a source for an HTML video or audio element
 **/
export function SrcObject(value: MediaProvider) {
    return new HtmlAttr(
        "srcObject",
        value,
        false,
        "audio",
        "video"
    );
}

/**
 * One or more responsive image candidates.
 **/
export function SrcSet(value: string) {
    return new HtmlAttr(
        "srcset",
        value,
        false,
        "img",
        "source"
    );
}

/**
 * Defines the first number if other than 1.
 **/
export function Start(value: number) {
    return new HtmlAttr(
        "start",
        value,
        false,
        "ol"
    );
}

/**
 * The step attribute
 **/
export function Step(value: number) {
    return new HtmlAttr(
        "step",
        value,
        false,
        "input"
    );
}

class HtmlStyleAttr extends HtmlAttr {
    constructor(dict: Record<string, string>) {
        super("style", dict, false);
    }

    override apply(tag: Node): void {
        if (tag instanceof HTMLElement) {
            for (const [key, value] of Object.entries(this.value)) {
                tag.style.setProperty(key, value);
            }
        }
    }
}

/**
 * Creates an assigner function for objects with a style attribute.
 * @param dict the value to assign. Use the full, kebob-case CSS field names as keys, e.g. "grid-template-columns".
 */
export function StyleAttr(dict: Record<string, string>): HtmlAttr {
    return new HtmlStyleAttr(dict);
}

/**
 * The summary attribute.
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function SummaryAttr(value: string) {
    return new HtmlAttr(
        "summary",
        value,
        false,
        "table"
    );
}

/**
 * Overrides the browser's default tab order and follows the one specified instead.
 **/
export function TabIndex(value: number) {
    return new HtmlAttr(
        "tabindex",
        value,
        false
    );
}

export function TextContent(text: { toString(): string }) {
    return new HtmlAttr(
        "textContent",
        text,
        false
    );
}

/**
 * Text to be displayed in a tooltip when hovering over the element. 
 * 
 * This function needs the "Attr" postfix to differentiate it from the HTML tag of the same name.
 **/
export function TitleAttr(value: string) {
    return new HtmlAttr(
        "title",
        value,
        false
    );
}

/**
 * The target attribute
 **/
export function Target(value: string) {
    return new HtmlAttr(
        "target",
        value,
        false,
        "a",
        "area",
        "base",
        "form"
    );
}

/**
 * Specify whether an element’s attribute values and the values of its Text node children are to be translated when the page is localized, or whether to leave them unchanged.
 **/
export function Translate(value: boolean) {
    return new HtmlAttr(
        "translate",
        value,
        false
    );
}

/**
 * Defines the type of the element.
 **/
export function Type(value: string | { value: string }) {
    if (typeof value !== "string") {
        value = value.value;
    }

    return new HtmlAttr(
        "type",
        value,
        false,
        "button",
        "input",
        "command",
        "embed",
        "link",
        "object",
        "script",
        "source",
        "style",
        "menu"
    );
}

/**
 * Defines a default value which will be displayed in the element on page load.
 */
export function Value(value: { toString(): string }) {
    return new HtmlAttr(
        "value",
        value,
        false,
        "button",
        "data",
        "input",
        "li",
        "meter",
        "option",
        "progress",
        "param",
        "select"
    );
}

/**
 * Defines a default value which will be displayed in the element on page load.
 **/
export function ValueAsNumber(value: number) {
    return new HtmlAttr(
        "valueAsNumber",
        value,
        false,
        "input"
    );
}

/**
 * Defines a default value which will be displayed in the element on page load.
 **/
export function ValueAsDate(value: Date) {
    return new HtmlAttr(
        "valueAsDate",
        value,
        false,
        "input"
    );
}

/**
 * setting the volume at which a media element plays.
 **/
export function Volume(value: number) {
    return new HtmlAttr(
        "volume",
        value,
        false,
        "audio",
        "video"
    );
}

/**
 * The usemap attribute
 **/
export function UseMap(value: boolean) {
    return new HtmlAttr(
        "usemap",
        value,
        false,
        "img",
        "input",
        "object"
    );
}

/**
 * For the elements listed here, this establishes the element's width.
 **/
export function Width(value: number | string) {
    return new HtmlAttr(
        "width",
        value,
        false,
        "canvas",
        "embed",
        "iframe",
        "img",
        "input",
        "object",
        "video"
    );
}

/**
 * Indicates whether the text should be wrapped.
 **/
export function Wrap(value: HTMLWrapValue) {
    return new HtmlAttr(
        "wrap",
        value,
        false,
        "textarea"
    );
}

type HTMLAutoCompleteAttributeValue =
    | "off" // The browser is not permitted to automatically enter or select a value for this field.It is possible that the document or application provides its own autocomplete feature, or that security concerns require that the field's value not be automatically entered. Note: In most modern browsers, setting autocomplete to "off" will not prevent a password manager from asking the user if they would like to save username and password information, or from automatically filling in those values in a site's login form. See the autocomplete attribute and login fields.
    | "on" // The browser is allowed to automatically complete the input.No guidance is provided as to the type of data expected in the field, so the browser may use its own judgement."name" // The field expects the value to be a person's full name. Using "name" rather than breaking the name down into its components is generally preferred because it avoids dealing with the wide diversity of human names and how they are structured; however, you can use the following autocomplete values if you do need to break the name down into its components:
    | "honorific-prefix" // The prefix or title, such as "Mrs.", "Mr.", "Miss", "Ms.", "Dr.", or "Mlle.".
    | "given-name" // The given(or "first") name.
    | "additional-name" // The middle name.
    | "family-name" // The family(or "last") name.
    | "honorific-suffix" // The suffix, such as "Jr.", "B.Sc.", "PhD.", "MBASW", or "IV".
    | "nickname" // A nickname or handle.
    | "email" // An email address.
    | "username" // A username or account name.
    | "new-password" // A new password.When creating a new account or changing passwords, this should be used for an "Enter your new password" or "Confirm new password" field, as opposed to a general "Enter your current password" field that might be present.This may be used by the browser both to avoid accidentally filling in an existing password and to offer assistance in creating a secure password(see also Preventing autofilling with autocomplete = "new-password").
    | "current-password" // The user's current password.
    | "one-time-code" // A one - time password(OTP) for verifying user information, most commonly a phone number used as an additional factor in a sign -in flow.
    | "organization-title" // A job title, or the title a person has within an organization, such as "Senior Technical Writer", "President", or "Assistant Troop Leader".
    | "organization" // A company or organization name, such as "Acme Widget Company" or "Girl Scouts of America".
    | "street-address" // A street address.This can be multiple lines of text, and should fully identify the location of the address within its second administrative level(typically a city or town), but should not include the city name, ZIP or postal code, or country name.
    | "address-line1"
    | "address-line2"
    | "address-line3" // Each individual line of the street address.These should only be present if the "street-address" is not present.
    | "address-level4" // The finest - grained administrative level, in addresses which have four levels.
    | "address-level3" // The third administrative level, in addresses with at least three administrative levels.
    | "address-level2" // The second administrative level, in addresses with at least two of them.In countries with two administrative levels, this would typically be the city, town, village, or other locality in which the address is located.
    | "address-level1" // The first administrative level in the address.This is typically the province in which the address is located.In the United States, this would be the state.In Switzerland, the canton.In the United Kingdom, the post town.
    | "country" // A country or territory code.
    | "country-name" // A country or territory name.
    | "postal-code" // A postal code(in the United States, this is the ZIP code).
    | "cc-name" // The full name as printed on or associated with a payment instrument such as a credit card.Using a full name field is preferred, typically, over breaking the name into pieces.
    | "cc-given-name" // A given(first) name as given on a payment instrument like a credit card.
    | "cc-additional-name" // A middle name as given on a payment instrument or credit card.
    | "cc-family-name" // A family name, as given on a credit card.
    | "cc-number" // A credit card number or other number identifying a payment method, such as an account number.
    | "cc-exp" // A payment method expiration date, typically in the form "MM/YY" or "MM/YYYY".
    | "cc-exp-month" // The month in which the payment method expires.
    | "cc-exp-year" // The year in which the payment method expires.
    | "cc-csc" // The security code for the payment instrument; on credit cards, this is the 3 - digit verification number on the back of the card.
    | "cc-type" // The type of payment instrument(such as "Visa" or "Master Card").
    | "transaction-currency" // The currency in which the transaction is to take place.
    | "transaction-amount" // The amount, given in the currency specified by "transaction-currency", of the transaction, for a payment form.
    | "language" // A preferred language, given as a valid BCP 47 language tag.
    | "bday" // A birth date, as a full date.
    | "bday-day" // The day of the month of a birth date.
    | "bday-month" // The month of the year of a birth date.
    | "bday-year" // The year of a birth date.
    | "sex" // A gender identity(such as "Female", "Fa'afafine", "Hijra", "Male", "Nonbinary"), as freeform text without newlines.
    | "tel" // A full telephone number, including the country code.If you need to break the phone number up into its components, you can use these values for those fields:
    | "tel-country-code" // The country code, such as "1" for the United States, Canada, and other areas in North America and parts of the Caribbean.
    | "tel-national" // The entire phone number without the country code component, including a country - internal prefix.For the phone number "1-855-555-6502", this field's value would be "855-555-6502".
    | "tel-area-code" // The area code, with any country - internal prefix applied if appropriate.
    | "tel-local" // The phone number without the country or area code.This can be split further into two parts, for phone numbers which have an exchange number and then a number within the exchange.For the phone number "555-6502", use "tel-local-prefix" for "555" and "tel-local-suffix" for "6502".
    | "tel-extension" // A telephone extension code within the phone number, such as a room or suite number in a hotel or an office extension in a company.
    | "impp" // A URL for an instant messaging protocol endpoint, such as "xmpp:username@example.net".
    | "url" // A URL, such as a home page or company website address as appropriate given the context of the other fields in the form.
    | "photo" // The URL of an image representing the person, company, or contact information given in the other fields in the form.
    | "webauthn" // Passkeys generated by the Web Authentication API, as requested by a conditional navigator.credentials.get() call (i.e., one that includes mediation: 'conditional'). See Sign in with a passkey through form autofill for more details.


type HTMLCrossOriginValue = "anonymous" | "use-credentials";

type HTMLWrapValue = "hard" | "soft" | "off";