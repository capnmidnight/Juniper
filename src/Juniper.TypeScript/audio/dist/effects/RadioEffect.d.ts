import type { AudioConnection, WrappedAudioNode } from "../nodes";
export declare function RadioEffect(name: string, audioCtx: AudioContext, connectTo?: AudioConnection): RadioEffectNode;
declare class RadioEffectNode implements WrappedAudioNode {
    readonly node: BiquadFilterNode;
    constructor(name: string, audioCtx: AudioContext, connectTo?: AudioConnection);
    dispose(): void;
}
export {};
