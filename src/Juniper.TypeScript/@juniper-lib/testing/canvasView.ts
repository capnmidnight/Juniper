import { canvasToBlob, CanvasTypes } from "@juniper-lib/dom/canvas";
import { makeBlobURL } from "@juniper-lib/dom/makeBlobURL";
import { isWorker } from "@juniper-lib/tslib";
import { openWindow } from "./windowing";

export async function canvasView(canvas: CanvasTypes): Promise<void> {
    if (isWorker) {
        return;
    }

    const blob: Blob = await canvasToBlob(canvas);
    const url = makeBlobURL(blob);

    openWindow(url, 0, 0, canvas.width + 10, canvas.height + 100);
}