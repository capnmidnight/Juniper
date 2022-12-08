import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { IPoseReader } from "../IPoseReader";
import { Pose } from "../Pose";

/**
 * Base class providing functionality for audio listeners.
 **/
export abstract class BaseListener implements IPoseReader {

    constructor(public readonly type: string, protected readonly context: JuniperAudioContext) {
    }

    /**
     * Performs the spatialization operation for the audio source's latest location.
     */

    abstract readPose(loc: Pose): void;
}