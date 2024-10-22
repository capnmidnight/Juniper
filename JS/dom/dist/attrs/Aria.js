import { HtmlAttr } from "./HtmlAttr";
import { HtmlProp } from "./HtmlProp";
/**
 * Identifies the currently active element when DOM focus is on a composite widget, textbox, group, or application.
 **/
export function AriaActiveDescendant(value) {
    return new HtmlProp("ariaActiveDescendant", value);
}
/**
 * Indicates whether assistive technologies will present all, or only parts of, the changed region based on the change notifications defined by the aria-relevant attribute.
 **/
export function AriaAtomic(value) {
    return new HtmlProp("ariaAtomic", value);
}
/**
 * Indicates whether inputting text could trigger display of one or more predictions of the user's intended value for an input and specifies how predictions would be presented if they are made.
 **/
export function AriaAutoComplete(value) {
    return new HtmlProp("ariaAutoComplete", value);
}
/**
 * Indicates an element is being modified and that assistive technologies MAY want to wait until the modifications are complete before exposing them to the user.
 **/
export function AriaBusy(value) {
    return new HtmlProp("ariaBusy", value);
}
/**
 * Indicates the current "checked" state of checkboxes, radio buttons, and other widgets. See related aria-pressed and aria-selected.
 **/
export function AriaChecked(value) {
    return new HtmlProp("ariaChecked", value);
}
/**
 * Defines the total number of columns in a table, grid, or treegrid. See related aria-colindex.
 **/
export function AriaColCount(value) {
    return new HtmlProp("ariaColCount", value);
}
/**
 * Defines an element's column index or position with respect to the total number of columns within a table, grid, or treegrid. See related aria-colcount and aria-colspan.
 **/
export function AriaColIndex(value) {
    return new HtmlProp("ariaColIndex", value);
}
/**
 * Defines the number of columns spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-colindex and aria-rowspan.
 **/
export function AriaColSpan(value) {
    return new HtmlProp("ariaColSpan", value);
}
/**
 * Identifies the element (or elements) whose contents or presence are controlled by the current element. See related aria-owns.
 **/
export function AriaControls(value) {
    return new HtmlAttr("aria-controls", value);
}
/**
 * Indicates the element that represents the current item within a container or set of related elements.
 **/
export function AriaCurrent(value) {
    return new HtmlProp("ariaCurrent", value);
}
/**
 * Identifies the element (or elements) that describes the object. See related aria-labelledby.
 **/
export function AriaDescribedBy(value) {
    return new HtmlProp("ariaDescribedBy", value);
}
/**
 * Identifies the element that provides a detailed, extended description for the object. See related aria-describedby.
 **/
export function AriaDetails(value) {
    return new HtmlProp("ariaDetails", value);
}
/**
 * Indicates that the element is perceivable but disabled, so it is not editable or otherwise operable. See related aria-hidden and aria-readonly.
 **/
export function AriaDisabled(value) {
    return new HtmlProp("ariaDisabled", value);
}
/**
 * Identifies the element that provides an error message for the object. See related aria-invalid and aria-describedby.
 **/
export function AriaErrorMessage(value) {
    return new HtmlProp("ariaErrorMessage", value);
}
/**
 * Indicates whether the element, or another grouping element it controls, is currently expanded or collapsed.
 **/
export function AriaExpanded(value) {
    return new HtmlProp("ariaExpanded", value);
}
/**
 * Identifies the next element (or elements) in an alternate reading order of content which, at the user's discretion, allows assistive technology to override the general default of reading in document source order.
 **/
export function AriaFlowTo(value) {
    return new HtmlProp("ariaFlowTo", value);
}
/**
 * Indicates the availability and type of interactive popup element, such as menu or dialog, that can be triggered by an element.
 **/
export function AriaHasPopup(value) {
    return new HtmlProp("ariaHasPopup", value);
}
/**
 * Indicates whether the element is exposed to an accessibility API. See related aria-disabled.
 **/
export function AriaHidden(value) {
    return new HtmlProp("ariaHidden", value);
}
/**
 * Indicates the entered value does not conform to the format expected by the application. See related aria-error_message.
 **/
export function AriaInvalid(value) {
    return new HtmlProp("ariaInvalid", value);
}
/**
 * Indicates keyboard shortcuts that an author has implemented to activate or give focus to an element.
 **/
export function AriaKeyShortcuts(value) {
    return new HtmlProp("ariaKeyShortcuts", value);
}
/**
 * Defines a string value that labels the current element. See related aria-labelledby.
 **/
export function AriaLabel(value) {
    return new HtmlProp("ariaLabel", value);
}
/**
 * Identifies the element (or elements) that labels the current element. See related aria-describedby.
 **/
