export declare abstract class BaseGraphNode<ValueT> {
    readonly value: ValueT;
    protected readonly _forward: BaseGraphNode<ValueT>[];
    protected readonly _reverse: BaseGraphNode<ValueT>[];
    constructor(value: ValueT);
    connectSorted<KeyT>(child: BaseGraphNode<ValueT>, keySelector: (value: ValueT) => KeyT): void;
    connectTo(child: BaseGraphNode<ValueT>): void;
    connectAt(child: BaseGraphNode<ValueT>, index: number): void;
    disconnectFrom(child: BaseGraphNode<ValueT>): void;
    isConnectedTo(node: BaseGraphNode<ValueT>): boolean;
    flatten(): BaseGraphNode<ValueT>[];
    protected get _isEntryPoint(): boolean;
    protected get _isExitPoint(): boolean;
    get isDisconnected(): boolean;
    get isConnected(): boolean;
    get isTerminus(): boolean;
    get isInternal(): boolean;
}
