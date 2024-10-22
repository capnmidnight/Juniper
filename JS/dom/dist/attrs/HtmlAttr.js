import { isBoolean, isNullOrUndefined } from "@juniper-lib/util";
import { AbstractKeyValueAppliable } from "./AbstractKeyValueAppliable";
export class HtmlAttr extends AbstractKeyValueAppliable {
    /**
     * Validates the tag type before assigning an attribute.
     * @param name the name of the attribute, for debugging purposes.
     * @param value the value to assign to the attribute
     */
    constructor(name, value) {
        super(name, value);
    }
    apply(tag) {
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
//# sourceMappingURL=HtmlAttr.js.map