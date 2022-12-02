import { Analyser } from "@juniper-lib/audio/nodes";
import { ErsatzAudioNode, removeVertex } from "@juniper-lib/audio/util";

export class ActivityDetector implements ErsatzAudioNode {

    private _level = 0;
    private maxLevel = 0;
    private analyzer: AnalyserNode;
    private buffer: Uint8Array;

    constructor(private readonly name: string, audioCtx: AudioContext) {
        this.analyzer = Analyser(this.name, audioCtx, {
            fftSize: 32,
            minDecibels: -70
        });

        this.buffer = new Uint8Array(this.analyzer.frequencyBinCount);
    }

    dispose() {
        removeVertex(this.analyzer);
    }

    get level() {
        this.analyzer.getByteFrequencyData(this.buffer);
        this._level = Math.max(...this.buffer);
        if (isFinite(this._level)) {
            this.maxLevel = Math.max(this.maxLevel, this._level);
            if (this.maxLevel > 0) {
                this._level /= this.maxLevel;
            }
        }

        return this._level;
    }

    get input() {
        return this.analyzer;
    }

    get output() {
        return this.analyzer;
    }
}
