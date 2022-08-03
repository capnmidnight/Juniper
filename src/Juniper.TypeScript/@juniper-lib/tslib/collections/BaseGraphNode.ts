import { isDefined } from "../";
import { arrayInsertAt } from "./arrayInsertAt";
import { arrayRemove } from "./arrayRemove";
import { arraySortedInsert } from "./arraySortedInsert";

export abstract class BaseGraphNode<ValueT> {

    protected readonly _forward = new Array<BaseGraphNode<ValueT>>();
    protected readonly _reverse = new Array<BaseGraphNode<ValueT>>();

    constructor(public readonly value: ValueT) {
    }

    connectSorted<KeyT>(child: BaseGraphNode<ValueT>, keySelector: (value: ValueT) => KeyT): void {
        if (isDefined(keySelector)) {
            arraySortedInsert(this._forward, child, (n) => keySelector(n.value));
            arraySortedInsert(child._reverse, this, (n) => keySelector(n.value));
        }
        else {
            this.connectTo(child);
        }
    }

    connectTo(child: BaseGraphNode<ValueT>) {
        this.connectAt(child, this._forward.length);
    }

    connectAt(child: BaseGraphNode<ValueT>, index: number) {
        arrayInsertAt(this._forward, child, index);
        child._reverse.push(this);
    }

    disconnectFrom(child: BaseGraphNode<ValueT>) {
        arrayRemove(this._forward, child);
        arrayRemove(child._reverse, this);
    }

    isConnectedTo(node: BaseGraphNode<ValueT>): boolean {
        return this._forward.indexOf(node) >= 0
            || this._reverse.indexOf(node) >= 0;
    }

    flatten(): BaseGraphNode<ValueT>[] {
        const nodes = new Set<BaseGraphNode<ValueT>>();
        const queue: BaseGraphNode<ValueT>[] = [this];
        while (queue.length > 0) {
            const here = queue.shift();
            if (isDefined(here) && !nodes.has(here)) {
                nodes.add(here);
                queue.push(...here._forward);
            }
        }

        return Array.from(nodes);
    }

    protected get _isEntryPoint() {
        return this._reverse.length === 0;
    }

    protected get _isExitPoint() {
        return this._forward.length === 0;
    }

    get isDisconnected() {
        return this._isEntryPoint
            && this._isExitPoint;
    }

    get isConnected() {
        return !this._isExitPoint
            || !this._isEntryPoint;
    }

    get isTerminus() {
        return this._isEntryPoint
            || this._isExitPoint;
    }

    get isInternal() {
        return !this._isEntryPoint
            && !this._isExitPoint;
    }
}