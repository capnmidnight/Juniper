import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { GraphNode } from "@juniper-lib/tslib/collections/GraphNode";
import { once } from "@juniper-lib/tslib/events/once";
import { assertNever } from "@juniper-lib/tslib/typeChecks";
import { isEndpoint, isIAudioNode } from "./IAudioNode";
import { JuniperAnalyserNode } from "./JuniperAnalyserNode";
import { JuniperAudioBufferSourceNode } from "./JuniperAudioBufferSourceNode";
import { JuniperAudioDestinationNode } from "./JuniperAudioDestinationNode";
import { JuniperBiquadFilterNode } from "./JuniperBiquadFilterNode";
import { JuniperChannelMergerNode } from "./JuniperChannelMergerNode";
import { JuniperChannelSplitterNode } from "./JuniperChannelSplitterNode";
import { JuniperConstantSourceNode } from "./JuniperConstantSourceNode";
import { JuniperConvolverNode } from "./JuniperConvolverNode";
import { JuniperDelayNode } from "./JuniperDelayNode";
import { JuniperDynamicsCompressorNode } from "./JuniperDynamicsCompressorNode";
import { JuniperGainNode } from "./JuniperGainNode";
import { JuniperIIRFilterNode } from "./JuniperIIRFilterNode";
import { JuniperMediaElementAudioSourceNode } from "./JuniperMediaElementAudioSourceNode";
import { JuniperMediaStreamAudioDestinationNode } from "./JuniperMediaStreamAudioDestinationNode";
import { JuniperMediaStreamAudioSourceNode } from "./JuniperMediaStreamAudioSourceNode";
import { JuniperOscillatorNode } from "./JuniperOscillatorNode";
import { JuniperPannerNode } from "./JuniperPannerNode";
import { JuniperStereoPannerNode } from "./JuniperStereoPannerNode";
import { JuniperWaveShaperNode } from "./JuniperWaveShaperNode";


if (!("AudioContext" in globalThis) && "webkitAudioContext" in globalThis) {
    globalThis.AudioContext = (globalThis as any).webkitAudioContext;
}

if (!("OfflineAudioContext" in globalThis) && "webkitOfflineAudioContext" in globalThis) {
    globalThis.OfflineAudioContext = (globalThis as any).webkitOfflineAudioContext;
}


export type ClassID =
    | "jnode"
    | "node"
    | "jparam"
    | "param"
    | "unknown";

export interface Vertex {
    type: string;
    name: string;
    classID: ClassID;
}

export class JuniperAudioContext extends AudioContext {
    private readonly counters = new Map<string, number>();

    private readonly _destination: JuniperAudioDestinationNode;

    private readonly nodes = new Set<AudioNode | AudioParam>();
    private readonly types = new Map<AudioNode | AudioParam, string>();
    private readonly names = new Map<AudioNode | AudioParam, string>();

    public readonly ready: Promise<void>;
    private _isReady = false;
    get isReady() { return this._isReady; }

    constructor(contextOptions?: AudioContextOptions) {
        super(contextOptions);
        this._destination = new JuniperAudioDestinationNode(this, super.destination);
        this.ready = this.checkReady();
    }

    private async checkReady(): Promise<void> {
        if (this.state !== "running") {
            if (this.state === "closed") {
                await this.resume();
            }
            else if (this.state === "suspended") {
                const stateChange = once<BaseAudioContextEventMap>(this, "statechange");
                onUserGesture(() => this.resume());
                await stateChange;
            }
            else {
                assertNever(this.state);
            }

            this._isReady = true;
        }
    }

    _init(node: AudioNode | AudioParam, type: string): void {
        if (!this.counters.has(type)) {
            this.counters.set(type, 0);
        }

        const count = this.counters.get(type);
        const name = `${type}-${count}`;

        this.nodes.add(node);
        this.types.set(node, type);
        this.names.set(node, name);

        if (isEndpoint(node)) {
            node.name = name;
        }

        this.counters.set(type, count + 1);
    }

