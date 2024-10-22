import { AbstractKeyValueAppliable } from "./AbstractKeyValueAppliable";
export class HtmlProp extends AbstractKeyValueAppliable {
    /**
     * Validates the tag type before assigning an attribute.
     * @param name the name of the attribute, for debugging purposes.
     * @param value the value to assign to the attribute
     * @param allowableTags
     */
    constructor(name, value) {
        super(name, value);
    }
    apply(tag) {
        tag[this.name] = this.value;
    }
}
export function TypedHtmlProp(name, value) {
    return new HtmlProp(name, value);
}
//# sourceMappingURL=HtmlProp.js.map