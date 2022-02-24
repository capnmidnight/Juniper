import { CubeMapFaceIndex } from "juniper-2d/CubeMapFaceIndex";
import { canvasToBlob, CanvasTypes, createUtilityCanvas, snapshot } from "juniper-dom/canvas";
import type { IProgress } from "juniper-tslib";
import { usingAsync } from "juniper-tslib";
import { Camera } from "./Camera";
import { Context3D } from "./Context3D";
import { RenderTargetManager } from "./FramebufferManager";
import { Geometry } from "./Geometry";
import { invCube } from "./geometry/cubes";
import { TextureImageArray, TextureImageStereo } from "./managed/TextureImage";
import { Mesh } from "./Mesh";
import { MaterialEquirectangular } from "./programs/MaterialEquirectangular";

interface CaptureOrientation {
    heading: number;
    pitch: number;
}

const captureParams = new Map<CubeMapFaceIndex, CaptureOrientation>([
    [CubeMapFaceIndex.Left, { heading: 1, pitch: 0 }],
    [CubeMapFaceIndex.Right, { heading: -1, pitch: 0 }],
    [CubeMapFaceIndex.Up, { heading: 2, pitch: 1 }],
    [CubeMapFaceIndex.Down, { heading: 2, pitch: -1 }],
    [CubeMapFaceIndex.Back, { heading: 2, pitch: 0 }],
    [CubeMapFaceIndex.Front, { heading: 0, pitch: 0 }]
]);

async function equirectangularToCubemap<T>(image: TexImageSource | OffscreenCanvas, isStereo: boolean, size: number, saveImage: (canvas: CanvasTypes) => Promise<T>, onProgress?: IProgress): Promise<T[]> {
    const ctx3d = new Context3D(createUtilityCanvas(size, size), {
        alpha: true,
        antialias: false,
        powerPreference: "low-power"
    });

    const { gl } = ctx3d;
    const fbManager = new RenderTargetManager(gl);

    const cam = new Camera(ctx3d, {
        fov: 90
    });

    return await usingAsync(isStereo ? new TextureImageStereo(gl, image) : new TextureImageArray(gl, image, 1), async (texture) =>
        await usingAsync(new MaterialEquirectangular(gl), async (material) =>
            await usingAsync(new Geometry(gl, invCube), async (geom) => {
                const mesh = new Mesh(gl, geom, texture, material);
                const output = new Array<T>(captureParams.size);
                let count = 0;
                for (const [i, directions] of captureParams) {
                    const { heading, pitch } = directions;
                    if (onProgress) {
                        onProgress.report(count++, captureParams.size, "rendering");
                    }

                    cam.rotateTo(heading * 90, pitch * 90);

                    fbManager.beginFrame();
                    mesh.material.use();
                    mesh.render(cam);
                    fbManager.endFrame();

                    if (onProgress) {
                        onProgress.report(count, captureParams.size, "rendering");
                    }

                    output[i] = await saveImage(gl.canvas);
                }

                return output;
            })));
}

export function equirectangularToCubemapBlobs(image: TexImageSource | OffscreenCanvas, isStereo: boolean, size: number, onProgress?: IProgress): Promise<Blob[]> {
    return equirectangularToCubemap(image, isStereo, size, canvasToBlob, onProgress);
}

export async function equirectangularToCubemapCanvases(image: TexImageSource | OffscreenCanvas, isStereo: boolean, size: number, onProgress?: IProgress): Promise<CanvasTypes[]> {
    return equirectangularToCubemap(image, isStereo, size, snapshot, onProgress);
}