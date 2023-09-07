import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { arrayInsertAt, arrayRemove, compareBy, insertSorted } from "./arrays";
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
    }
    connectSorted(child, keySelector) {
        if (isDefined(keySelector)) {
            const comparer = compareBy((n) => keySelector(n.value));
            insertSorted(this._forward, child, comparer);
            insertSorted(child._reverse, this, comparer);
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
    *traverse(breadthFirst) {
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