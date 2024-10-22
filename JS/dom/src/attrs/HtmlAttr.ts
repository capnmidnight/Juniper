import { isBoolean, isNullOrUndefined, IToStringable } from "@juniper-lib/util";
import { AbstractKeyValueAppliable } from "./AbstractKeyValueAppliable";

export class HtmlAttr<ValueT extends string | boolean | IToStringable, ElementT extends Node = Node> extends AbstractKeyValueAppliable<ElementT, string, ValueT> {
    /**
     * Validates the tag type before assigning an attribute.
     * @param name the name of the attribute, for debugging purposes.
     * @param value the value to assign to the attribute
     */
    constructor(name: string, value: ValueT) {
        super(name, value);
    }

    override apply(tag: ElementT) {
        if (tag instanceof Element) {
            if (isBoolean(this.value)) {
                tag.toggleAttribute(this.name, this.value);
            }
            else if (isNullOrUndefined(this.value)) {
                tag.removeAttribute(this.name);
            }
            else {
                tag.setAttribute(this.name, this.value?.toString());
            }
        }
    }
}
