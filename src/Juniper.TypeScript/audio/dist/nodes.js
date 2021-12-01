import { onUserGesture } from "juniper-dom";
import { assertNever, GraphNode, isArray, isDefined, isNullOrUndefined, once } from "juniper-tslib";
export function isWrappedAudioNode(value) {
    return isDefined(value)
        && value.node instanceof AudioNode;
}
export function isErsatzAudioNode(value) {
    return isDefined(value)
        && value.input instanceof AudioNode
        && value.output instanceof AudioNode;
}
const connections = new Map();
const names = new Map();
function resolveOutput(node) {
    if (isErsatzAudioNode(node)) {
        return node.output;
    }
    else if (isWrappedAudioNode(node)) {
        return node.node;
    }
    return node;
}
export function resolveInput(node) {
    if (isNullOrUndefined(node)) {
        return undefined;
    }
    let n = null;
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
    let n2 = null;
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
export function resolveArray(node) {
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
function isAudioNode(a) {
    return isDefined(a)
        && isDefined(a.context);
}
function nameVertex(name, v) {
    names.set(v, name);
}
export function getVertexName(v) {
    return names.get(v);
}
export function removeVertex(v) {
    names.delete(v);
    if (isAudioNode(v)) {
        disconnect(v);
    }
}
export function chain(...nodes) {
    for (let i = 1; i < nodes.length; ++i) {
        connect(nodes[i - 1], nodes[i]);
    }
}
export function connect(left, right) {
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
        connections.set(a, new Set());
    }
    const g = connections.get(a);
    if (g.has(b)) {
        return false;
    }
    g.add(b);
    return true;
}
export function disconnect(left, right) {
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
export function getAudioGraph() {
    const nodes = new Map();
    function maybeAdd(node) {
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
globalThis.getAudioGraph = getAudioGraph;
export const hasAudioContext = "AudioContext" in globalThis;
export const hasAudioListener = hasAudioContext && "AudioListener" in globalThis;
export const hasOldAudioListener = hasAudioListener && "setPosition" in AudioListener.prototype;
export const hasNewAudioListener = hasAudioListener && "positionX" in AudioListener.prototype;
export function audioReady(audioCtx) {
    nameVertex("speakers", audioCtx.destination);
    return new Promise((resolve) => {
        if (audioCtx.state === "running") {
            resolve();
        }
        else if (audioCtx.state === "closed") {
            audioCtx.resume().then(resolve);
        }
        else if (audioCtx.state === "suspended") {
            once(audioCtx, "statechange")
                .then(() => resolve());
            onUserGesture(() => audioCtx.resume());
        }
    });
}
export function initAudio(name, left, ...rest) {
    nameVertex(name, left);
    for (const right of rest) {
        if (isDefined(right)) {
            connect(left, right);
        }
    }
    return left;
}
export function Analyser(name, audioCtx, options, ...rest) {
    return initAudio(name, new AnalyserNode(audioCtx, options), ...rest);
}
export function AudioWorklet(nodeName, audioCtx, workletName, options, ...rest) {
    return initAudio(nodeName, new AudioWorkletNode(audioCtx, workletName, options), ...rest);
}
export function BiquadFilter(name, audioCtx, options, ...rest) {
    return initAudio(name, new BiquadFilterNode(audioCtx, options), ...rest);
}
export function BufferSource(name, audioCtx, options, ...rest) {
    return initAudio(name, new AudioBufferSourceNode(audioCtx, options), ...rest);
}
export function ChannelMerger(name, audioCtx, options, ...rest) {
    return initAudio(name, new ChannelMergerNode(audioCtx, options), ...rest);
}
export function ChannelSplitter(name, audioCtx, options, ...rest) {
    return initAudio(name, new ChannelSplitterNode(audioCtx, options), ...rest);
}
export function ConstantSource(name, audioCtx, options, ...rest) {
    return initAudio(name, new ConstantSourceNode(audioCtx, options), ...rest);
}
export function Convolver(name, audioCtx, options, ...rest) {
    return initAudio(name, new ConvolverNode(audioCtx, options), ...rest);
}
export function Delay(name, audioCtx, options, ...rest) {
    return initAudio(name, new DelayNode(audioCtx, options), ...rest);
}
export function DynamicsCompressor(name, audioCtx, options, ...rest) {
    return initAudio(name, new DynamicsCompressorNode(audioCtx, options), ...rest);
}
export function Gain(name, audioCtx, options, ...rest) {
    return initAudio(name, new GainNode(audioCtx, options), ...rest);
}
export function IIRFilter(name, audioCtx, options, ...rest) {
    return initAudio(name, new IIRFilterNode(audioCtx, options), ...rest);
}
export function MediaElementSource(name, audioCtx, mediaElement, ...rest) {
    return initAudio(name, audioCtx.createMediaElementSource(mediaElement), ...rest);
}
export function MediaStreamDestination(name, audioCtx, options, ...rest) {
    return initAudio(name, new MediaStreamAudioDestinationNode(audioCtx, options), ...rest);
}
export function MediaStreamSource(name, audioCtx, mediaStream, ...rest) {
    return initAudio(name, audioCtx.createMediaStreamSource(mediaStream), ...rest);
}
export function Oscillator(name, audioCtx, options, ...rest) {
    return initAudio(name, new OscillatorNode(audioCtx, options), ...rest);
}
export function Panner(name, audioCtx, options, ...rest) {
    return initAudio(name, new PannerNode(audioCtx, options), ...rest);
}
export function StereoPanner(name, audioCtx, options, ...rest) {
    return initAudio(name, new StereoPannerNode(audioCtx, options), ...rest);
}
export function WaveShaper(name, audioCtx, options, ...rest) {
    return initAudio(name, new WaveShaperNode(audioCtx, options), ...rest);
}
