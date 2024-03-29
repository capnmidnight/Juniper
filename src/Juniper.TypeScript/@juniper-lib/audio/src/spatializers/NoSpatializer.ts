import { IAudioNode } from "../IAudioNode";
import { Pose } from "../Pose";
import { BaseSpatializer } from "./BaseSpatializer";

export class NoSpatializer extends BaseSpatializer {
    constructor(node: IAudioNode) {
        super("no-spatializer", node.context, false, [node], [node]);
    }

    readPose(_loc: Pose): void {
        // nothing to do
    }

    getGainAtDistance(_: number): number {
        return 1;
    }
}
