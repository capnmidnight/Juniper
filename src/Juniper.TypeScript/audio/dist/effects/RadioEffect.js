import { BiquadFilter, removeVertex } from "../nodes";
export function RadioEffect(name, audioCtx, connectTo) {
    return new RadioEffectNode(name, audioCtx, connectTo);
}
;
class RadioEffectNode {
    node;
    constructor(name, audioCtx, connectTo) {
        this.node = BiquadFilter(`${name}-biquad-filter`, audioCtx, {
            type: "bandpass",
            frequency: 2500,
            Q: 4.5
        }, connectTo);
    }
    dispose() {
        removeVertex(this.node);
    }
}
