import { IAudioNode } from "../context/IAudioNode";
import type { JuniperAudioContext } from "../context/JuniperAudioContext";
import { RadioEffect } from "./RadioEffect";
import { WallEffect } from "./WallEffect";

export type EffectFactory = (name: string, context: JuniperAudioContext) => IAudioNode;

export const effectStore = /*@__PURE__*/ new Map<string, EffectFactory>([
    ["Radio", RadioEffect],
    ["Wall", WallEffect]
]);