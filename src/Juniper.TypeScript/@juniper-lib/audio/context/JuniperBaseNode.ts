import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { assertNever, isDefined, isNullOrUndefined, isNumber } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode, IAudioParam, isEndpoint, isIAudioNode } from "./IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";


export type AudioConnection = [AudioNode | AudioParam, number, number];
export interface OutputResolution {
    source: AudioNode;
    output?: number;
}
export interface InputResolution {
    destination: AudioNode | AudioParam;
    input?: number;
}

export abstract class JuniperBaseNode<EventsT = void>
    extends TypedEventBase<EventsT & void>
    implements IAudioNode {

    private readonly conns = new Set<AudioConnection>()
    get connections(): ReadonlySet<AudioConnection> { return this.conns; }
    parent(p: AudioNode | AudioParam) {
        this.conns.add([p, null, null]);
    }

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
        this.context._init(this, nodeType);
    }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            this.onDisposing();
        }
    }

    protected onDisposing() {
        this.context._dispose(this);
    }

    get connected() { return this.conns.size > 0; }

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

        this.conns.add([source, outp, null]);
        this.conns.add([dest, null, inp]);
        this.conns.add([destination, output, input]);

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

        this.conns.add([source, outp, null]);
        this.conns.add([dest, null, inp]);
        this.conns.add([destination, output, input]);

        const toDelete = new Set<AudioConnection>();
        for (const conn of this.conns) {
            if (isMatchingConnection(conn, source, outp, null)
                || isMatchingConnection(conn, dest, null, inp)
                || isMatchingConnection(conn, destination, output, input)) {
                toDelete.add(conn);
            }
        }

        for (const conn of toDelete) {
            this.conns.delete(conn);
        }

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


function isMatchingConnection(conn: AudioConnection, destinationOrOutput?: AudioNode | AudioParam | number, output?: number, input?: number): boolean {
    let destination: AudioNode | AudioParam = null;
    if (isNumber(destinationOrOutput)) {
        output = destinationOrOutput;
    }
    else {
        destination = destinationOrOutput;
    }

    return (isNullOrUndefined(destination)
        || destination === conn[0])
        && (isNullOrUndefined(output)
            || output === conn[1])
        && (isNullOrUndefined(input)
            || input === conn[2]);
}