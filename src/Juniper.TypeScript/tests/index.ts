import { haxTests } from "./hax";
import { tests as FetcherTests } from "./fetcher";
import { tests as TSLibTests } from "./tslib";

export const tests = [
    haxTests,
    ...FetcherTests,
    ...TSLibTests
];