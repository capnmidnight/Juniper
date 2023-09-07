import { Task } from "@juniper-lib/events/Task";
import { once } from "@juniper-lib/events/once";
import { MediaType } from "@juniper-lib/mediatypes";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { dispose as disposeOld } from "@juniper-lib/tslib/using";
import { Height, Src, Width } from "./attrs";
import { Canvas, Img } from "./tags";
export const hasHTMLCanvas = /*@__PURE__*/ !IS_WORKER && "HTMLCanvasElement" in globalThis;
export const hasHTMLImage = /*@__PURE__*/ !IS_WORKER && "HTMLImageElement" in globalThis;
export const disableAdvancedSettings = /*@__PURE__*/ false;
export const hasOffscreenCanvas = /*@__PURE__*/ !disableAdvancedSettings && "OffscreenCanvas" in globalThis;
export const hasImageBitmap = /*@__PURE__*/ !disableAdvancedSettings && "createImageBitmap" in globalThis;
export function isHTMLCanvas(obj) {
    return !IS_WORKER && hasHTMLCanvas && obj instanceof HTMLCanvasElement;
}
export function isHTMLImage(img) {
    return !IS_WORKER && hasHTMLImage && img instanceof HTMLImageElement;
}
export function isOffscreenCanvas(obj) {
    return hasOffscreenCanvas && obj instanceof OffscreenCanvas;
}
export function isImageBitmap(img) {
    return hasImageBitmap && img instanceof ImageBitmap;
}
export function isImageData(img) {
    return img instanceof ImageData;
}
/**
 * Returns true if the given object is either an HTMLCanvasElement or an OffscreenCanvas.
 */
export function isCanvas(obj) {
    return isHTMLCanvas(obj)
        || isOffscreenCanvas(obj);
}
export function isCanvasArray(arr) {
    return isDefined(arr)
        && arr.length > 0
        && isCanvas(arr[0]);
}
export function drawImageBitmapToCanvas(canv, img) {
    const g = canv.getContext("2d");
    if (isNullOrUndefined(g)) {
        throw new Error("Could not create 2d context for canvas");
    }
    g.drawImage(img, 0, 0);
}
export function drawImageDataToCanvas(canv, img) {
    const g = canv.getContext("2d");
    if (isNullOrUndefined(g)) {
        throw new Error("Could not create 2d context for canvas");
    }
    g.putImageData(img, 0, 0);
}
function testOffscreen2D() {
    try {
        const canv = new OffscreenCanvas(1, 1);
        const g = canv.getContext("2d");
        return g != null;
    }
    catch (exp) {
        return false;
    }
}
export const hasOffscreenCanvasRenderingContext2D = /*@__PURE__*/ hasOffscreenCanvas && testOffscreen2D();
export const createUtilityCanvas = /*@__PURE__*/ hasOffscreenCanvasRenderingContext2D && createOffscreenCanvas
    || !IS_WORKER && hasHTMLCanvas && createCanvas
    || null;
export const createUICanvas = /*@__PURE__*/ !IS_WORKER && hasHTMLCanvas
    ? createCanvas
    : createUtilityCanvas;
function testOffscreen3D() {
    try {
        const canv = new OffscreenCanvas(1, 1);
        const g = canv.getContext("webgl2");
        return g != null;
    }
    catch (exp) {
        return false;
    }
}
export const hasOffscreenCanvasRenderingContext3D = /*@__PURE__*/ hasOffscreenCanvas && testOffscreen3D();
export function createOffscreenCanvas(width, height) {
    return new OffscreenCanvas(width, height);
}
export function createCanvas(w, h) {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }
    return Canvas(Width(w), Height(h));
}
export function createOffscreenCanvasFromImageBitmap(img) {
    const canv = createOffscreenCanvas(img.width, img.height);
    drawImageBitmapToCanvas(canv, img);
    return canv;
}
export function createCanvasFromImageBitmap(img) {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }
    const canv = createCanvas(img.width, img.height);
    drawImageBitmapToCanvas(canv, img);
    return canv;
}
export const createUtilityCanvasFromImageBitmap = /*@__PURE__*/ hasOffscreenCanvasRenderingContext2D && createOffscreenCanvasFromImageBitmap
    || !IS_WORKER && hasHTMLCanvas && createCanvasFromImageBitmap
    || null;
export function createOffscreenCanvasFromImageData(img) {
    const canv = createOffscreenCanvas(img.width, img.height);
    drawImageDataToCanvas(canv, img);
    return canv;
}
export function createCanvasFromImageData(img) {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }
    const canv = createCanvas(img.width, img.height);
    drawImageDataToCanvas(canv, img);
    return canv;
}
export const createUtilityCanvasFromImageData = /*@__PURE__*/ hasOffscreenCanvasRenderingContext2D && createOffscreenCanvasFromImageData
    || !IS_WORKER && hasHTMLCanvas && createCanvasFromImageData
    || null;
