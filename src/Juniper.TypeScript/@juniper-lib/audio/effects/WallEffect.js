import { JuniperBiquadFilterNode } from "../context/JuniperBiquadFilterNode";
export function WallEffect(name, context) {
    const node = new JuniperBiquadFilterNode(context, {
        type: "bandpass",
        frequency: 400,
        Q: 4.5
    });
    node.name = `${name}-wall-effect`;
    return node;
}
//# sourceMappingURL=WallEffect.js.map