import { SvgTag } from "./HtmlTag";
import { Height, Src, Width } from "./attrs";
import { HtmlAttr } from "./attrs/HtmlAttr";
import { OnLoad } from "./events";
import { Canvas, Img } from "./tags";
export function Svg(...sub) { return SvgTag("svg", ...sub); }
export function G(...sub) { return SvgTag("g", ...sub); }
export function Path(...sub) { return SvgTag("path", ...sub); }
export function TitleSvg(...sub) { return SvgTag("title", ...sub); }
export function TextSvg(...sub) { return SvgTag("text", ...sub); }
export function Circle(...sub) { return SvgTag("circle", ...sub); }
export function CX(value) { return new HtmlAttr("cx", value); }
export function CY(value) { return new HtmlAttr("cy", value); }
export function R(value) { return new HtmlAttr("r", value); }
export function Transform(value) { return new HtmlAttr("transform", value); }
export function D(value) { return new HtmlAttr("d", value); }
export function ViewBox(value) { return new HtmlAttr("viewBox", value); }
export function WidthSvg(value) { return new HtmlAttr("width", value); }
export function HeightSvg(value) { return new HtmlAttr("height", value); }
export function DominantBaseline(value) { return new HtmlAttr("dominant-baseline", value); }
export function FontSize(value) { return new HtmlAttr("font-size", value); }
export function TextAnchor(value) { return new HtmlAttr("text-anchor", value); }
export function X(value) { return new HtmlAttr("x", value); }
export function Y(value) { return new HtmlAttr("y", value); }
/**
 * Render an SVG element to Canvas
 */
export function renderSVGElement(svg) {
    return new Promise(resolve => {
        const svgData = new XMLSerializer().serializeToString(svg);
        const image = Img(Src("data:image/svg+xml;charset=utf-8," + encodeURIComponent(svgData)), OnLoad(() => {
            const canvas = Canvas(Width(image.width), Height(image.height));
            const context = canvas.getContext('2d');
            context.drawImage(image, 0, 0);
            resolve(canvas);
        }));
    });
}
//# sourceMappingURL=svg.js.map