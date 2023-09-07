import { arrayClear } from "@juniper-lib/collections/arrays";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { dispose } from "@juniper-lib/tslib/using";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { Pose } from "../Pose";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { effectStore } from "../effects";
export class BaseAudioSource extends BaseNodeCluster {
    constructor(type, context, spatializer, effectNames, extras) {
        const volumeControl = new JuniperGainNode(context);
        volumeControl.name = "volume-control";
        extras = extras || [];
        super(type, context, [], [spatializer], extras);
        this.spatializer = spatializer;
        this.effects = new Array();
        this.pose = new Pose();
        this.volumeControl = volumeControl;
        this.setEffects(...effectNames);
    }
    onDisposing() {
        arrayClear(this.effects);
        super.onDisposing();
    }
    setEffects(...effectNames) {
        this.disable();
        for (const effect of this.effects) {
            this.remove(effect);
            dispose(effect);
        }
        arrayClear(this.effects);
        let last = this.volumeControl;
        for (const effectName of effectNames) {
            if (isDefined(effectName)) {
                const createEffect = effectStore.get(effectName);
                if (isDefined(createEffect)) {
                    const effect = createEffect(effectName, this.context);
                    this.add(effect);
                    this.effects.push(effect);
                    last = last.connect(effect);
                }
            }
        }
        this.enable();
    }
    get spatialized() {
        return this.spatializer.spatialized;
    }
    get lastInternal() {
        return this.effects[this.effects.length - 1] || this.volumeControl;
    }
    enable() {
        if (!this.lastInternal.isConnected()) {
            this.lastInternal.connect(this.spatializer);
        }
    }
    disable() {
        if (this.lastInternal.isConnected()) {
            this.lastInternal.disconnect();
        }
    }
    tog() {
        if (this.lastInternal.isConnected()) {
            this.disable();
        }
        else {
            this.enable();
        }
    }
    get volume() {
        return this.volumeControl.gain.value;
    }
    set volume(v) {
        this.volumeControl.gain.value = v;
    }
    get minDistance() {
        if (isDefined(this.spatializer)) {
            return this.spatializer.minDistance;
        }
        return null;
    }
    get maxDistance() {
        if (isDefined(this.spatializer)) {
            return this.spatializer.maxDistance;
        }
        return null;
    }
    get algorithm() {
        if (isDefined(this.spatializer)) {
            return this.spatializer.algorithm;
        }
        return null;
    }
    setPosition(px, py, pz) {
        if (isDefined(this.spatializer)) {
            this.pose.setPosition(px, py, pz);
            this.spatializer.readPose(this.pose);
        }
    }
    setOrientation(qx, qy, qz, qw) {
        if (isDefined(this.spatializer)) {
            this.pose.setOrientation(qx, qy, qz, qw);
            this.spatializer.readPose(this.pose);
        }
    }
    set(px, py, pz, qx, qy, qz, qw) {
        if (isDefined(this.spatializer)) {
            this.pose.set(px, py, pz, qx, qy, qz, qw);
            this.spatializer.readPose(this.pose);
        }
    }
    setAudioProperties(minDistance, maxDistance, algorithm) {
        if (isDefined(this.spatializer)) {
            this.spatializer.setAudioProperties(minDistance, maxDistance, algorithm);
        }
    }
}
//# sourceMappingURL=BaseAudioSource.js.map