import { AbstractAppliable } from "../AbstractAppliable";
export declare abstract class AbstractKeyValueAppliable<ElementT extends Node, AttributeT, ValueT> extends AbstractAppliable<ElementT> {
    #private;
    get name(): AttributeT;
    get value(): ValueT;
    /**
     * @param name the name of the attribute, for debugging purposes.
     * @param value the value to assign to the attribute
     */
    constructor(name: AttributeT, value: ValueT);
}
//# sourceMappingURL=AbstractKeyValueAppliable.d.ts.map