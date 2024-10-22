import { formatBytes } from "@juniper-lib/util";
import { TestCase } from "@juniper-lib/testing";
import { toBytes } from '@juniper-lib/units';

export class FileSizeTests extends TestCase {
    test_FormatBytes1() {
        const value = formatBytes(500);
        this.areExact(value, "500 B");
    }

    test_ToBytes1() {
        const value = toBytes(2, "KiB");
        this.areExact(value, 2048);
    }
}