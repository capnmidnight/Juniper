import { onUserGesture } from "@juniper/dom/onUserGesture";
import { assertNever, GraphNode, IDisposable, isArray, isDefined, isFunction, isNullOrUndefined, once, singleton } from "@juniper/tslib";

export const hasAudioContext = "AudioContext" in globalThis;
export const hasAudioListener = hasAudioContext && "AudioListener" in globalThis;
export const hasOldAudioListener = hasAudioListener && "setPosition" in AudioListener.prototype;
export const hasNewAudioListener = hasAudioListener && "positionX" in AudioListener.prototype;
export const canCaptureStream = isFunction(HTMLMediaElement.prototype.captureStream)
    || isFunction(HTMLMediaElement.prototype.mozCaptureStream);

export interface WrappedAudioNode extends IDisposable {
    node: AudioNode;
}

export function isWrappedAudioNode(value: any): value is WrappedAudioNode {
    return isDefined(value)
        && value.node instanceof AudioNode;
}

export interface ErsatzAudioNode extends IDisposable {
    input: AudioNode;
    output: AudioNode;
}

export function isErsatzAudioNode(value: any): value is ErsatzAudioNode {
    return isDefined(value)
        && value.input instanceof AudioNode
        && value.output instanceof AudioNode;
}

export type AudioNodeType = AudioNode | WrappedAudioNode | ErsatzAudioNode;

export type AudioVertex = AudioNodeType | AudioParam;

export type AudioConnection
    = AudioVertex
    | [number, AudioVertex]
    | [number, number, AudioNodeType];

export type BaseAudioNodeParamType = number | ChannelCountMode | ChannelInterpretation;

const connections = singleton("Juniper:Audio:connections", () => new Map<AudioNode, Set<AudioNode | AudioParam>>());
const names = singleton("Juniper:Audio:names", () => new Map<AudioNode | AudioParam, string>());

function resolveOutput(node: AudioNodeType): AudioNode {
    if (isErsatzAudioNode(node)) {
        return node.output;
    }
    else if (isWrappedAudioNode(node)) {
        return node.node;
    }

    return node;
}

export function resolveInput(node: AudioConnection): AudioNode | AudioParam {
    if (isNullOrUndefined(node)) {
        return undefined;
    }

    let n: AudioVertex = null;
    if (isArray(node)) {
        if (node.length === 2) {
            n = node[1];
        }
        else {
            n = node[2];
        }
    }
    else {
        n = node;
    }

    let n2: AudioNode | AudioParam = null;
    if (isErsatzAudioNode(n)) {
        n2 = n.input;
    }
    else if (isWrappedAudioNode(n)) {
        n2 = n.node;
    }
    else {
        n2 = n;
    }

    return n2;
}

export function resolveArray(node: AudioConnection): number[] {
    if (!isArray(node)) {
        return [];
    }
    else if (node.length === 2) {
        return [node[0]];
    }
    else {
        return [node[0], node[1]];
    }
}

function isAudioNode(a: AudioVertex | number): a is AudioNode {
    return isDefined(a)
        && isDefined((a as AudioNode).context);
}

function nameVertex(name: string, v: AudioNode | AudioParam): void {
    names.set(v, name);
}

export function getVertexName(v: AudioNode | AudioParam): string {
    return names.get(v);
}

export function removeVertex(v: AudioNode | AudioParam): void {
    names.delete(v);
    if (isAudioNode(v)) {
        disconnect(v);
    }
}

export function chain(...nodes: AudioNodeType[]): void {
    for (let i = 1; i < nodes.length; ++i) {
        connect(nodes[i - 1], nodes[i]);
    }
}

export function connect(left: AudioNodeType, right: AudioConnection): boolean {
    const a = resolveOutput(left);
    const b = resolveInput(right);
    const c = resolveArray(right);

    if (isNullOrUndefined(b)) {
        throw new Error("Must have a target to connect to");
    }
    else if (b instanceof AudioParam) {
        a.connect(b, c[0]);
    }
    else if (b instanceof AudioNode) {
        a.connect(b, c[0], c[1]);
    }
    else {
        assertNever(b);
    }

    if (!connections.has(a)) {
        connections.set(a, new Set<AudioNode | AudioParam>());
    }

    const g = connections.get(a);
    if (g.has(b)) {
        return false;
    }

    g.add(b);

    return true;
}

export function disconnect(left: AudioNodeType, right?: AudioConnection): boolean {
    const a = resolveOutput(left);
    const b = resolveInput(right);
    const c = resolveArray(right);

    if (isNullOrUndefined(b)) {
        a.disconnect();
    }
    else if (b instanceof AudioParam) {
        a.disconnect(b, c[0]);
    }
    else if (b instanceof AudioNode) {
        a.disconnect(b, c[0], c[1]);
    }
    else {
        assertNever(b);
    }

    if (!connections.has(a)) {
        return false;
    }

    const g = connections.get(a);

    let removed = false;
    if (isNullOrUndefined(b)) {
        removed = g.size > 0;
        g.clear();
    }
    else if (g.has(b)) {
        removed = true;
        g.delete(b);
    }

    if (g.size === 0) {
        connections.delete(a);
    }

    return removed;
}

export function getAudioGraph(): Array<GraphNode<AudioNode | AudioParam>> {
    const nodes = new Map<AudioNode | AudioParam, GraphNode<AudioNode | AudioParam>>();

    function maybeAdd(node: AudioNode | AudioParam) {
        if (!nodes.has(node)) {
            nodes.set(node, new GraphNode(node));
        }
    }

    for (const node of names.keys()) {
        maybeAdd(node);
    }

    for (const [parent, children] of connections) {
        maybeAdd(parent);
        for (const child of children) {
            maybeAdd(child);
        }
    }

    for (const [parent, children] of connections) {
        const branch = nodes.get(parent);
        for (const child of children) {
            if (nodes.has(child)) {
                branch.connectTo(nodes.get(child));
            }
        }
    }

    return Array.from(nodes.values());
}

(globalThis as any).getAudioGraph = getAudioGraph;

export async function audioReady(audioCtx: AudioContext) {
    nameVertex("speakers", audioCtx.destination);
    if (audioCtx.state !== "running") {
        if (audioCtx.state === "closed") {
            await audioCtx.resume();
        }
        else if (audioCtx.state === "suspended") {
            const stateChange = once<BaseAudioContextEventMap>(audioCtx, "statechange");
            onUserGesture(() => audioCtx.resume());
            await stateChange;
        }
        else {
            assertNever(audioCtx.state);
        }
    }
}

export function initAudio<NodeT extends AudioNode>(name: string, left: NodeT, ...rest: AudioConnection[]) {
    nameVertex(name, left);

    for (const right of rest) {
        if (isDefined(right)) {
            connect(left, right);
        }
    }

    return left;
}


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

export function MediaElementSource(name: string, audioCtx: AudioContext, mediaElement: HTMLMediaElement, ...rest: AudioConnection[]): MediaElementAudioSourceNode {
    return initAudio(name, audioCtx.createMediaElementSource(mediaElement), ...rest);
}

export function MediaStreamDestination(name: string, audioCtx: AudioContext, options?: AudioNodeOptions, ...rest: AudioConnection[]): MediaStreamAudioDestinationNode {
    return initAudio(name, new MediaStreamAudioDestinationNode(audioCtx, options), ...rest);
}

export function MediaStreamSource(name: string, audioCtx: AudioContext, mediaStream: MediaStream, ...rest: AudioConnection[]): MediaStreamAudioSourceNode {
    return initAudio(name, audioCtx.createMediaStreamSource(mediaStream), ...rest);
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
