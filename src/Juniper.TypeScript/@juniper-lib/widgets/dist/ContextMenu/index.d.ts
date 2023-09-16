import { ErsatzElement } from "@juniper-lib/dom/dist/tags";
import "./styles.css";
export declare class ContextMenu implements ErsatzElement {
    readonly element: HTMLElement;
    private currentTask;
    private mouseX;
    private mouseY;
    constructor();
    cancel(): Promise<void>;
    show<T>(displayNames: Map<T, string>, ...options: (T | HTMLHRElement)[]): Promise<T | null>;
    show<T>(...options: (T | HTMLHRElement)[]): Promise<T | null>;
}
//# sourceMappingURL=index.d.ts.map