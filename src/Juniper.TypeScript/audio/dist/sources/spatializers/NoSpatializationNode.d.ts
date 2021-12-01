import type { Pose } from "../../Pose";
import { BaseEmitter } from "./BaseEmitter";
export declare class NoSpatializationNode extends BaseEmitter {
    /**
     * Creates a new "spatializer" that performs no panning. An anti-spatializer.
     */
    constructor(id: string, audioCtx: AudioContext);
    private disposed;
    dispose(): void;
    update(_loc: Pose, _t: number): void;
}
