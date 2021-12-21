import { arrayClear, arrayRemove, once } from "juniper-tslib";
import { BufferSource, removeVertex } from "../nodes";
import { Pose } from "../Pose";
import { BaseAudioSource } from "./BaseAudioSource";
import type { IPlayableSource } from "./IPlayableSource";
import { BaseEmitter } from "./spatializers/BaseEmitter";
import { NoSpatializationNode } from "./spatializers/NoSpatializationNode";

class WrappedEmitter extends BaseEmitter {
    constructor(private readonly sub: BaseEmitter) {
        super(`${sub.id}-wrapped`);
        this.input = this.sub.input;
        this.output = this.sub.output;
    }

    dispose(): void {
        // do nothing, someone else is disposing for us.
    }

    update(loc: Pose, t: number): void {
        this.sub.update(loc, t);
    }
}

class AudioBufferSource
    extends BaseAudioSource<AudioBufferSourceNode>
    implements IPlayableSource {

    public isPlaying = false;

    constructor(id: string, audioCtx: AudioContext, source: AudioBufferSourceNode, randomize: boolean, spatializer: BaseEmitter, ...effectNames: string[]) {
        super(id, audioCtx, spatializer, randomize, ...effectNames);

        this.input = source;
        this.disconnect();
    }

    private disposed3 = false;
    override dispose(): void {
        if (!this.disposed3) {
            this.stop();
            removeVertex(this.input);
            this.disposed3 = true;
        }

        super.dispose();
    }

    async play(): Promise<void> {
        this.isPlaying = true;
        this.input.start();
        await once<AudioScheduledSourceNodeEventMap, "ended">(this.input, "ended");
        this.stop();
    }

    stop(): void {
        this.input.stop();
        this.isPlaying = false;
    }
}

export class AudioBufferSpawningSource implements IPlayableSource {
    private counter = 0;
    private playingSources = new Array<AudioBufferSource>();
    private _volume = 1;
    private effectNames: string[];

    constructor(public readonly id: string,
        private readonly audioCtx: AudioContext,
        private readonly source: AudioBufferSourceNode,
        private readonly randomize: boolean,
        private readonly spatializer: BaseEmitter,
        ...effectNames: string[]) {
        this.effectNames = effectNames;
    }

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            this.stop();
            this.spatializer.dispose();
            this.disposed = true;
        }
    }

    get volume(): number {
        return this._volume;
    }

    set volume(v: number) {
        if (v !== this._volume) {
            this._volume = v;
            for (const source of this.playingSources) {
                source.volume = v;
            }
        }
    }

    get spatialized(): boolean {
        return !(this.spatializer instanceof NoSpatializationNode);
    }

    readonly pose = new Pose();

    get isPlaying(): boolean {
        for (const source of this.playingSources) {
            if (source.isPlaying) {
                return true;
            }
        }

        return false;
    }

    setEffects(...effectNames: string[]) {
        for (const source of this.playingSources) {
            source.setEffects(...effectNames);
        }

        this.effectNames = effectNames;
    }


    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void {
        this.spatializer.setAudioProperties(minDistance, maxDistance, algorithm);
        for (const source of this.playingSources) {
            source.setAudioProperties(minDistance, maxDistance, algorithm);
        }
    }

    async play(): Promise<void> {
        const newBuffer = BufferSource(
            `buffer-source-${this.id}`,
            this.audioCtx, {
            buffer: this.source.buffer,
            loop: this.source.loop
        });

        const newSource = new AudioBufferSource(
            `${this.id}-${this.counter++}`,
            this.audioCtx,
            newBuffer,
            this.randomize,
            new WrappedEmitter(this.spatializer),
            ...this.effectNames);

        this.playingSources.push(newSource);

        newSource.play();

        if (!this.source.loop) {
            await once<AudioScheduledSourceNodeEventMap, "ended">(newBuffer, "ended");
            if (this.playingSources.indexOf(newSource) >= 0) {
                arrayRemove(this.playingSources, newSource);
                newSource.dispose();
            }
        }
    }

    stop(): void {
        for (const source of this.playingSources) {
            source.dispose();
        }

        arrayClear(this.playingSources);
    }

    update(t: number): void {
        for (const source of this.playingSources) {
            source.pose.copy(this.pose);
            source.update(t);
        }
    }
}
