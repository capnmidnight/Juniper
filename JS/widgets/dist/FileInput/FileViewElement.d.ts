import { ElementChild, TypedHTMLElement } from "@juniper-lib/dom";
import { RemovingEvent } from "../ArrayViewElement";
import { IndexChangedEvent } from "./IndexChangedEvent";
import { RequestInputEvent } from "./RequestInputEvent";
export interface IBasicFile {
    name: string;
    type: string;
    size: number;
}
export type FileViewValue = string | URL | IBasicFile;
export type FileViewElementEvents = {
    "indexchanged": IndexChangedEvent;
    "removing": RemovingEvent<FileViewValue>;
    "requestinput": RequestInputEvent<IBasicFile, string>;
};
export declare class FileViewElement extends TypedHTMLElement<FileViewElementEvents> {
    #private;
    static observedAttributes: string[];
    constructor();
    connectedCallback(): void;
    attributeChangedCallback(name: string, oldValue: string, newValue: string): void;
    get file(): FileViewValue;
    set file(v: FileViewValue);
    get index(): number;
    get count(): number;
    setPosition(index: number, count: number): void;
    get deleting(): boolean;
    set deleting(v: boolean);
    get disabled(): boolean;
    set disabled(v: boolean);
    get readOnly(): boolean;
    set readOnly(v: boolean);
    static install(): import("@juniper-lib/dom").ElementFactory<FileViewElement>;
}
export declare function FileView(...rest: ElementChild<FileViewElement>[]): FileViewElement;
//# sourceMappingURL=FileViewElement.d.ts.map