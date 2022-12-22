import { onUserGesture } from "@juniper-lib/dom/onUserGesture";
import { GraphNode } from "@juniper-lib/tslib/collections/GraphNode";
import { IReadyable } from "@juniper-lib/tslib/events/IReadyable";
import { Task } from "@juniper-lib/tslib/events/Task";
import { assertNever, isDefined, isNullOrUndefined, isNumber } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode, IAudioParam, isEndpoint } from "../IAudioNode";
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


export type NodeClass =
    | "node"
    | "param"
    | "unknown";

export type ConnectionType =
    | "conn"
    | "parent";

export interface OutputResolution {
    source: AudioNode;
    output?: number;
}

export interface InputResolution {
    destination: AudioNode | AudioParam;
    input?: number;
}

export interface AudioConnection {
    type: ConnectionType;
    src: IAudioNode;
    dest: IAudioNode | IAudioParam;
    outp?: number;
    inp?: number;
    source: AudioNode;
    destination: AudioNode | AudioParam;
    output?: number;
    input?: number;
}

export interface Vertex {
    nodeClass: NodeClass;
    type: string;
    name: string;
}

class NodeInfo {
    public readonly connections = new Set<AudioConnection>();

    constructor(public readonly type: string, public name: string) {

    }
}


function isMatchingConnection(conn: AudioConnection, type: ConnectionType, destination?: AudioNode | AudioParam, output?: number, input?: number): boolean {
    return conn.type === type
        && (isNullOrUndefined(destination)
            || destination === conn.destination)
        && (isNullOrUndefined(output)
            || output === conn.output)
        && (isNullOrUndefined(input)
            || input === conn.input);
}

function resolveInput(dest?: IAudioParam | IAudioNode, inp?: number): InputResolution {
    let destination: AudioNode | AudioParam = null;
    let input: number = null;
    if (isDefined(dest)) {
        ({ destination, input } = dest.resolveInput(inp));
    }
    return { destination, input };
}

export class JuniperAudioContext
    extends AudioContext
    implements IReadyable {
    private readonly counters = new Map<string, number>();

    private readonly _destination: JuniperAudioDestinationNode;

    private readonly nodes = new Map<AudioNode | AudioParam, NodeInfo>();

    private readonly _ready = new Task();
    get ready(): Promise<void> { return this._ready; }
    get isReady() { return this._ready.finished && this._ready.resolved; }

    constructor(contextOptions?: AudioContextOptions) {
        super(contextOptions);
        this._destination = new JuniperAudioDestinationNode(this, super.destination);
        if (this.state === "running") {
            this._ready.resolve();
        }
        else if (this.state === "closed") {
            this.resume()
                .then(() => this._ready.resolve());
        }
        else {
            onUserGesture(() => this.resume()
                .then(() => this._ready.resolve()));
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
        const { destination } = resolveInput(dest);
        if (this.nodes.has(destination)) {
            const info = this.nodes.get(destination);
            info.name = `${name}-${info.type}`;
        }
    }

    _dispose(node: AudioNode | AudioParam): void {
        this.nodes.delete(node);
    }

    _isConnected(src: IAudioNode, dest?: IAudioNode | IAudioParam, outp?: number, inp?: number): boolean {
        const { source, output } = src.resolveOutput(outp);
        const { destination, input } = resolveInput(dest, inp);

        if (isNullOrUndefined(source)
            || !this.nodes.has(source)) {
            return null;
        }
        else {
            const info = this.nodes.get(source);
            for (const conn of info.connections) {
                if (isMatchingConnection(conn, "conn", destination, output, input)) {
                    return true;
                }
            }
            return false;
        }
    }

    _parent(src: IAudioNode, dest: IAudioNode | IAudioParam) {
        const { source } = src.resolveOutput();
        const { destination } = resolveInput(dest);
        if (this.nodes.has(source)) {
            const conns = this.nodes.get(source).connections;
            conns.add({
                type: "parent",
                src,
                dest,
                destination,
                source
            });
        }
    }

    _unparent(src: IAudioNode, dest: IAudioNode | IAudioParam) {
        const { source } = src.resolveOutput();
        const { destination } = resolveInput(dest);
        if (this.nodes.has(source)) {
            const conns = this.nodes.get(source).connections;
            const toDelete = new Set<AudioConnection>();
            for (const conn of conns) {
                if (isMatchingConnection(conn, "parent", destination)) {
                    toDelete.add(conn);
                }
            }

            for (const conn of toDelete) {
                conns.delete(conn);
            }
        }
    }

    _getConnections(node: IAudioNode): Set<AudioConnection> {
        if (!this.nodes.has(node)) {
            return null;
        }

        return this.nodes.get(node).connections;
    }

    _connect(src: IAudioNode, dest?: IAudioNode | IAudioParam, outp?: number, inp?: number): IAudioNode | void {
        const { source, output } = src.resolveOutput(outp);
        const { destination, input } = resolveInput(dest, inp);

        if (this.nodes.has(source)) {
            const conns = this.nodes.get(source).connections;
            let matchFound = false;
            for (const conn of conns) {
                if (isMatchingConnection(conn, "conn", destination, output, input)) {
                    matchFound = true;
                }
            }

            if (!matchFound) {
                conns.add({
                    type: "conn",
                    src,
                    dest,
                    outp,
                    inp,
                    source,
                    destination,
                    output,
                    input
                });
            }
        }

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

    _disconnect(src: IAudioNode, destinationOrOutput?: IAudioNode | IAudioParam | number, outp?: number, inp?: number) {
        let dest: IAudioNode | IAudioParam;
        if (isNumber(destinationOrOutput)) {
            dest = undefined;
            outp = destinationOrOutput;
        }
        else {
            dest = destinationOrOutput;
        }

        const { source, output } = src.resolveOutput(outp);
        const { destination, input } = resolveInput(dest, inp);

        if (this.nodes.has(source)) {
            const conns = this.nodes.get(source).connections;
            const toDelete = new Set<AudioConnection>();
            for (const conn of conns) {
                if (isMatchingConnection(conn, "conn", destination, output, input)) {
                    toDelete.add(conn);
                }
            }

            for (const conn of toDelete) {
                conns.delete(conn);
            }
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


    getAudioGraph(includeParams: boolean): Array<GraphNode<Vertex>> {
        const nodes = new Map<AudioNode | AudioParam, GraphNode<Vertex>>();

        for (const [node, info] of this.nodes) {
            const nodeClass = node instanceof AudioNode
                ? "node"
                : node instanceof AudioParam
                    ? "param"
                    : "unknown";
            if (includeParams || nodeClass !== "param") {
                nodes.set(node, new GraphNode({
                    name: info.name,
                    type: info.type,
                    nodeClass
                }));
            }
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
