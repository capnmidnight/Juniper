import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { Pose } from "../Pose";
import { BaseSpatializer } from "./BaseSpatializer";


export class NoSpatializer extends BaseSpatializer {
    constructor(context: JuniperAudioContext) {
        const node = new JuniperGainNode(context);
        super("no-spatializer", context, false, [node], [node]);
    }

    readPose(_loc: Pose): void {
        // nothing to do
    }
}
