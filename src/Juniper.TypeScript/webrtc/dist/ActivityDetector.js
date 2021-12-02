import { Analyser, removeVertex } from "juniper-audio";
export class ActivityDetector {
    name;
    _level = 0;
    maxLevel = 0;
    analyzer;
    buffer;
    constructor(name, audioCtx) {
        this.name = name;
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
