import type { AudioConnection, WrappedAudioNode } from "../nodes";
export declare function WallEffect(name: string, audioCtx: AudioContext, connectTo?: AudioConnection): WallEffectNode;
declare class WallEffectNode implements WrappedAudioNode {
    readonly node: BiquadFilterNode;
    constructor(name: string, audioCtx: AudioContext, connectTo?: AudioConnection);
    dispose(): void;
}
export {};
