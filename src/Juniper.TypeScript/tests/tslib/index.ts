import { tests as EventTests } from "./events";
import { tests as GISTests } from "./gis";
import { URLBuilderTests } from "./URLBuilder";
import { UsingTests } from "./using";

export const tests = [
    ...EventTests,
    ...GISTests,
    URLBuilderTests,
    UsingTests
];