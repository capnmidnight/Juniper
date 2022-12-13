import { JuniperGainNode } from "../context/JuniperGainNode";
import { Pose } from "../Pose";
import { BaseSpatializer } from "./BaseSpatializer";

export class NoSpatializer extends BaseSpatializer {
    constructor(private readonly node: JuniperGainNode) {
        super("no-spatializer", node.context, false, [node], [node]);
    }

    readPose(_loc: Pose): void {
        // nothing to do
    }

    getGainAtDistance(_: number): number {
        return this.node.gain.value;
    }
}
