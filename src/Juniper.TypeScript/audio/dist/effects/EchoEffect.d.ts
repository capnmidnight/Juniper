import type { AudioConnection, ErsatzAudioNode } from "../nodes";
export declare function EchoEffect(name: string, audioCtx: AudioContext, connectTo?: AudioConnection): EchoEffectNode;
declare class EchoEffectNode implements ErsatzAudioNode {
    readonly input: GainNode;
    private readonly layers;
    readonly output: GainNode;
    constructor(name: string, audioCtx: AudioContext, numLayers: number, connectTo?: AudioConnection);
    dispose(): void;
}
export {};
