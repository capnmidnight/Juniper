import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperBiquadFilterNode } from "../context/JuniperBiquadFilterNode";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { IAudioNode } from "../IAudioNode";

class RadioEffectNode extends BaseNodeCluster {
    constructor(context: JuniperAudioContext) {
        const filter = new JuniperBiquadFilterNode(context, {
            type: "bandpass",
            frequency: 2500,
            Q: 4.5
        });
        const gain = new JuniperGainNode(context, {
            gain: 10
        });
        filter.connect(gain);
        super("radio-effect", context, [filter], [gain]);
    }
}

export function RadioEffect(name: string, context: JuniperAudioContext): IAudioNode {
    const node = new RadioEffectNode(context);
    node.name = `${name}-radio-effect`;
    return node;
}
