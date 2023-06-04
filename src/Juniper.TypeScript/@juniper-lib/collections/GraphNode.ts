import { BaseGraphNode } from "./BaseGraphNode";

export class GraphNode<ValueT> extends BaseGraphNode<ValueT>{
    get connections(): this[] {
        return this._forward;
    }

    get isEntryPoint() {
        return this._isEntryPoint;
    }

    get isExitPoint() {
        return this._isExitPoint;
    }
}