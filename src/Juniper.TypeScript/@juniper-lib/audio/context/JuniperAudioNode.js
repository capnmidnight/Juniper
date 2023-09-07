import { BaseNode } from "../BaseNode";
export class JuniperAudioNode extends BaseNode {
    constructor(type, context, _node) {
        super(type, context);
        this._node = _node;
        this.context._init(this._node, this.nodeType);
    }
    onDisposing() {
        this.disconnect();
        this.context._dispose(this._node);
        super.onDisposing();
    }
    parent(param) {
        this.context._parent(this, param);
    }
    get channelCount() { return this._node.channelCount; }
    set channelCount(v) { this._node.channelCount = v; }
    get channelCountMode() { return this._node.channelCountMode; }
    set channelCountMode(v) { this._node.channelCountMode = v; }
    get channelInterpretation() { return this._node.channelInterpretation; }
    set channelInterpretation(v) { this._node.channelInterpretation = v; }
    get numberOfInputs() { return this._node.numberOfInputs; }
    get numberOfOutputs() { return this._node.numberOfOutputs; }
    _resolveInput(input) {
        return {
            destination: this._node,
            input
        };
    }
    _resolveOutput(output) {
        return {
            source: this._node,
            output
        };
    }
}
//# sourceMappingURL=JuniperAudioNode.js.map