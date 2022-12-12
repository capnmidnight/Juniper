import { BaseSpatializer } from "../../BaseSpatializer";
import { ErsatzAudioNode, removeVertex } from "../../util";

/**
 * Base class providing functionality for audio listeners.
 **/
export abstract class BaseEmitter
    extends BaseSpatializer
    implements ErsatzAudioNode {

    input: AudioNode;
    output: AudioNode;

    minDistance = 1;
    maxDistance = 10;
    protected algorithm: DistanceModelType = "inverse";

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            this.onDisposing();
            this.disposed = true;
        }
    }

    protected onDisposing() {
        removeVertex(this.input);
    }

    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        this.minDistance = minDistance;
        this.maxDistance = maxDistance;
        this.algorithm = algorithm;
    }

    abstract getGainAtDistance(distance: number): number;
}

