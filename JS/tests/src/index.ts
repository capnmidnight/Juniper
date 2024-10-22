import { tests as FetcherTests } from "./fetcher";
import { haxTests } from "./hax";
import { tests as TSLibTests } from "./tslib";
import { tests as UnitsTests } from "./units";
import { tests as CollectionsTests } from "./collections";

export const tests = /*@__PURE__*/[
    ...CollectionsTests,
    haxTests,
    ...UnitsTests,
    ...FetcherTests,
    ...TSLibTests
];