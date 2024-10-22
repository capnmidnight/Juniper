import { canvasToBlob } from "@juniper-lib/dom";
import { blobToObjectURL } from "@juniper-lib/util";
import { openWindow } from "./windowing";
export async function canvasView(canvas) {
    if (IS_WORKER) {
        return;
    }
    const blob = await canvasToBlob(canvas);
    const url = blobToObjectURL(blob);
    openWindow(url, 0, 0, canvas.width + 10, canvas.height + 100);
}
//# sourceMappingURL=canvasView.js.map