import { IAudioParam } from "../IAudioNode";
import type { JuniperAudioContext } from "./JuniperAudioContext";
import { JuniperAudioNode } from "./JuniperAudioNode";
export declare class JuniperPannerNode extends JuniperAudioNode<PannerNode> implements PannerNode {
    readonly positionX: IAudioParam;
    readonly positionY: IAudioParam;
    readonly positionZ: IAudioParam;
    readonly orientationX: IAudioParam;
    readonly orientationY: IAudioParam;
    readonly orientationZ: IAudioParam;
    constructor(context: JuniperAudioContext, options?: PannerOptions);
    get coneInnerAngle(): number;
    set coneInnerAngle(v: number);
    get coneOuterAngle(): number;
    set coneOuterAngle(v: number);
    get coneOuterGain(): number;
    set coneOuterGain(v: number);
    get distanceModel(): DistanceModelType;
    set distanceModel(v: DistanceModelType);
    get maxDistance(): number;
    set maxDistance(v: number);
    get panningModel(): PanningModelType;
    set panningModel(v: PanningModelType);
    get refDistance(): number;
    set refDistance(v: number);
    get rolloffFactor(): number;
    set rolloffFactor(v: number);
    setOrientation(x: number, y: number, z: number): void;
    setPosition(x: number, y: number, z: number): void;
}
//# sourceMappingURL=JuniperPannerNode.d.ts.map