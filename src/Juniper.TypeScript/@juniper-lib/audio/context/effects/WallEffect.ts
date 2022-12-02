import { JuniperAudioContext } from "../JuniperAudioContext";
import { JuniperBiquadFilterNode } from "../JuniperBiquadFilterNode";
import { init } from "../util";


export function WallEffect(name: string, audioCtx: JuniperAudioContext): JuniperBiquadFilterNode {
    return init(`${name}-wall-effect`, new JuniperBiquadFilterNode(audioCtx, {
        type: "bandpass",
        frequency: 400,
        Q: 4.5
    }));
}
