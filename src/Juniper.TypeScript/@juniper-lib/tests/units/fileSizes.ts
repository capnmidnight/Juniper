import { TestCase } from "@juniper-lib/testing/tdd";
import { formatBytes, toBytes } from "@juniper-lib/tslib";

export class FileSizeTests extends TestCase {
    test_FormatBytes1() {
        const value = formatBytes(500, 2);
        this.areExact(value, "500 B");
    }

    test_ToBytes1() {
        const value = toBytes(2, "KiB");
        this.areExact(value, 2048);
    }
}