import { canvasToBlob, CanvasTypes } from "@juniper-lib/dom/canvas";
import { makeBlobURL } from "@juniper-lib/dom/makeBlobURL";
import { openWindow } from "./windowing";

declare const IS_WORKER: boolean;

export async function canvasView(canvas: CanvasTypes): Promise<void> {
    if (IS_WORKER) {
        return;
    }

    const blob: Blob = await canvasToBlob(canvas);
    const url = makeBlobURL(blob);

    openWindow(url, 0, 0, canvas.width + 10, canvas.height + 100);
}