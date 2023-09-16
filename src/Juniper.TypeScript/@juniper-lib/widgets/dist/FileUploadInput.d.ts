import { ErsatzElement } from "@juniper-lib/dom/dist/tags";
import { MediaType } from "@juniper-lib/mediatypes/dist";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
export declare class FileUploadInputEvent extends TypedEvent<"input"> {
    readonly files: File[];
    constructor(files: File[]);
}
type FileUploadInputEvents = {
    input: FileUploadInputEvent;
};
export declare class FileUploadInput extends TypedEventTarget<FileUploadInputEvents> implements ErsatzElement {
    private readonly file;
    private readonly typeFilters;
    private readonly onDragOver;
    private readonly onDragEnd;
    private readonly onDrop;
    readonly element: HTMLButtonElement;
    private _dragTarget;
    get dragTarget(): HTMLElement;
    set dragTarget(v: HTMLElement);
    constructor(buttonText: string, buttonStyle: "primary" | "danger", file: HTMLInputElement, dragTarget?: HTMLElement);
    show(): void;
    setTypeFilters(...types: MediaType[]): void;
    isExpectedType(contentType: string): boolean;
    get accept(): string;
    get enabled(): boolean;
    set enabled(v: boolean);
    get disabled(): boolean;
    set disabled(v: boolean);
    get files(): FileList;
    clear(): void;
}
export {};
//# sourceMappingURL=FileUploadInput.d.ts.map