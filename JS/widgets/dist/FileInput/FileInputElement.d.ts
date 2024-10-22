import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
import { FileViewElement, FileViewValue } from "./FileViewElement";
export declare class FileInputElement extends TypedHTMLElement {
    #private;
    static observedAttributes: string[];
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get fileViews(): NodeListOf<FileViewElement>;
    get multiple(): boolean;
    set multiple(v: boolean);
    get disabled(): boolean;
    set disabled(v: boolean);
    get required(): boolean;
    set required(v: boolean);
    get readOnly(): boolean;
    set readOnly(v: boolean);
    get placeholder(): string;
    set placeholder(_v: string);
    get accept(): string;
    set accept(v: string);
    clear(): void;
    addFiles(files: FileViewValue[]): void;
    getFiles(): FileViewValue[];
    static install(): import("@juniper-lib/dom").ElementFactory<FileInputElement>;
}
export declare function FileInput(...rest: ElementChild<FileInputElement>[]): FileInputElement;
//# sourceMappingURL=FileInputElement.d.ts.map