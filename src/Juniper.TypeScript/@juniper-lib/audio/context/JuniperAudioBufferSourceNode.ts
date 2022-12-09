import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { IAudioParam } from "./IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioParam } from "./JuniperAudioParam";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperAudioBufferSourceNode
    extends JuniperWrappedNode<AudioBufferSourceNode, {
        ended: TypedEvent<"ended">;
    }>
    implements AudioBufferSourceNode {

    public readonly playbackRate: IAudioParam;
    public readonly detune: IAudioParam;

    constructor(context: JuniperAudioContext, options?: AudioBufferSourceOptions) {
        super("audio-buffer-source", context, new AudioBufferSourceNode(context, options));
        this._node.addEventListener("ended", () => this.dispatchEvent(new TypedEvent("ended")));
        this.parent(this.playbackRate = new JuniperAudioParam("playbackRate", context, this._node.playbackRate));
        this.parent(this.detune = new JuniperAudioParam("detune", context, this._node.detune));
    }

    get buffer(): AudioBuffer { return this._node.buffer; }
    set buffer(v: AudioBuffer) { this._node.buffer = v; }
    get loop(): boolean { return this._node.loop; }
    set loop(v: boolean) { this._node.loop = v; }
    get loopEnd(): number { return this._node.loopEnd; }
    set loopEnd(v: number) { this._node.loopEnd = v; }
    get loopStart(): number { return this._node.loopStart; }
    set loopStart(v: number) { this._node.loopStart = v; }

    get onended(): (this: AudioScheduledSourceNode, ev: Event) => any { return this._node.onended; }
    set onended(v: (this: AudioScheduledSourceNode, ev: Event) => any) { this._node.onended = v; }

    start(when?: number, offset?: number, duration?: number): void { this._node.start(when, offset, duration); }
    stop(when?: number): void { this._node.stop(when); }
}