export function createCanvasFromOffscreenCanvas(canv) {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }
    const c = createCanvas(canv.width, canv.height);
    drawImageToCanvas(c, canv);
    return c;
}
export function drawImageToCanvas(canv, img) {
    const g = canv.getContext("2d");
    if (isNullOrUndefined(g)) {
        throw new Error("Could not create 2d context for canvas");
    }
    g.drawImage(img, 0, 0);
}
export function createOffscreenCanvasFromImage(img) {
    const canv = createOffscreenCanvas(img.width, img.height);
    drawImageToCanvas(canv, img);
    return canv;
}
export function createCanvasFromImage(img) {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }
    const canv = createCanvas(img.width, img.height);
    drawImageToCanvas(canv, img);
    return canv;
}
export const createUtilityCanvasFromImage = /*@__PURE__*/ hasOffscreenCanvasRenderingContext2D && createOffscreenCanvasFromImage
    || !IS_WORKER && hasHTMLCanvas && createCanvasFromImage
    || null;
export async function createImageFromFile(file) {
    if (IS_WORKER) {
        throw new Error("HTML Image is not supported in workers");
    }
    const img = Img(Src(file));
    await once(img, "load", "error");
    return img;
}
/**
 * Resizes a canvas element
 * @param canv
 * @param w - the new width of the canvas
 * @param h - the new height of the canvas
 * @param [superscale=1] - a value by which to scale width and height to achieve supersampling. Defaults to 1.
 * @returns true, if the canvas size changed, false if the given size (with super sampling) resulted in the same size.
 */
export function setCanvasSize(canv, w, h, superscale = 1) {
    w = Math.floor(w * superscale);
    h = Math.floor(h * superscale);
    if (canv.width != w
        || canv.height != h) {
        canv.width = w;
        canv.height = h;
        return true;
    }
    return false;
}
export function is2DRenderingContext(ctx) {
    return isDefined(ctx.textBaseline);
}
export function setCanvas2DContextSize(ctx, w, h, superscale = 1) {
    const oldImageSmoothingEnabled = ctx.imageSmoothingEnabled, oldTextBaseline = ctx.textBaseline, oldTextAlign = ctx.textAlign, oldFont = ctx.font, resized = setCanvasSize(ctx.canvas, w, h, superscale);
    if (resized) {
        ctx.imageSmoothingEnabled = oldImageSmoothingEnabled;
        ctx.textBaseline = oldTextBaseline;
        ctx.textAlign = oldTextAlign;
        ctx.font = oldFont;
    }
    return resized;
}
/**
 * Resizes the canvas element of a given rendering context.
 *
 * Note: the imageSmoothingEnabled, textBaseline, textAlign, and font
 * properties of the context will be restored after the context is resized,
 * as these values are usually reset to their default values when a canvas
 * is resized.
 * @param ctx
 * @param w - the new width of the canvas
 * @param h - the new height of the canvas
 * @param [superscale=1] - a value by which to scale width and height to achieve supersampling. Defaults to 1.
 * @returns true, if the canvas size changed, false if the given size (with super sampling) resulted in the same size.
 */
export function setContextSize(ctx, w, h, superscale = 1) {
    if (is2DRenderingContext(ctx)) {
        return setCanvas2DContextSize(ctx, w, h, superscale);
    }
    else {
        return setCanvasSize(ctx.canvas, w, h, superscale);
    }
}
/**
 * Resizes a canvas element to match the proportions of the size of the element in the DOM.
 * @param canv
 * @param [superscale=1] - a value by which to scale width and height to achieve supersampling. Defaults to 1.
 * @returns true, if the canvas size changed, false if the given size (with super sampling) resulted in the same size.
 */
export function resizeCanvas(canv, superscale = 1) {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }
    return setCanvasSize(canv, canv.clientWidth, canv.clientHeight, superscale);
}
/**
 * Resizes a canvas element of a given rendering context to match the proportions of the size of the element in the DOM.
 * @param ctx
 * @param [superscale=1] - a value by which to scale width and height to achieve supersampling. Defaults to 1.
 * @returns true, if the canvas size changed, false if the given size (with super sampling) resulted in the same size.
 */
export function resizeContext(ctx, superscale = 1) {
    return setContextSize(ctx, ctx.canvas.clientWidth, ctx.canvas.clientHeight, superscale);
}
export function canvasToBlob(canvas, type, quality) {
    if (type instanceof MediaType) {
        type = type.value;
    }
    if (isOffscreenCanvas(canvas)) {
        return canvas.convertToBlob({ type, quality });
    }
    else if (isHTMLCanvas(canvas)) {
        const blobCreated = new Task();
        canvas.toBlob(blobCreated.resolve, type, quality);
        return blobCreated;
    }
    else {
        throw new Error("Cannot save image from canvas");
    }
}
export async function snapshot(canvas) {
    const copy = createUtilityCanvas(canvas.width, canvas.height);
    drawImageToCanvas(copy, canvas);
    return copy;
}
export function dispose(val) {
    if (isCanvas(val)) {
        val.width = val.height = 0;
    }
    else {
        disposeOld(val);
    }
}
//# sourceMappingURL=canvas.js.map