import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { Exception } from "@juniper-lib/tslib/Exception";
import { isNumber } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode } from "./IAudioNode";
import { JuniperAudioContext } from "./JuniperAudioContext";


export abstract class JuniperAudioNode<EventsT = void>
    extends TypedEventBase<EventsT & void>
    implements IAudioNode {

    private readonly outputs: ReadonlyArray<IAudioNode>;
    private readonly allNodes: ReadonlyArray<IAudioNode>;

    constructor(type: string,
        public readonly context: JuniperAudioContext,
        private readonly inputs: ReadonlyArray<IAudioNode>,
        outputs?: ReadonlyArray<IAudioNode>,
        extra?: ReadonlyArray<IAudioNode>) {
        super();
        this.context._init(type, this);
        this.allNodes = [...extra, ...inputs, ...outputs];
    }

    get name(): string { return this.context._getName(this); }
    set name(v: string) { this.context._setName(v, this); }

    private disposed = false;
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            this.onDispose();
        }
    }

    protected onDispose() {
        this.allNodes.forEach(v => v.dispose());
        this.context._dispose(this);
    }

    private get exemplar() { return this.allNodes[0]; }

    get channelCount(): number { return this.exemplar.channelCount; }
    get channelCountMode(): ChannelCountMode { return this.exemplar.channelCountMode; }
    get channelInterpretation(): ChannelInterpretation { return this.exemplar.channelInterpretation; }

    get numberOfInputs(): number {
        return this.inputs.length;
    }

    get numberOfOutputs(): number {
        return this.outputs.length;
    }

    private static resolve(type: string, source: ReadonlyArray<IAudioNode>, index?: number): IAudioNode {
        index = index || 0;
        if (index < 0 || source.length <= index) {
            throw new Exception(`Index out of range: ${type}`);
        }

        return source[index];
    }

    private resolveOutput(output?: number): IAudioNode {
        return JuniperAudioNode.resolve("output", this.outputs, output);
    }

    private resolveInput(input?: number): IAudioNode {
        return JuniperAudioNode.resolve("input", this.inputs, input);
    }

    connect(destinationNode: IAudioNode, output?: number, input?: number): IAudioNode;
    connect(destinationParam: AudioParam, output?: number): void;
    connect(destination: IAudioNode | AudioParam, output?: number, input?: number): IAudioNode | void {
        this.context._connect(this, destination, output, input);

        const source = this.resolveOutput(output);

        if (destination instanceof AudioNode) {
            if (destination instanceof JuniperAudioNode) {
                destination = destination.resolveInput(input);
                output = undefined;
            }

            source.connect(destination, output, input);
            return destination;
        }
        else {
            source.connect(destination, output);
        }
    }

    disconnect(): void;
    disconnect(output: number): void;
    disconnect(destinationNode: IAudioNode): void;
    disconnect(destinationNode: IAudioNode, output: number): void;
    disconnect(destinationNode: IAudioNode, output: number, input: number): void;
    disconnect(destinationParam: AudioParam): void;
    disconnect(destinationParam: AudioParam, output: number): void;
    disconnect(destinationOrOutput?: IAudioNode | AudioParam | number, output?: number, input?: number): void {
        this.context._disconnect(this, destinationOrOutput, output, input);

        let destination: IAudioNode | AudioParam = undefined;
        if (isNumber(destinationOrOutput)) {
            output = destinationOrOutput;
        }
        else {
            destination = destinationOrOutput;
            output = undefined;
        }

        const source = this.resolveOutput(output);

        if (destination instanceof AudioNode) {
            if (destination instanceof JuniperAudioNode) {
                destination = destination.resolveInput(input);
                output = undefined;
            }

            source.disconnect(destination, output, input);
        }
        else {
            source.disconnect(destination, output);
        }
    }
}
