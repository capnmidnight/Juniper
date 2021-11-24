import { BaseGraphNode } from "../";
export declare class GraphNode<ValueT> extends BaseGraphNode<ValueT> {
    connectTo(node: GraphNode<ValueT>): void;
    connectAt(child: GraphNode<ValueT>, index: number): void;
    connectSorted<KeyT>(child: GraphNode<ValueT>, sortKey: (value: ValueT) => KeyT): void;
    disconnectFrom(node: GraphNode<ValueT>): void;
    isConnectedTo(node: GraphNode<ValueT>): boolean;
    flatten(): GraphNode<ValueT>[];
    get connections(): GraphNode<ValueT>[];
    get isEntryPoint(): any;
    get isExitPoint(): any;
}