export function AriaLabelledBy(value) {
    return new HtmlAttr("aria-labelledby", value);
}
/**
 * Defines the hierarchical level of an element within a structure.
 **/
export function AriaLevel(value) {
    return new HtmlProp("ariaLevel", value);
}
/**
 * Indicates that an element will be updated, and describes the types of updates the user agents, assistive technologies, and user can expect from the live region.
 **/
export function AriaLive(value) {
    return new HtmlProp("ariaLive", value);
}
/**
 * Indicates whether an element is modal when displayed
 **/
export function AriaModal(value) {
    return new HtmlProp("ariaModal", value);
}
/**
 * Indicates whether a text box accepts multiple lines of input or only a single line.
 **/
export function AriaMultiline(value) {
    return new HtmlProp("ariaMultiLine", value);
}
/**
 * Indicates that the user may select more than one item from the current selectable descendants.
 **/
export function AriaMultiSelectable(value) {
    return new HtmlProp("ariaMultiSelectable", value);
}
/**
 * Indicates that the user may select more than one item from the current selectable descendants.
 **/
export function AriaOrientation(value) {
    return new HtmlProp("ariaOrientation", value);
}
/**
 * Identifies an element (or elements) in order to define a visual, functional, or contextual parent/child relationship between DOM elements where the DOM hierarchy cannot be used to represent the relationship. See related aria-controls.
 **/
export function AriaOwns(value) {
    return new HtmlProp("ariaOwns", value);
}
/**
 * Defines a short hint (a word or short phrase) intended to aid the user with data entry when the control has no value. A hint could be a sample value or a brief description of the expected format.
 **/
export function AriaPlaceholder(value) {
    return new HtmlProp("ariaPlaceholder", value);
}
/**
 * Defines an element's number or position in the current set of list items or treeitems. Not required if all elements in the set are present in the DOM. See related aria-setsize.
 **/
export function AriaPosInSet(value) {
    return new HtmlProp("ariaPosInSet", value);
}
/**
 * Indicates the current "pressed" state of toggle buttons. See related aria-checked and aria-selected.
 **/
export function AriaPressed(value) {
    return new HtmlProp("ariaPressed", value);
}
/**
 * Indicates that the element is not editable, but is otherwise operable. See related aria-disabled.
 **/
export function AriaReadOnly(value) {
    return new HtmlProp("ariaReadOnly", value);
}
/**
 * Indicates what notifications the user agent will trigger when the accessibility tree within a live region is modified. See related aria-atomic.
 **/
export function AriaRelevant(value) {
    return new HtmlProp("ariaRelevant", value);
}
/**
 * Indicates that user input is required on the element before a form may be submitted.
 **/
export function AriaRequired(value) {
    return new HtmlProp("ariaRequired", value);
}
/**
 * Defines a human-readable, author-localized description for the role of an element.
 **/
export function AriaRoleDescription(value) {
    return new HtmlProp("ariaRoleDescription", value);
}
/**
 * Defines the total number of rows in a table, grid, or treegrid. See related aria-rowindex.
 **/
export function AriaRowCount(value) {
    return new HtmlProp("ariaRowCount", value);
}
/**
 * Defines an element's row index or position with respect to the total number of rows within a table, grid, or treegrid. See related aria-rowcount and aria-rowspan.
 **/
export function AriaRowIndex(value) {
    return new HtmlProp("ariaRowIndex", value);
}
/**
 Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-rowindex and aria-colspan.
 **/
export function AriaRowSpan(value) {
    return new HtmlProp("ariaRowSpan", value);
}
/**
 * Indicates the current "selected" state of various widgets. See related aria-checked and aria-pressed.
 **/
export function AriaSelected(value) {
    return new HtmlProp("ariaSelected", value);
}
/**
 * Defines the number of items in the current set of list items or tree items. Not required if all elements in the set are present in the DOM. See related aria-posinset.
 **/
export function AriaSetSize(value) {
    return new HtmlProp("ariaSetSize", value);
}
/**
 * Indicates if items in a table or grid are sorted in ascending or descending order.
 **/
export function AriaSort(value) {
    return new HtmlProp("ariaSort", value);
}
/**
 * Defines the maximum allowed value for a range widget.
 **/
export function AriaValueMax(value) {
    return new HtmlAttr("aria-valuemax", value);
}
/**
 * Defines the minimum allowed value for a range widget.
 **/
export function AriaValueMin(value) {
    return new HtmlAttr("aria-valuemin", value);
}
/**
 * Defines the current value for a range widget. See related aria-valuetext.
 **/
export function AriaValueNow(value) {
    return new HtmlAttr("aria-valuenow", value);
}
/**
 * Defines the human readable text alternative of aria-valuenow for a range widget.
 **/
export function AriaValueText(value) {
    return new HtmlProp("ariaValueText", value);
}
//# sourceMappingURL=Aria.js.map