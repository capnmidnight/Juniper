import { canvasToBlob, createUtilityCanvas, snapshot } from "@juniper-lib/dom/canvas";
import { CubeMapFaceIndex } from "@juniper-lib/graphics2d/CubeMapFaceIndex";
import { usingAsync } from "@juniper-lib/tslib/using";
import { Camera } from "./Camera";
import { Context3D } from "./Context3D";
import { Geometry } from "./Geometry";
import { invCube } from "./geometry/cubes";
import { TextureImageArray, TextureImageStereo } from "./managed/resource/Texture";
import { Mesh } from "./Mesh";
import { MaterialEquirectangular } from "./programs/MaterialEquirectangular";
import { RenderTargetManager } from "./RenderTargetManager";
const captureParams = new Map([
    [CubeMapFaceIndex.Left, { headingDegrees: 90, pitchDegrees: 0 }],
    [CubeMapFaceIndex.Right, { headingDegrees: -90, pitchDegrees: 0 }],
    [CubeMapFaceIndex.Up, { headingDegrees: 180, pitchDegrees: 90 }],
    [CubeMapFaceIndex.Down, { headingDegrees: 180, pitchDegrees: -90 }],
    [CubeMapFaceIndex.Back, { headingDegrees: 180, pitchDegrees: 0 }],
    [CubeMapFaceIndex.Front, { headingDegrees: 0, pitchDegrees: 0 }]
]);
async function equirectangularToCubemap(image, isStereo, size, saveImage, prog) {
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
    return await usingAsync(isStereo ? new TextureImageStereo(gl, image) : new TextureImageArray(gl, image, 1), async (texture) => await usingAsync(new MaterialEquirectangular(gl), async (material) => await usingAsync(new Geometry(gl, invCube), async (geom) => {
        const mesh = new Mesh(gl, geom, texture, material);
        const output = new Array(captureParams.size);
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
export function equirectangularToCubemapBlobs(image, isStereo, size, prog) {
    return equirectangularToCubemap(image, isStereo, size, canvasToBlob, prog);
}
export async function equirectangularToCubemapCanvases(image, isStereo, size, prog) {
    return equirectangularToCubemap(image, isStereo, size, snapshot, prog);
}
//# sourceMappingURL=equirectangularToCubemap.js.map