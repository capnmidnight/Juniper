import { onUserGesture } from "@juniper-lib/dom/dist/onUserGesture";
import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
import { Task } from "@juniper-lib/events/dist/Task";
import { assertNever, isDefined, isNullOrUndefined, isNumber } from "@juniper-lib/tslib/dist/typeChecks";
import { isEndpoint } from "../IAudioNode";
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
    globalThis.AudioContext = globalThis.webkitAudioContext;
}
if (!("OfflineAudioContext" in globalThis) && "webkitOfflineAudioContext" in globalThis) {
    globalThis.OfflineAudioContext = globalThis.webkitOfflineAudioContext;
}
class NodeInfo {
    constructor(type, name) {
        this.type = type;
        this.name = name;
        this.connections = new Set();
    }
}
function isMatchingConnection(conn, type, destination, output, input) {
    return conn.type === type
        && (isNullOrUndefined(destination)
            || destination === conn.destination)
        && (isNullOrUndefined(output)
            || output === conn.output)
        && (isNullOrUndefined(input)
            || input === conn.input);
}
function resolveInput(dest, inp) {
    let destination = null;
    let input = null;
    if (isDefined(dest)) {
        ({ destination, input } = dest.resolveInput(inp));
    }
    return { destination, input };
}
export class JuniperAudioContext extends AudioContext {
    get ready() { return this._ready; }
    get isReady() { return this._ready.finished && this._ready.resolved; }
    constructor(contextOptions) {
        super(contextOptions);
        this.counters = new Map();
        this.nodes = new Map();
        this._ready = new Task();
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
        this.ready.then(() => console.log("Audio is now ready"));
    }
    _init(node, type) {
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
    _name(dest, name) {
        const { destination } = resolveInput(dest);
        if (this.nodes.has(destination)) {
            const info = this.nodes.get(destination);
            info.name = `${name}-${info.type}`;
        }
    }
    _dispose(node) {
        this.nodes.delete(node);
    }
    _isConnected(src, dest, outp, inp) {
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
    _parent(src, dest) {
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
    _unparent(src, dest) {
        const { source } = src.resolveOutput();
        const { destination } = resolveInput(dest);
        if (this.nodes.has(source)) {
            const conns = this.nodes.get(source).connections;
            const toDelete = new Set();
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
    _getConnections(node) {
        if (!this.nodes.has(node)) {
            return null;
        }
        return this.nodes.get(node).connections;
    }
    _connect(src, dest, outp, inp) {
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
            dest = dest;
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
    _disconnect(src, destinationOrOutput, outp, inp) {
        let dest;
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
            const toDelete = new Set();
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
    getAudioGraph(includeParams) {
        const nodes = new Map();
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
    get destination() {
        return this._destination;
    }
    createAnalyser() {
        return new JuniperAnalyserNode(this);
    }
    createBiquadFilter() {
        return new JuniperBiquadFilterNode(this);
    }
    createBufferSource() {
        return new JuniperAudioBufferSourceNode(this);
    }
    createChannelMerger(numberOfInputs) {
        return new JuniperChannelMergerNode(this, {
            numberOfInputs
        });
    }
    createChannelSplitter(numberOfOutputs) {
        return new JuniperChannelSplitterNode(this, {
            numberOfOutputs
        });
    }
    createConstantSource() {
        return new JuniperConstantSourceNode(this);
    }
    createConvolver() {
        return new JuniperConvolverNode(this);
    }
    createDelay(maxDelayTime) {
        return new JuniperDelayNode(this, {
            maxDelayTime
        });
    }
    createDynamicsCompressor() {
        return new JuniperDynamicsCompressorNode(this);
    }
    createGain() {
        return new JuniperGainNode(this);
    }
    createIIRFilter(feedforward, feedback) {
        return new JuniperIIRFilterNode(this, {
            feedforward,
            feedback
        });
    }
    createMediaElementSource(mediaElement) {
        return new JuniperMediaElementAudioSourceNode(this, {
            mediaElement
        });
    }
    createMediaStreamDestination() {
        return new JuniperMediaStreamAudioDestinationNode(this);
    }
    createMediaStreamSource(mediaStream) {
        return new JuniperMediaStreamAudioSourceNode(this, {
            mediaStream
        });
    }
    createOscillator() {
        return new JuniperOscillatorNode(this);
    }
    createPanner() {
        return new JuniperPannerNode(this);
    }
    createStereoPanner() {
        return new JuniperStereoPannerNode(this);
    }
    createWaveShaper() {
        return new JuniperWaveShaperNode(this);
    }
    createScriptProcessor() {
        throw new Error("Script processor nodes are deprecated");
    }
}
//# sourceMappingURL=JuniperAudioContext.js.map