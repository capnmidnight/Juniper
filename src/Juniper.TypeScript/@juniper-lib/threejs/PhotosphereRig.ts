import { canvasToBlob, CanvasTypes, Context2D, createUtilityCanvas } from "@juniper-lib/dom/canvas";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { unwrapResponse } from "@juniper-lib/fetcher/unwrapResponse";
import { Image_Jpeg } from "@juniper-lib/mediatypes";
import { deg2rad, HalfPi, Pi } from "@juniper-lib/tslib/math";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { progressOfArray } from "@juniper-lib/tslib/progress/progressOfArray";
import { dispose, IDisposable } from "@juniper-lib/tslib/using";
import { AmbientLight, DoubleSide, Euler, Group, MeshBasicMaterial, PerspectiveCamera, PlaneGeometry, Scene, Texture, WebGLRenderer } from "three";
import { cleanup } from "./cleanup";
import { mesh, objGraph } from "./objects";
import { CUBEMAP_PATTERN } from "./Skybox";

const QUAD_SIZE = 2;
export const FACE_SIZE = /*@__PURE__*/ 1 << 11;
export type getImagePathCallback = (fovDegrees: number, headingDegrees: number, pitchDegrees: number) => string;
const E = new Euler();

export enum PhotosphereCaptureResolution {
    Low = 90,
    Medium = 60,
    High = 45,
    Fine = 30
}

const FOVOffsetsDegrees = new Map<PhotosphereCaptureResolution, number>([
    [PhotosphereCaptureResolution.Low, 4],
    [PhotosphereCaptureResolution.Medium, 8],
    [PhotosphereCaptureResolution.High, 3],
    [PhotosphereCaptureResolution.Fine, 2],
]);

const captureParamsRadians = [
    [HalfPi, 0, 0, 1],
    [-HalfPi, 0, 2, 1],
    [0, HalfPi, 1, 0],
    [0, -HalfPi, 1, 2],
    [Pi, 0, 3, 1],
    [0, 0, 1, 1]
];

