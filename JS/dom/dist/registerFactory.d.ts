import { ElementChild, ElementFactory } from "./HtmlTag";
export interface TypedCustomElementConstructor<ElementT extends HTMLElement> {
    new (...params: any[]): ElementT;
}
export declare function registerFactory<ElementT extends HTMLElement>(tagName: string, Constructor: TypedCustomElementConstructor<ElementT>, ...defaults: ElementChild[]): ElementFactory<ElementT>;
export declare function registerFactory<ElementT extends HTMLElement>(tagName: string, Constructor: TypedCustomElementConstructor<ElementT>, options: ElementDefinitionOptions, ...defaults: ElementChild[]): ElementFactory<ElementT>;
//# sourceMappingURL=registerFactory.d.ts.map