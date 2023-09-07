import { ErsatzElement } from "@juniper-lib/dom/tags";
import "./styles.css";
export declare abstract class DialogBox implements ErsatzElement {
    readonly element: HTMLDivElement;
    private readonly task;
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
    private show;
    get isOpen(): boolean;
    hide(): void;
    toggle(): Promise<void>;
    showDialog(): Promise<boolean>;
}
//# sourceMappingURL=index.d.ts.map