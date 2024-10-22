import { AbstractAppliable } from "./AbstractAppliable";
import { AbstractKeyValueAppliable } from "./attrs/AbstractKeyValueAppliable";
import { CssDisplayValue, CssGlobalValue } from "./css";
import { Nodes } from './ErsatzNode';
import { ElementChild } from "./HtmlTag";
/**********************************
 * UTILITIES
 *********************************/
/**
 * Set the document element and the body's scrollTop to 0
 */
export declare function scrollWindowToTop(): void;
declare class HtmlClearAttr extends AbstractAppliable<HTMLElement> {
    apply(tag: HTMLElement): void;
}
/**
 * Clears all children out of an element.
 */
export declare function Clear(): HtmlClearAttr;
declare class CssClearAttr extends AbstractAppliable<Element> {
    apply(tag: Element): void;
}
export declare function ClearCss(): CssClearAttr;
export declare function Optional(test: any, ...rest: ElementChild[]): ElementChild[];
export type InlineStylableElement = Element & ElementCSSInlineStyle;
/**
 * Set the style.display property for a tag
 * @param selectorOrElement
 * @param type the display type to set. Defaults to "block"
 */
export declare function show<T extends InlineStylableElement = InlineStylableElement>(selectorOrElement: string | Nodes<T>, inType?: string): void;
/**
 * Set the style.display property for a tag to "none"
 */
export declare function hide<T extends InlineStylableElement = InlineStylableElement>(selectorOrElement: string | Nodes<T>): void;
export declare function elementSetDisplay<T extends InlineStylableElement = InlineStylableElement>(elem: Nodes<T>, visible: boolean, visibleDisplayType?: CssGlobalValue | CssDisplayValue): void;
export declare function isDisplayed<T extends Element = Element>(elem: Nodes<T>): boolean;
export declare function elementToggleDisplay<T extends InlineStylableElement = InlineStylableElement>(elem: Nodes<T>, visibleDisplayType?: CssGlobalValue | CssDisplayValue): void;
export declare function ImportTemplate(path: URL | RequestInfo): Promise<HTMLTemplateElement>;
export declare function ImportDOM(path: URL | RequestInfo): Promise<Element[]>;
/**
 * Parses the given HTML text and returns the elements it defines.
 */
export declare function RawHtmlElements(html: string): Element[];
/**
 * Parses the given HTML text and returns the first element it defines.
 */
export declare function RawHtmlElement(html: string): Element;
/**
 * Builds an HTML structure in a tag.
 * @param node
 * @param  rest attribute assigner functions and sub element children objects.
 * @returns the same tag back out again
 */
export declare function HtmlRender<T extends ParentNode>(node: Nodes<T>, ...rest: ElementChild[]): T;
export declare class HtmlQueryAttr<ElementT extends Element = Element> extends AbstractKeyValueAppliable<Node, string, ElementT> {
    constructor(errorOnNotFound: boolean, rootOrSelector: string | Nodes<ParentNode>, selector?: string);
    apply(_node: Node): void;
}
/**
 * A fake attribute that allows you to recover elements already in the document
 * via an document.querySelector(selector) call.
 * @param rootOrSelector If a string is provided, this parameter will be used as a query path on the document.
 * If an HTMLElement is provided, this parameter will be used as the root for the query.
 * @param selector If `rootOrSelector` is an HTMLElement, this parameter must be a string and will be used as the query path.
 */
export declare function MaybeQuery<ElementT extends Element = Element>(rootOrSelector: string | Nodes<ParentNode>, selector?: string): HtmlQueryAttr<ElementT>;
/**
 * A fake attribute that allows you to recover elements already in the document
 * via an document.querySelector(selector) call.
 * @param rootOrSelector If a string is provided, this parameter will be used as a query path on the document.
 * If an HTMLElement is provided, this parameter will be used as the root for the query.
 * @param selector If `rootOrSelector` is an HTMLElement, this parameter must be a string and will be used as the query path.
 */
export declare function Query<ElementT extends Element = Element>(rootOrSelector: string | Nodes<ParentNode>, selector?: string): HtmlQueryAttr<ElementT>;
/**
 * Retrieve elements already in the document
 * via an document.querySelectorAll(selector) call.
 * @param rootOrSelector If a string is provided, this parameter will be used as a query path on the document.
 * If an HTMLElement is provided, this parameter will be used as the root for the query.
 * @param selector * If `rootOrSelector` is an HTMLElement, this parameter must be a string and will be used as the query path.
 */
export declare function QueryAll<T extends Element = Element>(rootOrSelector: string | Nodes<ParentNode>, selector?: string): T[];
/**
 * Insert an item into an array, making sure to handle the return value of binarySearch correctly
 * @returns the index at which the item was inserted.
 */
export declare function optionsInsert(arr: HTMLOptionsCollection, item: HTMLOptionElement, index: number): number;
export declare function nodeWalkBreadthFirst<T extends Element>(node: Nodes<T>): Generator<Element, void, unknown>;
export declare function nodeWalkDepthFirst<T extends Element>(node: Nodes<T>): Generator<Element, void, unknown>;
export interface IBounds {
    left: number;
    top: number;
    right: number;
    bottom: number;
    width: number;
    height: number;
}
export interface IFullBounds {
    styles: CSSStyleDeclaration;
    margin: IBounds;
    border: IBounds;
    padding: IBounds;
    interior: IBounds;
}
export type BoundsCache = Map<Element, IFullBounds>;
export declare function elementComputeBounds<T extends Element>(element: Nodes<T>, cache?: BoundsCache): IFullBounds;
export declare function elementMaxBounds<T extends Element = Element>(element: Nodes<T>, cache?: BoundsCache): IBounds;
/**
 * Render an element to a canvas
 * @param element the element to render
 * @param resolution the number of pixels to render per inch. Defaults to 300 (a typical resolution for printing)
 * @returns
 */
export declare function renderHTMLElement<T extends Element = Element>(element: Nodes<T>, resolution?: number): HTMLCanvasElement;
export type HTMLDisablableElement = HTMLButtonElement | HTMLInputElement | HTMLFieldSetElement | HTMLSelectElement | HTMLTextAreaElement;
/**
 * Disables all of the disablable elements that are children of the given
 * parent element and returns the set of elements that were not disabled before
 * the function was called.
 * @param parent the element to search for disablable children
 */
export declare function disableChildren(parent: Nodes<ParentNode>): Set<HTMLDisablableElement>;
/**
 * Find the position of an element within it's parent element. Returns null
 * if the element is not currently a child of another element.
 * @param elem
 * @returns
 */
export declare function elementGetIndexInParent(elem: Nodes<ChildNode>): number;
export interface IDisableable {
    disabled: boolean;
}
export declare function isDisableable(obj: any): obj is IDisableable;
export declare function elementSetEnabled(node: Nodes, enabled: boolean): void;
export declare function elementRemoveFromParent(node: Nodes<ChildNode>): void;
export declare function elementClearChildren(node: Nodes<Element>): void;
export {};
//# sourceMappingURL=util.d.ts.map