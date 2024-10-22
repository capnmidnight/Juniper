import { arrayClear } from "@juniper-lib/collections/dist/arrays";
import { TypedEventMap } from "@juniper-lib/events/dist/TypedEventTarget";
import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { dispose } from "@juniper-lib/tslib/dist/using";
import { BaseNodeCluster } from "../BaseNodeCluster";
import { IAudioNode } from "../IAudioNode";
import { Pose } from "../Pose";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { effectStore } from "../effects";
import { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { IAudioSource } from "./IAudioSource";

export abstract class BaseAudioSource<EventTypeT extends TypedEventMap<string> = TypedEventMap<string>>
    extends BaseNodeCluster<EventTypeT>
    implements IAudioSource {

    private readonly effects = new Array<IAudioNode>();

    protected readonly volumeControl: JuniperGainNode;

    private readonly pose = new Pose();

    constructor(
        type: string,
        context: JuniperAudioContext,
        public readonly spatializer: BaseSpatializer,
        effectNames: string[],
        extras?: ReadonlyArray<IAudioNode>) {

        const volumeControl = new JuniperGainNode(context);
        volumeControl.name = "volume-control";

        extras = extras || [];

        super(type, context, [], [spatializer], extras);

        this.volumeControl = volumeControl;
        this.setEffects(...effectNames);
    }

    protected override onDisposing(): void {
        arrayClear(this.effects);
        super.onDisposing();
    }

    setEffects(...effectNames: string[]) {
        this.disable();

        for (const effect of this.effects) {
            this.remove(effect);
            dispose(effect);
        }

        arrayClear(this.effects);

        let last: IAudioNode = this.volumeControl;
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

    private get lastInternal() {
        return this.effects[this.effects.length - 1] || this.volumeControl;
    }

    enable(): void {
        if (!this.lastInternal.isConnected()) {
            this.lastInternal.connect(this.spatializer);
        }
    }

    disable(): void {
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

    get volume(): number {
        return this.volumeControl.gain.value;
    }

    set volume(v: number) {
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

    get algorithm(): DistanceModelType {
        if (isDefined(this.spatializer)) {
            return this.spatializer.algorithm;
        }

        return null;
    }

    setPosition(px: number, py: number, pz: number): void {
        if (isDefined(this.spatializer)) {
            this.pose.setPosition(px, py, pz);
            this.spatializer.readPose(this.pose);
        }
    }

    setOrientation(qx: number, qy: number, qz: number, qw: number): void {
        if (isDefined(this.spatializer)) {
            this.pose.setOrientation(qx, qy, qz, qw);
            this.spatializer.readPose(this.pose);
        }
    }


    set(px: number, py: number, pz: number, qx: number, qy: number, qz: number, qw: number): void {
        if (isDefined(this.spatializer)) {
            this.pose.set(px, py, pz, qx, qy, qz, qw);
            this.spatializer.readPose(this.pose);
        }
    }

    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        if (isDefined(this.spatializer)) {
            this.spatializer.setAudioProperties(minDistance, maxDistance, algorithm);
        }
    }
}