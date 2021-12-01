import { arrayClear } from "juniper-tslib";
import { Delay, Gain, removeVertex } from "../nodes";
function* fibGen(a, b, count = -1) {
    let i = 0;
    while (count < 0 || i < count) {
        yield a;
        ++i;
        const c = a + b;
        a = b;
        b = c;
    }
}
export function EchoEffect(name, audioCtx, connectTo) {
    return new EchoEffectNode(name, audioCtx, 3, connectTo);
}
class EchoEffectLayerNode {
    input;
    output;
    constructor(name, audioCtx, volume, delay, connectTo) {
        this.input = Gain(`${name}-input`, audioCtx, {
            gain: volume
        }, this.output = Delay(`${name}-delay`, audioCtx, {
            maxDelayTime: delay,
            delayTime: delay
        }, connectTo));
    }
    dispose() {
        removeVertex(this.input);
        removeVertex(this.output);
    }
}
class EchoEffectNode {
    input;
    layers;
    output;
    constructor(name, audioCtx, numLayers, connectTo) {
        this.output = Gain(`${name}-output`, audioCtx, null, connectTo);
        const delays = Array.from(fibGen(1, 2, numLayers));
        this.layers = delays.map((delay) => new EchoEffectLayerNode(`${name}-echo-layer-${delay}`, audioCtx, 0.5 / delay, delay, this.output));
        this.input = Gain(`${name}-input`, audioCtx, null, ...this.layers);
    }
    dispose() {
        removeVertex(this.input);
        for (const layer of this.layers) {
            layer.dispose();
        }
        removeVertex(this.output);
        arrayClear(this.layers);
    }
}
