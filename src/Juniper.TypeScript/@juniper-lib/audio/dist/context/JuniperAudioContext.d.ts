import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
import { IReadyable } from "@juniper-lib/events/dist/IReadyable";
import { IAudioNode, IAudioParam } from "../IAudioNode";
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
export type NodeClass = "node" | "param" | "unknown";
export type ConnectionType = "conn" | "parent";
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
export declare class JuniperAudioContext extends AudioContext implements IReadyable {
    private readonly counters;
    private readonly _destination;
    private readonly nodes;
    private readonly _ready;
    get ready(): Promise<void>;
    get isReady(): boolean;
    constructor(contextOptions?: AudioContextOptions);
    _init(node: AudioNode | AudioParam, type: string): void;
    _name(dest: IAudioNode | IAudioParam, name: string): void;
    _dispose(node: AudioNode | AudioParam): void;
    _isConnected(src: IAudioNode, dest?: IAudioNode | IAudioParam, outp?: number, inp?: number): boolean;
    _parent(src: IAudioNode, dest: IAudioNode | IAudioParam): void;
    _unparent(src: IAudioNode, dest: IAudioNode | IAudioParam): void;
    _getConnections(node: IAudioNode): Set<AudioConnection>;
    _connect(src: IAudioNode, dest?: IAudioNode | IAudioParam, outp?: number, inp?: number): IAudioNode | void;
    _disconnect(src: IAudioNode, destinationOrOutput?: IAudioNode | IAudioParam | number, outp?: number, inp?: number): void;
    getAudioGraph(includeParams: boolean): Array<GraphNode<Vertex>>;
    get destination(): JuniperAudioDestinationNode;
    createAnalyser(): JuniperAnalyserNode;
    createBiquadFilter(): JuniperBiquadFilterNode;
    createBufferSource(): JuniperAudioBufferSourceNode;
    createChannelMerger(numberOfInputs?: number): JuniperChannelMergerNode;
    createChannelSplitter(numberOfOutputs?: number): JuniperChannelSplitterNode;
    createConstantSource(): JuniperConstantSourceNode;
    createConvolver(): JuniperConvolverNode;
    createDelay(maxDelayTime?: number): JuniperDelayNode;
    createDynamicsCompressor(): JuniperDynamicsCompressorNode;
    createGain(): JuniperGainNode;
    createIIRFilter(feedforward: number[], feedback: number[]): JuniperIIRFilterNode;
    createMediaElementSource(mediaElement: HTMLMediaElement): JuniperMediaElementAudioSourceNode;
    createMediaStreamDestination(): JuniperMediaStreamAudioDestinationNode;
    createMediaStreamSource(mediaStream: MediaStream): JuniperMediaStreamAudioSourceNode;
    createOscillator(): JuniperOscillatorNode;
    createPanner(): JuniperPannerNode;
    createStereoPanner(): JuniperStereoPannerNode;
    createWaveShaper(): JuniperWaveShaperNode;
    createScriptProcessor(): ScriptProcessorNode;
}
//# sourceMappingURL=JuniperAudioContext.d.ts.map