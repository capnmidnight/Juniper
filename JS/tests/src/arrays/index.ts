import { arrayInsert, assert, binarySearch, compareBy, identity } from "@juniper-lib/util";
import { test } from "@juniper-lib/testing";

function binarySearchAndInsertTest(inArr: number[], direction: "ascending" | "descending", mode: "append" | "prepend", item: number, expectedSearchIndex: number) {
    const ascending = direction === "ascending";
    const expectedInsertIndex = expectedSearchIndex < 0 ? -1 - expectedSearchIndex : expectedSearchIndex;
    const outArr = Array.from(inArr);
    const comparer = compareBy(ascending, identity);
    const searchIndex = binarySearch(outArr, item, comparer, mode);
    const insertIndex = arrayInsert(outArr, item, searchIndex);
    const parts = [
        outArr.slice(0, insertIndex),
        `|${outArr[insertIndex]}|`,
        outArr.slice(insertIndex + 1)
    ].filter(v => v.length > 0);
    const args = `([${inArr.join(",")}], ${direction}, ${mode}, ${item}) => [${parts.join(",")}]`;
    test("binarySearchAndInsert", args, () => {
        assert("search", searchIndex, expectedSearchIndex);
        assert("index", insertIndex, expectedInsertIndex);
    });
}

binarySearchAndInsertTest([], "ascending", "append", 0, -1);
binarySearchAndInsertTest([], "ascending", "prepend", 0, -1);
binarySearchAndInsertTest([], "descending", "append", 0, -1);
binarySearchAndInsertTest([], "descending", "prepend", 0, -1);


binarySearchAndInsertTest([1], "ascending", "append", 0, -1);
binarySearchAndInsertTest([1], "ascending", "prepend", 0, -1);
binarySearchAndInsertTest([1], "descending", "append", 0, -2);
binarySearchAndInsertTest([1], "descending", "prepend", 0, -2);

binarySearchAndInsertTest([1], "ascending", "append", 1, 1);
binarySearchAndInsertTest([1], "ascending", "prepend", 1, 0);
binarySearchAndInsertTest([1], "descending", "append", 1, 1);
binarySearchAndInsertTest([1], "descending", "prepend", 1, 0);

binarySearchAndInsertTest([1], "ascending", "append", 2, -2);
binarySearchAndInsertTest([1], "ascending", "prepend", 2, -2);
binarySearchAndInsertTest([1], "descending", "append", 2, -1);
binarySearchAndInsertTest([1], "descending", "prepend", 2, -1);


binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "prepend", 0.5, -1);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "prepend", 1, 0);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "prepend", 1.5, -3);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "prepend", 2, 2);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "prepend", 2.5, -5);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "prepend", 3, 4);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "prepend", 3.5, -7);


binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "append", 0.5, -1);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "append", 1, 2);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "append", 1.5, -3);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "append", 2, 4);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "append", 2.5, -5);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "append", 3, 6);
binarySearchAndInsertTest([1, 1, 2, 2, 3, 3], "ascending", "append", 3.5, -7);


binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "prepend", 0.5, -7);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "prepend", 1, 4);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "prepend", 1.5, -5);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "prepend", 2, 2);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "prepend", 2.5, -3);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "prepend", 3, 0);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "prepend", 3.5, -1);


binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "append", 0.5, -7);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "append", 1, 6);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "append", 1.5, -5);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "append", 2, 4);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "append", 2.5, -3);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "append", 3, 2);
binarySearchAndInsertTest([3, 3, 2, 2, 1, 1], "descending", "append", 3.5, -1);