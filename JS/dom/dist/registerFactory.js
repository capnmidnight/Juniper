import { Exception, isDefined, isObject, isString } from "@juniper-lib/util";
import { HtmlTag } from "./HtmlTag";
import { Is } from "./attrs";
function isElementDefinitionOptions(obj) {
    return isObject(obj)
        && "extends" in obj
        && isString(obj.extends);
}
export function registerFactory(tagName, Constructor, optionsOrFirstDefault, ...defaults) {
    const existingConstructor = customElements.get(tagName);
    if (isDefined(existingConstructor) && existingConstructor !== Constructor) {
        console.error("Attempted to re-use a custom element name: ", tagName, existingConstructor);
        throw new Exception("Attempted to re-use a custom element name: " + tagName);
    }
    const existingName = customElements.getName(Constructor);
    if (isDefined(existingName) && existingName !== tagName) {
        console.error("Attempted to overload a custom element: ", tagName, existingName);
        throw new Exception(`Attempted to overload a custom element: ${tagName}, ${existingName}`);
    }
    let options;
    if (isElementDefinitionOptions(optionsOrFirstDefault)) {
        options = optionsOrFirstDefault;
    }
    else if (isDefined(optionsOrFirstDefault)) {
        defaults.unshift(optionsOrFirstDefault);
    }
    if (!customElements.get(tagName)) {
        customElements.define(tagName, Constructor, options);
    }
    if (isDefined(options)) {
        return function (...rest) {
            return HtmlTag(options.extends, Is(tagName), ...defaults, ...rest);
        };
    }
    else {
        return function (...rest) {
            return HtmlTag(tagName, ...defaults, ...rest);
        };
    }
}
//# sourceMappingURL=registerFactory.js.map