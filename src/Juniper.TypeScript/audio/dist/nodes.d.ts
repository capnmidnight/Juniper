import { GraphNode, IDisposable } from "juniper-tslib";
export interface WrappedAudioNode extends IDisposable {
    node: AudioNode;
}
export declare function isWrappedAudioNode(value: any): value is WrappedAudioNode;
export interface ErsatzAudioNode extends IDisposable {
    input: AudioNode;
    output: AudioNode;
}
export declare function isErsatzAudioNode(value: any): value is ErsatzAudioNode;
export declare type AudioNodeType = AudioNode | WrappedAudioNode | ErsatzAudioNode;
export declare type AudioVertex = AudioNodeType | AudioParam;
export declare type AudioConnection = AudioVertex | [number, AudioVertex] | [number, number, AudioNodeType];
export declare type BaseAudioNodeParamType = number | ChannelCountMode | ChannelInterpretation;
export declare function resolveInput(node: AudioConnection): AudioNode | AudioParam;
export declare function resolveArray(node: AudioConnection): number[];
export declare function getVertexName(v: AudioNode | AudioParam): string;
export declare function removeVertex(v: AudioNode | AudioParam): void;
export declare function chain(...nodes: AudioNodeType[]): void;
export declare function connect(left: AudioNodeType, right: AudioConnection): boolean;
export declare function disconnect(left: AudioNodeType, right?: AudioConnection): boolean;
export declare function getAudioGraph(): Array<GraphNode<AudioNode | AudioParam>>;
export declare const hasAudioContext: boolean;
export declare const hasAudioListener: boolean;
export declare const hasOldAudioListener: boolean;
export declare const hasNewAudioListener: boolean;
export declare function audioReady(audioCtx: AudioContext): Promise<void>;
export declare function initAudio<NodeT extends AudioNode>(name: string, left: NodeT, ...rest: AudioConnection[]): NodeT;
export declare function Analyser(name: string, audioCtx: BaseAudioContext, options?: AnalyserOptions, ...rest: AudioConnection[]): AnalyserNode;
export declare function AudioWorklet(nodeName: string, audioCtx: BaseAudioContext, workletName: string, options?: AudioWorkletNodeOptions, ...rest: AudioConnection[]): AudioWorkletNode;
export declare function BiquadFilter(name: string, audioCtx: BaseAudioContext, options?: BiquadFilterOptions, ...rest: AudioConnection[]): BiquadFilterNode;
export declare function BufferSource(name: string, audioCtx: BaseAudioContext, options?: AudioBufferSourceOptions, ...rest: AudioConnection[]): AudioBufferSourceNode;
export declare function ChannelMerger(name: string, audioCtx: BaseAudioContext, options?: ChannelMergerOptions, ...rest: AudioConnection[]): ChannelMergerNode;
export declare function ChannelSplitter(name: string, audioCtx: BaseAudioContext, options?: ChannelSplitterOptions, ...rest: AudioConnection[]): ChannelSplitterNode;
export declare function ConstantSource(name: string, audioCtx: BaseAudioContext, options?: ConstantSourceOptions, ...rest: AudioConnection[]): ConstantSourceNode;
export declare function Convolver(name: string, audioCtx: BaseAudioContext, options?: ConvolverOptions, ...rest: AudioConnection[]): ConvolverNode;
export declare function Delay(name: string, audioCtx: BaseAudioContext, options?: DelayOptions, ...rest: AudioConnection[]): DelayNode;
export declare function DynamicsCompressor(name: string, audioCtx: BaseAudioContext, options?: DynamicsCompressorOptions, ...rest: AudioConnection[]): DynamicsCompressorNode;
export declare function Gain(name: string, audioCtx: BaseAudioContext, options?: GainOptions, ...rest: AudioConnection[]): GainNode;
export declare function IIRFilter(name: string, audioCtx: BaseAudioContext, options?: IIRFilterOptions, ...rest: AudioConnection[]): IIRFilterNode;
export declare function MediaElementSource(name: string, audioCtx: AudioContext, mediaElement: HTMLMediaElement, ...rest: AudioConnection[]): MediaElementAudioSourceNode;
export declare function MediaStreamDestination(name: string, audioCtx: AudioContext, options?: AudioNodeOptions, ...rest: AudioConnection[]): MediaStreamAudioDestinationNode;
export declare function MediaStreamSource(name: string, audioCtx: AudioContext, mediaStream: MediaStream, ...rest: AudioConnection[]): MediaStreamAudioSourceNode;
export declare function Oscillator(name: string, audioCtx: BaseAudioContext, options?: OscillatorOptions, ...rest: AudioConnection[]): OscillatorNode;
export declare function Panner(name: string, audioCtx: BaseAudioContext, options?: PannerOptions, ...rest: AudioConnection[]): PannerNode;
export declare function StereoPanner(name: string, audioCtx: BaseAudioContext, options?: StereoPannerOptions, ...rest: AudioConnection[]): StereoPannerNode;
export declare function WaveShaper(name: string, audioCtx: BaseAudioContext, options?: WaveShaperOptions, ...rest: AudioConnection[]): WaveShaperNode;
