import { IAudioNode } from "../IAudioNode";
import type { JuniperAudioContext } from "../JuniperAudioContext";
import { EchoEffect } from "./EchoEffect";
import { RadioEffect } from "./RadioEffect";
import { WallEffect } from "./WallEffect";

export type EffectFactory = (name: string, audioCtx: JuniperAudioContext) => IAudioNode;

export const effectStore = /*@__PURE__*/ new Map<string, EffectFactory>([
    ["Radio", RadioEffect],
    ["Wall", WallEffect],
    ["Echo", EchoEffect]
]);