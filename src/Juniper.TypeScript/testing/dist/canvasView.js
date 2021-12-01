import { isWorker } from "juniper-tslib";
import { isOffscreenCanvas } from "juniper-dom";
import { openWindow } from "./windowing";
export async function canvasView(canvas) {
    if (isWorker) {
        return;
    }
    let url;
    if (isOffscreenCanvas(canvas)) {
        const blob = await canvas.convertToBlob();
        url = URL.createObjectURL(blob);
    }
    else {
        url = canvas.toDataURL();
    }
    openWindow(url, 0, 0, canvas.width + 10, canvas.height + 100);
}
