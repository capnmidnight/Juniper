var _BinarySearchTests_instances, _BinarySearchTests_testSearch;
import { __classPrivateFieldGet } from "tslib";
import { binarySearch, compareBy } from "@juniper-lib/collections/arrays";
import { TestCase } from "@juniper-lib/testing/tdd/TestCase";
import { identity } from "@juniper-lib/tslib/identity";
export class BinarySearchTests extends TestCase {
    constructor() {
        super(...arguments);
        _BinarySearchTests_instances.add(this);
    }
    test_AscendingSort() {
        const arr = [8, 2, 1, 9, 3, 4, 7, 5, 6];
        arr.sort(compareBy("ascending", identity));
        this.arraysMatch(arr, [1, 2, 3, 4, 5, 6, 7, 8, 9]);
    }
    test_DescendingSort() {
        const arr = [8, 2, 1, 9, 3, 4, 7, 5, 6];
        arr.sort(compareBy("descending", identity));
        this.arraysMatch(arr, [9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }
    test_AscendingSearchingEmpty() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "search", [], 0, -1);
    }
    test_AscendingSearchingMissBeforeBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "search", [1, 2, 3], 0.5, -1);
    }
    test_AscendingSearchingMatchBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "search", [1, 2, 3], 1, 0);
    }
    test_AscendingSearchingMissAfterBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "search", [1, 2, 3], 1.5, -2);
    }
    test_AscendingSearchingMatchMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "search", [1, 2, 3], 2, 1);
    }
    test_AscendingSearchingMissBeforeEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "search", [1, 2, 3], 2.5, -3);
    }
    test_AscendingSearchingMatchEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "search", [1, 2, 3], 3, 2);
    }
    test_AscendingSearchingMissAfterEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "search", [1, 2, 3], 3.5, -4);
    }
    test_AscendingAppendingRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "append", [1, 2, 3], 1, 1);
    }
    test_AscendingAppendingRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "append", [1, 2, 3], 2, 2);
    }
    test_AscendingAppendingRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "append", [1, 2, 3], 3, 3);
    }
    test_AscendingAppendingLongerRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "append", [1, 1, 2, 2, 3, 3], 1, 2);
    }
    test_AscendingAppendingLongerRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "append", [1, 1, 2, 2, 3, 3], 2, 4);
    }
    test_AscendingAppendingLongerRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "append", [1, 1, 2, 2, 3, 3], 3, 6);
    }
    test_AscendingAppendingLongererRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "append", [1, 1, 1, 2, 2, 2, 3, 3, 3], 1, 3);
    }
    test_AscendingAppendingLongererRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "append", [1, 1, 1, 2, 2, 2, 3, 3, 3], 2, 6);
    }
    test_AscendingAppendingLongererRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "append", [1, 1, 1, 2, 2, 2, 3, 3, 3], 3, 9);
    }
    test_AscendingPrependingRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "prepend", [1, 2, 3], 1, 0);
    }
    test_AscendingPrependingRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "prepend", [1, 2, 3], 2, 1);
    }
    test_AscendingPrependingRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "prepend", [1, 2, 3], 3, 2);
    }
    test_AscendingPrependingLongerRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "prepend", [1, 1, 2, 2, 3, 3], 1, 0);
    }
    test_AscendingPrependingLongerRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "prepend", [1, 1, 2, 2, 3, 3], 2, 2);
    }
    test_AscendingPrependingLongerRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "prepend", [1, 1, 2, 2, 3, 3], 3, 4);
    }
    test_AscendingPrependingLongererRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "prepend", [1, 1, 1, 2, 2, 2, 3, 3, 3], 1, 0);
    }
    test_AscendingPrependingLongererRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "prepend", [1, 1, 1, 2, 2, 2, 3, 3, 3], 2, 3);
    }
    test_AscendingPrependingLongererRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "ascending", "prepend", [1, 1, 1, 2, 2, 2, 3, 3, 3], 3, 6);
    }
    test_DescendingSearchingEmpty() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "search", [], 0, -1);
    }
    test_DescendingSearchingMissAfterEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "search", [3, 2, 1], 0.5, -4);
    }
    test_DescendingSearchingMatchEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "search", [3, 2, 1], 1, 2);
    }
    test_DescendingSearchingMissBeforeEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "search", [3, 2, 1], 1.5, -3);
    }
    test_DescendingSearchingMatchMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "search", [3, 2, 1], 2, 1);
    }
    test_DescendingSearchingMissAfterBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "search", [3, 2, 1], 2.5, -2);
    }
    test_DescendingSearchingMatchBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "search", [3, 2, 1], 3, 0);
    }
    test_DescendingSearchingMissBeforeBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "search", [3, 2, 1], 3.5, -1);
    }
    test_DescendingAppendingRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "append", [3, 2, 1], 3, 1);
    }
    test_DescendingAppendingRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "append", [3, 2, 1], 2, 2);
    }
    test_DescendingAppendingRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "append", [3, 2, 1], 1, 3);
    }
    test_DescendingAppendingLongerRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "append", [3, 3, 2, 2, 1, 1], 3, 2);
    }
    test_DescendingAppendingLongerRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "append", [3, 3, 2, 2, 1, 1], 2, 4);
    }
    test_DescendingAppendingLongerRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "append", [3, 3, 2, 2, 1, 1], 1, 6);
    }
    test_DescendingAppendingLongererRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "append", [3, 3, 3, 2, 2, 2, 1, 1, 1], 3, 3);
    }
    test_DescendingAppendingLongererRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "append", [3, 3, 3, 2, 2, 2, 1, 1, 1], 2, 6);
    }
    test_DescendingAppendingLongererRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "append", [3, 3, 3, 2, 2, 2, 1, 1, 1], 1, 9);
    }
    test_DescendingPrependingRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "prepend", [3, 2, 1], 3, 0);
    }
    test_DescendingPrependingRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "prepend", [3, 2, 1], 2, 1);
    }
    test_DescendingPrependingRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "prepend", [3, 2, 1], 1, 2);
    }
    test_DescendingPrependingLongerRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "prepend", [3, 3, 2, 2, 1, 1], 3, 0);
    }
    test_DescendingPrependingLongerRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "prepend", [3, 3, 2, 2, 1, 1], 2, 2);
    }
    test_DescendingPrependingLongerRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "prepend", [3, 3, 2, 2, 1, 1], 1, 4);
    }
    test_DescendingPrependingLongererRunAtBeginning() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "prepend", [3, 3, 3, 2, 2, 2, 1, 1, 1], 3, 0);
    }
    test_DescendingPrependingLongererRunAtMiddle() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "prepend", [3, 3, 3, 2, 2, 2, 1, 1, 1], 2, 3);
    }
    test_DescendingPrependingLongererRunAtEnd() {
        __classPrivateFieldGet(this, _BinarySearchTests_instances, "m", _BinarySearchTests_testSearch).call(this, "descending", "prepend", [3, 3, 3, 2, 2, 2, 1, 1, 1], 1, 6);
    }
}
_BinarySearchTests_instances = new WeakSet(), _BinarySearchTests_testSearch = function _BinarySearchTests_testSearch(direction, mode, arr, val, expectedIdx) {
    const idx = binarySearch(arr, val, compareBy(direction, identity), mode);
    this.areExact(idx, expectedIdx);
};
//# sourceMappingURL=binarySearch.js.map