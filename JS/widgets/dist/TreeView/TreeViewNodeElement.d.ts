import { TreeNode } from "@juniper-lib/collections";
import { ElementChild } from "@juniper-lib/dom";
import { ITypedEventTarget, TypedEvent, TypedEventListenerOrEventListenerObject } from "@juniper-lib/events";
declare abstract class BaseTreeViewNodeEvent<EventT extends string, ValueT> extends TypedEvent<EventT, TreeViewNodeElement<ValueT>> {
    constructor(type: EventT);
}
export declare class TreeViewNodeClickedEvent<ValueT> extends BaseTreeViewNodeEvent<"click", ValueT> {
    constructor();
}
export declare class TreeViewNodeSelectedEvent<ValueT> extends BaseTreeViewNodeEvent<"select", ValueT> {
    constructor();
}
export declare class TreeViewNodeCreateEvent<ValueT> extends BaseTreeViewNodeEvent<"create", ValueT> {
    readonly node: TreeNode<ValueT>;
    constructor(node: TreeNode<ValueT>);
}
export declare abstract class BaseTreeViewNodeActionEvent<EventT extends string, ValueT> extends BaseTreeViewNodeEvent<EventT, ValueT> {
    private readonly _finished;
    get finished(): Promise<void>;
    constructor(type: EventT);
    complete(): void;
}
export declare class TreeViewNodeAddEvent<T> extends BaseTreeViewNodeActionEvent<"add", T> {
    constructor();
}
export declare class TreeViewNodeContextMenuEvent<T> extends BaseTreeViewNodeActionEvent<"contextmenu", T> {
    constructor();
}
export type TreeViewNodeEvents<T> = {
    click: TreeViewNodeClickedEvent<T>;
    select: TreeViewNodeSelectedEvent<T>;
    create: TreeViewNodeCreateEvent<T>;
    add: TreeViewNodeAddEvent<T>;
    contextmenu: TreeViewNodeContextMenuEvent<T>;
};
export declare class TreeViewNodeElement<T> extends HTMLElement implements ITypedEventTarget<TreeViewNodeEvents<T>> {
    static create<T>(node: TreeNode<T>, defaultLabel: string, getLabel: (value: T) => string, getDescription: (value: T) => string, canChangeOrder: (value: T) => boolean, getChildDescription: (node: T) => string, canHaveChildren: (node: TreeNode<T>) => boolean): TreeViewNodeElement<T>;
    private readonly eventTarget;
    private readonly root;
    readonly childTreeNodes: HTMLElement;
    private readonly infoView;
    private readonly subView;
    private readonly collapser;
    private readonly labeler;
    private readonly adder;
    readonly upper: HTMLDivElement;
    readonly lower: HTMLDivElement;
    readonly refresh: () => void;
    private _node;
    get node(): TreeNode<T>;
    private set node(value);
    private defaultLabel;
    private _getLabel;
    private _getDescription;
    private _canChangeOrder;
    private _getChildDescription;
    private _canHaveChildren;
    constructor();
    connectedCallback(): void;
    addEventListener<EventTypeT extends keyof TreeViewNodeEvents<T>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TreeViewNodeEvents<T>, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<EventTypeT extends keyof TreeViewNodeEvents<T>>(type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TreeViewNodeEvents<T>, EventTypeT>): void;
    dispatchEvent(evt: Event): boolean;
    addBubbler(bubbler: ITypedEventTarget<TreeViewNodeEvents<T>>): void;
    removeBubbler(bubbler: ITypedEventTarget<TreeViewNodeEvents<T>>): void;
    addScopedEventListener<EventTypeT extends keyof TreeViewNodeEvents<T>>(scope: object, type: EventTypeT, callback: TypedEventListenerOrEventListenerObject<TreeViewNodeEvents<T>, EventTypeT>, options?: boolean | AddEventListenerOptions): void;
    removeScope(scope: object): void;
    clearEventListeners<EventTypeT extends keyof TreeViewNodeEvents<T>>(type?: EventTypeT): void;
    _launchMenu(parentEvt: Event, evt: BaseTreeViewNodeActionEvent<string, T>): Promise<void>;
    private onRefresh;
    get specialSelectMode(): boolean;
    set specialSelectMode(v: boolean);
    get canHaveChildren(): boolean;
    private get label();
    private get description();
    private get childDescription();
    private get canChangeOrder();
    private get collapserTitle();
    private get adderTitle();
    get disabled(): boolean;
    set disabled(v: boolean);
    get filtered(): boolean;
    set _filtered(v: boolean);
    get selected(): boolean;
    set _selected(v: boolean);
    get highlighted(): number;
    set highlighted(v: number);
    get open(): boolean;
    set open(v: boolean);
    add(value: T): void;
    _select(bubbles: boolean): void;
    scrollIntoView(arg?: boolean | ScrollIntoViewOptions): void;
    static install(): import("@juniper-lib/dom").ElementFactory<TreeViewNodeElement<unknown>>;
}
export declare function TreeViewNode<T>(...rest: ElementChild[]): TreeViewNodeElement<T>;
export {};
//# sourceMappingURL=TreeViewNodeElement.d.ts.map