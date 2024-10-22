import { JuniperAudioNode } from "./JuniperAudioNode";
import { JuniperAudioParam } from "./JuniperAudioParam";
export class JuniperPannerNode extends JuniperAudioNode {
    constructor(context, options) {
        super("panner", context, new PannerNode(context, options));
        this.parent(this.positionX = new JuniperAudioParam("positionX", this.context, this._node.positionX));
        this.parent(this.positionY = new JuniperAudioParam("positionY", this.context, this._node.positionY));
        this.parent(this.positionZ = new JuniperAudioParam("positionZ", this.context, this._node.positionZ));
        this.parent(this.orientationX = new JuniperAudioParam("orientationX", this.context, this._node.orientationX));
        this.parent(this.orientationY = new JuniperAudioParam("orientationY", this.context, this._node.orientationY));
        this.parent(this.orientationZ = new JuniperAudioParam("orientationZ", this.context, this._node.orientationZ));
    }
    get coneInnerAngle() { return this._node.coneInnerAngle; }
    set coneInnerAngle(v) { this._node.coneInnerAngle = v; }
    get coneOuterAngle() { return this._node.coneOuterAngle; }
    set coneOuterAngle(v) { this._node.coneOuterAngle = v; }
    get coneOuterGain() { return this._node.coneOuterGain; }
    set coneOuterGain(v) { this._node.coneOuterGain = v; }
    get distanceModel() { return this._node.distanceModel; }
    set distanceModel(v) { this._node.distanceModel = v; }
    get maxDistance() { return this._node.maxDistance; }
    set maxDistance(v) { this._node.maxDistance = v; }
    get panningModel() { return this._node.panningModel; }
    set panningModel(v) { this._node.panningModel = v; }
    get refDistance() { return this._node.refDistance; }
    set refDistance(v) { this._node.refDistance = v; }
    get rolloffFactor() { return this._node.rolloffFactor; }
    set rolloffFactor(v) { this._node.rolloffFactor = v; }
    setOrientation(x, y, z) { this._node.setOrientation(x, y, z); }
    setPosition(x, y, z) { this._node.setPosition(x, y, z); }
}
//# sourceMappingURL=JuniperPannerNode.js.map