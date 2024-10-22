import { TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { isEndpoint, isIAudioNode } from "./IAudioNode";
export class BaseNode extends TypedEventTarget {
    get name() { return this._name; }
    set name(v) {
        this._name = v;
        this.context._name(this, v);
    }
    constructor(nodeType, context) {
        super();
        this.nodeType = nodeType;
        this.context = context;
        this._name = null;
        this.disposed = false;
    }
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            this.onDisposing();
        }
    }
    onDisposing() {
    }
    isConnected(dest, output, input) {
        return this.context._isConnected(this, dest, output, input);
    }
    resolveOutput(output) {
        let resolution = {
            source: this,
            output
        };
        while (isIAudioNode(resolution.source)) {
            resolution = resolution.source._resolveOutput(resolution.output);
        }
        return resolution;
    }
    resolveInput(input) {
        let resolution = {
            destination: this,
            input
        };
        while (isEndpoint(resolution.destination)) {
            resolution = resolution.destination._resolveInput(resolution.input);
        }
        return resolution;
    }
    toggle(dest, outp, inp) {
        this._toggle(dest, outp, inp);
    }
    _toggle(dest, outp, inp) {
        if (this.isConnected(dest, outp, inp)) {
            this._disconnect(dest, outp, inp);
        }
        else {
            return this._connect(dest, outp, inp);
        }
    }
    connect(dest, outp, inp) {
        return this._connect(dest, outp, inp);
    }
    _connect(dest, outp, inp) {
        return this.context._connect(this, dest, outp, inp);
    }
    disconnect(destinationOrOutput, outp, inp) {
        this._disconnect(destinationOrOutput, outp, inp);
    }
    _disconnect(destinationOrOutput, outp, inp) {
        this.context._disconnect(this, destinationOrOutput, outp, inp);
    }
}
//# sourceMappingURL=BaseNode.js.map