import { ActivityDetector } from "@juniper-lib/audio/ActivityDetector";
import { unproject } from "@juniper-lib/tslib/math";
export class GainDecayer extends ActivityDetector {
    constructor(context, control, min, max, threshold, attack, decay, sustain, hold, release) {
        super(context);
        this.control = control;
        this.min = min;
        this.max = max;
        this.threshold = threshold;
        this.attack = attack;
        this.decay = decay;
        this.sustain = sustain;
        this.hold = hold;
        this.release = release;
        this.curLength = 0;
        this._enabled = true;
        this.shouldRun = false;
        this.name = "remote-audio-activity";
        let lastT = null;
        this.addEventListener("activity", (evt) => {
            const now = performance.now();
            if (lastT != null) {
                const dt = now - lastT;
                if (this.enabled) {
                    const level = evt.level;
                    if (level >= this.threshold && this.time >= this.length) {
                        this.time = 0;
                    }
                    this.time += dt;
                    if (this.time > this.length) {
                        this.time = this.length;
                    }
                    if (level >= this.threshold && this.holding) {
                        this.time = this.holdStart;
                    }
                    this.control.gain.value = this.gain;
                }
            }
            lastT = now;
        });
    }
    get enabled() {
        return this._enabled;
    }
    set enabled(v) {
        this._enabled = v;
        this.refresh();
    }
    start() {
        this.shouldRun = true;
        this.refresh();
    }
    stop() {
        this.shouldRun = false;
        this.refresh();
    }
    refresh() {
        const canRun = this.shouldRun && this.enabled;
        if (canRun !== this.timer.isRunning) {
            if (canRun) {
                this.timer.start();
            }
            else {
                this.timer.stop();
                this.control.gain.value = 1;
            }
        }
    }
    get length() {
        return this.attack + this.decay + this.hold + this.release;
    }
    get time() {
        return this.length - this.curLength;
    }
    set time(v) {
        this.curLength = this.length - v;
    }
    get attackStart() {
        return 0;
    }
    get attackEnd() {
        return this.attackStart + this.attack;
    }
    get decayStart() {
        return this.attackEnd;
    }
    get decayEnd() {
        return this.decayStart + this.decay;
    }
    get holdStart() {
        return this.decayEnd;
    }
    get holdEnd() {
        return this.holdStart + this.hold;
    }
    get releaseStart() {
        return this.holdEnd;
    }
    get releaseEnd() {
        return this.releaseStart + this.release;
    }
    get attacking() {
        return this.attackStart <= this.time && this.time < this.attackEnd;
    }
    get decaying() {
        return this.decayStart <= this.time && this.time < this.decayEnd;
    }
    get holding() {
        return this.holdStart <= this.time && this.time < this.holdEnd;
    }
    get releasing() {
        return this.releaseStart < this.time && this.time < this.releaseEnd;
    }
    get pAttack() {
        return (this.time - this.attackStart) / this.attack;
    }
    get pDecay() {
        return (this.time - this.decayStart) / this.decay;
    }
    //private get pHold() {
    //    return (this.time - this.holdStart) / this.hold;
    //}
    get pRelease() {
        return (this.time - this.releaseStart) / this.release;
    }
    get value() {
        if (this.attacking) {
            return 1 - this.pAttack;
        }
        else if (this.decaying) {
            return this.sustain * this.pDecay;
        }
        else if (this.holding) {
            return this.sustain;
        }
        else if (this.releasing) {
            return this.sustain + (1 - this.sustain) * this.pRelease;
        }
        else {
            return 1;
        }
    }
    get gain() {
        return unproject(this.value, this.min, this.max);
    }
}
//# sourceMappingURL=GainDecayer.js.map