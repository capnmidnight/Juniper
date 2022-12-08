import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperBiquadFilterNode } from "../context/JuniperBiquadFilterNode";

export function RadioEffect(name: string, context: JuniperAudioContext): JuniperBiquadFilterNode {
    const node = new JuniperBiquadFilterNode(context, {
        type: "bandpass",
        frequency: 2500,
        Q: 4.5
    });
    node.name = `${name}-radio-effect`;
    return node;
}
