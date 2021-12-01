import { Pose } from "../../Pose";
import { BaseEmitter } from "./BaseEmitter";
/**
 * Base class for spatializers that uses WebAudio's PannerNode
 **/
export declare abstract class BaseWebAudioPanner extends BaseEmitter {
    protected readonly panner: PannerNode;
    /**
     * Creates a new spatializer that uses WebAudio's PannerNode.
     */
    constructor(id: string, audioCtx: AudioContext);
    private disposed;
    dispose(): void;
    copyAudioProperties(from: BaseWebAudioPanner): void;
    /**
     * Sets parameters that alter spatialization.
     **/
    setAudioProperties(minDistance: number, maxDistance: number, algorithm: DistanceModelType): void;
    private lpx;
    private lpy;
    private lpz;
    private lox;
    private loy;
    private loz;
    /**
     * Performs the spatialization operation for the audio source's latest location.
     */
    update(loc: Pose, t: number): void;
    protected abstract setPosition(x: number, y: number, z: number, t: number): void;
    protected abstract setOrientation(x: number, y: number, z: number, t: number): void;
}
