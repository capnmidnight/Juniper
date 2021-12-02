import { connect } from "juniper-audio";
import { SetIntervalTimer, unproject } from "juniper-tslib";
import { ActivityDetector } from "./ActivityDetector";
export class DecayingGain {
    input;
    output;
    min;
    max;
    threshold;
    attack;
    decay;
    sustain;
    hold;
    release;
    activity;
    curLength = 0;
    timer = null;
    _enabled = true;
    shouldRun = false;
    constructor(audioCtx, input, output, min, max, threshold, attack, decay, sustain, hold, release) {
        this.input = input;
        this.output = output;
        this.min = min;
        this.max = max;
        this.threshold = threshold;
        this.attack = attack;
        this.decay = decay;
        this.sustain = sustain;
        this.hold = hold;
        this.release = release;
        this.activity = new ActivityDetector("remote-audio-activity", audioCtx);
        connect(this.input, this.activity);
        this.timer = new SetIntervalTimer(30);
        this.timer.addTickHandler((evt) => this.update(evt));
    }
    update(evt) {
        if (this.enabled) {
            const level = this.activity.level;
            if (level >= this.threshold && this.time >= this.length) {
                this.time = 0;
            }
            this.time += evt.dt;
            if (this.time > this.length) {
                this.time = this.length;
            }
            if (level >= this.threshold && this.holding) {
                this.time = this.holdStart;
            }
            this.output.gain.value = this.gain;
        }
    }
    get enabled() {
        return this._enabled;
    }
    setEnabled(v) {
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
        if (this.timer != null) {
            const canRun = this.shouldRun && this.enabled;
            if (canRun !== this.timer.isRunning) {
                if (canRun) {
                    this.timer.start();
                }
                else {
                    this.timer.stop();
                    this.output.gain.value = 1;
                }
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
