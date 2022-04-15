import { BaseGraphNode } from "./BaseGraphNode";

export class GraphNode<ValueT> extends BaseGraphNode<ValueT>{

    override connectTo(node: GraphNode<ValueT>): void {
        super.connectTo(node);
    }

    override connectAt(child: GraphNode<ValueT>, index: number): void {
        super.connectAt(child, index);
    }

    override connectSorted<KeyT>(child: GraphNode<ValueT>, sortKey: (value: ValueT) => KeyT): void {
        super.connectSorted(child, sortKey);
    }

    override disconnectFrom(node: GraphNode<ValueT>): void {
        super.disconnectFrom(node);
    }

    override isConnectedTo(node: GraphNode<ValueT>): boolean {
        return super.isConnectedTo(node);
    }

    override flatten(): GraphNode<ValueT>[] {
        return super.flatten() as GraphNode<ValueT>[];
    }

    get connections(): GraphNode<ValueT>[] {
        return this._forward as GraphNode<ValueT>[];
    }

    get isEntryPoint() {
        return this._isEntryPoint;
    }

    get isExitPoint() {
        return this._isExitPoint;
    }
}