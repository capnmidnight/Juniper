import type { AudioNodeType, AudioVertex } from "../nodes";
import { EchoEffect } from "./EchoEffect";
import { RadioEffect } from "./RadioEffect";
import { WallEffect } from "./WallEffect";

export type EffectFactory = (name: string, audioCtx: AudioContext, connectTo?: AudioVertex) => AudioNodeType;

export const effectStore = new Map<string, EffectFactory>([
    ["Radio", RadioEffect],
    ["Wall", WallEffect],
    ["Echo", EchoEffect]
]);

export * from "./EchoEffect";
export * from "./RadioEffect";
export * from "./WallEffect";
