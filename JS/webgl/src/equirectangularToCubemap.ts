import { usingAsync } from "@juniper-lib/util";
import { canvasToBlob, CanvasTypes, createUtilityCanvas, snapshot } from "@juniper-lib/dom";
import { CubeMapFaceIndex } from "@juniper-lib/graphics2d";
import { IProgress } from "@juniper-lib/progress";
import { Camera } from "@juniper-lib/three-dee";
import { Context3DWebGL } from "./Context3DWebGL";
import { Geometry } from "./Geometry";
import { invCube } from "./geometry/cubes";
import { TextureImageArray, TextureImageStereo } from "./managed/resource/Texture";
import { Mesh } from "./Mesh";
import { MaterialEquirectangular } from "./programs/MaterialEquirectangular";
import { RenderTargetManager } from "./RenderTargetManager";

interface CaptureOrientation {
    headingDegrees: number;
    pitchDegrees: number;
}

const captureParams = new Map<CubeMapFaceIndex, CaptureOrientation>([
    [CubeMapFaceIndex.Left, { headingDegrees: 90, pitchDegrees: 0 }],
    [CubeMapFaceIndex.Right, { headingDegrees: -90, pitchDegrees: 0 }],
    [CubeMapFaceIndex.Up, { headingDegrees: 180, pitchDegrees: 90 }],
    [CubeMapFaceIndex.Down, { headingDegrees: 180, pitchDegrees: -90 }],
    [CubeMapFaceIndex.Back, { headingDegrees: 180, pitchDegrees: 0 }],
    [CubeMapFaceIndex.Front, { headingDegrees: 0, pitchDegrees: 0 }]
]);

async function equirectangularToCubemap<T>(image: TexImageSource, isStereo: boolean, size: number, saveImage: (canvas: CanvasTypes) => Promise<T>, prog?: IProgress): Promise<T[]> {
    const ctx3d = new Context3DWebGL(createUtilityCanvas(size, size), {
        alpha: true,
        antialias: false,
        powerPreference: "low-power"
    });

    const { gl } = ctx3d;
    const fbManager = new RenderTargetManager(gl);

    const cam = new Camera({
        fov: 90
    });

    ctx3d.addEventListener("resize", (evt) =>
        cam.refreshProjection(evt));

    return await usingAsync(isStereo ? new TextureImageStereo(gl, image) : new TextureImageArray(gl, image, 1), async (texture) =>
        await usingAsync(new MaterialEquirectangular(gl), async (material) =>
            await usingAsync(new Geometry(gl, invCube), async (geom) => {
                const mesh = new Mesh(gl, geom, texture, material);
                const output = new Array<T>(captureParams.size);
                let count = 0;
                for (const [i, directions] of captureParams) {
                    const { headingDegrees, pitchDegrees } = directions;
                    if (prog) {
                        prog.report(count++, captureParams.size, "rendering");
                    }

                    cam.rotateTo(headingDegrees, pitchDegrees);

                    fbManager.beginFrame();
                    mesh.material.use();
                    mesh.render(cam);
                    fbManager.endFrame();

                    if (prog) {
                        prog.report(count, captureParams.size, "rendering");
                    }

                    output[i] = await saveImage(gl.canvas);
                }

                return output;
            })));
}

export function equirectangularToCubemapBlobs(image: TexImageSource, isStereo: boolean, size: number, prog?: IProgress): Promise<Blob[]> {
    return equirectangularToCubemap(image, isStereo, size, canvasToBlob, prog);
}

export async function equirectangularToCubemapCanvases(image: TexImageSource, isStereo: boolean, size: number, prog?: IProgress): Promise<CanvasTypes[]> {
    return equirectangularToCubemap(image, isStereo, size, snapshot, prog);
}