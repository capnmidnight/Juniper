import { CanvasTypes, isOffscreenCanvas } from "juniper-dom/canvas";
import { isWorker } from "juniper-tslib";
import { Image_Jpeg } from "../mediatypes";
import { openWindow } from "./windowing";


export async function canvasView(canvas: CanvasTypes): Promise<void> {
    if (isWorker) {
        return;
    }

    let blob: Blob;
    if (isOffscreenCanvas(canvas)) {
        blob = await canvas.convertToBlob({
            type: Image_Jpeg.value
        });
    }
    else {
        blob = await new Promise(resolve => canvas.toBlob(resolve, Image_Jpeg.value));
    }

    const url = URL.createObjectURL(blob);

    openWindow(url, 0, 0, canvas.width + 10, canvas.height + 100);
}