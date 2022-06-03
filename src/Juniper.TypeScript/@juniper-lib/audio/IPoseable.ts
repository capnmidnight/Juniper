import type { BaseSpatializer } from "./BaseSpatializer";
import type { Pose } from "./Pose";


export interface IPoseable {
    pose: Pose;
    spatializer: BaseSpatializer;
}
