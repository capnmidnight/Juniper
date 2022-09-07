import { tests as CollectionsTests } from "./collections";
import { tests as EventsTests } from "./events";
import { tests as GISTests } from "./gis";
import { URLBuilderTests } from "./URLBuilder";
import { UsingTests } from "./using";

export const tests = /*@__PURE__*/[
    ...CollectionsTests,
    ...EventsTests,
    ...GISTests,
    URLBuilderTests,
    UsingTests
];