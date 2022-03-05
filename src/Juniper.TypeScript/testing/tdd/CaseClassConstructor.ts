import type { TestCase } from "./TestCase";

export type CaseClassConstructor = {
    new(): TestCase;
};
