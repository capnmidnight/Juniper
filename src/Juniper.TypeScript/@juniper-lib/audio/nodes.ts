import { AudioConnection, initAudio } from "./util";

export function Analyser(name: string, audioCtx: BaseAudioContext, options?: AnalyserOptions, ...rest: AudioConnection[]): AnalyserNode {
    return initAudio(name, new AnalyserNode(audioCtx, options), ...rest);
}

export function AudioWorklet(nodeName: string, audioCtx: BaseAudioContext, workletName: string, options?: AudioWorkletNodeOptions, ...rest: AudioConnection[]): AudioWorkletNode {
    return initAudio(nodeName, new AudioWorkletNode(audioCtx, workletName, options), ...rest);
}

export function BiquadFilter(name: string, audioCtx: BaseAudioContext, options?: BiquadFilterOptions, ...rest: AudioConnection[]): BiquadFilterNode {
    return initAudio(name, new BiquadFilterNode(audioCtx, options), ...rest);
}

export function BufferSource(name: string, audioCtx: BaseAudioContext, options?: AudioBufferSourceOptions, ...rest: AudioConnection[]): AudioBufferSourceNode {
    return initAudio(name, new AudioBufferSourceNode(audioCtx, options), ...rest);
}

export function ChannelMerger(name: string, audioCtx: BaseAudioContext, options?: ChannelMergerOptions, ...rest: AudioConnection[]): ChannelMergerNode {
    return initAudio(name, new ChannelMergerNode(audioCtx, options), ...rest);
}

export function ChannelSplitter(name: string, audioCtx: BaseAudioContext, options?: ChannelSplitterOptions, ...rest: AudioConnection[]): ChannelSplitterNode {
    return initAudio(name, new ChannelSplitterNode(audioCtx, options), ...rest);
}

export function ConstantSource(name: string, audioCtx: BaseAudioContext, options?: ConstantSourceOptions, ...rest: AudioConnection[]): ConstantSourceNode {
    return initAudio(name, new ConstantSourceNode(audioCtx, options), ...rest);
}

export function Convolver(name: string, audioCtx: BaseAudioContext, options?: ConvolverOptions, ...rest: AudioConnection[]): ConvolverNode {
    return initAudio(name, new ConvolverNode(audioCtx, options), ...rest);
}

export function Delay(name: string, audioCtx: BaseAudioContext, options?: DelayOptions, ...rest: AudioConnection[]): DelayNode {
    return initAudio(name, new DelayNode(audioCtx, options), ...rest);
}

export function DynamicsCompressor(name: string, audioCtx: BaseAudioContext, options?: DynamicsCompressorOptions, ...rest: AudioConnection[]): DynamicsCompressorNode {
    return initAudio(name, new DynamicsCompressorNode(audioCtx, options), ...rest);
}

export function Gain(name: string, audioCtx: BaseAudioContext, options?: GainOptions, ...rest: AudioConnection[]): GainNode {
    return initAudio(name, new GainNode(audioCtx, options), ...rest);
}

export function IIRFilter(name: string, audioCtx: BaseAudioContext, options?: IIRFilterOptions, ...rest: AudioConnection[]): IIRFilterNode {
    return initAudio(name, new IIRFilterNode(audioCtx, options), ...rest);
}

export function MediaElementSource(name: string, audioCtx: AudioContext, options?: MediaElementAudioSourceOptions, ...rest: AudioConnection[]): MediaElementAudioSourceNode {
    return initAudio(name, new MediaElementAudioSourceNode(audioCtx, options), ...rest);
}

export function MediaStreamDestination(name: string, audioCtx: AudioContext, options?: AudioNodeOptions, ...rest: AudioConnection[]): MediaStreamAudioDestinationNode {
    return initAudio(name, new MediaStreamAudioDestinationNode(audioCtx, options), ...rest);
}

export function MediaStreamSource(name: string, audioCtx: AudioContext, options?: MediaStreamAudioSourceOptions, ...rest: AudioConnection[]): MediaStreamAudioSourceNode {
    return initAudio(name, new MediaStreamAudioSourceNode(audioCtx, options), ...rest);
}

export function Oscillator(name: string, audioCtx: BaseAudioContext, options?: OscillatorOptions, ...rest: AudioConnection[]): OscillatorNode {
    return initAudio(name, new OscillatorNode(audioCtx, options), ...rest);
}

export function Panner(name: string, audioCtx: BaseAudioContext, options?: PannerOptions, ...rest: AudioConnection[]): PannerNode {
    return initAudio(name, new PannerNode(audioCtx, options), ...rest);
}

export function StereoPanner(name: string, audioCtx: BaseAudioContext, options?: StereoPannerOptions, ...rest: AudioConnection[]): StereoPannerNode {
    return initAudio(name, new StereoPannerNode(audioCtx, options), ...rest);
}

export function WaveShaper(name: string, audioCtx: BaseAudioContext, options?: WaveShaperOptions, ...rest: AudioConnection[]): WaveShaperNode {
    return initAudio(name, new WaveShaperNode(audioCtx, options), ...rest);
}
