import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { Comparable, arrayInsertAt, arrayRemove, compareBy, insertSorted } from "./arrays";

function breadthFirstPeek<ValueT>(arr: ValueT[]) {
    return arr[0];
}
function breadthFirstRemove<ValueT>(arr: ValueT[]) {
    return arr.shift();
}

function depthFirstPeek<ValueT>(arr: ValueT[]) {
    return arr[arr.length - 1];
}

function depthFirstRemove<ValueT>(arr: ValueT[]) {
    return arr.pop();
}

export abstract class BaseGraphNode<ValueT> {

    protected readonly _forward = new Array<this>();
    protected readonly _reverse = new Array<this>();

    constructor(public readonly value: ValueT) {
    }

    connectSorted<KeyT extends Comparable>(child: this, keySelector: (value: ValueT) => KeyT): void {
        if (isDefined(keySelector)) {
            const comparer = compareBy<this>((n) => keySelector(n.value));
            insertSorted(this._forward, child, comparer);
            insertSorted(child._reverse, this, comparer);
        }
        else {
            this.connectTo(child);
        }
    }

    connectTo(child: this) {
        this.connectAt(child, this._forward.length);
    }

    connectAt(child: this, index: number) {
        arrayInsertAt(this._forward, child, index);
        child._reverse.push(this);
    }

    disconnectFrom(child: this) {
        arrayRemove(this._forward, child);
        arrayRemove(child._reverse, this);
    }

    isConnectedTo(node: this): boolean {
        return this._forward.indexOf(node) >= 0
            || this._reverse.indexOf(node) >= 0;
    }

    flatten(): this[] {
        const visited = new Set<this>();
        const queue: this[] = [this];
        while (queue.length > 0) {
            const here = queue.shift();
            if (isDefined(here) && !visited.has(here)) {
                visited.add(here);
                queue.push(...here._forward);
            }
        }

        return Array.from(visited);
    }

    *traverse(breadthFirst: boolean): Iterable<this> {
        const visited = new Set<this>();
        const queue = [this];
        const peek = breadthFirst
            ? breadthFirstPeek
            : depthFirstPeek;
        const remove = breadthFirst
            ? breadthFirstRemove
            : depthFirstRemove;

        while (queue.length > 0) {
            const here = peek(queue);
            if (!visited.has(here)) {
                visited.add(here);

                if (breadthFirst) {
                    remove(queue);
                    yield here;
                }

                if (here._forward.length > 0) {
                    queue.push(...here._forward);
                }
            }
            else if (!breadthFirst) {
                remove(queue);
                yield here;
            }
        }
    }

    breadthFirst(): Iterable<this> {
        return this.traverse(true);
    }

    depthFirst(): Iterable<this> {
        return this.traverse(false);
    }

    search(predicate: (n: this) => boolean, breadthFirst = true): this {
        for (const node of this.traverse(breadthFirst)) {
            if (predicate(node)) {
                return node;
            }
        }

        return null;
    }

    *searchAll(predicate: (n: this) => boolean, breadthFirst = true): Iterable<this> {
        for (const node of this.traverse(breadthFirst)) {
            if (predicate(node)) {
                yield node;
            }
        }
    }

    find(v: ValueT, breadthFirst = true): this {
        return this.search((n) => n.value === v, breadthFirst);
    }

    findAll(v: ValueT, breadthFirst = true): Iterable<this> {
        return this.searchAll((n) => n.value === v, breadthFirst);
    }

    contains(node: this, breadthFirst = true): boolean {
        for (const child of this.traverse(breadthFirst)) {
            if (child === node) {
                return true;
            }
        }

        return false;
    }

    containsValue(v: ValueT, breadthFirst = true): boolean {
        for (const child of this.traverse(breadthFirst)) {
            if (child.value === v) {
                return true;
            }
        }

        return false;
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