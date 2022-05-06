import { CanvasTypes, Context2D, createUtilityCanvas } from "@juniper/dom/canvas";
import { IFetcher } from "@juniper/fetcher";
import { deg2rad, IDisposable, IProgress, isNullOrUndefined, progressOfArray, TypedEvent, TypedEventBase } from "@juniper/tslib";
import { cleanup } from "./cleanup";
import { CUBEMAP_PATTERN } from "./Skybox";

const QUAD_SIZE = 2;
const FACE_SIZE = 2048;
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
    [0, Math.PI / 2, 1, 0],
    [Math.PI / 2, 0, 0, 1],
    [0, 0, 1, 1],
    [-Math.PI / 2, 0, 2, 1],
    [Math.PI, 0, 3, 1],
    [0, -Math.PI / 2, 1, 2]
];

export abstract class PhotosphereRig extends TypedEventBase<{
    "framesinitialized": TypedEvent<"framesinitialized">;
    "framesupdated": TypedEvent<"framesupdated">;
}> implements IDisposable {
    private readonly levels: PhotosphereCaptureResolution[];
    private readonly canvas: CanvasTypes;
    private readonly renderer: THREE.WebGLRenderer;
    private readonly camera: THREE.PerspectiveCamera;
    private readonly photosphere: THREE.Group;
    private readonly scene: THREE.Scene;
    private readonly geometry: THREE.PlaneGeometry;
    public readonly frames: [CanvasTypes, CanvasTypes, CanvasTypes, CanvasTypes, CanvasTypes, CanvasTypes];
    private readonly contexts: Context2D[];
    private readonly framesInitializedEvt = new TypedEvent("framesinitialized");
    private readonly framesUpdatedEvt = new TypedEvent("framesupdated");

    private disposed = false;

    constructor(private readonly fetcher: IFetcher, private readonly fixWatermarks: boolean, ...levels: PhotosphereCaptureResolution[]) {
        super();

        if (isNullOrUndefined(levels)) {
            levels = [];
        }

        if (levels.length === 0) {
            levels.push(PhotosphereCaptureResolution.Fine);
        }

        this.levels = levels;

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
        this.frames = [
            createUtilityCanvas(FACE_SIZE, FACE_SIZE),
            createUtilityCanvas(FACE_SIZE, FACE_SIZE),
            createUtilityCanvas(FACE_SIZE, FACE_SIZE),
            createUtilityCanvas(FACE_SIZE, FACE_SIZE),
            createUtilityCanvas(FACE_SIZE, FACE_SIZE),
            createUtilityCanvas(FACE_SIZE, FACE_SIZE)
        ];

        this.contexts = this.frames.map(f => f.getContext("2d"));
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

    async constructPhotosphere(getImagePath: (fov: number, heading: number, pitch: number) => string, progress: IProgress): Promise<void> {
        for (let levelIndex = 0; levelIndex < this.levels.length; ++levelIndex) {
            await this.renderLevel(getImagePath, this.levels[levelIndex], progress);
            progress = null;

            if (levelIndex > 0) {
                this.dispatchEvent(this.framesUpdatedEvt);
            }
            else {
                this.dispatchEvent(this.framesInitializedEvt);
            }
        }
    }

    private async renderLevel(getImagePath: (fov: number, heading: number, pitch: number) => string, level: PhotosphereCaptureResolution, progress: IProgress) {
        this.clear();

        await this.loadImages(level, progress, getImagePath);

        for (let i = 0; i < captureParams.length; ++i) {
            const [heading, pitch, dx, dy] = captureParams[i];
            const faceIndex = CUBEMAP_PATTERN.indices[dy][dx];
            const roll = CUBEMAP_PATTERN.rotations[dy][dx];
            const g = this.contexts[faceIndex];
            this.drawFrame(g, heading, pitch, roll, 0, 0);
        }
    }

    capturePhotosphere(): CanvasTypes {

        const canv = createUtilityCanvas(FACE_SIZE * 4, FACE_SIZE * 3);
        const g = canv.getContext("2d");

        for (let i = 0; i < captureParams.length; ++i) {
            const [heading, pitch, dx, dy] = captureParams[i];
            this.drawFrame(g, heading, pitch, 0, dx, dy);
        }

        this.clear();

        return canv;
    }

    private drawFrame(g: Context2D, heading: number, pitch: number, roll: number, dx: number, dy: number) {
        E.set(pitch, heading, roll, "YXZ");
        this.camera.setRotationFromEuler(E);
        this.renderer.render(this.scene, this.camera);
        g.drawImage(this.renderer.domElement, dx * FACE_SIZE, dy * FACE_SIZE);
    }

    private async loadImages(level: PhotosphereCaptureResolution, progress: IProgress, getImagePath: (fov: number, heading: number, pitch: number) => string) {
        const angles = this.getImageAngles(level);

        await progressOfArray(progress, angles, (set, prog) => this.loadImage(getImagePath, ...set, prog));
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

    private async loadImage(getImagePath: (fov: number, heading: number, pitch: number) => string, heading: number, pitch: number, fov: number, size: number, prog: IProgress) {
        const halfFOV = 0.5 * deg2rad(fov);
        const k = Math.tan(halfFOV);
        const dist = 0.5 * size / k;
        const path = getImagePath(fov, heading, pitch);
        const response = await this.fetcher
            .get(path)
            .progress(prog)
            .useCache()
            .image();

        const texture = new THREE.Texture(response.content);
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