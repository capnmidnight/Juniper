import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";


export class JuniperPannerNode
    extends JuniperAudioNode<PannerNode>
    implements PannerNode {

    readonly positionX: IAudioParam;
    readonly positionY: IAudioParam;
    readonly positionZ: IAudioParam;
    readonly orientationX: IAudioParam;
    readonly orientationY: IAudioParam;
    readonly orientationZ: IAudioParam;

    constructor(context: JuniperAudioContext, options?: PannerOptions) {
        super("panner", context, new PannerNode(context, options));
        this.parent(this.positionX = new JuniperAudioParam("positionX", this.context, this._node.positionX));
        this.parent(this.positionY = new JuniperAudioParam("positionY", this.context, this._node.positionY));
        this.parent(this.positionZ = new JuniperAudioParam("positionZ", this.context, this._node.positionZ));
        this.parent(this.orientationX = new JuniperAudioParam("orientationX", this.context, this._node.orientationX));
        this.parent(this.orientationY = new JuniperAudioParam("orientationY", this.context, this._node.orientationY));
        this.parent(this.orientationZ = new JuniperAudioParam("orientationZ", this.context, this._node.orientationZ));
    }
    
    get coneInnerAngle(): number { return this._node.coneInnerAngle; }
    set coneInnerAngle(v: number) { this._node.coneInnerAngle = v; }
    get coneOuterAngle(): number { return this._node.coneOuterAngle; }
    set coneOuterAngle(v: number) { this._node.coneOuterAngle = v; }
    get coneOuterGain(): number { return this._node.coneOuterGain; }
    set coneOuterGain(v: number) { this._node.coneOuterGain = v; }
    get distanceModel(): DistanceModelType { return this._node.distanceModel; }
    set distanceModel(v: DistanceModelType) { this._node.distanceModel = v; }
    get maxDistance(): number { return this._node.maxDistance; }
    set maxDistance(v: number) { this._node.maxDistance = v; }
    get panningModel(): PanningModelType { return this._node.panningModel; }
    set panningModel(v: PanningModelType) { this._node.panningModel = v; }
    get refDistance(): number { return this._node.refDistance; }
    set refDistance(v: number) { this._node.refDistance = v; }
    get rolloffFactor(): number { return this._node.rolloffFactor; }
    set rolloffFactor(v: number) { this._node.rolloffFactor = v; }
    setOrientation(x: number, y: number, z: number): void { this._node.setOrientation(x, y, z); }
    setPosition(x: number, y: number, z: number): void { this._node.setPosition(x, y, z); }
}
