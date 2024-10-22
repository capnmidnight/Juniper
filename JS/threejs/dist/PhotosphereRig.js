import { deg2rad, dispose, HalfPi, Pi } from "@juniper-lib/util";
import { canvasToBlob, createUtilityCanvas } from "@juniper-lib/dom";
import { unwrapResponse } from "@juniper-lib/fetcher";
import { Image_Jpeg } from "@juniper-lib/mediatypes";
import { progressOfArray } from "@juniper-lib/progress";
import { AmbientLight, DoubleSide, Euler, Group, MeshBasicMaterial, PerspectiveCamera, PlaneGeometry, Scene, Texture, WebGLRenderer } from "three";
import { cleanup } from "./cleanup";
import { mesh, objGraph } from "./objects";
import { CUBEMAP_PATTERN } from "./Skybox";
const QUAD_SIZE = 2;
export const FACE_SIZE = /*@__PURE__*/ 1 << 11;
const E = new Euler();
export var PhotosphereCaptureResolution;
(function (PhotosphereCaptureResolution) {
    PhotosphereCaptureResolution[PhotosphereCaptureResolution["Low"] = 90] = "Low";
    PhotosphereCaptureResolution[PhotosphereCaptureResolution["Medium"] = 60] = "Medium";
    PhotosphereCaptureResolution[PhotosphereCaptureResolution["High"] = 45] = "High";
    PhotosphereCaptureResolution[PhotosphereCaptureResolution["Fine"] = 30] = "Fine";
})(PhotosphereCaptureResolution || (PhotosphereCaptureResolution = {}));
const FOVOffsetsDegrees = new Map([
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
export class PhotosphereRig {
    constructor(fetcher, fixWatermarks) {
        this.fetcher = fetcher;
        this.fixWatermarks = fixWatermarks;
        this.baseURL = null;
        this.isDebug = false;
        this.disposed = false;
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
        this.scene = objGraph(new Scene(), new AmbientLight(0xffffff, 1), this.camera, this.photosphere);
        this.geometry = new PlaneGeometry(1, 1, 1, 1);
    }
    init(baseURL, isDebug) {
        this.baseURL = baseURL;
        this.isDebug = isDebug;
    }
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            this.onDisposing();
        }
    }
    onDisposing() {
        for (const child of this.scene.children) {
            cleanup(child);
        }
        dispose(this.renderer);
    }
    async renderFaces(getImagePath, level, progress) {
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
    async renderCubeMap(getImagePath, level, progress) {
        this.clear();
        const canv = createUtilityCanvas(FACE_SIZE * 4, FACE_SIZE * 3);
        const g = canv.getContext("2d", { alpha: false });
        await this.loadFrames(level, progress, getImagePath);
        for (const [headingRadians, pitchRadians, dx, dy] of captureParamsRadians) {
            E.set(pitchRadians, headingRadians, 0, "YXZ");
            this.camera.setRotationFromEuler(E);
            this.renderer.render(this.scene, this.camera);
            g.drawImage(this.renderer.domElement, dx * FACE_SIZE, dy * FACE_SIZE);
        }
        const blob = await canvasToBlob(canv, Image_Jpeg.value, 1);
        const file = URL.createObjectURL(blob);
        this.clear();
        return file;
    }
    getImageAngles(level) {
        const anglesDegrees = new Array();
        const FOVDegrees = level;
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
    async loadFrames(level, progress, getImagePath) {
        const angles = this.getImageAngles(level);
        await progressOfArray(progress, angles, (set, prog) => this.loadFrame(getImagePath, ...set, prog));
    }
    async loadFrame(getImagePath, headingDegrees, pitchDegrees, fovDegrees, size, prog) {
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
        const texture = new Texture(canvas);
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
        objGraph(this.photosphere, frame);
    }
    clear() {
        for (const child of this.photosphere.children) {
            cleanup(child);
        }
        this.photosphere.clear();
    }
}
//# sourceMappingURL=PhotosphereRig.js.map