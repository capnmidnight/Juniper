import { Comparable } from "./arrays";
export declare abstract class BaseGraphNode<ValueT> {
    readonly value: ValueT;
    protected readonly _forward: this[];
    protected readonly _reverse: this[];
    constructor(value: ValueT);
    connectSorted<KeyT extends Comparable>(child: this, keySelector: (value: ValueT) => KeyT): void;
    connectTo(child: this): void;
    connectAt(child: this, index: number): void;
    disconnectFrom(child: this): void;
    isConnectedTo(node: this): boolean;
    flatten(): this[];
    traverse(breadthFirst: boolean): Iterable<this>;
    breadthFirst(): Iterable<this>;
    depthFirst(): Iterable<this>;
    search(predicate: (n: this) => boolean, breadthFirst?: boolean): this;
    searchAll(predicate: (n: this) => boolean, breadthFirst?: boolean): Iterable<this>;
    find(v: ValueT, breadthFirst?: boolean): this;
    findAll(v: ValueT, breadthFirst?: boolean): Iterable<this>;
    contains(node: this, breadthFirst?: boolean): boolean;
    containsValue(v: ValueT, breadthFirst?: boolean): boolean;
    protected get _isEntryPoint(): boolean;
    protected get _isExitPoint(): boolean;
    get isDisconnected(): boolean;
    get isConnected(): boolean;
    get isTerminus(): boolean;
    get isInternal(): boolean;
}
//# sourceMappingURL=BaseGraphNode.d.ts.map