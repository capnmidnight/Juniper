import { arrayInsert, arrayRemove, insertSorted, isDefined } from "@juniper-lib/util";
function breadthFirstPeek(arr) {
    return arr[0];
}
function breadthFirstRemove(arr) {
    return arr.shift();
}
function depthFirstPeek(arr) {
    return arr[arr.length - 1];
}
function depthFirstRemove(arr) {
    return arr.pop();
}
export class BaseGraphNode {
    constructor(value) {
        this.value = value;
        this._forward = new Array();
        this._reverse = new Array();
        this._connected = new Set();
    }
    connectSorted(child, comparer) {
        const comparerValues = (a, b) => comparer(a.value, b.value);
        Object.assign(comparerValues, { descending: comparer.descending });
        insertSorted(this._forward, child, comparerValues);
        insertSorted(child._reverse, this, comparerValues);
        this._connected.add(child);
        child._connected.add(this);
    }
    connectTo(child) {
        this.connectAt(child, this._forward.length);
    }
    connectAt(child, index) {
        arrayInsert(this._forward, child, index);
        child._reverse.push(this);
        this._connected.add(child);
        child._connected.add(this);
    }
    disconnectFrom(child) {
        arrayRemove(this._forward, child);
        arrayRemove(child._reverse, this);
        this._connected.delete(child);
        child._connected.delete(this);
    }
    isConnectedTo(node) {
        return this._connected.has(node);
    }
    flatten() {
        const visited = new Set();
        const queue = [this];
        while (queue.length > 0) {
            const here = queue.shift();
            if (isDefined(here) && !visited.has(here)) {
                visited.add(here);
                queue.push(...here._forward);
            }
        }
        return Array.from(visited);
    }
    *traverse(breadthFirst, reverse = false) {
        const visited = new Set();
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
                const next = reverse
                    ? here._reverse
                    : here._forward;
                if (next.length > 0) {
                    queue.push(...next);
                }
            }
            else if (!breadthFirst) {
                remove(queue);
                yield here;
            }
        }
    }
    breadthFirst() {
        return this.traverse(true);
    }
    depthFirst() {
        return this.traverse(false);
    }
    search(predicate, breadthFirst = true) {
        for (const node of this.traverse(breadthFirst)) {
            if (predicate(node)) {
                return node;
            }
        }
        return null;
    }
    *searchAll(predicate, breadthFirst = true) {
        for (const node of this.traverse(breadthFirst)) {
            if (predicate(node)) {
                yield node;
            }
        }
    }
    find(v, breadthFirst = true) {
        return this.search((n) => n.value === v, breadthFirst);
    }
    findAll(v, breadthFirst = true) {
        return this.searchAll((n) => n.value === v, breadthFirst);
    }
    contains(node, breadthFirst = true) {
        for (const child of this.traverse(breadthFirst)) {
            if (child === node) {
                return true;
            }
        }
        return false;
    }
    containsValue(v, breadthFirst = true) {
        for (const child of this.traverse(breadthFirst)) {
            if (child.value === v) {
                return true;
            }
        }
        return false;
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
//# sourceMappingURL=BaseGraphNode.js.map