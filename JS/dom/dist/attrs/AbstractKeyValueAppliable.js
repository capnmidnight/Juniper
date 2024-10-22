import { AbstractAppliable } from "../AbstractAppliable";
export class AbstractKeyValueAppliable extends AbstractAppliable {
    #name;
    get name() { return this.#name; }
    #value;
    get value() { return this.#value; }
    /**
     * @param name the name of the attribute, for debugging purposes.
     * @param value the value to assign to the attribute
     */
    constructor(name, value) {
        super();
        this.#name = name;
        this.#value = value;
    }
}
//# sourceMappingURL=AbstractKeyValueAppliable.js.map