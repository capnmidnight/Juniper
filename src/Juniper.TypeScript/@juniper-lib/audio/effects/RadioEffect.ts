import type { AudioConnection, WrappedAudioNode } from "../nodes";
import { BiquadFilter, removeVertex } from "../nodes";

export function RadioEffect(name: string, audioCtx: AudioContext, connectTo?: AudioConnection): RadioEffectNode {
    return new RadioEffectNode(name, audioCtx, connectTo);
};

class RadioEffectNode implements WrappedAudioNode {
    readonly node: BiquadFilterNode;

    constructor(name: string, audioCtx: AudioContext, connectTo?: AudioConnection) {
        this.node = BiquadFilter(`${name}-biquad-filter`, audioCtx, {
            type: "bandpass",
            frequency: 2500,
            Q: 4.5
        }, connectTo);
    }

    dispose() {
        removeVertex(this.node);
    }
}

