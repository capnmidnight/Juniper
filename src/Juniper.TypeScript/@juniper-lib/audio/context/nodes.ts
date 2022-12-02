import { isArray } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode } from "./IAudioNode";
import { JuniperAnalyserNode as AnalyserNode } from "./JuniperAnalyserNode";
import { JuniperAudioBufferSourceNode as AudioBufferSourceNode } from "./JuniperAudioBufferSourceNode";
import type { JuniperAudioContext as AudioContext } from "./JuniperAudioContext";
import { AudioConnection } from "./util";
import { JuniperAudioWorkletNode as AudioWorkletNode } from "./JuniperAudioWorkletNode";
import { JuniperBiquadFilterNode as BiquadFilterNode } from "./JuniperBiquadFilterNode";
import { JuniperChannelMergerNode as ChannelMergerNode } from "./JuniperChannelMergerNode";
import { JuniperChannelSplitterNode as ChannelSplitterNode } from "./JuniperChannelSplitterNode";
import { JuniperConstantSourceNode as ConstantSourceNode } from "./JuniperConstantSourceNode";
import { JuniperConvolverNode as ConvolverNode } from "./JuniperConvolverNode";
import { JuniperDelayNode as DelayNode } from "./JuniperDelayNode";
import { JuniperDynamicsCompressorNode as DynamicsCompressorNode } from "./JuniperDynamicsCompressorNode";
import { JuniperGainNode as GainNode } from "./JuniperGainNode";
import { JuniperIIRFilterNode as IIRFilterNode } from "./JuniperIIRFilterNode";
import { JuniperMediaElementAudioSourceNode as MediaElementAudioSourceNode } from "./JuniperMediaElementAudioSourceNode";
import { JuniperMediaStreamAudioDestinationNode as MediaStreamAudioDestinationNode } from "./JuniperMediaStreamAudioDestinationNode";
import { JuniperMediaStreamAudioSourceNode as MediaStreamAudioSourceNode } from "./JuniperMediaStreamAudioSourceNode";
import { JuniperOscillatorNode as OscillatorNode } from "./JuniperOscillatorNode";
import { JuniperPannerNode as PannerNode } from "./JuniperPannerNode";
import { JuniperStereoPannerNode as StereoPannerNode } from "./JuniperStereoPannerNode";
import { JuniperWaveShaperNode as WaveShaperNode } from "./JuniperWaveShaperNode";

function init<T extends IAudioNode>(name: string, node: T, ...rest: AudioConnection[]): T {
    node.name = name;
    return fan(node, ...rest);
}

function fan<T extends IAudioNode>(node: T, ...rest: AudioConnection[]): T {
    for (const right of rest) {
        let dest: AudioNode | AudioParam;
        let output: number = undefined;
        let input: number = undefined;
        if (isArray(right)) {
            [dest, output, input] = right;
        }
        else {
            dest = right;
        }

        node.connect(dest as any, output, input);
    }
    return node;
}

export function Analyser(name: string, audioCtx: AudioContext, options?: AnalyserOptions, ...rest: AudioConnection[]): AnalyserNode {
    return init(name, new AnalyserNode(audioCtx, options), ...rest);
}

export function AudioWorklet(name: string, audioCtx: AudioContext, workletName: string, options?: AudioWorkletNodeOptions, ...rest: AudioConnection[]): AudioWorkletNode {
    return init(name, new AudioWorkletNode(audioCtx, workletName, options), ...rest);
}

export function BiquadFilter(name: string, audioCtx: AudioContext, options?: BiquadFilterOptions, ...rest: AudioConnection[]): BiquadFilterNode {
    return init(name, new BiquadFilterNode(audioCtx, options), ...rest);
}

export function BufferSource(name: string, audioCtx: AudioContext, options?: AudioBufferSourceOptions, ...rest: AudioConnection[]): AudioBufferSourceNode {
    return init(name, new AudioBufferSourceNode(audioCtx, options), ...rest);
}

export function ChannelMerger(name: string, audioCtx: AudioContext, options?: ChannelMergerOptions, ...rest: AudioConnection[]): ChannelMergerNode {
    return init(name, new ChannelMergerNode(audioCtx, options), ...rest);
}

export function ChannelSplitter(name: string, audioCtx: AudioContext, options?: ChannelSplitterOptions, ...rest: AudioConnection[]): ChannelSplitterNode {
    return init(name, new ChannelSplitterNode(audioCtx, options), ...rest);
}

export function ConstantSource(name: string, audioCtx: AudioContext, options?: ConstantSourceOptions, ...rest: AudioConnection[]): ConstantSourceNode {
    return init(name, new ConstantSourceNode(audioCtx, options), ...rest);
}

export function Convolver(name: string, audioCtx: AudioContext, options?: ConvolverOptions, ...rest: AudioConnection[]): ConvolverNode {
    return init(name, new ConvolverNode(audioCtx, options), ...rest);
}

export function Delay(name: string, audioCtx: AudioContext, options?: DelayOptions, ...rest: AudioConnection[]): DelayNode {
    return init(name, new DelayNode(audioCtx, options), ...rest);
}

export function DynamicsCompressor(name: string, audioCtx: AudioContext, options?: DynamicsCompressorOptions, ...rest: AudioConnection[]): DynamicsCompressorNode {
    return init(name, new DynamicsCompressorNode(audioCtx, options), ...rest);
}

export function Gain(name: string, audioCtx: AudioContext, options?: GainOptions, ...rest: AudioConnection[]): GainNode {
    return init(name, new GainNode(audioCtx, options), ...rest);
}

export function IIRFilter(name: string, audioCtx: AudioContext, options?: IIRFilterOptions, ...rest: AudioConnection[]): IIRFilterNode {
    return init(name, new IIRFilterNode(audioCtx, options), ...rest);
}

export function MediaElementSource(name: string, audioCtx: AudioContext, options?: MediaElementAudioSourceOptions, ...rest: AudioConnection[]): MediaElementAudioSourceNode {
    return init(name, new MediaElementAudioSourceNode(audioCtx, options), ...rest);
}

export function MediaStreamDestination(name: string, audioCtx: AudioContext, options?: AudioNodeOptions, ...rest: AudioConnection[]): MediaStreamAudioDestinationNode {
    return init(name, new MediaStreamAudioDestinationNode(audioCtx, options), ...rest);
}

export function MediaStreamSource(name: string, audioCtx: AudioContext, options?: MediaStreamAudioSourceOptions, ...rest: AudioConnection[]): MediaStreamAudioSourceNode {
    return init(name, new MediaStreamAudioSourceNode(audioCtx,  options), ...rest);
}

export function Oscillator(name: string, audioCtx: AudioContext, options?: OscillatorOptions, ...rest: AudioConnection[]): OscillatorNode {
    return init(name, new OscillatorNode(audioCtx, options), ...rest);
}

export function Panner(name: string, audioCtx: AudioContext, options?: PannerOptions, ...rest: AudioConnection[]): PannerNode {
    return init(name, new PannerNode(audioCtx, options), ...rest);
}

export function StereoPanner(name: string, audioCtx: AudioContext, options?: StereoPannerOptions, ...rest: AudioConnection[]): StereoPannerNode {
    return init(name, new StereoPannerNode(audioCtx, options), ...rest);
}

export function WaveShaper(name: string, audioCtx: AudioContext, options?: WaveShaperOptions, ...rest: AudioConnection[]): WaveShaperNode {
    return init(name, new WaveShaperNode(audioCtx, options), ...rest);
}
