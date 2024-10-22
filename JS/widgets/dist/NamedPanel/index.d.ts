import { ElementChild, ErsatzElement } from "@juniper-lib/dom/dist/tags";
import "./style.css";
export declare class NamedPanel implements ErsatzElement {
    private _title;
    readonly element: HTMLElement;
    private readonly header;
    private readonly titleText;
    private readonly body;
    private _open;
    refresh: () => void;
    constructor(_title: string, ...rest: ElementChild[]);
    get title(): string;
    set title(v: string);
    get open(): boolean;
    set open(v: boolean);
    private onRefresh;
}
//# sourceMappingURL=index.d.ts.map