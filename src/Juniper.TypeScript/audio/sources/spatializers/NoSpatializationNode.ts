import { singleton, stringRandom } from "juniper-tslib";
import { Gain } from "../../nodes";
import type { Pose } from "../../Pose";
import { BaseEmitter } from "./BaseEmitter";

const nodes = singleton("Juniper:Audio:Destinations:Spatializers:NoSpatializationNode:nodes", () =>
    new WeakMap<AudioContext, NoSpatializationNode>());

export class NoSpatializationNode extends BaseEmitter {

    static instance(audioCtx: AudioContext): NoSpatializationNode {
        if (!nodes.has(audioCtx)) {
            const id = `no-spatialization-hook-${stringRandom(8)}`;
            nodes.set(audioCtx, new NoSpatializationNode(id, audioCtx));
        }

        return nodes.get(audioCtx);
    }

    /**
     * Creates a new "spatializer" that performs no panning. An anti-spatializer.
     */
    constructor(id: string, audioCtx: AudioContext) {
        super(id);
        this.input = this.output = Gain(this.id, audioCtx);
        Object.seal(this);
    }

    setPose(_loc: Pose, _t: number): void {
        // do nothing
    }
}
