import { BaseGraphNode } from "./BaseGraphNode";
import { Comparable } from "./arrays";
export declare function buildTree<V>(items: readonly V[], getParent: (v: V) => V, _getOrder?: (v: V) => number): TreeNode<V>;
/**
 * A TreeNode is a GraphNode that can have only one parent.
 **/
export declare class TreeNode<ValueT> extends BaseGraphNode<ValueT> {
    get depth(): number;
    removeFromParent(): void;
    connectTo(child: this): void;
    connectAt(child: this, index: number): void;
    connectSorted<KeyT extends Comparable>(child: this, sortKey: (value: ValueT) => KeyT): void;
    get parent(): this;
    get children(): this[];
    get isRoot(): boolean;
    get isChild(): boolean;
    get isLeaf(): boolean;
    get hasChildren(): boolean;
}
//# sourceMappingURL=TreeNode.d.ts.map