import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperWrappedNode } from "./JuniperWrappedNode";


export class JuniperPannerNode
    extends JuniperWrappedNode<PannerNode>
    implements PannerNode {

    constructor(context: JuniperAudioContext, options?: PannerOptions) {
        super("panner", context, new PannerNode(context, options));
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
    get orientationX(): AudioParam { return this._node.orientationX; }
    get orientationY(): AudioParam { return this._node.orientationY; }
    get orientationZ(): AudioParam { return this._node.orientationZ; }
    get panningModel(): PanningModelType { return this._node.panningModel; }
    set panningModel(v: PanningModelType) { this._node.panningModel = v; }
    get positionX(): AudioParam { return this._node.positionX; }
    get positionY(): AudioParam { return this._node.positionY; }
    get positionZ(): AudioParam { return this._node.positionZ; }
    get refDistance(): number { return this._node.refDistance; }
    set refDistance(v: number) { this._node.refDistance = v; }
    get rolloffFactor(): number { return this._node.rolloffFactor; }
    set rolloffFactor(v: number) { this._node.rolloffFactor = v; }
    setOrientation(x: number, y: number, z: number): void { this._node.setOrientation(x, y, z); }
    setPosition(x: number, y: number, z: number): void { this._node.setPosition(x, y, z); }
}
