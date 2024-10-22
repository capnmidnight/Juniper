import { Exception, isDefined, isObject, isString } from "@juniper-lib/util";
import { ElementChild, ElementFactory, HtmlTag } from "./HtmlTag";
import { Is } from "./attrs";


export interface TypedCustomElementConstructor<ElementT extends HTMLElement> {
    new(...params: any[]): ElementT;
}

function isElementDefinitionOptions(obj: unknown): obj is ElementDefinitionOptions {
    return isObject(obj)
        && "extends" in obj
        && isString(obj.extends);
}

export function registerFactory<ElementT extends HTMLElement>(
    tagName: string,
    Constructor: TypedCustomElementConstructor<ElementT>,
    ...defaults: ElementChild[]
): ElementFactory<ElementT>;
export function registerFactory<ElementT extends HTMLElement>(
    tagName: string,
    Constructor: TypedCustomElementConstructor<ElementT>,
    options: ElementDefinitionOptions,
    ...defaults: ElementChild[]
): ElementFactory<ElementT>;
export function registerFactory<ElementT extends HTMLElement>(
    tagName: string,
    Constructor: TypedCustomElementConstructor<ElementT>,
    optionsOrFirstDefault?: ElementDefinitionOptions | ElementChild,
    ...defaults: ElementChild[]
): ElementFactory<ElementT> {

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

    let options: ElementDefinitionOptions;
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
        return function (...rest: ElementChild<ElementT>[]) {
            return HtmlTag<ElementT>(options.extends, Is(tagName), ...defaults, ...rest);
        };
    }
    else {
        return function (...rest: ElementChild<ElementT>[]) {
            return HtmlTag<ElementT>(tagName, ...defaults, ...rest);
        };
    }
}
