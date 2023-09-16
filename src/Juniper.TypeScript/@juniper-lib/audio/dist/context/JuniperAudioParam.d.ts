import { IAudioParam } from "../IAudioNode";
import { InputResolution, JuniperAudioContext } from "./JuniperAudioContext";
export declare class JuniperAudioParam implements IAudioParam {
    readonly nodeType: string;
    private readonly context;
    private readonly param;
    private _name;
    get name(): string;
    set name(v: string);
    constructor(nodeType: string, context: JuniperAudioContext, param: AudioParam);
    private disposed;
    dispose(): void;
    protected onDisposing(): void;
    get automationRate(): AutomationRate;
    set automationRate(v: AutomationRate);
    get defaultValue(): number;
    get maxValue(): number;
    get minValue(): number;
    get value(): number;
    set value(v: number);
    cancelAndHoldAtTime(cancelTime: number): IAudioParam;
    cancelScheduledValues(cancelTime: number): IAudioParam;
    exponentialRampToValueAtTime(value: number, endTime: number): IAudioParam;
    linearRampToValueAtTime(value: number, endTime: number): IAudioParam;
    setTargetAtTime(target: number, startTime: number, timeConstant: number): IAudioParam;
    setValueAtTime(value: number, startTime: number): IAudioParam;
    setValueCurveAtTime(values: Float32Array | number[], startTime: number, duration: number): IAudioParam;
    setValueCurveAtTime(values: Iterable<number>, startTime: number, duration: number): IAudioParam;
    _resolveInput(): InputResolution;
    resolveInput(): InputResolution;
}
//# sourceMappingURL=JuniperAudioParam.d.ts.map