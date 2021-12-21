import type { AudioConnection, WrappedAudioNode } from "../nodes";
import { BiquadFilter, removeVertex } from "../nodes";


export function WallEffect(name: string, audioCtx: AudioContext, connectTo?: AudioConnection): WallEffectNode {
    return new WallEffectNode(name, audioCtx, connectTo);
}

class WallEffectNode implements WrappedAudioNode {
    readonly node: BiquadFilterNode;

    constructor(name: string, audioCtx: AudioContext, connectTo?: AudioConnection) {
        this.node = BiquadFilter(`${name}-biquad-filter`, audioCtx, {
            type: "bandpass",
            frequency: 400,
            Q: 4.5
        }, connectTo);
    }

    dispose() {
        removeVertex(this.node);
    }
}
