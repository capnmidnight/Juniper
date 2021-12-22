import { CanvasImageTypes, createCanvasFromOffscreenCanvas, isOffscreenCanvas } from "juniper-dom/canvas";

export function canvases2CubeTexture(canvs: CanvasImageTypes[]) {
    const texture = new THREE.CubeTexture(canvs);
    texture.needsUpdate = true;
    return texture;
}

export function canvases2Texture(img: CanvasImageTypes) {
    if (isOffscreenCanvas(img)) {
        img = createCanvasFromOffscreenCanvas(img);
    }
    const texture = new THREE.CanvasTexture(img);
    texture.needsUpdate = true;
    return texture;
}