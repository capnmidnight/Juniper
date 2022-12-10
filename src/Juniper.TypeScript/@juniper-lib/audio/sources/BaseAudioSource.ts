import { arrayClear } from "@juniper-lib/tslib/collections/arrays";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { IAudioNode } from "../context/IAudioNode";
import { JuniperAudioContext } from "../context/JuniperAudioContext";
import { JuniperAudioNode } from "../context/JuniperAudioNode";
import { JuniperGainNode } from "../context/JuniperGainNode";
import { effectStore } from "../effects";
import { Pose } from "../Pose";
import { BaseSpatializer } from "../spatializers/BaseSpatializer";
import { IAudioSource } from "./IAudioSource";

export abstract class BaseAudioSource<EventTypeT = void>
    extends JuniperAudioNode<EventTypeT>
    implements IAudioSource {

    private readonly effects = new Array<IAudioNode>();

    protected readonly volumeControl: JuniperGainNode;

    private readonly pose = new Pose();
    private readonly output: IAudioNode;

    constructor(
        type: string,
        context: JuniperAudioContext,
        public readonly spatializer: BaseSpatializer,
        effectNames: string[],
        extras?: ReadonlyArray<IAudioNode>) {

        const volumeControl = new JuniperGainNode(context);
        volumeControl.name = "volume-control";

        extras = extras || [];

        let output: IAudioNode = spatializer;
        if (isNullOrUndefined(output)) {
            output = new JuniperGainNode(context);
            output.name = "output";
        }

        super(type, context, [], [output], extras);

        this.volumeControl = volumeControl;
        this.output = output;
        this.setEffects(...effectNames);

        this.enable();
    }

    protected override onDisposing(): void {
        arrayClear(this.effects);
        super.onDisposing();
    }

    setEffects(...effectNames: string[]) {
        for (const effect of this.effects) {
            this.remove(effect);
            effect.dispose();
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
    }

    get spatialized() {
        return isDefined(this.spatializer);
    }

    private get lastInternal() {
        return this.effects[this.effects.length - 1] || this.volumeControl;
    }

    enable(): void {
        if (!this.lastInternal.isConnected()) {
            this.lastInternal.connect(this.output);
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

    setOrientation(fx: number, fy: number, fz: number): void;
    setOrientation(fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    setOrientation(fx: number, fy: number, fz: number, ux?: number, uy?: number, uz?: number): void {
        if (isDefined(this.spatializer)) {
            this.pose.setOrientation(fx, fy, fz, ux, uy, uz);
            this.spatializer.readPose(this.pose);
        }
    }


    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number): void;
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux?: number, uy?: number, uz?: number): void {
        if (isDefined(this.spatializer)) {
            this.pose.set(px, py, pz, fx, fy, fz, ux, uy, uz);
            this.spatializer.readPose(this.pose);
        }
    }

    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        if (isDefined(this.spatializer)) {
            this.spatializer.setAudioProperties(minDistance, maxDistance, algorithm);
        }
    }
}