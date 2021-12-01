import type { Pose } from "../../Pose";
import type { BaseEmitter } from "../../sources/spatializers/BaseEmitter";
import { BaseListener } from "./BaseListener";
/**
 * A positioner that uses WebAudio's playback dependent time progression.
 **/
export declare class WebAudioListenerOld extends BaseListener {
    /**
     * Creates a new positioner that uses WebAudio's playback dependent time progression.
     */
    constructor(audioCtx: AudioContext);
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    update(loc: Pose, _t: number): void;
    /**
     * Creates a spatialzer for an audio source.
     */
    newSpatializer(id: string): BaseEmitter;
}
