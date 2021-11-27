import { ElementChild, ErsatzElement } from "./tags";
declare type PropertyElement = [string, ...ElementChild[]] | string | ElementChild;
declare class PropertyGroup {
    readonly name: string;
    readonly properties: PropertyElement[];
    constructor(name: string, ...properties: PropertyElement[]);
}
export declare function group(name: string, ...properties: PropertyElement[]): PropertyGroup;
declare type Property = PropertyElement | PropertyGroup;
export declare class PropertyList implements ErsatzElement {
    readonly element: HTMLElement;
    private readonly rowGroups;
    constructor(...rest: Property[]);
    append(...rest: Property[]): void;
    private createElements;
    private createGroups;
    private createRow;
    setGroupVisible(id: string, v: boolean): void;
}
export declare function PropList(...rest: Property[]): PropertyList;
export {};
