import { AbstractKeyValueAppliable } from "./AbstractKeyValueAppliable";

type ExtendedNode<AttributeT extends string, ValueT> = Node & Record<AttributeT, ValueT>;

export class HtmlProp<
    AttributeT extends string, 
    ValueT, 
    ElementT extends Node = ExtendedNode<AttributeT, ValueT>
> extends AbstractKeyValueAppliable<ElementT, AttributeT, ValueT> {

    /**
     * Validates the tag type before assigning an attribute.
     * @param name the name of the attribute, for debugging purposes.
     * @param value the value to assign to the attribute
     * @param allowableTags
     */
    constructor(name: AttributeT, value: ValueT) {
        super(name, value);
    }

    override apply(tag: ElementT) {
        (tag as any)[this.name] = this.value;
    }
}

export function TypedHtmlProp<
    ElementT extends HTMLElement,
    AttributeT extends string & keyof ElementT = string & keyof ElementT,
    ValueT extends ElementT[AttributeT] = ElementT[AttributeT]>(name: AttributeT, value: ValueT) {
    return new HtmlProp<AttributeT, ValueT, ElementT>(name, value);
}