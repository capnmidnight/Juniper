import { arrayRemove } from "@juniper-lib/util";
import { HtmlAttr } from "./attrs";
import { ElementChild } from "./tags";
import { HtmlRender } from "./util";

/**
 * Creates an SVG element.
 * @param name the name of the element to create
 * @param sub attribute assigner functions and sub element children objects.
 */
export function SvgTag<T extends keyof SVGElementTagNameMap>(name: T, ...sub: ElementChild[]): SVGElementTagNameMap[T] {
    sub = sub.flat(1);

    let tag: SVGElementTagNameMap[T];

    const finders: HtmlAttr[] = sub
        .filter(v => v instanceof HtmlAttr && (v.name === "id" || v.name === "query"))
        .map(v => v as HtmlAttr);

    for (const finder of finders) {
        if (finder.name === "id") {
            const id = finder.value?.toString();
            if (id) {
                tag = document.getElementById(id) as unknown as SVGElementTagNameMap[T];
                if (tag) {
                    arrayRemove(sub, finder);
                }
            }
        }
        else {
            arrayRemove(sub, finder);
            tag = finder.value as unknown as SVGElementTagNameMap[T];
        }

        if (tag) {
            break;
        }
    }

    if (!tag) {
        tag = document.createElementNS<T>("http://www.w3.org/2000/svg", name);
    }

    return HtmlRender(tag, ...sub);
}

export function Svg(...sub: ElementChild[]) { return SvgTag("svg", ...sub); }
export function G(...sub: ElementChild[]) { return SvgTag("g", ...sub); }
export function Path(...sub: ElementChild[]) { return SvgTag("path", ...sub); }
export function TitleSvg(...sub: ElementChild[]) { return SvgTag("title", ...sub); }
export function TextSvg(...sub: ElementChild[]) { return SvgTag("text", ...sub); }
export function Circle(...sub: ElementChild[]) { return SvgTag("circle", ...sub); }
export function CX(value: number) { return new HtmlAttr("cx", value, true, "circle"); }
export function CY(value: number) { return new HtmlAttr("cy", value, true, "circle"); }
export function R(value: number) { return new HtmlAttr("r", value, true, "circle"); }
export function Transform(value: string) { return new HtmlAttr("transform", value, true, "g"); }
export function D(value: string) { return new HtmlAttr("d", value, true, "path"); }
export function ViewBox(value: string) { return new HtmlAttr("viewBox", value, true, "svg"); }
export function WidthSvg(value: string) { return new HtmlAttr("width", value, true, "svg"); }
export function HeightSvg(value: string) { return new HtmlAttr("height", value, true, "svg"); }
export function DominantBaseline(value: string) { return new HtmlAttr("dominant-baseline", value, true, "text"); }
export function FontSize(value: string) { return new HtmlAttr("font-size", value, true, "text"); }
export function TextAnchor(value: string) { return new HtmlAttr("text-anchor", value, true, "text"); }
export function X(value: number) { return new HtmlAttr("x", value, true, "text"); }
export function Y(value: number) { return new HtmlAttr("y", value, true, "text"); }