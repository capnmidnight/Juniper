import type { AudioNodeType, AudioVertex } from "../nodes";
export declare type EffectFactory = (name: string, audioCtx: AudioContext, connectTo?: AudioVertex) => AudioNodeType;
export declare const effectStore: Map<string, EffectFactory>;
export * from "./EchoEffect";
export * from "./RadioEffect";
export * from "./WallEffect";
