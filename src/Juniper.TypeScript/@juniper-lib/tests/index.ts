import { tests as FetcherTests } from "./fetcher";
import { haxTests } from "./hax";
import { tests as TSLibTests } from "./tslib";
import { tests as UnitsTests } from "./units";

export const tests = [
    haxTests,
    ...UnitsTests,
    ...FetcherTests,
    ...TSLibTests
];