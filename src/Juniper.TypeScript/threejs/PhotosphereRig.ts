import { canvasToBlob, CanvasTypes, createUtilityCanvas } from "@juniper/dom/canvas";
import { IFetcher } from "@juniper/fetcher";
import { deg2rad, IDisposable, IProgress, progressOfArray } from "@juniper/tslib";
import { Image_Jpeg } from "@juniper/tslib/mediatypes/image";
import { cleanup } from "./cleanup";
import { CUBEMAP_PATTERN } from "./Skybox";

const QUAD_SIZE = 2;
export const FACE_SIZE = 1 << 11;
const E = new THREE.Euler();

export enum PhotosphereCaptureResolution {
    Low = 90,
    Medium = 60,
    High = 45,
    Fine = 30
}

const FOVOffsets = new Map<PhotosphereCaptureResolution, number>([
    [PhotosphereCaptureResolution.Low, 4],
    [PhotosphereCaptureResolution.Medium, 8],
    [PhotosphereCaptureResolution.High, 3],
    [PhotosphereCaptureResolution.Fine, 2],
]);

const captureParams = [
    [Math.PI / 2, 0, 0, 1],
    [-Math.PI / 2, 0, 2, 1],
    [0, Math.PI / 2, 1, 0],
    [0, -Math.PI / 2, 1, 2],
    [Math.PI, 0, 3, 1],
    [0, 0, 1, 1]
];

export abstract class PhotosphereRig
    implements IDisposable {
    private baseURL: string = null;

    private readonly canvas: CanvasTypes;
    private readonly renderer: THREE.WebGLRenderer;
    private readonly camera: THREE.PerspectiveCamera;
    private readonly photosphere: THREE.Group;
    private readonly scene: THREE.Scene;
    private readonly geometry: THREE.PlaneGeometry;

    private isDebug = false;
    private disposed = false;

    constructor(
        private readonly fetcher: IFetcher,
        private readonly fixWatermarks: boolean) {
        this.canvas = createUtilityCanvas(FACE_SIZE, FACE_SIZE);
        this.renderer = new THREE.WebGLRenderer({
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

        this.camera = new THREE.PerspectiveCamera(90);
        this.photosphere = new THREE.Group();
        this.scene = new THREE.Scene();
        this.scene.add(new THREE.AmbientLight(0xffffff, 1));
        this.scene.add(this.camera, this.photosphere);
        this.geometry = new THREE.PlaneGeometry(1, 1, 1, 1);
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
        this.renderer.dispose();
    }

    protected async renderFaces(getImagePath: (fov: number, heading: number, pitch: number) => string, level: PhotosphereCaptureResolution, progress: IProgress): Promise<string[]> {
        this.clear();

        await this.loadFrames(level, progress, getImagePath);

        const files = await Promise.all(captureParams.map(async ([heading, pitch, dx, dy]) => {
            const roll = CUBEMAP_PATTERN.rotations[dy][dx];
            E.set(pitch, heading, roll, "YXZ");
            this.camera.setRotationFromEuler(E);
            this.renderer.render(this.scene, this.camera);
            const blob = await canvasToBlob(this.renderer.domElement, Image_Jpeg.value, 1);
            return URL.createObjectURL(blob);
        }));

        this.clear();

        return files;
    }

    protected async renderCubeMap(getImagePath: (fov: number, heading: number, pitch: number) => string, level: PhotosphereCaptureResolution, progress: IProgress): Promise<string> {
        this.clear();

        const canv = createUtilityCanvas(FACE_SIZE * 4, FACE_SIZE * 3);
        const g = canv.getContext("2d");

        await this.loadFrames(level, progress, getImagePath);

        for (const [heading, pitch, dx, dy] of captureParams) {
            E.set(pitch, heading, 0, "YXZ");
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
        const angles = new Array<[number, number, number, number]>();
        const FOV = level as number;

        // Overlap images to crop copyright notices out of
        // most of the images...
        const dFOV = this.fixWatermarks
            ? FOVOffsets.get(FOV)
            : 0;

        for (let pitch = -90 + FOV; pitch < 90; pitch += FOV) {
            for (let heading = -180; heading < 180; heading += FOV) {
                angles.push([heading, pitch, FOV + dFOV, QUAD_SIZE]);
            }
        }

        // Include the top and the bottom
        angles.push([0, -90, FOV + dFOV, QUAD_SIZE]);
        angles.push([0, 90, FOV + dFOV, QUAD_SIZE]);

        if (this.fixWatermarks) {
            // Include an uncropped image so that
            // at least one of the copyright notices is visible.
            angles.push([0, -90, FOV, 0.5 * QUAD_SIZE]);
            angles.push([0, 90, FOV, 0.5 * QUAD_SIZE]);
        }
        return angles;
    }

    private async loadFrames(level: PhotosphereCaptureResolution, progress: IProgress, getImagePath: (fov: number, heading: number, pitch: number) => string) {
        const angles = this.getImageAngles(level);

        await progressOfArray(progress, angles, (set, prog) => this.loadFrame(getImagePath, ...set, prog));
    }

    private async loadFrame(getImagePath: (fov: number, heading: number, pitch: number) => string, heading: number, pitch: number, fov: number, size: number, prog: IProgress) {
        const halfFOV = 0.5 * deg2rad(fov);
        const k = Math.tan(halfFOV);
        const dist = 0.5 * size / k;
        const path = getImagePath(fov, heading, pitch);
        const { content: canvas } = await this.fetcher
            .get(path, this.baseURL)
            .progress(prog)
            .useCache(this.isDebug)
            .canvas();

        const texture = new THREE.Texture(canvas as any);
        const material = new THREE.MeshBasicMaterial({
            map: texture,
            side: THREE.DoubleSide
        });
        const mesh = new THREE.Mesh(this.geometry, material);
        texture.needsUpdate = true;
        material.needsUpdate = true;
        E.set(deg2rad(pitch), -deg2rad(heading), 0, "YXZ");
        mesh.scale.setScalar(size);
        mesh.quaternion.setFromEuler(E);
        mesh.position.set(0, 0, -dist)
            .applyQuaternion(mesh.quaternion);
        this.photosphere.add(mesh);
    }

    private clear() {
        for (const child of this.photosphere.children) {
            cleanup(child);
        }
        this.photosphere.clear();
    }
}