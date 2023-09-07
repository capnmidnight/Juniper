import { HtmlAttr } from "@juniper-lib/dom/attrs";
import { CssElementStyleProp } from "@juniper-lib/dom/css";
import { ElementChild, ErsatzElement, IElementAppliable } from "@juniper-lib/dom/tags";
import "./styles.css";
type PropertyChild = Exclude<ElementChild<HTMLElement>, IElementAppliable>;
type PropertyElement = [string, ...PropertyChild[]] | string | PropertyChild;
declare class PropertyGroup {
    readonly name: string;
    readonly properties: PropertyElement[];
    constructor(name: string, ...properties: PropertyElement[]);
}
export declare function group(name: string, ...properties: PropertyElement[]): PropertyGroup;
export type Property = PropertyElement | PropertyGroup;
export type PropertyDef = Property | HtmlAttr | CssElementStyleProp;
export declare class PropertyList implements ErsatzElement {
    readonly element: HTMLElement;
    private readonly groups;
    private readonly controls;
    private _disabled;
    static find(): PropertyList[];
    private static _find;
    static create(...rest: PropertyDef[]): PropertyList;
    constructor(element: HTMLElement);
    append(...props: Property[]): void;
    private checkGroup;
    get disabled(): boolean;
    set disabled(v: boolean);
    get enabled(): boolean;
    set enabled(v: boolean);
    setGroupVisible(id: string, v: boolean): void;
    getGroupVisible(id: string): boolean;
}
export {};
//# sourceMappingURL=index.d.ts.map