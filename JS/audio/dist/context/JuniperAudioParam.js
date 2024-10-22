export class JuniperAudioParam {
    get name() { return this._name; }
    set name(v) {
        this._name = v;
        this.context._name(this, v);
    }
    constructor(nodeType, context, param) {
        this.nodeType = nodeType;
        this.context = context;
        this.param = param;
        this._name = null;
        this.disposed = false;
        this.context._init(this.param, this.nodeType);
    }
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            this.onDisposing();
        }
    }
    onDisposing() {
        this.context._dispose(this.param);
    }
    get automationRate() { return this.param.automationRate; }
    set automationRate(v) { this.param.automationRate = v; }
    get defaultValue() { return this.param.defaultValue; }
    get maxValue() { return this.param.maxValue; }
    get minValue() { return this.param.minValue; }
    get value() { return this.param.value; }
    set value(v) { this.param.value = v; }
    cancelAndHoldAtTime(cancelTime) {
        this.param.cancelAndHoldAtTime(cancelTime);
        return this;
    }
    cancelScheduledValues(cancelTime) {
        this.param.cancelScheduledValues(cancelTime);
        return this;
    }
    exponentialRampToValueAtTime(value, endTime) {
        this.param.exponentialRampToValueAtTime(value, endTime);
        return this;
    }
    linearRampToValueAtTime(value, endTime) {
        this.param.linearRampToValueAtTime(value, endTime);
        return this;
    }
    setTargetAtTime(target, startTime, timeConstant) {
        this.param.setTargetAtTime(target, startTime, timeConstant);
        return this;
    }
    setValueAtTime(value, startTime) {
        this.param.setValueAtTime(value, startTime);
        return this;
    }
    setValueCurveAtTime(values, startTime, duration) {
        this.param.setValueCurveAtTime(values, startTime, duration);
        return this;
    }
    _resolveInput() {
        return {
            destination: this.param
        };
    }
    resolveInput() { return this._resolveInput(); }
}
//# sourceMappingURL=JuniperAudioParam.js.map