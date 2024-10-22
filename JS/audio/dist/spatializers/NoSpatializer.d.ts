import { IAudioNode } from "../IAudioNode";
import { Pose } from "../Pose";
import { BaseSpatializer } from "./BaseSpatializer";
export declare class NoSpatializer extends BaseSpatializer {
    constructor(node: IAudioNode);
    readPose(_loc: Pose): void;
    getGainAtDistance(_: number): number;
}
//# sourceMappingURL=NoSpatializer.d.ts.map