import { IAudioNode } from "./IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";


export class JuniperBiquadFilterNode extends BiquadFilterNode implements IAudioNode {
    constructor(private readonly jctx: JuniperAudioContext, options?: BiquadFilterOptions) {
        super(jctx, options);
        this.jctx._init("biquad-filter", this);
    }

    dispose() { this.jctx._dispose(this); }

    get name(): string { return this.jctx._getName(this); }
    set name(v: string) { this.jctx._setName(v, this); }

    override connect(destinationNode: AudioNode, output?: number, input?: number): AudioNode;
    override connect(destinationParam: AudioParam, output?: number): void;
    override connect(destination: AudioNode | AudioParam, output?: number, input?: number): AudioNode | void {
        this.jctx._connect(this, destination, output, input);
        return super.connect(destination as any, output, input);
    }

    override disconnect(): void;
    override disconnect(output: number): void;
    override disconnect(destinationNode: AudioNode): void;
    override disconnect(destinationNode: AudioNode, output: number): void;
    override disconnect(destinationNode: AudioNode, output: number, input: number): void;
    override disconnect(destinationParam: AudioParam): void;
    override disconnect(destinationParam: AudioParam, output: number): void;
    override disconnect(destination?: AudioNode | AudioParam | number, output?: number, input?: number): void {
        this.jctx._disconnect(this, destination, output, input);
        super.disconnect(destination as any, output, input);
    }
}
