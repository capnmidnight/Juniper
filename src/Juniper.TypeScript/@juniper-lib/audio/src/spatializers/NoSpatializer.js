import { BaseSpatializer } from "./BaseSpatializer";
export class NoSpatializer extends BaseSpatializer {
    constructor(node) {
        super("no-spatializer", node.context, false, [node], [node]);
    }
    readPose(_loc) {
        // nothing to do
    }
    getGainAtDistance(_) {
        return 1;
    }
}
//# sourceMappingURL=NoSpatializer.js.map