export abstract class PhotosphereRig
    implements IDisposable {
    private baseURL: string = null;

    private readonly canvas: CanvasTypes;
    private readonly renderer: WebGLRenderer;
    private readonly camera: PerspectiveCamera;
    private readonly photosphere: Group;
    private readonly scene: Scene;
    private readonly geometry: PlaneGeometry;

    private isDebug = false;
    private disposed = false;

    constructor(
        private readonly fetcher: IFetcher,
        private readonly fixWatermarks: boolean) {
        this.canvas = createUtilityCanvas(FACE_SIZE, FACE_SIZE);
        this.renderer = new WebGLRenderer({
            canvas: this.canvas,
            alpha: false,
            antialias: false,
            depth: true,
            logarithmicDepthBuffer: false,
            powerPreference: "low-power",
            precision: "lowp",
            stencil: false,
            premultipliedAlpha: false,
            preserveDrawingBuffer: false
        });

        this.camera = new PerspectiveCamera(90);
        this.photosphere = new Group();
        this.scene = objGraph(new Scene(),
            new AmbientLight(0xffffff, 1),
            this.camera,
            this.photosphere
        );
        this.geometry = new PlaneGeometry(1, 1, 1, 1);
    }

    init(baseURL: string, isDebug: boolean): void {
        this.baseURL = baseURL;
        this.isDebug = isDebug;
    }

    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            this.onDisposing();
        }
    }

    private onDisposing() {
        for (const child of this.scene.children) {
            cleanup(child);
        }
        dispose(this.renderer);
    }

    protected async renderFaces(getImagePath: getImagePathCallback, level: PhotosphereCaptureResolution, progress: IProgress): Promise<string[]> {
        this.clear();

        await this.loadFrames(level, progress, getImagePath);

        const files = await Promise.all(captureParamsRadians.map(async ([headingRadians, pitchRadians, dx, dy]) => {
            const rollRadians = CUBEMAP_PATTERN.rotations[dy][dx];
            E.set(pitchRadians, headingRadians, rollRadians, "YXZ");
            this.camera.setRotationFromEuler(E);
            this.renderer.render(this.scene, this.camera);
            const blob = await canvasToBlob(this.renderer.domElement, Image_Jpeg.value, 1);
            return URL.createObjectURL(blob);
        }));

        this.clear();

        return files;
    }

    protected async renderCubeMap(getImagePath: getImagePathCallback, level: PhotosphereCaptureResolution, progress: IProgress): Promise<string> {
        this.clear();

        const canv = createUtilityCanvas(FACE_SIZE * 4, FACE_SIZE * 3);
        const g = canv.getContext("2d", { alpha: false }) as Context2D;

        await this.loadFrames(level, progress, getImagePath);

        for (const [headingRadians, pitchRadians, dx, dy] of captureParamsRadians) {
            E.set(pitchRadians, headingRadians, 0, "YXZ");
            this.camera.setRotationFromEuler(E);
            this.renderer.render(this.scene, this.camera);
            g.drawImage(this.renderer.domElement, dx * FACE_SIZE, dy * FACE_SIZE);
        }

        const blob = await canvasToBlob(canv, Image_Jpeg.value, 1);
        const file = URL.createObjectURL(blob);;

        this.clear();

        return file;
    }

    private getImageAngles(level: PhotosphereCaptureResolution) {
        const anglesDegrees = new Array<[number, number, number, number]>();
        const FOVDegrees = level as number;

        // Overlap images to crop copyright notices out of
        // most of the images...
        const dFOVDegrees = this.fixWatermarks
            ? FOVOffsetsDegrees.get(FOVDegrees)
            : 0;

        for (let pitchDegrees = -90 + FOVDegrees; pitchDegrees < 90; pitchDegrees += FOVDegrees) {
            for (let headingDegrees = -180; headingDegrees < 180; headingDegrees += FOVDegrees) {
                anglesDegrees.push([headingDegrees, pitchDegrees, FOVDegrees + dFOVDegrees, QUAD_SIZE]);
            }
        }

        // Include the top and the bottom
        anglesDegrees.push([0, -90, FOVDegrees + dFOVDegrees, QUAD_SIZE]);
        anglesDegrees.push([0, 90, FOVDegrees + dFOVDegrees, QUAD_SIZE]);

        if (this.fixWatermarks) {
            // Include an uncropped image so that
            // at least one of the copyright notices is visible.
            anglesDegrees.push([0, -90, FOVDegrees, 0.5 * QUAD_SIZE]);
            anglesDegrees.push([0, 90, FOVDegrees, 0.5 * QUAD_SIZE]);
        }
        return anglesDegrees;
    }

    private async loadFrames(level: PhotosphereCaptureResolution, progress: IProgress, getImagePath: getImagePathCallback) {
        const angles = this.getImageAngles(level);

        await progressOfArray(progress, angles, (set, prog) => this.loadFrame(getImagePath, ...set, prog));
    }

    private async loadFrame(getImagePath: getImagePathCallback, headingDegrees: number, pitchDegrees: number, fovDegrees: number, size: number, prog: IProgress) {
        const halfFOV = 0.5 * deg2rad(fovDegrees);
        const k = Math.tan(halfFOV);
        const dist = 0.5 * size / k;
        const path = getImagePath(fovDegrees, headingDegrees, pitchDegrees);
        const canvas = await this.fetcher
            .get(path, this.baseURL)
            .progress(prog)
            .useCache(!this.isDebug)
            .canvas()
            .then(unwrapResponse);

        const texture = new Texture(canvas as any);
        const material = new MeshBasicMaterial({
            map: texture,
            side: DoubleSide
        });
        const frame = mesh(`frame-${fovDegrees}-${headingDegrees}-${pitchDegrees}`, this.geometry, material);
        texture.needsUpdate = true;
        material.needsUpdate = true;
        E.set(deg2rad(pitchDegrees), -deg2rad(headingDegrees), 0, "YXZ");
        frame.scale.setScalar(size);
        frame.quaternion.setFromEuler(E);
        frame.position
            .set(0, 0, -dist)
            .applyQuaternion(frame.quaternion);
        objGraph(this.photosphere, frame)
    }

    private clear() {
        for (const child of this.photosphere.children) {
            cleanup(child);
        }
        this.photosphere.clear();
    }
}