import { TypedEvent } from "@juniper-lib/events";
import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";
export class JuniperAudioBufferSourceNode extends JuniperAudioNode {
    constructor(context, options) {
        super("audio-buffer-source", context, new AudioBufferSourceNode(context, options));
        this._node.addEventListener("ended", () => this.dispatchEvent(new TypedEvent("ended")));
        this.parent(this.playbackRate = new JuniperAudioParam("playbackRate", context, this._node.playbackRate));
        this.parent(this.detune = new JuniperAudioParam("detune", context, this._node.detune));
    }
    get buffer() { return this._node.buffer; }
    set buffer(v) { this._node.buffer = v; }
    get loop() { return this._node.loop; }
    set loop(v) { this._node.loop = v; }
    get loopEnd() { return this._node.loopEnd; }
    set loopEnd(v) { this._node.loopEnd = v; }
    get loopStart() { return this._node.loopStart; }
    set loopStart(v) { this._node.loopStart = v; }
    get onended() { return this._node.onended; }
    set onended(v) { this._node.onended = v; }
    start(when, offset, duration) { this._node.start(when, offset, duration); }
    stop(when) { this._node.stop(when); }
}
//# sourceMappingURL=JuniperAudioBufferSourceNode.js.map