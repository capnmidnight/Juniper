import { JuniperAudioContext as AudioContext } from "../JuniperAudioContext";
import { JuniperAudioNode } from "../JuniperAudioNode";
import { Delay, Gain } from "../nodes";
import type { AudioConnection } from "../util";


function* fibGen(a: number, b: number, count: number = -1) {
    let i = 0;
    while (count < 0 || i < count) {
        yield a;
        ++i;
        const c = a + b;
        a = b;
        b = c;
    }
}

export function EchoEffect(name: string, audioCtx: AudioContext, connectTo?: AudioConnection) {
    return new EchoEffectNode(name, audioCtx, 3, connectTo);
}

class EchoEffectLayerNode extends JuniperAudioNode {
    constructor(name: string, audioCtx: AudioContext, volume: number, delay: number, connectTo?: AudioConnection) {
        const output = Delay(`${name}-delay`,
            audioCtx, {
            maxDelayTime: delay,
            delayTime: delay
        },
            connectTo);
        const input = Gain(
            `${name}-input`,
            audioCtx, {
            gain: volume
        },
            output);
        super("echo-effect-layer", audioCtx, [input], [output]);
    }
}

class EchoEffectNode extends JuniperAudioNode {
    private readonly layers: EchoEffectLayerNode[];

    constructor(name: string, audioCtx: AudioContext, numLayers: number, connectTo?: AudioConnection) {
        const output = Gain(`${name}-output`, audioCtx, null, connectTo);

        const delays = Array.from(fibGen(1, 2, numLayers));
        const layers = delays.map((delay) =>
            new EchoEffectLayerNode(
                `${name}-echo-layer-${delay}`,
                audioCtx,
                0.5 / delay,
                delay,
                output
            ));

        const input = Gain(`${name}-input`, audioCtx, null, ...layers);
        super("echo-effect", audioCtx, [input], [output]);
        this.layers = layers;
    }

    override onDispose() {
        this.layers.forEach(v => v.dispose());
        super.onDispose();
    }
}
