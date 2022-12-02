import { IAudioNode } from "../IAudioNode";
import { JuniperAudioContext as AudioContext } from "../JuniperAudioContext";
import { AudioConnection } from "../util";
import { EchoEffect } from "./EchoEffect";
import { RadioEffect } from "./RadioEffect";
import { WallEffect } from "./WallEffect";

export type EffectFactory = (name: string, audioCtx: AudioContext, connectTo?: AudioConnection) => IAudioNode;

export const effectStore = /*@__PURE__*/ new Map<string, EffectFactory>([
    ["Radio", RadioEffect],
    ["Wall", WallEffect],
    ["Echo", EchoEffect]
]);