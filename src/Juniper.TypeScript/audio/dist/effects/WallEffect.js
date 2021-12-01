import { BiquadFilter, removeVertex } from "../nodes";
export function WallEffect(name, audioCtx, connectTo) {
    return new WallEffectNode(name, audioCtx, connectTo);
}
class WallEffectNode {
    node;
    constructor(name, audioCtx, connectTo) {
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
