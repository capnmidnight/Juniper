import { ElementChild, SvgTag } from "./HtmlTag";
import { Height, Src, Width } from "./attrs";
import { HtmlAttr } from "./attrs/HtmlAttr";
import { OnLoad } from "./events";
import { Canvas, Img } from "./tags";

export function Svg(...sub: ElementChild[]) { return SvgTag("svg", ...sub); }
export function G(...sub: ElementChild[]) { return SvgTag("g", ...sub); }
export function Path(...sub: ElementChild[]) { return SvgTag("path", ...sub); }
export function TitleSvg(...sub: ElementChild[]) { return SvgTag("title", ...sub); }
export function TextSvg(...sub: ElementChild[]) { return SvgTag("text", ...sub); }
export function Circle(...sub: ElementChild[]) { return SvgTag("circle", ...sub); }
export function CX(value: number) { return new HtmlAttr("cx", value); }
export function CY(value: number) { return new HtmlAttr("cy", value); }
export function R(value: number) { return new HtmlAttr("r", value); }
export function Transform(value: string) { return new HtmlAttr("transform", value); }
export function D(value: string) { return new HtmlAttr("d", value); }
export function ViewBox(value: string) { return new HtmlAttr("viewBox", value); }
export function WidthSvg(value: string) { return new HtmlAttr("width", value); }
export function HeightSvg(value: string) { return new HtmlAttr("height", value); }
export function DominantBaseline(value: string) { return new HtmlAttr("dominant-baseline", value); }
export function FontSize(value: string) { return new HtmlAttr("font-size", value); }
export function TextAnchor(value: string) { return new HtmlAttr("text-anchor", value); }
export function X(value: number) { return new HtmlAttr("x", value); }
export function Y(value: number) { return new HtmlAttr("y", value); }


/**
 * Render an SVG element to Canvas
 */
export function renderSVGElement(svg: SVGSVGElement) {
    return new Promise<HTMLCanvasElement>(resolve => {
        const svgData = new XMLSerializer().serializeToString(svg);
        const image = Img(
            Src("data:image/svg+xml;charset=utf-8," + encodeURIComponent(svgData)),
            OnLoad(() => {

                const canvas = Canvas(
                    Width(image.width),
                    Height(image.height)
                );
                const context = canvas.getContext('2d');

                context.drawImage(image, 0, 0);
                resolve(canvas);
            }));
    });
}
