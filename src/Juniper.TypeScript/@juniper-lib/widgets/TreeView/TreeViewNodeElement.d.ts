import { TreeNode } from "@juniper-lib/collections/TreeNode";
import { ITypedEventTarget, TypedEvent, TypedEventListenerOrEventListenerObject } from "@juniper-lib/events/TypedEventTarget";
export declare class TreeViewNodeClickedEvent<T> extends TypedEvent<"click"> {
    readonly node: TreeNode<T>;
    constructor(node: TreeNode<T>);
}
export declare class TreeViewNodeSelectedEvent<T> extends TypedEvent<"select"> {
    readonly node: TreeNode<T>;
    constructor(node: TreeNode<T>);
}
export declare abstract class TreeViewNodeEvent<EventT extends string, ValueT> extends TypedEvent<EventT> {
    readonly parent: TreeViewNodeElement<ValueT>;
    private readonly _finished;
    get finished(): Promise<void>;
    constructor(type: EventT, parent: TreeViewNodeElement<ValueT>);
    complete(): void;
}
export declare class TreeViewNodeAddEvent<T> extends TreeViewNodeEvent<"add", T> {
    constructor(parent: TreeViewNodeElement<T>);
}
export declare class TreeViewNodeContextMenuEvent<T> extends TreeViewNodeEvent<"contextmenu", T> {
    constructor(parent: TreeViewNodeElement<T>);
}
export type TreeViewNodeEvents<T> = {
    click: TreeViewNodeClickedEvent<T>;
    select: TreeViewNodeSelectedEvent<T>;
    add: TreeViewNodeAddEvent<T>;
    contextmenu: TreeViewNodeContextMenuEvent<T>;
};
export declare class TreeViewNodeElement<T> extends HTMLElement implements ITypedEventTarget<TreeViewNodeEvents<T>> {
    static create<T>(node: TreeNode<T>, defaultLabel: string, getLabel: (value: T) => string, getDescription: (value: T) => string, canChangeOrder: (value: T) => boolean, getChildDescription: (node: T) => string, canHaveChildren: (node: TreeNode<T>) => boolean, createElement: (node: TreeNode<T>) => TreeViewNodeElement<T>): TreeViewNodeElement<T>;
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
    private createElement;
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
    _launchMenu(parentEvt: Event, evt: TreeViewNodeEvent<string, T>): Promise<void>;
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
}
//# sourceMappingURL=TreeViewNodeElement.d.ts.map