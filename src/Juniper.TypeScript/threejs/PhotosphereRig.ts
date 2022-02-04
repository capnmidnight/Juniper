import { CanvasTypes, createUtilityCanvas } from "juniper-dom/canvas";
import { deg2rad, IProgress, progressOfArray } from "juniper-tslib";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { Image2DMesh } from "./Image2DMesh";
import { deepSetLayer, FOREGROUND, PHOTOSPHERE_CAPTURE } from "./layers";
import { obj, objGraph } from "./objects";
import { PhotosphereCaptureResolution } from "./PhotosphereCaptureResolution";

const FOVOffsets = new Map<PhotosphereCaptureResolution, number>([
    [PhotosphereCaptureResolution.Low, 4],
    [PhotosphereCaptureResolution.Medium, 8],
    [PhotosphereCaptureResolution.High, 3],
    [PhotosphereCaptureResolution.Fine, 2],
]);

export class PhotosphereRig {

    constructor(private readonly env: BaseEnvironment<unknown>) {

    }

    async constructPhotosphere(quality: PhotosphereCaptureResolution, fixWatermarks: boolean, getImagePath: (fov: number, heading: number, pitch: number) => string, downloadProg: IProgress): Promise<THREE.Object3D> {
        this.env.avatar.reset();

        const photosphere = obj("Photosphere");
        photosphere.layers.set(PHOTOSPHERE_CAPTURE);
        objGraph(this.env.foreground, photosphere);

        const FOV = quality as number;

        // Overlap images to crop copyright notices out of
        // most of the images...
        const dFOV = fixWatermarks
            ? FOVOffsets.get(FOV)
            : 0;

        photosphere.position.y = this.env.camera.getWorldPosition(new THREE.Vector3()).y;
        const size = 2;

        const angles = new Array<[number, number, number, number]>();

        for (let pitch = -90 + FOV; pitch < 90; pitch += FOV) {
            for (let heading = -180; heading < 180; heading += FOV) {
                angles.push([heading, pitch, FOV + dFOV, size]);
            }
        }

        // Include the top and the bottom
        angles.push([0, -90, FOV + dFOV, size]);
        angles.push([0, 90, FOV + dFOV, size]);

        if (fixWatermarks) {
            // Include an uncropped image so that
            // at least one of the copyright notices is visible.
            angles.push([0, -90, FOV, 0.5 * size]);
            angles.push([0, 90, FOV, 0.5 * size]);
        }

        this.env.camera.layers.set(PHOTOSPHERE_CAPTURE);

        await progressOfArray(downloadProg, angles, async (set, prog) => {
            const [heading, pitch, fov, size] = set;
            const halfFOV = 0.5 * deg2rad(fov);
            const dist = 0.5 * size / Math.tan(halfFOV);
            const path = getImagePath(fov, heading, pitch);
            const frame = new Image2DMesh(this.env, path, { transparent: false, side: THREE.DoubleSide });
            deepSetLayer(frame, PHOTOSPHERE_CAPTURE);
            await frame.mesh.loadImage(path, prog);
            const euler = new THREE.Euler(deg2rad(pitch), -deg2rad(heading), 0, "YXZ");
            const quat = new THREE.Quaternion().setFromEuler(euler);
            const pos = new THREE.Vector3(0, 0, -dist)
                .applyQuaternion(quat);
            frame.scale.setScalar(size);
            frame.quaternion.copy(quat);
            frame.position.copy(pos);
            photosphere.add(frame);
        });

        this.env.camera.layers.set(FOREGROUND);

        return photosphere;
    }

    async capturePhotosphere(renderProg: IProgress, quadFaceSize: number): Promise<CanvasTypes> {
        const metrics = this.env.screenControl.getMetrics();
        const { heading, pitch } = this.env.avatar;
        this.env.screenControl.setMetrics(quadFaceSize, quadFaceSize, 1, 90);

        const canv = createUtilityCanvas(quadFaceSize * 4, quadFaceSize * 3);
        const g = canv.getContext("2d");

        const captureParams = [
            [0, 1, 1, 0],
            [1, 0, 0, 1],
            [0, 0, 1, 1],
            [-1, 0, 2, 1],
            [2, 0, 3, 1],
            [0, -1, 1, 2]
        ];

        this.env.camera.layers.set(PHOTOSPHERE_CAPTURE);

        for (let i = 0; i < captureParams.length; ++i) {
            const [h, p, dx, dy] = captureParams[i];
            renderProg.report(i, captureParams.length, "rendering");
            this.env.avatar.setOrientationImmediate(h * Math.PI / 2, p * Math.PI / 2);
            this.env.renderer.render(this.env.scene, this.env.camera);
            g.drawImage(this.env.renderer.domElement, dx * quadFaceSize, dy * quadFaceSize);
            renderProg.report(i + 1, captureParams.length, "rendering");
        }

        this.env.camera.layers.set(FOREGROUND);


        this.env.screenControl.setMetrics(metrics.width, metrics.height, metrics.pixelRatio, metrics.fov);
        this.env.avatar.setOrientationImmediate(heading, pitch);
        return canv;
    }
}