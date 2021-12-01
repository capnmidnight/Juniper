import type { Pose } from "./Pose";


export interface IPoseable {
    pose: Pose;
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
}
