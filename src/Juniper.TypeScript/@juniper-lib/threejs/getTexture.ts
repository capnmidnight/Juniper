import { CanvasImageTypes, createCanvasFromOffscreenCanvas, isOffscreenCanvas } from "@juniper-lib/dom/canvas";
import { CanvasTexture, CubeTexture } from "three";

export function canvases2CubeTexture(canvs: CanvasImageTypes[]) {
    const texture = new CubeTexture(canvs);
    texture.needsUpdate = true;
    return texture;
}

export function canvases2Texture(img: CanvasImageTypes) {
    if (isOffscreenCanvas(img)) {
        img = createCanvasFromOffscreenCanvas(img);
    }
    const texture = new CanvasTexture(img);
    texture.needsUpdate = true;
    return texture;
}