import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { IPoseReader } from "../IPoseReader";
import { Pose } from "../Pose";
/**
 * Base class providing functionality for audio listeners.
 **/
export declare abstract class BaseListener implements IPoseReader {
    readonly type: string;
    protected readonly context: JuniperAudioContext;
    constructor(type: string, context: JuniperAudioContext);
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    abstract readPose(loc: Pose): void;
}
//# sourceMappingURL=BaseListener.d.ts.map