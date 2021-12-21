import { CanvasTypes, isOffscreenCanvas } from "juniper-dom/canvas";
import { isWorker } from "juniper-tslib";
import { openWindow } from "./windowing";


export async function canvasView(canvas: CanvasTypes): Promise<void> {
    if (isWorker) {
        return;
    }

    let url: string;
    if (isOffscreenCanvas(canvas)) {
        const blob = await canvas.convertToBlob();
        url = URL.createObjectURL(blob);
    }
    else {
        url = canvas.toDataURL();
    }
    openWindow(url, 0, 0, canvas.width + 10, canvas.height + 100);
}
