import { JuniperAudioContext } from "@juniper-lib/audio/context/JuniperAudioContext";
import { JuniperGainNode } from "@juniper-lib/audio/context/JuniperGainNode";
import { unproject } from "@juniper-lib/tslib/math";
import type { ITimer } from "@juniper-lib/tslib/timers/ITimer";
import { TimerTickEvent } from "@juniper-lib/tslib/timers/ITimer";
import { SetIntervalTimer } from "@juniper-lib/tslib/timers/SetIntervalTimer";
import { ActivityDetector } from "./ActivityDetector";

export class GainDecayer extends ActivityDetector {
    private curLength = 0;

    private timer: ITimer = null;

    private _enabled = true;
    private shouldRun = false;
    private readonly onTick: (evt: TimerTickEvent) => void;

    constructor(
        context: JuniperAudioContext,
        private readonly control: JuniperGainNode,
        public min: number,
        public max: number,
        public threshold: number,
        public attack: number,
        public decay: number,
        public sustain: number,
        public hold: number,
        public release: number) {

        super(context);

        this.name = "remote-audio-activity";

        this.timer = new SetIntervalTimer(30);
        this.timer.addTickHandler(this.onTick = (evt) => this.update(evt));
    }

    override onDisposing() {
        super.onDisposing();
        this.timer.removeTickHandler(this.onTick);
    }

    update(evt: TimerTickEvent) {
        if (this.enabled) {
            const level = this.level;

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

            this.control.gain.value = this.gain;
        }
    }

    get enabled(): boolean {
        return this._enabled;
    }

    setEnabled(v: boolean) {
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

    private refresh() {
        if (this.timer != null) {
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
    }

    get length() {
        return this.attack + this.decay + this.hold + this.release;
    }

    private get time() {
        return this.length - this.curLength;
    }

    private set time(v: number) {
        this.curLength = this.length - v;
    }

    private get attackStart() {
        return 0;
    }

    private get attackEnd() {
        return this.attackStart + this.attack;
    }

    private get decayStart() {
        return this.attackEnd;
    }

    private get decayEnd() {
        return this.decayStart + this.decay;
    }

    private get holdStart() {
        return this.decayEnd;
    }

    private get holdEnd() {
        return this.holdStart + this.hold;
    }

    private get releaseStart() {
        return this.holdEnd;
    }

    private get releaseEnd() {
        return this.releaseStart + this.release;
    }

    private get attacking() {
        return this.attackStart <= this.time && this.time < this.attackEnd;
    }

    private get decaying() {
        return this.decayStart <= this.time && this.time < this.decayEnd;
    }

    private get holding() {
        return this.holdStart <= this.time && this.time < this.holdEnd;
    }

    private get releasing() {
        return this.releaseStart < this.time && this.time < this.releaseEnd;
    }

    private get pAttack() {
        return (this.time - this.attackStart) / this.attack;
    }

    private get pDecay() {
        return (this.time - this.decayStart) / this.decay;
    }

    //private get pHold() {
    //    return (this.time - this.holdStart) / this.hold;
    //}

    private get pRelease() {
        return (this.time - this.releaseStart) / this.release;
    }

    private get value() {
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