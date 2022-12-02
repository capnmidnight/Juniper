import { JuniperAudioContext as AudioContext } from "../JuniperAudioContext";
import { JuniperAudioNode } from "../JuniperAudioNode";
import { BiquadFilter } from "../nodes";
import type { AudioConnection } from "../util";

export function RadioEffect(name: string, audioCtx: AudioContext, connectTo?: AudioConnection): RadioEffectNode {
    return new RadioEffectNode(name, audioCtx, connectTo);
};

class RadioEffectNode extends JuniperAudioNode {
    constructor(name: string, audioCtx: AudioContext, connectTo?: AudioConnection) {
        const node = BiquadFilter(`${name}-biquad-filter`, audioCtx, {
            type: "bandpass",
            frequency: 2500,
            Q: 4.5
        }, connectTo);
        super("radio-effect", audioCtx, [node], [node])
    }
}

