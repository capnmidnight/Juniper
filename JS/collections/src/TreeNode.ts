import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { BaseGraphNode } from "./BaseGraphNode";
import { Comparable } from "./arrays";
import { makeLookup } from "./makeLookup";

export function buildTree<V>(
    items: readonly V[],
    getParent: (v: V) => V,
    _getOrder?: (v: V) => number): TreeNode<V> {
    const getOrder = (v: V) => isDefined(v)
        && isDefined(_getOrder)
        && _getOrder(v);

    const rootNode = new TreeNode(null);
    const nodes = new Map<V, TreeNode<V>>();

    for (const item of items) {
        const node = new TreeNode(item);
        nodes.set(item, node);
    }

    for (const node of nodes.values()) {
        const parent = getParent(node.value);
        const hasParentNode = parent != null && nodes.has(parent);
        const parentNode = hasParentNode
            ? nodes.get(parent)
            : rootNode;

        parentNode.connectSorted(node, getOrder);
    }

    return rootNode;
}

export function buildTreeByID<V, K>(
    items: readonly V[],
    getItemID: (v: V) => K,
    getParentID: (v: V) => K,
    getOrder?: (v: V) => number): TreeNode<V> {
    const map = makeLookup(items, getItemID);
    return buildTree(items, v => map.get(getParentID(v)), getOrder);
}

/**
 * A TreeNode is a GraphNode that can have only one parent.
 **/
export class TreeNode<ValueT> extends BaseGraphNode<ValueT> {

    get depth() {
        let counter = 0;
        let here = this.parent;

        while (isDefined(here)) {
            ++counter;
            here = here.parent;
        }

        return counter;
    }

    removeFromParent() {
        while (this.parent) {
            this.parent.disconnectFrom(this);
        }
    }

    override connectTo(child: this): void {
        child.removeFromParent();
        super.connectTo(child);
    }

    override connectAt(child: this, index: number): void {
        child.removeFromParent();
        super.connectAt(child, index);
    }

    override connectSorted<KeyT extends Comparable>(child: this, sortKey: (value: ValueT) => KeyT): void {
        child.removeFromParent();
        super.connectSorted(child, sortKey);
    }

    get parent(): this {
        if (this._reverse.length === 0) {
            return null;
        }

        return this._reverse[0] as this;
    }

    get children(): this[] {
        return this._forward;
    }

    get isRoot() {
        return this._isEntryPoint;
    }

    get isChild() {
        return !this._isEntryPoint;
    }

    get isLeaf() {
        return this._isExitPoint;
    }

    get hasChildren() {
        return !this._isExitPoint;
    }
}