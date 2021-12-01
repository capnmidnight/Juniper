import { arrayInsertAt } from "../collections/arrayInsertAt";
import { arrayRemove } from "../collections/arrayRemove";
import { arraySortedInsert } from "../collections/arraySortedInsert";
import { isDefined } from "../typeChecks";
export class BaseGraphNode {
    value;
    _forward = new Array();
    _reverse = new Array();
    constructor(value) {
        this.value = value;
    }
    connectSorted(child, keySelector) {
        if (isDefined(keySelector)) {
            arraySortedInsert(this._forward, child, (n) => keySelector(n.value));
            child._reverse.push(this);
        }
        else {
            this.connectTo(child);
        }
    }
    connectTo(child) {
        this.connectAt(child, this._forward.length);
    }
    connectAt(child, index) {
        arrayInsertAt(this._forward, child, index);
        child._reverse.push(this);
    }
    disconnectFrom(child) {
        arrayRemove(this._forward, child);
        arrayRemove(child._reverse, this);
    }
    isConnectedTo(node) {
        return this._forward.indexOf(node) >= 0
            || this._reverse.indexOf(node) >= 0;
    }
    flatten() {
        const nodes = new Set();
        const queue = [this];
        while (queue.length > 0) {
            const here = queue.shift();
            if (!nodes.has(here)) {
                nodes.add(here);
                queue.push(...here._forward);
            }
        }
        return Array.from(nodes);
    }
    get _isEntryPoint() {
        return this._reverse.length === 0;
    }
    get _isExitPoint() {
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
