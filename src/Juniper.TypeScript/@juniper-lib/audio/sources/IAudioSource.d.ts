import { IPoseable } from "../IPoseable";
export interface IAudioSource extends IPoseable {
    readonly minDistance: number;
    readonly maxDistance: number;
    readonly algorithm: DistanceModelType;
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
}
//# sourceMappingURL=IAudioSource.d.ts.map