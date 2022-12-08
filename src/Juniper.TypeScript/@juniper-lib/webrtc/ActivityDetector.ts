import { JuniperAnalyserNode } from "@juniper-lib/audio/context/JuniperAnalyserNode";
import { JuniperAudioContext } from "@juniper-lib/audio/context/JuniperAudioContext";

export class ActivityDetector extends JuniperAnalyserNode {

    private _level = 0;
    private maxLevel = 0;
    private buffer: Uint8Array;

    constructor(context: JuniperAudioContext) {
        super(context, {
            fftSize: 32,
            minDecibels: -70
        });

        this.buffer = new Uint8Array(this.frequencyBinCount);
    }

    get level() {
        this.getByteFrequencyData(this.buffer);
        this._level = Math.max(...this.buffer);
        if (isFinite(this._level)) {
            this.maxLevel = Math.max(this.maxLevel, this._level);
            if (this.maxLevel > 0) {
                this._level /= this.maxLevel;
            }
        }

        return this._level;
    }
}
