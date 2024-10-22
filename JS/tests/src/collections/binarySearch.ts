import { BinarySearchMode, binarySearch, compareBy, identity } from "@juniper-lib/util";
import { TestCase } from "@juniper-lib/testing";

export class BinarySearchTests extends TestCase {
    test_AscendingSort() {
        const arr = [8, 2, 1, 9, 3, 4, 7, 5, 6];
        arr.sort(compareBy(true, identity));
        this.arraysMatch(arr, [1, 2, 3, 4, 5, 6, 7, 8, 9]);
    }

    test_DescendingSort() {
        const arr = [8, 2, 1, 9, 3, 4, 7, 5, 6];
        arr.sort(compareBy(false, identity));
        this.arraysMatch(arr, [9, 8, 7, 6, 5, 4, 3, 2, 1]);
    }

    #testSearch(ascending: boolean, mode: BinarySearchMode, arr: number[], val: number, expectedIdx: number) {
        const idx = binarySearch(arr, val, compareBy<number>(ascending, identity), mode);
        this.areExact(idx, expectedIdx);
    }

    test_AscendingSearchingEmpty() {
        this.#testSearch(true, "search", [], 0, -1);
    }

    test_AscendingSearchingMissBeforeBeginning() {
        this.#testSearch(true, "search", [1, 2, 3], 0.5, -1);
    }

    test_AscendingSearchingMatchBeginning() {
        this.#testSearch(true, "search", [1, 2, 3], 1, 0);
    }

    test_AscendingSearchingMissAfterBeginning() {
        this.#testSearch(true, "search", [1, 2, 3], 1.5, -2);
    }

    test_AscendingSearchingMatchMiddle() {
        this.#testSearch(true, "search", [1, 2, 3], 2, 1);
    }

    test_AscendingSearchingMissBeforeEnd() {
        this.#testSearch(true, "search", [1, 2, 3], 2.5, -3);
    }

    test_AscendingSearchingMatchEnd() {
        this.#testSearch(true, "search", [1, 2, 3], 3, 2);
    }

    test_AscendingSearchingMissAfterEnd() {
        this.#testSearch(true, "search", [1, 2, 3], 3.5, -4);
    }

    test_AscendingAppendingRunAtBeginning() {
        this.#testSearch(true, "append", [1, 2, 3], 1, 1);
    }

    test_AscendingAppendingRunAtMiddle() {
        this.#testSearch(true, "append", [1, 2, 3], 2, 2);
    }

    test_AscendingAppendingRunAtEnd() {
        this.#testSearch(true, "append", [1, 2, 3], 3, 3);
    }

    test_AscendingAppendingLongerRunAtBeginning() {
        this.#testSearch(true, "append", [1, 1, 2, 2, 3, 3], 1, 2);
    }

    test_AscendingAppendingLongerRunAtMiddle() {
        this.#testSearch(true, "append", [1, 1, 2, 2, 3, 3], 2, 4);
    }

    test_AscendingAppendingLongerRunAtEnd() {
        this.#testSearch(true, "append", [1, 1, 2, 2, 3, 3], 3, 6);
    }

    test_AscendingAppendingLongererRunAtBeginning() {
        this.#testSearch(true, "append", [1, 1, 1, 2, 2, 2, 3, 3, 3], 1, 3);
    }

    test_AscendingAppendingLongererRunAtMiddle() {
        this.#testSearch(true, "append", [1, 1, 1, 2, 2, 2, 3, 3, 3], 2, 6);
    }

    test_AscendingAppendingLongererRunAtEnd() {
        this.#testSearch(true, "append", [1, 1, 1, 2, 2, 2, 3, 3, 3], 3, 9);
    }

    test_AscendingPrependingRunAtBeginning() {
        this.#testSearch(true, "prepend", [1, 2, 3], 1, 0);
    }

    test_AscendingPrependingRunAtMiddle() {
        this.#testSearch(true, "prepend", [1, 2, 3], 2, 1);
    }

    test_AscendingPrependingRunAtEnd() {
        this.#testSearch(true, "prepend", [1, 2, 3], 3, 2);
    }

    test_AscendingPrependingLongerRunAtBeginning() {
        this.#testSearch(true, "prepend", [1, 1, 2, 2, 3, 3], 1, 0);
    }

    test_AscendingPrependingLongerRunAtMiddle() {
        this.#testSearch(true, "prepend", [1, 1, 2, 2, 3, 3], 2, 2);
    }

    test_AscendingPrependingLongerRunAtEnd() {
        this.#testSearch(true, "prepend", [1, 1, 2, 2, 3, 3], 3, 4);
    }

    test_AscendingPrependingLongererRunAtBeginning() {
        this.#testSearch(true, "prepend", [1, 1, 1, 2, 2, 2, 3, 3, 3], 1, 0);
    }

    test_AscendingPrependingLongererRunAtMiddle() {
        this.#testSearch(true, "prepend", [1, 1, 1, 2, 2, 2, 3, 3, 3], 2, 3);
    }

    test_AscendingPrependingLongererRunAtEnd() {
        this.#testSearch(true, "prepend", [1, 1, 1, 2, 2, 2, 3, 3, 3], 3, 6);
    }

    test_DescendingSearchingEmpty() {
        this.#testSearch(false, "search", [], 0, -1);
    }

    test_DescendingSearchingMissAfterEnd() {
        this.#testSearch(false, "search", [3, 2, 1], 0.5, -4);
    }

    test_DescendingSearchingMatchEnd() {
        this.#testSearch(false, "search", [3, 2, 1], 1, 2);
    }

    test_DescendingSearchingMissBeforeEnd() {
        this.#testSearch(false, "search", [3, 2, 1], 1.5, -3);
    }

    test_DescendingSearchingMatchMiddle() {
        this.#testSearch(false, "search", [3, 2, 1], 2, 1);
    }

    test_DescendingSearchingMissAfterBeginning() {
        this.#testSearch(false, "search", [3, 2, 1], 2.5, -2);
    }

    test_DescendingSearchingMatchBeginning() {
        this.#testSearch(false, "search", [3, 2, 1], 3, 0);
    }

    test_DescendingSearchingMissBeforeBeginning() {
        this.#testSearch(false, "search", [3, 2, 1], 3.5, -1);
    }

    test_DescendingAppendingRunAtBeginning() {
        this.#testSearch(false, "append", [3, 2, 1], 3, 1);
    }

    test_DescendingAppendingRunAtMiddle() {
        this.#testSearch(false, "append", [3, 2, 1], 2, 2);
    }

    test_DescendingAppendingRunAtEnd() {
        this.#testSearch(false, "append", [3, 2, 1], 1, 3);
    }

    test_DescendingAppendingLongerRunAtBeginning() {
        this.#testSearch(false, "append", [3, 3, 2, 2, 1, 1], 3, 2);
    }

    test_DescendingAppendingLongerRunAtMiddle() {
        this.#testSearch(false, "append", [3, 3, 2, 2, 1, 1], 2, 4);
    }

    test_DescendingAppendingLongerRunAtEnd() {
        this.#testSearch(false, "append", [3, 3, 2, 2, 1, 1], 1, 6);
    }

    test_DescendingAppendingLongererRunAtBeginning() {
        this.#testSearch(false, "append", [3, 3, 3, 2, 2, 2, 1, 1, 1], 3, 3);
    }

    test_DescendingAppendingLongererRunAtMiddle() {
        this.#testSearch(false, "append", [3, 3, 3, 2, 2, 2, 1, 1, 1], 2, 6);
    }

    test_DescendingAppendingLongererRunAtEnd() {
        this.#testSearch(false, "append", [3, 3, 3, 2, 2, 2, 1, 1, 1], 1, 9);
    }

    test_DescendingPrependingRunAtBeginning() {
        this.#testSearch(false, "prepend", [3, 2, 1], 3, 0);
    }

    test_DescendingPrependingRunAtMiddle() {
        this.#testSearch(false, "prepend", [3, 2, 1], 2, 1);
    }

    test_DescendingPrependingRunAtEnd() {
        this.#testSearch(false, "prepend", [3, 2, 1], 1, 2);
    }

    test_DescendingPrependingLongerRunAtBeginning() {
        this.#testSearch(false, "prepend", [3, 3, 2, 2, 1, 1], 3, 0);
    }

    test_DescendingPrependingLongerRunAtMiddle() {
        this.#testSearch(false, "prepend", [3, 3, 2, 2, 1, 1], 2, 2);
    }

    test_DescendingPrependingLongerRunAtEnd() {
        this.#testSearch(false, "prepend", [3, 3, 2, 2, 1, 1], 1, 4);
    }

    test_DescendingPrependingLongererRunAtBeginning() {
        this.#testSearch(false, "prepend", [3, 3, 3, 2, 2, 2, 1, 1, 1], 3, 0);
    }

    test_DescendingPrependingLongererRunAtMiddle() {
        this.#testSearch(false, "prepend", [3, 3, 3, 2, 2, 2, 1, 1, 1], 2, 3);
    }

    test_DescendingPrependingLongererRunAtEnd() {
        this.#testSearch(false, "prepend", [3, 3, 3, 2, 2, 2, 1, 1, 1], 1, 6);
    }
}