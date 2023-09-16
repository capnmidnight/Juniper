import { ManagedWebGLResource } from "./ManagedWebGLResource";
export declare class BaseTexture extends ManagedWebGLResource<WebGLTexture> {
    protected type: GLenum;
    constructor(gl: WebGL2RenderingContext, type: GLenum);
    get isStereo(): boolean;
    bind(): void;
    protected onDisposing(): void;
}
export declare class TextureImage extends BaseTexture {
    private readonly image;
    constructor(gl: WebGL2RenderingContext, image: TexImageSource | OffscreenCanvas, pixelType?: GLenum, componentType?: GLenum);
    protected onDisposing(): void;
}
export declare class TextureImageArray extends BaseTexture {
    readonly image: TexImageSource | OffscreenCanvas;
    protected readonly length: number;
    readonly width: number;
    readonly height: number;
    constructor(gl: WebGL2RenderingContext, image: TexImageSource | OffscreenCanvas, length: number, pixelType?: GLenum, componentType?: GLenum);
}
export declare class TextureImageStereo extends TextureImageArray {
    constructor(gl: WebGL2RenderingContext, image: TexImageSource | OffscreenCanvas, pixelType?: GLenum, componentType?: GLenum);
    get isStereo(): boolean;
}
//# sourceMappingURL=Texture.d.ts.map