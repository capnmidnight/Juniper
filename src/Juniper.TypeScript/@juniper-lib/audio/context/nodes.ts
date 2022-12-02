import { JuniperAnalyserNode } from "./JuniperAnalyserNode";
import { JuniperAudioBufferSourceNode } from "./JuniperAudioBufferSourceNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioWorkletNode } from "./JuniperAudioWorkletNode";
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
import { init } from "./util";

export function Analyser(name: string, audioCtx: JuniperAudioContext, options?: AnalyserOptions): JuniperAnalyserNode {
    return init(name, new JuniperAnalyserNode(audioCtx, options));
}

export function AudioWorklet(name: string, audioCtx: JuniperAudioContext, workletName: string, options?: AudioWorkletNodeOptions): JuniperAudioWorkletNode {
    return init(name, new JuniperAudioWorkletNode(audioCtx, workletName, options));
}

export function BiquadFilter(name: string, audioCtx: JuniperAudioContext, options?: BiquadFilterOptions): JuniperBiquadFilterNode {
    return init(name, new JuniperBiquadFilterNode(audioCtx, options));
}

export function BufferSource(name: string, audioCtx: JuniperAudioContext, options?: AudioBufferSourceOptions): JuniperAudioBufferSourceNode {
    return init(name, new JuniperAudioBufferSourceNode(audioCtx, options));
}

export function ChannelMerger(name: string, audioCtx: JuniperAudioContext, options?: ChannelMergerOptions): JuniperChannelMergerNode {
    return init(name, new JuniperChannelMergerNode(audioCtx, options));
}

export function ChannelSplitter(name: string, audioCtx: JuniperAudioContext, options?: ChannelSplitterOptions): JuniperChannelSplitterNode {
    return init(name, new JuniperChannelSplitterNode(audioCtx, options));
}

export function ConstantSource(name: string, audioCtx: JuniperAudioContext, options?: ConstantSourceOptions): JuniperConstantSourceNode {
    return init(name, new JuniperConstantSourceNode(audioCtx, options));
}

export function Convolver(name: string, audioCtx: JuniperAudioContext, options?: ConvolverOptions): JuniperConvolverNode {
    return init(name, new JuniperConvolverNode(audioCtx, options));
}

export function Delay(name: string, audioCtx: JuniperAudioContext, options?: DelayOptions): JuniperDelayNode {
    return init(name, new JuniperDelayNode(audioCtx, options));
}

export function DynamicsCompressor(name: string, audioCtx: JuniperAudioContext, options?: DynamicsCompressorOptions): JuniperDynamicsCompressorNode {
    return init(name, new JuniperDynamicsCompressorNode(audioCtx, options));
}

export function Gain(name: string, audioCtx: JuniperAudioContext, options?: GainOptions): JuniperGainNode {
    return init(name, new JuniperGainNode(audioCtx, options));
}

export function IIRFilter(name: string, audioCtx: JuniperAudioContext, options?: IIRFilterOptions): JuniperIIRFilterNode {
    return init(name, new JuniperIIRFilterNode(audioCtx, options));
}

export function MediaElementSource(name: string, audioCtx: JuniperAudioContext, options?: MediaElementAudioSourceOptions): JuniperMediaElementAudioSourceNode {
    return init(name, new JuniperMediaElementAudioSourceNode(audioCtx, options));
}

export function MediaStreamDestination(name: string, audioCtx: JuniperAudioContext, options?: AudioNodeOptions): JuniperMediaStreamAudioDestinationNode {
    return init(name, new JuniperMediaStreamAudioDestinationNode(audioCtx, options));
}

export function MediaStreamSource(name: string, audioCtx: JuniperAudioContext, options?: MediaStreamAudioSourceOptions): JuniperMediaStreamAudioSourceNode {
    return init(name, new JuniperMediaStreamAudioSourceNode(audioCtx, options));
}

export function Oscillator(name: string, audioCtx: JuniperAudioContext, options?: OscillatorOptions): JuniperOscillatorNode {
    return init(name, new JuniperOscillatorNode(audioCtx, options));
}

export function Panner(name: string, audioCtx: JuniperAudioContext, options?: PannerOptions): JuniperPannerNode {
    return init(name, new JuniperPannerNode(audioCtx, options));
}

export function StereoPanner(name: string, audioCtx: JuniperAudioContext, options?: StereoPannerOptions): JuniperStereoPannerNode {
    return init(name, new JuniperStereoPannerNode(audioCtx, options));
}

export function WaveShaper(name: string, audioCtx: JuniperAudioContext, options?: WaveShaperOptions): JuniperWaveShaperNode {
    return init(name, new JuniperWaveShaperNode(audioCtx, options));
}
