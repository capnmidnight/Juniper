import { JuniperAudioContext } from "../JuniperAudioContext";
import { JuniperAudioNode } from "../JuniperAudioNode";
import { Delay, Gain } from "../nodes";
import { chain, init } from "../util";


export function EchoEffect(name: string, audioCtx: JuniperAudioContext) {
    return init(name, new EchoEffectNode(audioCtx, 3));
}


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

export class EchoEffectNode extends JuniperAudioNode {
    private static counter = 0;

    constructor(audioCtx: JuniperAudioContext, numLayers: number) {
        const index = EchoEffectNode.counter++;
        const prefix = `echo-effect-${index}`;

        const input = Gain(`${prefix}-input`, audioCtx);
        const output = Gain(`${prefix}-output`, audioCtx);

        const delays = Array.from(fibGen(1, 2, numLayers));
        const layers = delays.map((delay, i) =>
            init(`${prefix}-layer-${i}`, new EchoEffectLayerNode(
                audioCtx,
                0.5 / delay,
                delay
            ))
        );

        layers.forEach(l => chain(input, l, output));

        super("echo-effect", audioCtx, [input], [output], layers);
    }
}

class EchoEffectLayerNode extends JuniperAudioNode {
    private static counter = 0;

    constructor(audioCtx: JuniperAudioContext, volume: number, delay: number) {
        const index = EchoEffectLayerNode.counter++;
        const prefix = `echo-effect-layer-${index}`;

        const input = Gain(
            `${prefix}-input`,
            audioCtx, {
            gain: volume
        });

        const output = Delay(`${prefix}-output`,
            audioCtx, {
            maxDelayTime: delay,
            delayTime: delay
        });

        input.connect(output);

        super("echo-effect-layer", audioCtx, [input], [output]);
    }
}