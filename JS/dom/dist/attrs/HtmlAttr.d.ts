import { IToStringable } from "@juniper-lib/util";
import { AbstractKeyValueAppliable } from "./AbstractKeyValueAppliable";
export declare class HtmlAttr<ValueT extends string | boolean | IToStringable, ElementT extends Node = Node> extends AbstractKeyValueAppliable<ElementT, string, ValueT> {
    /**
     * Validates the tag type before assigning an attribute.
     * @param name the name of the attribute, for debugging purposes.
     * @param value the value to assign to the attribute
     */
    constructor(name: string, value: ValueT);
    apply(tag: ElementT): void;
}
//# sourceMappingURL=HtmlAttr.d.ts.map