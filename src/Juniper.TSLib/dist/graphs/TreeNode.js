import { BaseGraphNode } from "../";
export function buildTree(items, getKey, getParentKey, getOrder) {
    const rootNode = new TreeNode(null);
    const nodes = new Map();
    for (const item of items) {
        const nodeID = getKey(item);
        const node = new TreeNode(item);
        nodes.set(nodeID, node);
    }
    for (const node of nodes.values()) {
        const parentNodeID = getParentKey(node.value);
        const isParentNodeDefined = parentNodeID != null;
        const hasParentNode = isParentNodeDefined && nodes.has(parentNodeID);
        const parentNode = !isParentNodeDefined
            ? rootNode
            : hasParentNode
                ? nodes.get(parentNodeID)
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
export class TreeNode extends BaseGraphNode {
    removeFromParent() {
        while (this.parent) {
            this.parent.disconnectFrom(this);
        }
    }
    contains(node) {
        for (const child of this.breadthFirst()) {
            if (child === node) {
                return true;
            }
        }
        return false;
    }
    connectTo(child) {
        child.removeFromParent();
        super.connectTo(child);
    }
    connectAt(child, index) {
        child.removeFromParent();
        super.connectAt(child, index);
    }
    connectSorted(child, sortKey) {
        child.removeFromParent();
        super.connectSorted(child, sortKey);
    }
    disconnectFrom(node) {
        super.disconnectFrom(node);
    }
    isConnectedTo(node) {
        return super.isConnectedTo(node);
    }
    flatten() {
        return super.flatten();
    }
    *traverse(getNext) {
        const queue = [this];
        while (queue.length > 0) {
            const here = getNext(queue);
            if (!here.isLeaf) {
                queue.push(...here.children);
            }
            yield here;
        }
    }
    breadthFirst() {
        return this.traverse(queue => queue.shift());
    }
    depthFirst() {
        return this.traverse(queue => queue.pop());
    }
    find(v) {
        return this.search(n => n.value === v);
    }
    search(predicate) {
        for (const node of this.breadthFirst()) {
            if (predicate(node)) {
                return node;
            }
        }
        return null;
    }
    get parent() {
        if (this._reverse.length === 0) {
            return null;
        }
        return this._reverse[0];
    }
    get children() {
        return this._forward;
    }
    get isRoot() {
        return this._isEntryPoint;
    }
    get isLeaf() {
        return this._isExitPoint;
    }
}
