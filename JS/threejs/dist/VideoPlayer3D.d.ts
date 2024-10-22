import { JuniperAudioContext } from "@juniper-lib/audio/dist/context/JuniperAudioContext";
import { BaseSpatializer } from "@juniper-lib/audio/dist/spatializers/BaseSpatializer";
import { BaseVideoPlayer } from "@juniper-lib/video";
import { BufferGeometry, MeshBasicMaterial, Object3D } from "three";
import { BaseEnvironment } from "./environment/BaseEnvironment";
import { ErsatzObject } from "./objects";
export type SphereEncodingName = "N/A" | "Cubemap" | "Cubemap Strips" | "Equi-Angular Cubemap (YouTube)" | "Equirectangular" | "Half Equirectangular" | "Panoramic";
export declare const SphereEncodingNames: SphereEncodingName[];
export type StereoLayoutName = "mono" | "left-right" | "right-left" | "top-bottom" | "bottom-top";
export declare const StereoLayoutNames: StereoLayoutName[];
export declare class VideoPlayer3D extends BaseVideoPlayer implements ErsatzObject {
    private readonly material;
    private readonly vidMeshes;
    readonly object: Object3D;
    constructor(env: BaseEnvironment, context: JuniperAudioContext, spatializer: BaseSpatializer);
    get meshes(): import("three").Mesh<BufferGeometry, MeshBasicMaterial>[];
    protected onDisposing(): void;
    isSupported(encoding: SphereEncodingName, layout: StereoLayoutName): boolean;
    setStereoParameters(encoding: SphereEncodingName, layout: StereoLayoutName): void;
}
//# sourceMappingURL=VideoPlayer3D.d.ts.map