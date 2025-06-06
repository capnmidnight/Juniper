import { ComparisonResult } from "@juniper-lib/util";
import { TreeNode } from "@juniper-lib/collections";
import { CssElementStyleProp, ElementChild, HtmlAttr, TypedHTMLElement } from "@juniper-lib/dom";
import { TypedEvent } from "@juniper-lib/events";
import { TreeViewNodeEvents } from './TreeViewNodeElement';
export interface TreeViewOptions<ValueT, FilterTypeT extends string = never> {
    defaultLabel?: string;
    getLabel: (value: ValueT) => string;
    getParent: (value: ValueT) => ValueT;
    getOrder?: (value: ValueT) => ComparisonResult;
    getDescription: (value: ValueT) => string;
    showNameFilter?: boolean;
    typeFilters?: {
        getTypes: () => FilterTypeT[];
        getTypeLabel: (type: FilterTypeT) => string;
        getTypeFor: (value: ValueT) => FilterTypeT;
    };
    canReorder?: (value: ValueT) => boolean;
    getChildDescription: (value: ValueT) => string;
    canHaveChildren: (node: TreeNode<ValueT>) => boolean;
    canParent?: (parent: TreeNode<ValueT>, child: TreeNode<ValueT>) => boolean;
    replaceElement?: HTMLElement;
    additionalProperties?: ElementChild[];
}
declare class TreeViewNodeEvent<EventT extends string, DataT> extends TypedEvent<EventT> {
    readonly node: TreeNode<DataT>;
    constructor(type: EventT, node: TreeNode<DataT>);
}
export declare class TreeViewNodeDeleteEvent<DataT> extends TreeViewNodeEvent<"delete", DataT> {
    constructor(node: TreeNode<DataT>);
}
export declare class TreeViewNodeMovedEvent<DataT> extends TreeViewNodeEvent<"moved", DataT> {
    readonly newIndex: number;
    constructor(node: TreeNode<DataT>, newIndex: number);
}
export declare class TreeViewNodeReparentedEvent<DataT> extends TreeViewNodeEvent<"reparented", DataT> {
    readonly newParent: TreeNode<DataT>;
    constructor(node: TreeNode<DataT>, newParent: TreeNode<DataT>);
}
type TreeViewEvents<T> = {
    moved: TreeViewNodeMovedEvent<T>;
    reparented: TreeViewNodeReparentedEvent<T>;
    delete: TreeViewNodeDeleteEvent<T>;
};
export declare class TreeViewElement<ValueT, FilterTypeT extends string = never> extends TypedHTMLElement<TreeViewNodeEvents<ValueT> & TreeViewEvents<ValueT>> {
    readonly element: HTMLElement;
    private readonly expandButton;
    private readonly collapseButton;
    private readonly filters;
    private readonly filterTypeInput;
    private readonly filterNameInput;
    private readonly treeViewNodes;
    private readonly options;
    private readonly _canChangeOrder;
    private readonly elements;
    private readonly nodes2Elements;
    private readonly htmlElements2Nodes;
    private readonly htmlElements2Elements;
    private _rootNode;
    private locked;
    private _disabled;
    private typeFilter;
    private nameFilter;
    readonly: boolean;
    constructor(options?: TreeViewOptions<ValueT, FilterTypeT>, ...styleProps: (CssElementStyleProp | HtmlAttr<"id">)[]);
    clearFilter(): void;
    private refreshFilter;
    private canReparent;
    get canChangeOrder(): boolean;
    get enabled(): boolean;
    set enabled(v: boolean);
    get disabled(): boolean;
    set disabled(v: boolean);
    withLock(action: () => Promise<void>): Promise<void>;
    findAll(predicate: (value: ValueT) => boolean): TreeNode<ValueT>[];
    enableOnlyValues(values: readonly ValueT[]): void;
    enableAllElements(): void;
    get values(): ValueT[];
    set values(arr: readonly ValueT[]);
    get rootNode(): TreeNode<ValueT>;
    set rootNode(v: TreeNode<ValueT>);
    clear(): void;
    get selectedValue(): ValueT;
    findNode(data: ValueT): TreeNode<ValueT>;
    set selectedValue(v: ValueT);
    get selectedNode(): TreeNode<ValueT>;
    set selectedNode(v: TreeNode<ValueT>);
    private get selectedElement();
    private set selectedElement(value);
    private findElement;
    private reorderChildren;
    private createElement;
    addValue(value: ValueT): void;
    updateNode(node: TreeNode<ValueT>): void;
    reparentNode(node: TreeNode<ValueT>, newParentNode: TreeNode<ValueT>, delta: number): void;
    removeValue(value: ValueT): void;
    removeNode(node: TreeNode<ValueT>): void;
    collapseAll(): void;
    expandAll(maxDepth?: number): void;
    static install(): import("@juniper-lib/dom").ElementFactory<TreeViewElement<unknown, never>>;
}
export declare function TreeView<T, FilterTypeT extends string = never>(...rest: ElementChild[]): TreeViewElement<T, FilterTypeT>;
export {};
//# sourceMappingURL=index.d.ts.map