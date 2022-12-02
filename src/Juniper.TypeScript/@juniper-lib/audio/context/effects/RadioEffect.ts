import { JuniperAudioContext } from "../JuniperAudioContext";
import { JuniperBiquadFilterNode } from "../JuniperBiquadFilterNode";
import { init } from "../util";

export function RadioEffect(name: string, audioCtx: JuniperAudioContext): JuniperBiquadFilterNode {
    return init(`${name}-radio-effect`, new JuniperBiquadFilterNode(audioCtx, {
        type: "bandpass",
        frequency: 2500,
        Q: 4.5
    }));
}
