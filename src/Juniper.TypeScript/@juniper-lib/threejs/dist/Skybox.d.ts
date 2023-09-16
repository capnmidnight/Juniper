import { CanvasImageTypes, CanvasTypes } from "@juniper-lib/dom/dist/canvas";
import { CubeMapFaceIndex } from "@juniper-lib/graphics2d/dist/CubeMapFaceIndex";
import { CubeTexture, Euler, Quaternion } from "three";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
type SkyboxRotation = Quaternion | Euler | number[] | number;
export declare const CUBEMAP_PATTERN: {
    rows: number;
    columns: number;
    indices: CubeMapFaceIndex[][];
    rotations: number[][];
};
export declare class Skybox {
    private readonly env;
    private readonly rt;
    private readonly rtScene;
    private readonly rtCamera;
    private readonly _rotation;
    private readonly layerRotation;
    private readonly stageRotation;
    private readonly canvases;
    private readonly contexts;
    private readonly flipped;
    private readonly flipper;
    private readonly onNeedsRedraw;
    private layerOrientation;
    private images;
    private _cube;
    get envMap(): CubeTexture;
    private curImagePath;
    private layer;
    private wasVisible;
    private stageHeading;
    private rotationNeedsUpdate;
    private imageNeedsUpdate;
    useWebXRLayers: boolean;
    private wasWebXRLayerAvailable;
    visible: boolean;
    constructor(env: BaseEnvironment<unknown>);
    clear(): void;
    setImage(imageID: string, image: CanvasImageTypes): CubeTexture;
    sliceImage(image: CanvasImageTypes): CanvasTypes[];
    setImages(imageID: string, images: CanvasImageTypes[]): CubeTexture;
    updateImages(): void;
    get rotation(): Quaternion;
    set rotation(rotation: SkyboxRotation);
    private checkWebXRLayer;
}
export {};
//# sourceMappingURL=Skybox.d.ts.map