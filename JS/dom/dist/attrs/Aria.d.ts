import { HtmlAttr } from "./HtmlAttr";
import { HtmlProp } from "./HtmlProp";
/**
 * Identifies the currently active element when DOM focus is on a composite widget, textbox, group, or application.
 **/
export declare function AriaActiveDescendant(value: string): HtmlProp<"ariaActiveDescendant", string, Node & Record<"ariaActiveDescendant", string>>;
/**
 * Indicates whether assistive technologies will present all, or only parts of, the changed region based on the change notifications defined by the aria-relevant attribute.
 **/
export declare function AriaAtomic(value: boolean): HtmlProp<"ariaAtomic", boolean, Node & Record<"ariaAtomic", boolean>>;
/**
 * Indicates whether inputting text could trigger display of one or more predictions of the user's intended value for an input and specifies how predictions would be presented if they are made.
 **/
export declare function AriaAutoComplete(value: string): HtmlProp<"ariaAutoComplete", string, Node & Record<"ariaAutoComplete", string>>;
/**
 * Indicates an element is being modified and that assistive technologies MAY want to wait until the modifications are complete before exposing them to the user.
 **/
export declare function AriaBusy(value: boolean): HtmlProp<"ariaBusy", boolean, Node & Record<"ariaBusy", boolean>>;
/**
 * Indicates the current "checked" state of checkboxes, radio buttons, and other widgets. See related aria-pressed and aria-selected.
 **/
export declare function AriaChecked(value: boolean): HtmlProp<"ariaChecked", boolean, Node & Record<"ariaChecked", boolean>>;
/**
 * Defines the total number of columns in a table, grid, or treegrid. See related aria-colindex.
 **/
export declare function AriaColCount(value: number): HtmlProp<"ariaColCount", number, Node & Record<"ariaColCount", number>>;
/**
 * Defines an element's column index or position with respect to the total number of columns within a table, grid, or treegrid. See related aria-colcount and aria-colspan.
 **/
export declare function AriaColIndex(value: number): HtmlProp<"ariaColIndex", number, Node & Record<"ariaColIndex", number>>;
/**
 * Defines the number of columns spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-colindex and aria-rowspan.
 **/
export declare function AriaColSpan(value: number): HtmlProp<"ariaColSpan", number, Node & Record<"ariaColSpan", number>>;
/**
 * Identifies the element (or elements) whose contents or presence are controlled by the current element. See related aria-owns.
 **/
export declare function AriaControls(value: string): HtmlAttr<string, Node>;
/**
 * Indicates the element that represents the current item within a container or set of related elements.
 **/
export declare function AriaCurrent(value: string): HtmlProp<"ariaCurrent", string, Node & Record<"ariaCurrent", string>>;
/**
 * Identifies the element (or elements) that describes the object. See related aria-labelledby.
 **/
export declare function AriaDescribedBy(value: string): HtmlProp<"ariaDescribedBy", string, Node & Record<"ariaDescribedBy", string>>;
/**
 * Identifies the element that provides a detailed, extended description for the object. See related aria-describedby.
 **/
export declare function AriaDetails(value: string): HtmlProp<"ariaDetails", string, Node & Record<"ariaDetails", string>>;
/**
 * Indicates that the element is perceivable but disabled, so it is not editable or otherwise operable. See related aria-hidden and aria-readonly.
 **/
export declare function AriaDisabled(value: boolean): HtmlProp<"ariaDisabled", boolean, Node & Record<"ariaDisabled", boolean>>;
/**
 * Identifies the element that provides an error message for the object. See related aria-invalid and aria-describedby.
 **/
export declare function AriaErrorMessage(value: string): HtmlProp<"ariaErrorMessage", string, Node & Record<"ariaErrorMessage", string>>;
/**
 * Indicates whether the element, or another grouping element it controls, is currently expanded or collapsed.
 **/
export declare function AriaExpanded(value: boolean): HtmlProp<"ariaExpanded", boolean, Node & Record<"ariaExpanded", boolean>>;
/**
 * Identifies the next element (or elements) in an alternate reading order of content which, at the user's discretion, allows assistive technology to override the general default of reading in document source order.
 **/
export declare function AriaFlowTo(value: string): HtmlProp<"ariaFlowTo", string, Node & Record<"ariaFlowTo", string>>;
/**
 * Indicates the availability and type of interactive popup element, such as menu or dialog, that can be triggered by an element.
 **/
export declare function AriaHasPopup(value: string): HtmlProp<"ariaHasPopup", string, Node & Record<"ariaHasPopup", string>>;
/**
 * Indicates whether the element is exposed to an accessibility API. See related aria-disabled.
 **/
export declare function AriaHidden(value: boolean): HtmlProp<"ariaHidden", boolean, Node & Record<"ariaHidden", boolean>>;
/**
 * Indicates the entered value does not conform to the format expected by the application. See related aria-error_message.
 **/
export declare function AriaInvalid(value: string): HtmlProp<"ariaInvalid", string, Node & Record<"ariaInvalid", string>>;
/**
 * Indicates keyboard shortcuts that an author has implemented to activate or give focus to an element.
 **/
export declare function AriaKeyShortcuts(value: string): HtmlProp<"ariaKeyShortcuts", string, Node & Record<"ariaKeyShortcuts", string>>;
/**
 * Defines a string value that labels the current element. See related aria-labelledby.
 **/
export declare function AriaLabel(value: string): HtmlProp<"ariaLabel", string, Node & Record<"ariaLabel", string>>;
/**
 * Identifies the element (or elements) that labels the current element. See related aria-describedby.
 **/
export declare function AriaLabelledBy(value: string): HtmlAttr<string, Node>;
/**
 * Defines the hierarchical level of an element within a structure.
 **/
