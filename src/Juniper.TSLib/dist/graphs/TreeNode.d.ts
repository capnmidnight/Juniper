import { BaseGraphNode } from "../";
export declare function buildTree<K, V>(items: readonly V[], getKey: (v: V) => K, getParentKey: (v: V) => K, getOrder?: (v: V) => number): TreeNode<V>;
/**
 * A TreeNode is a GraphNode that can have only one parent.
 **/
export declare class TreeNode<ValueT> extends BaseGraphNode<ValueT> {
    removeFromParent(): void;
    contains(node: TreeNode<ValueT>): boolean;
    connectTo(child: TreeNode<ValueT>): void;
    connectAt(child: TreeNode<ValueT>, index: number): void;
    connectSorted<KeyT>(child: TreeNode<ValueT>, sortKey: (value: ValueT) => KeyT): void;
    disconnectFrom(node: TreeNode<ValueT>): void;
    isConnectedTo(node: TreeNode<ValueT>): boolean;
    flatten(): TreeNode<ValueT>[];
    private traverse;
    breadthFirst(): Iterable<TreeNode<ValueT>>;
    depthFirst(): Iterable<TreeNode<ValueT>>;
    find(v: ValueT): TreeNode<ValueT>;
    search(predicate: (n: TreeNode<ValueT>) => boolean): TreeNode<ValueT>;
    get parent(): TreeNode<ValueT>;
    get children(): TreeNode<ValueT>[];
    get isRoot(): any;
    get isLeaf(): any;
}
