import { isDefined, isNullOrUndefined } from "juniper-tslib";
import { htmlHeight, htmlWidth } from "./attrs";
import { Canvas } from "./tags";

export type CanvasTypes = HTMLCanvasElement | OffscreenCanvas;
export type CanvasImageTypes = HTMLImageElement | HTMLCanvasElement | OffscreenCanvas | ImageBitmap;
export type Context2D = CanvasRenderingContext2D | OffscreenCanvasRenderingContext2D;
export type GraphicsContext = RenderingContext | OffscreenCanvasRenderingContext2D;

export const hasHTMLCanvas = "HTMLCanvasElement" in globalThis;
export const hasHTMLImage = "HTMLImageElement" in globalThis;
export const disableAdvancedSettings = false;
export const hasOffscreenCanvas = !disableAdvancedSettings && "OffscreenCanvas" in globalThis;
export const hasImageBitmap = !disableAdvancedSettings && "createImageBitmap" in globalThis;

export function isHTMLCanvas(obj: any): obj is HTMLCanvasElement {
    return hasHTMLCanvas && obj instanceof HTMLCanvasElement;
}

export function isHTMLImage(img: any): img is HTMLImageElement {
    return hasHTMLImage && img instanceof HTMLImageElement;
}

export function isOffscreenCanvas(obj: any): obj is OffscreenCanvas {
    return hasOffscreenCanvas && obj instanceof OffscreenCanvas;
}

export function isImageBitmap(img: any): img is ImageBitmap {
    return hasImageBitmap && img instanceof ImageBitmap;
}

/**
 * Returns true if the given object is either an HTMLCanvasElement or an OffscreenCanvas.
 */
export function isCanvas(obj: any): obj is CanvasTypes {
    return isHTMLCanvas(obj)
        || isOffscreenCanvas(obj);
}

export interface WebXRWebGLRenderingContext extends WebGLRenderingContext {
    makeXRCompatible(): Promise<void>;
}

export function isWebXRWebGLRenderingContext(ctx: any): ctx is WebXRWebGLRenderingContext {
    return "makeXRCompatible" in ctx
        && ctx.makeXRCompatible instanceof Function;
}

export function drawImageBitmapToCanvas2D(canv: CanvasTypes, img: ImageBitmap): void {
    const g = canv.getContext("2d");
    if (isNullOrUndefined(g)) {
        throw new Error("Could not create 2d context for canvas");
    }
    g.drawImage(img, 0, 0);
}

export function copyImageBitmapToCanvas(canv: CanvasTypes, img: ImageBitmap): void {
    const g = canv.getContext("bitmaprenderer");
    if (isNullOrUndefined(g)) {
        throw new Error("Could not create bitmaprenderer context for canvas");
    }
    g.transferFromImageBitmap(img);
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

export const hasOffscreenCanvasRenderingContext2D = hasOffscreenCanvas && testOffscreen2D();

export const createUtilityCanvas = hasOffscreenCanvasRenderingContext2D && createOffscreenCanvas
    || hasHTMLCanvas && createCanvas
    || null;

export const createUICanvas = hasHTMLCanvas
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

export const hasOffscreenCanvasRenderingContext3D = hasOffscreenCanvas && testOffscreen3D();

function testBitmapRenderer() {
    if (!hasHTMLCanvas && !hasOffscreenCanvas) {
        return false;
    }

    try {
        const canv = createUtilityCanvas(1, 1);
        const g = canv.getContext("bitmaprenderer");
        return g != null;
    }
    catch (exp) {
        return false;
    }
}

export const hasImageBitmapRenderingContext = hasImageBitmap && testBitmapRenderer();

export const drawImageBitmapToCanvas = hasImageBitmapRenderingContext
    ? copyImageBitmapToCanvas
    : drawImageBitmapToCanvas2D;

export function createOffscreenCanvas(width: number, height: number): OffscreenCanvas {
    return new OffscreenCanvas(width, height);
}

export function createCanvas(w: number, h: number): HTMLCanvasElement {
    return Canvas(htmlWidth(w), htmlHeight(h));
}

export function createOffscreenCanvasFromImageBitmap(img: ImageBitmap): OffscreenCanvas {
    const canv = createOffscreenCanvas(img.width, img.height);
    drawImageBitmapToCanvas(canv, img);
    return canv;
}

export function createCanvasFromImageBitmap(img: ImageBitmap): HTMLCanvasElement {
    const canv = createCanvas(img.width, img.height);
    drawImageBitmapToCanvas2D(canv, img);
    return canv;
}

export const createUtilityCanvasFromImageBitmap = hasOffscreenCanvasRenderingContext2D && createOffscreenCanvasFromImageBitmap
    || hasHTMLCanvas && createCanvasFromImageBitmap
    || null;

export function createCanvasFromOffscreenCanvas(canv: OffscreenCanvas): HTMLCanvasElement {
    const c = createCanvas(canv.width, canv.height);
    drawImageToCanvas(c, canv);
    return c;
}

export function drawImageToCanvas(canv: CanvasTypes, img: CanvasImageTypes): void {
    const g = canv.getContext("2d");
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
    const canv = createCanvas(img.width, img.height);
    drawImageToCanvas(canv, img);
    return canv;
}

export const createUtilityCanvasFromImage = hasOffscreenCanvasRenderingContext2D && createOffscreenCanvasFromImage
    || hasHTMLCanvas && createCanvasFromImage
    || null;

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

export async function canvasToBlob(canvas: CanvasTypes, type?: string, quality?: number): Promise<Blob> {
    if (isOffscreenCanvas(canvas)) {
        return await canvas.convertToBlob({ type, quality });
    }
    else {
        return await new Promise((resolve: (blob: Blob | null) => void) => {
            canvas.toBlob(resolve, type, quality);
        });
    }
}

export async function snapshot(canvas: CanvasTypes): Promise<CanvasTypes> {
    const copy = createUtilityCanvas(canvas.width, canvas.height);
    drawImageToCanvas(copy, canvas);
    return copy;
}