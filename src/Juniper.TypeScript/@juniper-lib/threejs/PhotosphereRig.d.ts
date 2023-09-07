import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { IProgress } from "@juniper-lib/progress/IProgress";
import { IDisposable } from "@juniper-lib/tslib/using";
export declare const FACE_SIZE: number;
export type getImagePathCallback = (fovDegrees: number, headingDegrees: number, pitchDegrees: number) => string;
export declare enum PhotosphereCaptureResolution {
    Low = 90,
    Medium = 60,
    High = 45,
    Fine = 30
}
export declare abstract class PhotosphereRig implements IDisposable {
    private readonly fetcher;
    private readonly fixWatermarks;
    private baseURL;
    private readonly canvas;
    private readonly renderer;
    private readonly camera;
    private readonly photosphere;
    private readonly scene;
    private readonly geometry;
    private isDebug;
    private disposed;
    constructor(fetcher: IFetcher, fixWatermarks: boolean);
    init(baseURL: string, isDebug: boolean): void;
    dispose(): void;
    private onDisposing;
    protected renderFaces(getImagePath: getImagePathCallback, level: PhotosphereCaptureResolution, progress: IProgress): Promise<string[]>;
    protected renderCubeMap(getImagePath: getImagePathCallback, level: PhotosphereCaptureResolution, progress: IProgress): Promise<string>;
    private getImageAngles;
    private loadFrames;
    private loadFrame;
    private clear;
}
//# sourceMappingURL=PhotosphereRig.d.ts.map