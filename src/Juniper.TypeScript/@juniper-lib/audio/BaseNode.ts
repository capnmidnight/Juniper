import { TypedEventTarget, TypedEventMap } from "@juniper-lib/events/TypedEventTarget";
import { IAudioNode, IAudioParam, isEndpoint, isIAudioNode } from "./IAudioNode";
import type { InputResolution, JuniperAudioContext, OutputResolution } from "./context/JuniperAudioContext";


export abstract class BaseNode<EventMapT extends TypedEventMap<string>>
    extends TypedEventTarget<EventMapT>
    implements IAudioNode {

    private _name: string = null;
    get name(): string { return this._name; }
    set name(v: string) {
        this._name = v;
        this.context._name(this, v);
    }

    constructor(
        public readonly nodeType: string,
        public readonly context: JuniperAudioContext) {
        super();
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            this.onDisposing();
        }
    }

    protected onDisposing() {
    }

    isConnected(dest?: IAudioNode | IAudioParam, output?: number, input?: number): boolean {
        return this.context._isConnected(this, dest, output, input);
    }

    resolveOutput(output: number): OutputResolution {
        let resolution: OutputResolution = {
            source: this,
            output
        };

        while (isIAudioNode(resolution.source)) {
            resolution = resolution.source._resolveOutput(resolution.output);
        }

        return resolution;
    }

    abstract _resolveOutput(output?: number): OutputResolution;

    resolveInput(input: number): InputResolution {
        let resolution: InputResolution = {
            destination: this,
            input
        };

        while (isEndpoint(resolution.destination)) {
            resolution = resolution.destination._resolveInput(resolution.input);
        }

        return resolution;
    }

    abstract _resolveInput(input?: number): InputResolution;


    toggle(destinationParam: IAudioParam, output?: number): void;
    toggle(destinationNode: IAudioNode, output?: number, input?: number): IAudioNode;
    toggle(dest: IAudioNode | IAudioParam, outp?: number, inp?: number): IAudioNode | void {
        this._toggle(dest, outp, inp);
    }

    private _toggle(dest: IAudioNode | IAudioParam, outp?: number, inp?: number): IAudioNode | void {
        if (this.isConnected(dest, outp, inp)) {
            this._disconnect(dest, outp, inp);
        }
        else {
            return this._connect(dest, outp, inp);
        }
    }

    connect(destinationParam: IAudioParam, output?: number): void;
    connect(destinationNode: IAudioNode, output?: number, input?: number): IAudioNode;
    connect(dest: IAudioNode | IAudioParam, outp?: number, inp?: number): IAudioNode | void {
        return this._connect(dest, outp, inp);
    }

    private _connect(dest: IAudioNode | IAudioParam, outp?: number, inp?: number): IAudioNode | void {
        return this.context._connect(this, dest, outp, inp);
    }

    disconnect(): void;
    disconnect(output: number): void;
    disconnect(destinationParam: IAudioParam, output?: number): void;
    disconnect(destinationNode: IAudioNode, output?: number, input?: number): void;
    disconnect(destinationOrOutput?: IAudioNode | IAudioParam | number, outp?: number, inp?: number): void {
        this._disconnect(destinationOrOutput, outp, inp);
    }

    private _disconnect(destinationOrOutput?: IAudioNode | IAudioParam | number, outp?: number, inp?: number): void {
        this.context._disconnect(this, destinationOrOutput, outp, inp);
    }

    abstract get channelCount(): number;
    abstract set channelCount(v: number);
    abstract get channelCountMode(): ChannelCountMode;
    abstract set channelCountMode(v: ChannelCountMode);
    abstract get channelInterpretation(): ChannelInterpretation;
    abstract set channelInterpretation(v: ChannelInterpretation);
    abstract get numberOfInputs(): number;
    abstract get numberOfOutputs(): number;
}