import { TypedHTMLElement } from "@juniper-lib/dom";
export declare class ContextMenuElement extends TypedHTMLElement {
    private currentTask;
    private mouseX;
    private mouseY;
    constructor();
    cancel(): Promise<void>;
    show<T>(displayNames: Map<T, string>, ...options: (T | HTMLHRElement)[]): Promise<T | null>;
    show<T>(...options: (T | HTMLHRElement)[]): Promise<T | null>;
    static install(): import("@juniper-lib/dom").ElementFactory<ContextMenuElement>;
}
export declare function ContextMenu(): ContextMenuElement;
//# sourceMappingURL=index.d.ts.map