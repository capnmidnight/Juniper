import { BaseGraphNode } from "./BaseGraphNode";
export class GraphNode extends BaseGraphNode {
    connectTo(node) {
        super.connectTo(node);
    }
    connectAt(child, index) {
        super.connectAt(child, index);
    }
    connectSorted(child, sortKey) {
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
    get connections() {
        return this._forward;
    }
    get isEntryPoint() {
        return this._isEntryPoint;
    }
    get isExitPoint() {
        return this._isExitPoint;
    }
}
