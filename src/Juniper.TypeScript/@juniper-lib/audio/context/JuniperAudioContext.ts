import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { GraphNode } from "@juniper-lib/tslib/collections/GraphNode";
import { once } from "@juniper-lib/tslib/events/once";
import { assertNever, isNullOrUndefined, isNumber } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode, IAudioParam, isEndpoint } from "./IAudioNode";
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
    | "node"
    | "param"
    | "unknown";

export interface OutputResolution {
    source: AudioNode;
    output?: number;
}

export interface InputResolution {
    destination: AudioNode | AudioParam;
    input?: number;
}

export interface AudioConnection {
    type: "conn" | "parent";
    destination: AudioNode | AudioParam;
    output?: number;
    input?: number;
}

export interface Vertex {
    type: string;
    name: string;
    classID: ClassID;
}

class NodeInfo {
    public readonly connections = new Set<AudioConnection>();

    constructor(public readonly type: string, public name: string) {

    }
}


function isMatchingConnection(conn: AudioConnection, destinationOrOutput?: AudioNode | AudioParam | number, output?: number, input?: number): boolean {
    let destination: AudioNode | AudioParam = null;
    if (isNumber(destinationOrOutput)) {
        output = destinationOrOutput;
    }
    else {
        destination = destinationOrOutput;
    }

    return conn.type === "conn"
        && (isNullOrUndefined(destination)
            || destination === conn.destination)
        && (isNullOrUndefined(output)
            || output === conn.output)
        && (isNullOrUndefined(input)
            || input === conn.input);
}

export class JuniperAudioContext extends AudioContext {
    private readonly counters = new Map<string, number>();

    private readonly _destination: JuniperAudioDestinationNode;

    private readonly nodes = new Map<AudioNode | AudioParam, NodeInfo>();

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
        if (!this.nodes.has(node)) {
            if (!this.counters.has(type)) {
                this.counters.set(type, 0);
            }

            const count = this.counters.get(type);
            const name = `${type}-${count}`;

            this.nodes.set(node, new NodeInfo(type, name));

            if (isEndpoint(node)) {
                node.name = name;
            }

            this.counters.set(type, count + 1);
        }
    }

    _name(dest: IAudioNode | IAudioParam, name: string): void {
        const { destination } = dest._resolveInput();
        if (this.nodes.has(destination)) {
            const info = this.nodes.get(destination);
            info.name = `${name}-${info.type}`;
        }
    }

    _dispose(node: AudioNode | AudioParam): void {
        this.nodes.delete(node);
    }

    _isConnected(src: IAudioNode, outp?: number): boolean {
        const { source, output } = src._resolveOutput(outp);
        if (isNullOrUndefined(source)
            || !this.nodes.has(source)) {
            return null;
        }
        else {
            const info = this.nodes.get(source);
            for (const conn of info.connections) {
                if (isMatchingConnection(conn, null, output, null)) {
                    return true;
                }
            }
            return false;
        }
    }

    _parent(src: IAudioNode, dest: IAudioParam) {
        const { source } = src._resolveOutput();
        const { destination } = dest._resolveInput();
        if (this.nodes.has(source)) {
            const info = this.nodes.get(source);
            info.connections.add({
                type: "parent",
                destination
            });
        }
    }

    _connect(source: AudioNode, destination?: AudioNode | AudioParam, output?: number, input?: number) {
        if (this.nodes.has(source)) {
            const conns = this.nodes.get(source).connections;
            let matchFound = false;
            for (const conn of conns) {
                if (isMatchingConnection(conn, destination, output, input)) {
                    matchFound = true;
                }
            }

            if (!matchFound) {
                conns.add({
                    type: "conn",
                    destination,
                    output,
                    input
                });
            }
        }
    }

    _disconnect(source: AudioNode, destination?: AudioNode | AudioParam, output?: number, input?: number) {
        if (this.nodes.has(source)) {
            const conns = this.nodes.get(source).connections;
            const toDelete = new Set<AudioConnection>();
            for (const conn of conns) {
                if (isMatchingConnection(conn, destination, output, input)) {
                    toDelete.add(conn);
                }
            }

            for (const conn of toDelete) {
                conns.delete(conn);
            }
        }
    }


    getAudioGraph(): Array<GraphNode<Vertex>> {
        const nodes = new Map<AudioNode | AudioParam, GraphNode<Vertex>>();

        for (const [node, info] of this.nodes) {
            const classID = node instanceof AudioNode
                ? "node"
                : node instanceof AudioParam
                    ? "param"
                    : "unknown";
            nodes.set(node, new GraphNode({
                name: info.name,
                type: info.type,
                classID
            }));
        }

        for (const [source, info] of this.nodes) {
            const branch = nodes.get(source);
            for (const child of info.connections) {
                const destination = child.destination;
                if (nodes.has(destination)) {
                    const cnode = nodes.get(destination);
                    branch.connectTo(cnode);
                    cnode.connectTo(branch);
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
