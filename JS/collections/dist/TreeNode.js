import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { BaseGraphNode } from "./BaseGraphNode";
import { makeLookup } from "./makeLookup";
export function buildTree(items, getParent, _getOrder) {
    const getOrder = (v) => isDefined(v)
        && isDefined(_getOrder)
        && _getOrder(v);
    const rootNode = new TreeNode(null);
    const nodes = new Map();
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
export function buildTreeByID(items, getItemID, getParentID, getOrder) {
    const map = makeLookup(items, getItemID);
    return buildTree(items, v => map.get(getParentID(v)), getOrder);
}
/**
 * A TreeNode is a GraphNode that can have only one parent.
 **/
export class TreeNode extends BaseGraphNode {
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
//# sourceMappingURL=TreeNode.js.map