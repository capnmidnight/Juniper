import { BaseGraphNode } from "./BaseGraphNode";
export class GraphNode extends BaseGraphNode {
    get connections() {
        return this._forward;
    }
    get reverseConnections() {
        return this._reverse;
    }
    get isEntryPoint() {
        return this._isEntryPoint;
    }
    get isExitPoint() {
        return this._isExitPoint;
    }
}
//# sourceMappingURL=GraphNode.js.map