export declare function AriaLevel(value: number): HtmlProp<"ariaLevel", number, Node & Record<"ariaLevel", number>>;
/**
 * Indicates that an element will be updated, and describes the types of updates the user agents, assistive technologies, and user can expect from the live region.
 **/
export declare function AriaLive(value: string): HtmlProp<"ariaLive", string, Node & Record<"ariaLive", string>>;
/**
 * Indicates whether an element is modal when displayed
 **/
export declare function AriaModal(value: boolean): HtmlProp<"ariaModal", boolean, Node & Record<"ariaModal", boolean>>;
/**
 * Indicates whether a text box accepts multiple lines of input or only a single line.
 **/
export declare function AriaMultiline(value: boolean): HtmlProp<"ariaMultiLine", boolean, Node & Record<"ariaMultiLine", boolean>>;
/**
 * Indicates that the user may select more than one item from the current selectable descendants.
 **/
export declare function AriaMultiSelectable(value: boolean): HtmlProp<"ariaMultiSelectable", boolean, Node & Record<"ariaMultiSelectable", boolean>>;
/**
 * Indicates that the user may select more than one item from the current selectable descendants.
 **/
export declare function AriaOrientation(value: string): HtmlProp<"ariaOrientation", string, Node & Record<"ariaOrientation", string>>;
/**
 * Identifies an element (or elements) in order to define a visual, functional, or contextual parent/child relationship between DOM elements where the DOM hierarchy cannot be used to represent the relationship. See related aria-controls.
 **/
export declare function AriaOwns(value: string): HtmlProp<"ariaOwns", string, Node & Record<"ariaOwns", string>>;
/**
 * Defines a short hint (a word or short phrase) intended to aid the user with data entry when the control has no value. A hint could be a sample value or a brief description of the expected format.
 **/
export declare function AriaPlaceholder(value: string): HtmlProp<"ariaPlaceholder", string, Node & Record<"ariaPlaceholder", string>>;
/**
 * Defines an element's number or position in the current set of list items or treeitems. Not required if all elements in the set are present in the DOM. See related aria-setsize.
 **/
export declare function AriaPosInSet(value: number): HtmlProp<"ariaPosInSet", number, Node & Record<"ariaPosInSet", number>>;
/**
 * Indicates the current "pressed" state of toggle buttons. See related aria-checked and aria-selected.
 **/
export declare function AriaPressed(value: boolean): HtmlProp<"ariaPressed", boolean, Node & Record<"ariaPressed", boolean>>;
/**
 * Indicates that the element is not editable, but is otherwise operable. See related aria-disabled.
 **/
export declare function AriaReadOnly(value: boolean): HtmlProp<"ariaReadOnly", boolean, Node & Record<"ariaReadOnly", boolean>>;
/**
 * Indicates what notifications the user agent will trigger when the accessibility tree within a live region is modified. See related aria-atomic.
 **/
export declare function AriaRelevant(value: string): HtmlProp<"ariaRelevant", string, Node & Record<"ariaRelevant", string>>;
/**
 * Indicates that user input is required on the element before a form may be submitted.
 **/
export declare function AriaRequired(value: boolean): HtmlProp<"ariaRequired", boolean, Node & Record<"ariaRequired", boolean>>;
/**
 * Defines a human-readable, author-localized description for the role of an element.
 **/
export declare function AriaRoleDescription(value: string): HtmlProp<"ariaRoleDescription", string, Node & Record<"ariaRoleDescription", string>>;
/**
 * Defines the total number of rows in a table, grid, or treegrid. See related aria-rowindex.
 **/
export declare function AriaRowCount(value: number): HtmlProp<"ariaRowCount", number, Node & Record<"ariaRowCount", number>>;
/**
 * Defines an element's row index or position with respect to the total number of rows within a table, grid, or treegrid. See related aria-rowcount and aria-rowspan.
 **/
export declare function AriaRowIndex(value: number): HtmlProp<"ariaRowIndex", number, Node & Record<"ariaRowIndex", number>>;
/**
 Defines the number of rows spanned by a cell or gridcell within a table, grid, or treegrid. See related aria-rowindex and aria-colspan.
 **/
export declare function AriaRowSpan(value: number): HtmlProp<"ariaRowSpan", number, Node & Record<"ariaRowSpan", number>>;
/**
 * Indicates the current "selected" state of various widgets. See related aria-checked and aria-pressed.
 **/
export declare function AriaSelected(value: boolean): HtmlProp<"ariaSelected", boolean, Node & Record<"ariaSelected", boolean>>;
/**
 * Defines the number of items in the current set of list items or tree items. Not required if all elements in the set are present in the DOM. See related aria-posinset.
 **/
export declare function AriaSetSize(value: number): HtmlProp<"ariaSetSize", number, Node & Record<"ariaSetSize", number>>;
/**
 * Indicates if items in a table or grid are sorted in ascending or descending order.
 **/
export declare function AriaSort(value: string): HtmlProp<"ariaSort", string, Node & Record<"ariaSort", string>>;
/**
 * Defines the maximum allowed value for a range widget.
 **/
export declare function AriaValueMax(value: number): HtmlAttr<number, Node>;
/**
 * Defines the minimum allowed value for a range widget.
 **/
export declare function AriaValueMin(value: number): HtmlAttr<number, Node>;
/**
 * Defines the current value for a range widget. See related aria-valuetext.
 **/
export declare function AriaValueNow(value: number): HtmlAttr<number, Node>;
/**
 * Defines the human readable text alternative of aria-valuenow for a range widget.
 **/
export declare function AriaValueText(value: string): HtmlProp<"ariaValueText", string, Node & Record<"ariaValueText", string>>;
//# sourceMappingURL=Aria.d.ts.map