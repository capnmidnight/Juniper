import { isDefined } from "../typeChecks";
import { BaseGraphNode } from "./BaseGraphNode";

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
        const isParentNodeDefined = parent != null;
        const hasParentNode = isParentNodeDefined && nodes.has(parent);
        const parentNode = !isParentNodeDefined
            ? rootNode
            : hasParentNode
                ? nodes.get(parent)
                : null;

        if (parentNode) {
            parentNode.connectSorted(node, getOrder);
        }
    }

    return rootNode;
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

    contains(node: TreeNode<ValueT>): boolean {
        for (const child of this.breadthFirst()) {
            if (child === node) {
                return true;
            }
        }

        return false;
    }

    override connectTo(child: TreeNode<ValueT>): void {
        child.removeFromParent();
        super.connectTo(child);
    }

    override connectAt(child: TreeNode<ValueT>, index: number): void {
        child.removeFromParent();
        super.connectAt(child, index);
    }

    override connectSorted<KeyT>(child: TreeNode<ValueT>, sortKey: (value: ValueT) => KeyT): void {
        child.removeFromParent();
        super.connectSorted(child, sortKey);
    }

    override disconnectFrom(node: TreeNode<ValueT>): void {
        super.disconnectFrom(node);
    }

    override isConnectedTo(node: TreeNode<ValueT>): boolean {
        return super.isConnectedTo(node);
    }

    override flatten(): TreeNode<ValueT>[] {
        return super.flatten() as TreeNode<ValueT>[];
    }

    private *traverse(getNext: (queue: TreeNode<ValueT>[]) => TreeNode<ValueT>): Iterable<TreeNode<ValueT>> {
        const queue: TreeNode<ValueT>[] = [this];
        while (queue.length > 0) {
            const here = getNext(queue);
            if (here.hasChildren) {
                queue.push(...here.children);
            }
            yield here;
        }
    }

    breadthFirst() {
        return this.traverse((queue) => queue.shift());
    }

    depthFirst() {
        return this.traverse((queue) => queue.pop());
    }

    find(v: ValueT): TreeNode<ValueT> {
        return this.search((n) => n.value === v);
    }

    search(predicate: (n: TreeNode<ValueT>) => boolean): TreeNode<ValueT> {
        for (const node of this.breadthFirst()) {
            if (predicate(node)) {
                return node;
            }
        }

        return null;
    }

    get parent(): TreeNode<ValueT> {
        if (this._reverse.length === 0) {
            return null;
        }

        return this._reverse[0] as TreeNode<ValueT>;
    }

    get children(): TreeNode<ValueT>[] {
        return this._forward as TreeNode<ValueT>[];
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