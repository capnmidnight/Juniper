import { ComparisonResult, compareCallback } from "@juniper-lib/util";
import { BaseGraphNode } from "./BaseGraphNode";
export declare function buildTree<V>(items: readonly V[], getParent: (v: V) => V, _getOrder?: (v: V) => ComparisonResult): TreeNode<V>;
export declare function buildTreeByID<V, K>(items: readonly V[], getItemID: (v: V) => K, getParentID: (v: V) => K, getOrder?: (v: V) => ComparisonResult): TreeNode<V>;
/**
 * A TreeNode is a GraphNode that can have only one parent.
 **/
export declare class TreeNode<ValueT> extends BaseGraphNode<ValueT> {
    get depth(): number;
    removeFromParent(): void;
    connectTo(child: this): void;
    connectAt(child: this, index: number): void;
    connectSorted(child: this, comparer: compareCallback<ValueT>): void;
    get parent(): this;
    get children(): this[];
    get isRoot(): boolean;
    get isChild(): boolean;
    get isLeaf(): boolean;
    get hasChildren(): boolean;
}
//# sourceMappingURL=TreeNode.d.ts.map