import { MediaType } from "@juniper-lib/mediatypes/dist";
export type CanvasTypes = HTMLCanvasElement | OffscreenCanvas;
export type CanvasImageTypes = HTMLImageElement | HTMLCanvasElement | OffscreenCanvas | ImageBitmap;
export type Context2D = CanvasRenderingContext2D | OffscreenCanvasRenderingContext2D;
export type GraphicsContext = RenderingContext | OffscreenCanvasRenderingContext2D;
export declare const hasHTMLCanvas: boolean;
export declare const hasHTMLImage: boolean;
export declare const disableAdvancedSettings = false;
export declare const hasOffscreenCanvas: boolean;
export declare const hasImageBitmap: boolean;
export declare function isHTMLCanvas(obj: any): obj is HTMLCanvasElement;
export declare function isHTMLImage(img: any): img is HTMLImageElement;
export declare function isOffscreenCanvas(obj: any): obj is OffscreenCanvas;
export declare function isImageBitmap(img: any): img is ImageBitmap;
export declare function isImageData(img: any): img is ImageData;
/**
 * Returns true if the given object is either an HTMLCanvasElement or an OffscreenCanvas.
 */
export declare function isCanvas(obj: any): obj is CanvasTypes;
export declare function isCanvasArray(arr: any): arr is CanvasTypes[];
export declare function drawImageBitmapToCanvas(canv: CanvasTypes, img: ImageBitmap): void;
export declare function drawImageDataToCanvas(canv: CanvasTypes, img: ImageData): void;
export declare const hasOffscreenCanvasRenderingContext2D: boolean;
export declare const createUtilityCanvas: typeof createOffscreenCanvas | typeof createCanvas;
export declare const createUICanvas: typeof createOffscreenCanvas | typeof createCanvas;
export declare const hasOffscreenCanvasRenderingContext3D: boolean;
export declare function createOffscreenCanvas(width: number, height: number): OffscreenCanvas;
export declare function createCanvas(w: number, h: number): HTMLCanvasElement;
export declare function createOffscreenCanvasFromImageBitmap(img: ImageBitmap): OffscreenCanvas;
export declare function createCanvasFromImageBitmap(img: ImageBitmap): HTMLCanvasElement;
export declare const createUtilityCanvasFromImageBitmap: typeof createOffscreenCanvasFromImageBitmap | typeof createCanvasFromImageBitmap;
export declare function createOffscreenCanvasFromImageData(img: ImageData): OffscreenCanvas;
export declare function createCanvasFromImageData(img: ImageData): HTMLCanvasElement;
export declare const createUtilityCanvasFromImageData: typeof createOffscreenCanvasFromImageData | typeof createCanvasFromImageData;
export declare function createCanvasFromOffscreenCanvas(canv: OffscreenCanvas): HTMLCanvasElement;
export declare function drawImageToCanvas(canv: CanvasTypes, img: CanvasImageTypes): void;
export declare function createOffscreenCanvasFromImage(img: HTMLImageElement): OffscreenCanvas;
export declare function createCanvasFromImage(img: HTMLImageElement): HTMLCanvasElement;
export declare const createUtilityCanvasFromImage: typeof createOffscreenCanvasFromImage | typeof createCanvasFromImage;
export declare function createImageFromFile(file: string): Promise<HTMLImageElement>;
/**
 * Resizes a canvas element
 * @param canv
 * @param w - the new width of the canvas
 * @param h - the new height of the canvas
 * @param [superscale=1] - a value by which to scale width and height to achieve supersampling. Defaults to 1.
 * @returns true, if the canvas size changed, false if the given size (with super sampling) resulted in the same size.
 */
export declare function setCanvasSize(canv: CanvasTypes, w: number, h: number, superscale?: number): boolean;
export declare function is2DRenderingContext(ctx: GraphicsContext): ctx is Context2D;
export declare function setCanvas2DContextSize(ctx: Context2D, w: number, h: number, superscale?: number): boolean;
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
export declare function setContextSize(ctx: GraphicsContext, w: number, h: number, superscale?: number): boolean;
/**
 * Resizes a canvas element to match the proportions of the size of the element in the DOM.
 * @param canv
 * @param [superscale=1] - a value by which to scale width and height to achieve supersampling. Defaults to 1.
 * @returns true, if the canvas size changed, false if the given size (with super sampling) resulted in the same size.
 */
export declare function resizeCanvas(canv: HTMLCanvasElement, superscale?: number): boolean;
/**
 * Resizes a canvas element of a given rendering context to match the proportions of the size of the element in the DOM.
 * @param ctx
 * @param [superscale=1] - a value by which to scale width and height to achieve supersampling. Defaults to 1.
 * @returns true, if the canvas size changed, false if the given size (with super sampling) resulted in the same size.
 */
export declare function resizeContext(ctx: CanvasRenderingContext2D, superscale?: number): boolean;
export declare function canvasToBlob(canvas: CanvasTypes, type?: string | MediaType, quality?: number): Promise<Blob>;
export declare function snapshot(canvas: CanvasTypes): Promise<CanvasTypes>;
export declare function dispose(val: any): void;
//# sourceMappingURL=canvas.d.ts.map