    _name(node: AudioNode | AudioParam, name: string): void {
        if (this.names.has(node)) {
            this.names.set(node, name);
        }
    }

    _dispose(node: AudioNode | AudioParam): void {
        this.nodes.delete(node);
        this.types.delete(node);
        this.names.delete(node);
    }

    getAudioGraph(): Array<GraphNode<Vertex>> {
        const nodes = new Map<AudioNode | AudioParam, GraphNode<Vertex>>();

        for (const node of this.nodes) {
            const classID = isIAudioNode(node)
                ? "jnode"
                : isEndpoint(node)
                    ? "jparam"
                    : node instanceof AudioNode
                        ? "node"
                        : "param";
            nodes.set(node, new GraphNode({
                name: this.names.get(node),
                type: this.types.get(node),
                classID
            }));
        }

        for (const parent of this.nodes) {
            if (isIAudioNode(parent)) {
                const branch = nodes.get(parent);
                for (const child of parent.connections) {
                    const node = child[0];
                    if (nodes.has(node)) {
                        const cnode = nodes.get(node);
                        branch.connectTo(cnode);
                        cnode.connectTo(branch);
                    }
                }
            }
        }

        return Array.from(nodes.values());
    }

    override get destination(): JuniperAudioDestinationNode {
        return this._destination;
    }

    override createAnalyser(): JuniperAnalyserNode {
        return new JuniperAnalyserNode(this);
    }

    override createBiquadFilter(): JuniperBiquadFilterNode {
        return new JuniperBiquadFilterNode(this);
    }

    override createBufferSource(): JuniperAudioBufferSourceNode {
        return new JuniperAudioBufferSourceNode(this);
    }

    override createChannelMerger(numberOfInputs?: number): JuniperChannelMergerNode {
        return new JuniperChannelMergerNode(this, {
            numberOfInputs
        });
    }

    override createChannelSplitter(numberOfOutputs?: number): JuniperChannelSplitterNode {
        return new JuniperChannelSplitterNode(this, {
            numberOfOutputs
        });
    }

    override createConstantSource(): JuniperConstantSourceNode {
        return new JuniperConstantSourceNode(this);
    }

    override createConvolver(): JuniperConvolverNode {
        return new JuniperConvolverNode(this);
    }

    override createDelay(maxDelayTime?: number): JuniperDelayNode {
        return new JuniperDelayNode(this, {
            maxDelayTime
        });
    }

    override createDynamicsCompressor(): JuniperDynamicsCompressorNode {
        return new JuniperDynamicsCompressorNode(this);
    }

    override createGain(): JuniperGainNode {
        return new JuniperGainNode(this);
    }

    override createIIRFilter(feedforward: number[], feedback: number[]): JuniperIIRFilterNode {
        return new JuniperIIRFilterNode(this, {
            feedforward,
            feedback
        });
    }

    override createMediaElementSource(mediaElement: HTMLMediaElement): JuniperMediaElementAudioSourceNode {
        return new JuniperMediaElementAudioSourceNode(this, {
            mediaElement
        });
    }

    override createMediaStreamDestination(): JuniperMediaStreamAudioDestinationNode {
        return new JuniperMediaStreamAudioDestinationNode(this);
    }

    override createMediaStreamSource(mediaStream: MediaStream): JuniperMediaStreamAudioSourceNode {
        return new JuniperMediaStreamAudioSourceNode(this, {
            mediaStream
        });
    }

    override createOscillator(): JuniperOscillatorNode {
        return new JuniperOscillatorNode(this);
    }

    override createPanner(): JuniperPannerNode {
        return new JuniperPannerNode(this);
    }

    override createStereoPanner(): JuniperStereoPannerNode {
        return new JuniperStereoPannerNode(this);
    }

    override createWaveShaper(): JuniperWaveShaperNode {
        return new JuniperWaveShaperNode(this);
    }

    override createScriptProcessor(): ScriptProcessorNode {
        throw new Error("Script processor nodes are deprecated");
    }
}
