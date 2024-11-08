import { debounce, dispose as disposeOld, isDefined, isNullOrUndefined } from "@juniper-lib/util";
import { Task, once } from "@juniper-lib/events";
import { MediaType } from "@juniper-lib/mediatypes";
import { Height, Src, Width } from "./attrs";
import { Canvas, Img } from "./tags";

export type CanvasTypes = HTMLCanvasElement | OffscreenCanvas;
export type CanvasImageTypes = HTMLImageElement | HTMLCanvasElement | OffscreenCanvas | ImageBitmap;
export type Context2D = CanvasRenderingContext2D | OffscreenCanvasRenderingContext2D;
export type GraphicsContext = RenderingContext | OffscreenCanvasRenderingContext2D;

export function hasHTMLCanvas() { return !IS_WORKER && "HTMLCanvasElement" in globalThis; }
export function hasHTMLImage() { return !IS_WORKER && "HTMLImageElement" in globalThis; }
export function hasOffscreenCanvas() { return "OffscreenCanvas" in globalThis; }
export function hasImageBitmap() { return "createImageBitmap" in globalThis; }
export function isHTMLCanvas(obj: any): obj is HTMLCanvasElement { return !IS_WORKER && hasHTMLCanvas() && obj instanceof HTMLCanvasElement; }
export function isHTMLImage(img: any): img is HTMLImageElement { return !IS_WORKER && hasHTMLImage() && img instanceof HTMLImageElement; }
export function isOffscreenCanvas(obj: any): obj is OffscreenCanvas { return hasOffscreenCanvas() && obj instanceof OffscreenCanvas; }
export function isImageBitmap(img: any): img is ImageBitmap { return hasImageBitmap() && img instanceof ImageBitmap; }
export function isImageData(img: any): img is ImageData { return img instanceof ImageData; }

/**
 * Returns true if the given object is either an HTMLCanvasElement or an OffscreenCanvas.
 */
export function isCanvas(obj: any): obj is CanvasTypes {
    return isHTMLCanvas(obj)
        || isOffscreenCanvas(obj);
}

export function isCanvasArray(arr: any): arr is CanvasTypes[] {
    return isDefined(arr)
        && arr.length > 0
        && isCanvas(arr[0]);
}

export function drawImageBitmapToCanvas(canv: CanvasTypes, img: ImageBitmap): void {
    const g = canv.getContext("2d") as Context2D;
    if (isNullOrUndefined(g)) {
        throw new Error("Could not create 2d context for canvas");
    }
    g.drawImage(img, 0, 0);
}

export function drawImageDataToCanvas(canv: CanvasTypes, img: ImageData): void {
    const g = canv.getContext("2d") as Context2D;
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

export function hasOffscreenCanvasRenderingContext2D() { return hasOffscreenCanvas() && testOffscreen2D(); }

export const createUtilityCanvas = /*@__PURE__*/ (function () {
    return hasOffscreenCanvasRenderingContext2D() && createOffscreenCanvas
        || !IS_WORKER && hasHTMLCanvas() && createCanvas
        || null;
})();

export const createUICanvas = /*@__PURE__*/ (function () {
    return !IS_WORKER && hasHTMLCanvas()
        ? createCanvas
        : createUtilityCanvas;
})();

export function hasOffscreenCanvasRenderingContext3D() {
    if (!hasOffscreenCanvas()) {
        return false;
    }

    try {
        const canv = new OffscreenCanvas(1, 1);
        const g = canv.getContext("webgl2");
        return g != null;
    }
    catch (exp) {
        return false;
    }
}

export function createOffscreenCanvas(width: number, height: number): OffscreenCanvas {
    return new OffscreenCanvas(width, height);
}

export function createCanvas(w: number, h: number): HTMLCanvasElement {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }

    return Canvas(Width(w), Height(h));
}

export function createOffscreenCanvasFromImageBitmap(img: ImageBitmap): OffscreenCanvas {
    const canv = createOffscreenCanvas(img.width, img.height);
    drawImageBitmapToCanvas(canv, img);
    return canv;
}

export function createCanvasFromImageBitmap(img: ImageBitmap): HTMLCanvasElement {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }

    const canv = createCanvas(img.width, img.height);
    drawImageBitmapToCanvas(canv, img);
    return canv;
}

export const createUtilityCanvasFromImageBitmap = /*@__PURE__*/ (function () {
    return hasOffscreenCanvasRenderingContext2D() && createOffscreenCanvasFromImageBitmap
        || !IS_WORKER && hasHTMLCanvas() && createCanvasFromImageBitmap
        || null;
})();

export function createOffscreenCanvasFromImageData(img: ImageData): OffscreenCanvas {
    const canv = createOffscreenCanvas(img.width, img.height);
    drawImageDataToCanvas(canv, img);
    return canv;
}

export function createCanvasFromImageData(img: ImageData): HTMLCanvasElement {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }

    const canv = createCanvas(img.width, img.height);
    drawImageDataToCanvas(canv, img);
    return canv;
}

