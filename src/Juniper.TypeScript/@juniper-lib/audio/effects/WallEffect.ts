import { BiquadFilter } from "../nodes";
import type { AudioConnection, ErsatzAudioNode } from "../util";
import { removeVertex } from "../util";


export function WallEffect(name: string, audioCtx: AudioContext, connectTo?: AudioConnection): WallEffectNode {
    return new WallEffectNode(name, audioCtx, connectTo);
}

class WallEffectNode implements ErsatzAudioNode {

    private readonly node: BiquadFilterNode;
    get input() { return this.node; }
    get output() { return this.node; }

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
