import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { GraphNode } from "@juniper-lib/tslib/collections/GraphNode";
import { once } from "@juniper-lib/tslib/events/once";
import { assertNever } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode, IAudioParam } from "./IAudioNode";
import { JuniperAnalyserNode } from "./JuniperAnalyserNode";
import { JuniperAudioBufferSourceNode } from "./JuniperAudioBufferSourceNode";
import { JuniperAudioDestinationNode } from "./JuniperAudioDestinationNode";
import { JuniperBaseNode } from "./JuniperBaseNode";
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

export class JuniperAudioContext extends AudioContext {
    private readonly counters = new Map<string, number>();

    private readonly _destination: JuniperAudioDestinationNode;

    private readonly nodes = new Set<IAudioNode | IAudioParam>();

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

    _init(node: IAudioNode | IAudioParam): void {
        this.nodes.add(node);

        if (!this.counters.has(node.nodeType)) {
            this.counters.set(node.nodeType, 0);
        }

        const count = this.counters.get(node.nodeType);
        node.name = `${node.nodeType}-${count}`;

        this.counters.set(node.nodeType, count + 1);
    }

    _dispose(node: IAudioNode | IAudioParam): void {
        this.nodes.delete(node);
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

    getAudioGraph(): Array<GraphNode<IAudioNode | IAudioParam>> {
        const nodes = new Map<IAudioNode | IAudioParam, GraphNode<IAudioNode | IAudioParam>>();

        for (const node of this.nodes) {
            nodes.set(node, new GraphNode(node));
        }

        for (const parent of this.nodes) {
            if (parent instanceof JuniperBaseNode) {
                const branch = nodes.get(parent);
                for (const child of parent.connections) {
                    const node = child[0];
                    if (nodes.has(node)) {
                        branch.connectTo(nodes.get(node));
                    }
                }
            }
        }

        return Array.from(nodes.values());
    }
}
