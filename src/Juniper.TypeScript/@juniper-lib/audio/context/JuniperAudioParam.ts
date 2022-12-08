import { IAudioParam } from "./IAudioNode";
import { JuniperAudioContext } from "./JuniperAudioContext";


export class JuniperAudioParam implements IAudioParam {
    name: string = null;

    constructor(
        public readonly nodeType: string,
        private readonly context: JuniperAudioContext,
        private readonly param: AudioParam) {
        this.context._init(this);
    }

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            this.disposed = true;
            this.onDisposing();
        }
    }

    protected onDisposing() {
        this.context._dispose(this);
    }

    get automationRate(): AutomationRate { return this.param.automationRate; }
    set automationRate(v: AutomationRate) { this.param.automationRate = v; }

    get defaultValue(): number { return this.param.defaultValue; }

    get maxValue(): number { return this.param.maxValue; }

    get minValue(): number { return this.param.minValue; }

    get value(): number { return this.param.value; }
    set value(v: number) { this.param.value = v; }

    cancelAndHoldAtTime(cancelTime: number): IAudioParam {
        this.param.cancelAndHoldAtTime(cancelTime);
        return this;
    }

    cancelScheduledValues(cancelTime: number): IAudioParam {
        this.param.cancelScheduledValues(cancelTime);
        return this;
    }

    exponentialRampToValueAtTime(value: number, endTime: number): IAudioParam {
        this.param.exponentialRampToValueAtTime(value, endTime);
        return this;
    }

    linearRampToValueAtTime(value: number, endTime: number): IAudioParam {
        this.param.linearRampToValueAtTime(value, endTime);
        return this;
    }

    setTargetAtTime(target: number, startTime: number, timeConstant: number): IAudioParam {
        this.param.setTargetAtTime(target, startTime, timeConstant);
        return this;
    }

    setValueAtTime(value: number, startTime: number): IAudioParam {
        this.param.setValueAtTime(value, startTime);
        return this;
    }

    setValueCurveAtTime(values: Float32Array | number[], startTime: number, duration: number): IAudioParam;
    setValueCurveAtTime(values: Iterable<number>, startTime: number, duration: number): IAudioParam;
    setValueCurveAtTime(values: Float32Array | number[] | Iterable<number>, startTime: number, duration: number): IAudioParam {
        this.param.setValueCurveAtTime(values, startTime, duration);
        return this;
    }

    _resolveInput(_?: number): { destination: AudioNode | AudioParam; input?: number; } {
        return {
            destination: this.param
        };
    }
}
