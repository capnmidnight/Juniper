import { ElementChild } from "@juniper-lib/dom";
export declare class PropertyGroupElement extends HTMLElement {
    constructor();
    get name(): string;
    set name(v: string);
    static install(): import("@juniper-lib/dom").ElementFactory<PropertyGroupElement>;
}
export declare function PropertyGroup(name: string, ...rest: ElementChild<PropertyGroupElement>[]): PropertyGroupElement;
//# sourceMappingURL=PropertyGroupElement.d.ts.map