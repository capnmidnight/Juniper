import { isNullOrUndefined, isPowerOf2 } from "juniper-tslib";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import { IRenderTargetAttachment } from "./FrameBuffer";

export class BaseTexture extends ManagedWebGLResource<WebGLTexture> {
    constructor(gl: WebGL2RenderingContext,
        protected type: GLenum) {
        super(gl, gl.createTexture());

        this.bind();

        gl.texParameteri(this.type, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(this.type, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(this.type, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
        gl.texParameteri(this.type, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
    }

    get isStereo() {
        return false;
    }

    bind() {
        this.gl.bindTexture(this.type, this.handle);
    }

    protected onDisposing(): void {
        this.gl.deleteTexture(this.handle);
    }
}

export abstract class BaseTextureFrameBuffer extends BaseTexture implements IRenderTargetAttachment {
    constructor(gl: WebGL2RenderingContext, type: GLenum, private _attachment: GLenum) {
        super(gl, type);
    }

    abstract fbBind(fbType: GLenum): void;

    get attachment() {
        return this._attachment;
    }
}


export abstract class BaseTextureFrameBufferMultiview<MVExtT extends OVR_multiview2 | OCULUS_multiview> extends BaseTextureFrameBuffer {
    constructor(
        gl: WebGL2RenderingContext,
        protected readonly ext: MVExtT,
        width: number,
        height: number,
        protected readonly numViews: number,
        attachment: GLenum
    ) {
        super(gl, gl.TEXTURE_2D_ARRAY, attachment);

        const internalFormat = attachment === gl.DEPTH_ATTACHMENT
            ? gl.DEPTH_COMPONENT16
            : gl.RGBA8;

        gl.texStorage3D(gl.TEXTURE_2D_ARRAY, 1, internalFormat, width, height, this.numViews);
    }
}

export class TextureImage extends BaseTexture {
    constructor(
        gl: WebGL2RenderingContext,
        private readonly image: TexImageSource | OffscreenCanvas,
        pixelType: GLenum = gl.RGBA,
        componentType: GLenum = gl.UNSIGNED_BYTE) {
        super(gl, gl.TEXTURE_2D);

        gl.texImage2D(this.type, 0, pixelType, image.width, image.height, 0, pixelType, componentType, image);

        if (isPowerOf2(image.width) && isPowerOf2(image.height)) {
            gl.generateMipmap(this.type);
        }
    }

    protected override onDisposing(): void {
        super.onDisposing();
        if (this.image instanceof ImageBitmap) {
            this.image.close();
        }
    }
}

export class TextureImageArray extends BaseTexture {

    readonly width: number;
    readonly height: number;

    constructor(
        gl: WebGL2RenderingContext,
        public readonly image: TexImageSource | OffscreenCanvas,
        protected readonly length: number,
        pixelType: GLenum = gl.RGBA,
        componentType: GLenum = gl.UNSIGNED_BYTE) {
        super(gl, gl.TEXTURE_2D_ARRAY);

        this.height = image.height / length;
        this.width = image.width;
        gl.texImage3D(this.type, 0, pixelType, this.width, this.height, length, 0, pixelType, componentType, image);


        if (isPowerOf2(image.width) && isPowerOf2(image.height)) {
            gl.generateMipmap(this.type);
        }
    }
}

export class TextureImageStereo extends TextureImageArray {
    constructor(
        gl: WebGL2RenderingContext,
        image: TexImageSource | OffscreenCanvas,
        pixelType: GLenum = gl.RGBA,
        componentType: GLenum = gl.UNSIGNED_BYTE) {
        super(gl, image, 2, pixelType, componentType);
    }

    override get isStereo() {
        return true;
    }
}

export class TextureFrameBuffer extends BaseTextureFrameBuffer {
    constructor(gl: WebGL2RenderingContext, width: number, height: number, attachment: GLenum) {
        super(gl, gl.TEXTURE_2D, attachment);

        const internalFormat = attachment === gl.DEPTH_ATTACHMENT
            ? gl.DEPTH_COMPONENT16
            : gl.RGB8;

        const pixelType = attachment === gl.DEPTH_ATTACHMENT
            ? gl.DEPTH_COMPONENT
            : gl.RGB;

        const dataType = attachment === gl.DEPTH_ATTACHMENT
            ? gl.UNSIGNED_SHORT
            : gl.UNSIGNED_BYTE;

        gl.texParameteri(this.type, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
        gl.texParameteri(this.type, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
        gl.texParameteri(this.type, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(this.type, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);

        gl.texImage2D(this.type, 0, internalFormat, width, height, 0, pixelType, dataType, null);
    }

    override fbBind(fbType: GLenum): void {
        this.gl.framebufferTexture2D(fbType, this.attachment, this.gl.TEXTURE_2D, this.handle, 0);
    }
}


export class TextureFrameBufferMultiview extends BaseTextureFrameBufferMultiview<OVR_multiview2> {
    constructor(gl: WebGL2RenderingContext, ext: OVR_multiview2, width: number, height: number, numViews: number, attachment: GLenum) {
        super(gl, ext, width, height, numViews, attachment);
    }

    override fbBind(fbType: GLenum) {
        this.ext.framebufferTextureMultiviewOVR(fbType, this.attachment, this.handle, 0, 0, this.numViews);
    }
}


export class TextureFrameBufferMultiviewMultisampled extends BaseTextureFrameBufferMultiview<OCULUS_multiview> {
    private readonly samples: number;

    constructor(gl: WebGL2RenderingContext, ext: OCULUS_multiview, width: number, height: number, numViews: number, attachment: GLenum, samples?: number) {
        super(gl, ext, width, height, numViews, attachment);

        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }

        this.samples = samples;
    }

    override fbBind(fbType: GLenum) {
        this.ext.framebufferTextureMultisampleMultiviewOVR(fbType, this.attachment, this.handle, 0, this.samples, 0, this.numViews);
    }
}