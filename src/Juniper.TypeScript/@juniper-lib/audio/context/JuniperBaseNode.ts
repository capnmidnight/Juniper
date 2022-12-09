import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { assertNever, isDefined, isNumber } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode, IAudioParam, isEndpoint, isIAudioNode } from "./IAudioNode";
import type { InputResolution, JuniperAudioContext, OutputResolution } from "./JuniperAudioContext";


export abstract class JuniperBaseNode<EventsT = void>
    extends TypedEventBase<EventsT & void>
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

    isConnected(output?: number): boolean { return this.context._isConnected(this, output); }

    private __resolveOutput(output: number): OutputResolution {
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

    private __resolveInput(destination: IAudioNode | IAudioParam, input: number): InputResolution {
        let resolution: InputResolution = {
            destination,
            input
        };

        while (isEndpoint(resolution.destination)) {
            resolution = resolution.destination._resolveInput(resolution.input);
        }

        return resolution;
    }

    abstract _resolveInput(input?: number): InputResolution;

    connect(destinationParam: IAudioParam, output?: number): void;
    connect(destinationNode: IAudioNode, output?: number, input?: number): IAudioNode;
    connect(dest: IAudioNode | IAudioParam, outp?: number, inp?: number): IAudioNode | void {
        const sourceRes = this.__resolveOutput(outp);
        const destRes = this.__resolveInput(dest, inp);
        const { source, output } = sourceRes;
        const { destination, input } = destRes;

        this.context._connect(source, destination, output, input);

        if (destination instanceof AudioNode) {
            dest = dest as IAudioNode;
            if (isDefined(input)) {
                source.connect(destination, output, input);
                return dest;
            }
            else if (isDefined(output)) {
                source.connect(destination, output);
                return dest;
            }
            else {
                source.connect(destination);
                return dest;
            }
        }
        else if (destination instanceof AudioParam) {
            if (isDefined(output)) {
                source.connect(destination, output);
            }
            else if (isDefined(destination)) {
                source.connect(destination);
            }
            else {
                assertNever(destination);
            }
        }
        else {
            assertNever(destination);
        }
    }

    disconnect(): void;
    disconnect(output: number): void;
    disconnect(destinationParam: IAudioParam, output?: number): void;
    disconnect(destinationNode: IAudioNode, output?: number, input?: number): void;
    disconnect(destinationOrOutput?: IAudioNode | IAudioParam | number, outp?: number, inp?: number): void {

        let dest: IAudioNode | IAudioParam;
        if (isNumber(destinationOrOutput)) {
            dest = undefined;
            outp = destinationOrOutput;
        }

        const sourceRes = this.__resolveOutput(outp);
        const destRes = this.__resolveInput(dest, inp);
        const { source, output } = sourceRes;
        const { destination, input } = destRes;

        this.context._disconnect(source, destination, output, input);

        if (destination instanceof AudioNode) {
            if (isDefined(inp)) {
                source.disconnect(destination, outp, inp);
            }
            else if (isDefined(outp)) {
                source.disconnect(destination, outp);
            }
            else if (isDefined(destination)) {
                source.disconnect(destination);
            }
            else {
                source.disconnect();
            }
        }
        else if (isDefined(outp)) {
            source.disconnect(destination, outp);
        }
        else if (isDefined(destination)) {
            source.disconnect(destination);
        }
        else {
            source.disconnect();
        }
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