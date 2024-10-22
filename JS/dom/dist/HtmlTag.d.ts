import { AbstractAppliable } from "./AbstractAppliable";
import { HtmlIDAttr, HtmlIsAttr } from "./attrs";
import { HtmlQueryAttr } from "./util";
import { ErsatzNode } from "./ErsatzNode";
export type RawElementChild = Node | string | number | Date;
export type ElementChild<T extends Node = Node> = RawElementChild | ErsatzNode<Node> | AbstractAppliable<T>;
export type ElementFactory<ElementT extends Node = Node> = (...rest: ElementChild<ElementT>[]) => ElementT;
type FinderAttr = HtmlIDAttr | HtmlQueryAttr;
type HtmlFinderAttr = FinderAttr | HtmlIsAttr;
export declare function isFinderAttr(v: ElementChild): v is FinderAttr;
export declare function isHtmlFinderAttr(v: ElementChild): v is HtmlFinderAttr;
/**
 * Creates an DOM element.
 */
export declare function HtmlTag<T extends HTMLElement>(name: string, ...sub: ElementChild<T>[]): T;
/**
 * Creates an SVG element.
 * @param name the name of the element to create
 * @param sub attribute assigner functions and sub element children objects.
 */
export declare function SvgTag<T extends keyof SVGElementTagNameMap>(name: T, ...sub: ElementChild[]): SVGElementTagNameMap[T];
export {};
//# sourceMappingURL=HtmlTag.d.ts.map