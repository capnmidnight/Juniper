import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";


export class JuniperDynamicsCompressorNode
    extends JuniperAudioNode<DynamicsCompressorNode>
    implements DynamicsCompressorNode {

    public readonly attack: IAudioParam;
    public readonly knee: IAudioParam;
    public readonly ratio: IAudioParam;
    public readonly release: IAudioParam;
    public readonly threshold: IAudioParam;

    constructor(context: JuniperAudioContext, options?: DynamicsCompressorOptions) {
        super("dynamics-compressor", context, new DynamicsCompressorNode(context, options));
        this.parent(this.attack = new JuniperAudioParam("attack", this.context, this._node.attack));
        this.parent(this.knee = new JuniperAudioParam("knee", this.context, this._node.knee));
        this.parent(this.ratio = new JuniperAudioParam("ratio", this.context, this._node.ratio));
        this.parent(this.release = new JuniperAudioParam("release", this.context, this._node.release));
        this.parent(this.threshold = new JuniperAudioParam("threshold", this.context, this._node.threshold));
    }

    get reduction(): number { return this._node.reduction; }
}