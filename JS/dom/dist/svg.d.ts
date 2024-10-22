import { ElementChild } from "./HtmlTag";
import { HtmlAttr } from "./attrs/HtmlAttr";
export declare function Svg(...sub: ElementChild[]): SVGSVGElement;
export declare function G(...sub: ElementChild[]): SVGGElement;
export declare function Path(...sub: ElementChild[]): SVGPathElement;
export declare function TitleSvg(...sub: ElementChild[]): SVGTitleElement;
export declare function TextSvg(...sub: ElementChild[]): SVGTextElement;
export declare function Circle(...sub: ElementChild[]): SVGCircleElement;
export declare function CX(value: number): HtmlAttr<number, Node>;
export declare function CY(value: number): HtmlAttr<number, Node>;
export declare function R(value: number): HtmlAttr<number, Node>;
export declare function Transform(value: string): HtmlAttr<string, Node>;
export declare function D(value: string): HtmlAttr<string, Node>;
export declare function ViewBox(value: string): HtmlAttr<string, Node>;
export declare function WidthSvg(value: string): HtmlAttr<string, Node>;
export declare function HeightSvg(value: string): HtmlAttr<string, Node>;
export declare function DominantBaseline(value: string): HtmlAttr<string, Node>;
export declare function FontSize(value: string): HtmlAttr<string, Node>;
export declare function TextAnchor(value: string): HtmlAttr<string, Node>;
export declare function X(value: number): HtmlAttr<number, Node>;
export declare function Y(value: number): HtmlAttr<number, Node>;
/**
 * Render an SVG element to Canvas
 */
export declare function renderSVGElement(svg: SVGSVGElement): Promise<HTMLCanvasElement>;
//# sourceMappingURL=svg.d.ts.map