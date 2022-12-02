import { Exception } from "@juniper-lib/tslib/Exception";
import { isArray, isDefined, isNullOrUndefined, isNumber } from "@juniper-lib/tslib/typeChecks";
import { AudioConnection } from "./util";
import { JuniperAnalyserNode } from "./JuniperAnalyserNode";
import { JuniperAudioBufferSourceNode } from "./JuniperAudioBufferSourceNode";
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


export class JuniperAudioContext extends AudioContext {
    private readonly counters = new Map<string, number>();

    private readonly conns = new Map<AudioNode, Set<AudioConnection>>();
    private readonly types = new Map<AudioNode, string>();
    private readonly names = new Map<AudioNode, string>();

    constructor(contextOptions?: AudioContextOptions) {
        super(contextOptions);
        this._init("destination", this.destination);
    }

    _setName(name: string, node: AudioNode): void {
        this.names.set(node, name);
    }

    _getName(node: AudioNode): string {
        return this.names.get(node);
    }

    _init(type: string, node: AudioNode): void {
        if (!this.counters.has(type)) {
            this.counters.set(type, 0);
        }

        const count = this.counters.get(type);
        const name = `${type}-${count}`;

        this.counters.set(type, count + 1);
        this._setName(name, node);
        this.types.set(node, type);
    }

    _dispose(node: AudioNode): void {
        node.disconnect();
        this.names.delete(node);
        this.types.delete(node);
    }

    private getConnections(src: AudioNode) {
        if (!this.conns.has(src)) {
            this.conns.set(src, new Set());
        }

        return this.conns.get(src);
    }

    _connect(src: AudioNode, dest: AudioNode | AudioParam, output?: number, input?: number): AudioNode | void {
        const conns = this.getConnections(src);

        if (dest instanceof AudioNode) {
            if (!this.names.has(dest)) {
                throw new Exception("The given destination node was not a Juniper Audio Node and cannot be tracked.");
            }

            if (isDefined(input)) {
                conns.add([dest, output, input]);
            }
            else if (isDefined(output)) {
                conns.add([dest, output]);
            }
            else {
                conns.add(dest);
            }
        }
        else if (isDefined(output)) {
            conns.add([dest, output]);
        }
        else {
            conns.add(dest);
        }
    }

    _disconnect(src: AudioNode, destinationOrOutput?: AudioNode | AudioParam | number, output?: number, input?: number): void {
        const conns = this.getConnections(src);
        const toDelete = new Set<AudioConnection>();
        for (const conn of conns) {
            if (isMatchingConnection(conn, destinationOrOutput, output, input)) {
                toDelete.add(conn);
            }
        }

        for (const conn of toDelete) {
            conns.delete(conn);
        }

        if (conns.size === 0) {
            this.conns.delete(src);
        }
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

function isMatchingConnection(conn: AudioConnection, destinationOrOutput?: AudioNode | AudioParam | number, output?: number, input?: number): boolean {
    let destination: AudioNode | AudioParam = null;
    if (isNumber(destinationOrOutput)) {
        output = destinationOrOutput;
    }
    else {
        destination = destinationOrOutput;
    }

    if (isArray(conn)) {
        return (isNullOrUndefined(destination)
            || destination === conn[0])
            && (isNullOrUndefined(output)
                || output === conn[1])
            && (conn.length == 2
                || isNullOrUndefined(input)
                || input === conn[2]);
    }
    else {
        return (isNullOrUndefined(destination)
            || destination === conn);
    }
}