export const createUtilityCanvasFromImageData = /*@__PURE__*/ (function () {
    return hasOffscreenCanvasRenderingContext2D() && createOffscreenCanvasFromImageData
        || !IS_WORKER && hasHTMLCanvas() && createCanvasFromImageData
        || null;
})();

export function createCanvasFromOffscreenCanvas(canv: OffscreenCanvas): HTMLCanvasElement {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }

    const c = createCanvas(canv.width, canv.height);
    drawImageToCanvas(c, canv);
    return c;
}

export function drawImageToCanvas(canv: CanvasTypes, img: CanvasImageTypes): void {
    const g = canv.getContext("2d") as Context2D;
    if (isNullOrUndefined(g)) {
        throw new Error("Could not create 2d context for canvas");
    }
    g.drawImage(img, 0, 0);
}

export function createOffscreenCanvasFromImage(img: HTMLImageElement): OffscreenCanvas {
    const canv = createOffscreenCanvas(img.width, img.height);
    drawImageToCanvas(canv, img);
    return canv;
}

export function createCanvasFromImage(img: HTMLImageElement): HTMLCanvasElement {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }

    const canv = createCanvas(img.width, img.height);
    drawImageToCanvas(canv, img);
    return canv;
}

export const createUtilityCanvasFromImage = /*@__PURE__*/ (function () {
    return hasOffscreenCanvasRenderingContext2D() && createOffscreenCanvasFromImage
        || !IS_WORKER && hasHTMLCanvas() && createCanvasFromImage
        || null;
})();

export async function createImageFromFile(file: string): Promise<HTMLImageElement> {
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
export function setCanvasSize(canv: CanvasTypes, w: number, h: number, superscale = 1) {
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

export function is2DRenderingContext(ctx: GraphicsContext): ctx is Context2D {
    return isDefined((ctx as Context2D).textBaseline);
}

export function setCanvas2DContextSize(ctx: Context2D, w: number, h: number, superscale = 1) {
    const oldImageSmoothingEnabled = ctx.imageSmoothingEnabled,
        oldTextBaseline = ctx.textBaseline,
        oldTextAlign = ctx.textAlign,
        oldFont = ctx.font,
        resized = setCanvasSize(
            ctx.canvas,
            w,
            h,
            superscale);

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
export function setContextSize(ctx: GraphicsContext, w: number, h: number, superscale = 1) {
    if (is2DRenderingContext(ctx)) {
        return setCanvas2DContextSize(ctx, w, h, superscale);
    }
    else {
        return setCanvasSize(
            ctx.canvas,
            w,
            h,
            superscale);
    }
}

/**
 * Resizes a canvas element to match the proportions of the size of the element in the DOM.
 * @param canv
 * @param [superscale=1] - a value by which to scale width and height to achieve supersampling. Defaults to 1.
 * @returns true, if the canvas size changed, false if the given size (with super sampling) resulted in the same size.
 */
export function resizeCanvas(canv: HTMLCanvasElement, superscale = 1) {
    if (IS_WORKER) {
        throw new Error("HTML Canvas is not supported in workers");
    }

    return setCanvasSize(
        canv,
        canv.clientWidth,
        canv.clientHeight,
        superscale);
}

/**
 * Resizes a canvas element of a given rendering context to match the proportions of the size of the element in the DOM.
 * @param ctx
 * @param [superscale=1] - a value by which to scale width and height to achieve supersampling. Defaults to 1.
 * @returns true, if the canvas size changed, false if the given size (with super sampling) resulted in the same size.
 */
export function resizeContext(ctx: CanvasRenderingContext2D, superscale = 1) {
    return setContextSize(
        ctx,
        ctx.canvas.clientWidth,
        ctx.canvas.clientHeight,
        superscale);
}

export function canvasToBlob(canvas: CanvasTypes, type?: string | MediaType, quality?: number): Promise<Blob> {
    if (type instanceof MediaType) {
        type = type.value;
    }

    if (isOffscreenCanvas(canvas)) {
        return canvas.convertToBlob({ type, quality });
    }
    else if (isHTMLCanvas(canvas)) {
        const blobCreated = new Task<Blob>();
        canvas.toBlob(blobCreated.resolve, type, quality);
        return blobCreated;
    }
    else {
        throw new Error("Cannot save image from canvas");
    }
}

export async function snapshot(canvas: CanvasTypes): Promise<CanvasTypes> {
    const copy = createUtilityCanvas(canvas.width, canvas.height);
    drawImageToCanvas(copy, canvas);
    return copy;
}

export function dispose(val: any) {
    if (isCanvas(val)) {
        val.width = val.height = 0;
    }
    else {
        disposeOld(val);
    }
}

export function addCanvasResizer(canvas: HTMLCanvasElement) {
    const resize = debounce(() => {
        canvas.width = canvas.clientWidth * devicePixelRatio;
        canvas.height = canvas.clientHeight * devicePixelRatio;
    });

    const resizer = new ResizeObserver((evts) => {
        for (const evt of evts) {
            if (evt.target == canvas) {
                resize();
                canvas.dispatchEvent(new Event("resize"));
            }
        }
    });

    resize();
    resizer.observe(canvas);
}