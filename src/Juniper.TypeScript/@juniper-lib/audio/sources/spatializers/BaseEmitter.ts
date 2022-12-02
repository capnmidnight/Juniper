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
}

