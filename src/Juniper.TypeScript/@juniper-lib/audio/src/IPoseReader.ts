import { Pose } from "./Pose";


export interface IPoseReader {
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    readPose(loc: Pose): void;
}
