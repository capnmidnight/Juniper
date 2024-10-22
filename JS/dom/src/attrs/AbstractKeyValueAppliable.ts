import { AbstractAppliable } from "../AbstractAppliable";


export abstract class AbstractKeyValueAppliable<ElementT extends Node, AttributeT, ValueT> extends AbstractAppliable<ElementT> {
    #name: AttributeT;
    get name() { return this.#name; }

    #value: ValueT;
    get value() { return this.#value; }

    /**
     * @param name the name of the attribute, for debugging purposes.
     * @param value the value to assign to the attribute
     */
    constructor(name: AttributeT, value: ValueT) {
        super();

        this.#name = name;
        this.#value = value;
    }
}
