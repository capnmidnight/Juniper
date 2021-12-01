import { EchoEffect } from "./EchoEffect";
import { RadioEffect } from "./RadioEffect";
import { WallEffect } from "./WallEffect";
export const effectStore = new Map([
    ["Radio", RadioEffect],
    ["Wall", WallEffect],
    ["Echo", EchoEffect]
]);
export * from "./EchoEffect";
export * from "./RadioEffect";
export * from "./WallEffect";
