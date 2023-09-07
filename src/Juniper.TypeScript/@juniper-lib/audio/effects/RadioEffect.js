import { BaseNodeCluster } from "../BaseNodeCluster";
import { JuniperBiquadFilterNode } from "../context/JuniperBiquadFilterNode";
import { JuniperGainNode } from "../context/JuniperGainNode";
class RadioEffectNode extends BaseNodeCluster {
    constructor(context) {
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
export function RadioEffect(name, context) {
    const node = new RadioEffectNode(context);
    node.name = `${name}-radio-effect`;
    return node;
}
//# sourceMappingURL=RadioEffect.js.map