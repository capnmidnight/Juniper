import { ErsatzElement } from "./tags";
export declare abstract class DialogBox implements ErsatzElement {
    readonly element: HTMLDivElement;
    private subEventer;
    private _title;
    private readonly titleElement;
    protected readonly container: HTMLDivElement;
    protected readonly contentArea: HTMLDivElement;
    protected readonly confirmButton: HTMLButtonElement;
    protected readonly cancelButton: HTMLButtonElement;
    constructor(title: string);
    get title(): string;
    set title(v: string);
    protected onShowing(): Promise<void>;
    protected onShown(): void;
    protected onConfirm(): Promise<void>;
    protected onCancel(): void;
    protected onClosing(): Promise<void>;
    protected onClosed(): void;
    showDialog(): Promise<boolean>;
}
