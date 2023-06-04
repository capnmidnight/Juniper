import { arrayRemove } from "@juniper-lib/collections/arrays";
import { InputResolution, JuniperAudioContext, OutputResolution } from "./context/JuniperAudioContext";
import { BaseNode } from "./BaseNode";
import { IAudioNode, IAudioParam, isIAudioNode } from "./IAudioNode";
import { dispose } from "@juniper-lib/tslib/using";

export abstract class BaseNodeCluster<EventsT = void>
    extends BaseNode<EventsT>
    implements IAudioNode {

    private readonly inputs: ReadonlyArray<IAudioNode | IAudioParam>;
    private readonly outputs: ReadonlyArray<IAudioNode>;
    private readonly allNodes: IAudioNode[];

    private get exemplar() { return this.allNodes[0]; }

    constructor(type: string,
        context: JuniperAudioContext,
        inputs?: ReadonlyArray<IAudioNode | IAudioParam>,
        endpoints?: ReadonlyArray<IAudioNode | IAudioParam>,
        extras?: ReadonlyArray<IAudioNode>) {
        super(type, context);

        inputs = inputs || [];
        extras = extras || [];
        const exits = endpoints || inputs;

        this.inputs = inputs;
        const entries = inputs
            .filter(isIAudioNode)
            .map(o => o as IAudioNode)
        this.outputs = exits
            .filter(isIAudioNode)
            .map(o => o as IAudioNode);

        this.allNodes = Array.from(new Set([
            ...entries,
            ...this.outputs,
            ...extras
        ]));
    }

    protected add(node: IAudioNode) {
        this.allNodes.push(node);
        this.context._parent(this, node);
    }

    protected remove(node: IAudioNode) {
        arrayRemove(this.allNodes, node);
        this.context._unparent(this, node);
    }

    protected override onDisposing() {
        this.allNodes.forEach(dispose);
        super.onDisposing();
    }

    get channelCount(): number { return this.exemplar.channelCount; }
    set channelCount(v: number) { this.allNodes.forEach(n => n.channelCount = v); }
    get channelCountMode(): ChannelCountMode { return this.exemplar.channelCountMode; }
    set channelCountMode(v: ChannelCountMode) { this.allNodes.forEach(n => n.channelCountMode = v);; }
    get channelInterpretation(): ChannelInterpretation { return this.exemplar.channelInterpretation; }
    set channelInterpretation(v: ChannelInterpretation) { this.allNodes.forEach(n => n.channelInterpretation = v); }
    get numberOfInputs(): number { return this.inputs.length; }
    get numberOfOutputs(): number { return this.outputs.length; }

    private static resolve<T>(source: ReadonlyArray<T>, index?: number): T {
        index = index || 0;
        if (index < 0 || source.length <= index) {
            return null;
        }

        return source[index];
    }

    _resolveInput(input?: number): InputResolution {
        return {
            destination: BaseNodeCluster.resolve(this.inputs, input)
        };
    }

    _resolveOutput(output?: number): OutputResolution {
        return {
            source: BaseNodeCluster.resolve(this.outputs, output)
        };
    }
}