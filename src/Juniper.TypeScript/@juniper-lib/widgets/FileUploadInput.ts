import { ClassList } from "@juniper-lib/dom/attrs";
import { onClick, onDragEnd, onDragLeave, onDragOver, onDrop } from "@juniper-lib/dom/evts";
import { Button, HtmlRender, elementSetText, ErsatzElement } from "@juniper-lib/dom/tags";
import { MediaType } from "@juniper-lib/mediatypes";
import { mediaTypesToAcceptValue } from "@juniper-lib/mediatypes/util";
import { arrayReplace } from "@juniper-lib/collections/arrays";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventBase";
import { isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";

export class FileUploadInputEvent extends TypedEvent<"input"> {
    constructor(public readonly files: File[]) {
        super("input");
    }
}

type FileUploadInputEvents = {
    input: FileUploadInputEvent;
}

export class FileUploadInput
    extends TypedEventTarget<FileUploadInputEvents>
    implements ErsatzElement {

    private readonly typeFilters = new Array<MediaType>();
    private readonly onDragOver: (evt: DragEvent) => void;
    private readonly onDragEnd: (evt: DragEvent) => void;
    private readonly onDrop: (evt: DragEvent) => void;

    readonly element: HTMLButtonElement = null;

    private _dragTarget: HTMLElement = null;

    get dragTarget() {
        return this._dragTarget;
    }

    set dragTarget(v) {
        if (v !== this.dragTarget) {
            if (this.dragTarget) {
                this.dragTarget.removeEventListener("dragover", this.onDragOver);
                this.dragTarget.removeEventListener("dragend", this.onDragEnd);
                this.dragTarget.removeEventListener("dragleave", this.onDragEnd);
                this.dragTarget.removeEventListener("drop", this.onDrop);
            }

            this._dragTarget = v;

            if (this.dragTarget) {
                HtmlRender(this.dragTarget,
                    onDragOver(this.onDragOver),
                    onDragLeave(this.onDragEnd),
                    onDragEnd(this.onDragEnd),
                    onDrop(this.onDrop)
                );
            }
        }
    }

    constructor(buttonText: string, buttonStyle: "primary" | "danger", private readonly file: HTMLInputElement, dragTarget: HTMLElement = null) {
        super();

        const getMatchingFiles = (fileList: FileList) =>
            Array.from(fileList)
                .filter(f =>
                    this.typeFilters.length === 0 || this.typeFilters
                        .filter(t => t.matches(f.type))
                        .length > 0);

        const getMatchingItems = (itemList: DataTransferItemList) =>
            Array.from(itemList)
                .filter(f => f.kind == "file"
                    && (this.typeFilters.length === 0
                        || this.typeFilters
                            .filter(t => t.matches(f.type))
                            .length > 0));

        this.onDragOver = (evt) => {
            if (this.enabled) {
                const items = getMatchingItems(evt.dataTransfer.items);
                if (items.length > 0) {
                    elementSetText(this.element, "Drop file...");
                }
                else {
                    elementSetText(this.element, "No files matching expected type(s)");
                }
            }
            evt.preventDefault();
        };

        this.onDragEnd = (evt) => {
            if (this.enabled) {
                elementSetText(this.element, buttonText);
            }
            evt.preventDefault();
        };

        this.onDrop = (evt) => {
            if (this.enabled) {
                select(evt.dataTransfer.files);
            }
            this.onDragEnd(evt);
        };

        const select = (fileList: FileList) => {
            const files = getMatchingFiles(fileList);
            if (files.length > 0) {
                this.dispatchEvent(new FileUploadInputEvent(files));
            }
        };

        this.file.style.display = "none";
        this.file.insertAdjacentElement("afterend",
            this.element = Button(
                ClassList("btn", `btn-${buttonStyle}`),
                onClick(() => this.show()),
                buttonText
            )
        );

        this.dragTarget = dragTarget || this.element;

        this.file.addEventListener("input", () => select(this.file.files));

        this.setTypeFilters();
        this.enabled = true;
    }

    show() {
        if (this.file.showPicker) {
            this.file.showPicker();
        }
        else {
            this.file.click();
        }
    }

    setTypeFilters(...types: MediaType[]): void {
        arrayReplace(this.typeFilters, ...types);
        this.file.accept = mediaTypesToAcceptValue(types);
    }

    isExpectedType(contentType: string): boolean {
        if (isNullOrUndefined(contentType)) {
            return false;
        }

        if (this.typeFilters.length === 0) {
            return true;
        }

        return this.typeFilters
            .map(t => t.matches(contentType))
            .reduce((a, b) => a || b, false);
    }

    get accept(): string {
        return this.file.accept;
    }

    get enabled(): boolean {
        return !this.file.disabled;
    }

    set enabled(v: boolean) {
        this.file.disabled = !v;
        this.element.disabled = !v;
    }

    get disabled() {
        return !this.enabled;
    }

    set disabled(v) {
        this.enabled = !v;
    }

    get files(): FileList {
        return this.file.files;
    }

    clear(): void {
        this.file.value = null;
    }
}
