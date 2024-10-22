import { arrayRemove, dispose } from "@juniper-lib/util";
import { BaseNode } from "./BaseNode";
import { isIAudioNode } from "./IAudioNode";
export class BaseNodeCluster extends BaseNode {
    get exemplar() { return this.allNodes[0]; }
    constructor(type, context, inputs, endpoints, extras) {
        super(type, context);
        inputs = inputs || [];
        extras = extras || [];
        const exits = endpoints || inputs;
        this.inputs = inputs;
        const entries = inputs
            .filter(isIAudioNode)
            .map(o => o);
        this.outputs = exits
            .filter(isIAudioNode)
            .map(o => o);
        this.allNodes = Array.from(new Set([
            ...entries,
            ...this.outputs,
            ...extras
        ]));
    }
    add(node) {
        this.allNodes.push(node);
        this.context._parent(this, node);
    }
    remove(node) {
        arrayRemove(this.allNodes, node);
        this.context._unparent(this, node);
    }
    onDisposing() {
        this.allNodes.forEach(dispose);
        super.onDisposing();
    }
    get channelCount() { return this.exemplar.channelCount; }
    set channelCount(v) { this.allNodes.forEach(n => n.channelCount = v); }
    get channelCountMode() { return this.exemplar.channelCountMode; }
    set channelCountMode(v) { this.allNodes.forEach(n => n.channelCountMode = v); }
    get channelInterpretation() { return this.exemplar.channelInterpretation; }
    set channelInterpretation(v) { this.allNodes.forEach(n => n.channelInterpretation = v); }
    get numberOfInputs() { return this.inputs.length; }
    get numberOfOutputs() { return this.outputs.length; }
    static resolve(source, index) {
        index = index || 0;
        if (index < 0 || source.length <= index) {
            return null;
        }
        return source[index];
    }
    _resolveInput(input) {
        return {
            destination: BaseNodeCluster.resolve(this.inputs, input)
        };
    }
    _resolveOutput(output) {
        return {
            source: BaseNodeCluster.resolve(this.outputs, output)
        };
    }
}
//# sourceMappingURL=BaseNodeCluster.js.map