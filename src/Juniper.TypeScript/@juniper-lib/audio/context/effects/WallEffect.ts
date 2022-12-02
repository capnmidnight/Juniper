import { JuniperAudioContext as AudioContext } from "../JuniperAudioContext";
import { JuniperAudioNode } from "../JuniperAudioNode";
import { BiquadFilter } from "../nodes";
import type { AudioConnection } from "../util";


export function WallEffect(name: string, audioCtx: AudioContext, connectTo?: AudioConnection): WallEffectNode {
    return new WallEffectNode(name, audioCtx, connectTo);
}

class WallEffectNode extends JuniperAudioNode {
    constructor(name: string, audioCtx: AudioContext, connectTo?: AudioConnection) {
        const node = BiquadFilter(`${name}-biquad-filter`, audioCtx, {
            type: "bandpass",
            frequency: 400,
            Q: 4.5
        }, connectTo);
        super("wall-effect", audioCtx, [node], [node]);
    }
}
