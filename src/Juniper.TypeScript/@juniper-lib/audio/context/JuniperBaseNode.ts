import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { assertNever, isDefined, isNullOrUndefined, isNumber } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode, IAudioParam } from "./IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioParam } from "./JuniperAudioParam";


export type AudioConnection = [IAudioNode | IAudioParam, number, number];

export abstract class JuniperBaseNode<EventsT = void>
    extends TypedEventBase<EventsT & void>
    implements IAudioNode {

    private readonly conns = new Set<AudioConnection>()
    get connections(): ReadonlySet<AudioConnection> { return this.conns; }

    name: string = null;

    constructor(
        public readonly nodeType: string,
        public readonly context: JuniperAudioContext) {
        super();
        this.context._init(this);
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

    private __resolveOutput(output: number): { source: AudioNode, output: number } {
        let source: AudioNode = this;

        while (source instanceof JuniperBaseNode) {
            ({ source, output } = source._resolveOutput(output));
        }

        return {
            source,
            output
        };
    }

    protected abstract _resolveOutput(output?: number): { source: AudioNode; output?: number }

    private __resolveInput(dest: IAudioNode | IAudioParam, input: number): { destination: AudioNode | AudioParam, input: number } {
        let destination: AudioNode | AudioParam = dest;

        while (destination instanceof JuniperBaseNode
            || destination instanceof JuniperAudioParam) {
            ({ destination, input } = destination._resolveInput(input));
        }

        return {
            destination,
            input
        };
    }

    abstract _resolveInput(input?: number): { destination: AudioNode | AudioParam; input?: number; }

    connect(destinationParam: IAudioParam, output?: number): void;
    connect(destinationNode: IAudioNode, output?: number, input?: number): IAudioNode;
    connect(dest: IAudioNode | IAudioParam, output?: number, input?: number): IAudioNode | void {
        let source: AudioNode;
        let destination: AudioNode | AudioParam;

        ({ source, output } = this.__resolveOutput(output));
        ({ destination, input } = this.__resolveInput(dest, input));

        if (destination instanceof AudioNode) {
            dest = dest as IAudioNode;
            if (isDefined(input)) {
                source.connect(destination, output, input);
                this.conns.add([dest, output, input]);
                return dest;
            }
            else if (isDefined(output)) {
                source.connect(destination, output);
                this.conns.add([dest, output, input]);
                return dest;
            }
            else {
                source.connect(destination);
                this.conns.add([dest, output, input]);
                return dest;
            }
        }
        else {
            dest = dest as IAudioParam;
            if (isDefined(output)) {
                source.connect(destination, output);
                this.conns.add([dest, output, input]);
            }
            else if (isDefined(destination)) {
                source.connect(destination);
                this.conns.add([dest, output, input]);
            }
            else {
                assertNever(destination);
            }
        }
    }

    disconnect(): void;
    disconnect(output: number): void;
    disconnect(destinationParam: IAudioParam, output?: number): void;
    disconnect(destinationNode: IAudioNode, output?: number, input?: number): void;
    disconnect(destinationOrOutput?: IAudioNode | IAudioParam | number, output?: number, input?: number): void {

        let dest: IAudioNode | IAudioParam;
        if (isNumber(destinationOrOutput)) {
            dest = undefined;
            output = destinationOrOutput;
        }

        let source: AudioNode;
        let destination: AudioNode | AudioParam;

        ({ source, output } = this.__resolveOutput(output));
        ({ destination, input } = this.__resolveInput(dest, input));

        const toDelete = new Set<AudioConnection>();
        for (const conn of this.conns) {
            if (isMatchingConnection(conn, dest, output, input)) {
                toDelete.add(conn);
            }
        }

        for (const conn of toDelete) {
            this.conns.delete(conn);
        }

        if (destination instanceof AudioNode) {
            if (isDefined(input)) {
                source.disconnect(destination, output, input);
            }
            else if (isDefined(output)) {
                source.disconnect(destination, output);
            }
            else if (isDefined(destination)) {
                source.disconnect(destination);
            }
            else {
                source.disconnect();
            }
        }
        else {
            if (isDefined(output)) {
                source.disconnect(destination, output);
            }
            else if (isDefined(destination)) {
                source.disconnect(destination);
            }
            else {
                source.disconnect();
            }
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


function isMatchingConnection(conn: AudioConnection, destinationOrOutput?: IAudioNode | IAudioParam | number, output?: number, input?: number): boolean {
    let destination: IAudioNode | IAudioParam = null;
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