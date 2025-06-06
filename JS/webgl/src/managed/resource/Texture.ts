import { dispose, isPowerOf2 } from "@juniper-lib/util";
import { getHeight, getWidth } from "@juniper-lib/dom";
import { ManagedWebGLResource } from "./ManagedWebGLResource";

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

export class TextureImage extends BaseTexture {
    constructor(
        gl: WebGL2RenderingContext,
        private readonly image: TexImageSource | OffscreenCanvas,
        pixelType: GLenum = gl.RGBA,
        componentType: GLenum = gl.UNSIGNED_BYTE) {
        super(gl, gl.TEXTURE_2D);

        const width = getWidth(image);
        const height = getHeight(image);
        gl.texImage2D(this.type, 0, pixelType, width, height, 0, pixelType, componentType, image);

        if (isPowerOf2(width) && isPowerOf2(height)) {
            gl.generateMipmap(this.type);
        }
    }

    protected override onDisposing(): void {
        super.onDisposing();
        dispose(this.image);
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

        const width = getWidth(image);
        const height = getHeight(image);

        this.height = height / length;
        this.width = width;
        gl.texImage3D(this.type, 0, pixelType, this.width, this.height, length, 0, pixelType, componentType, image);


        if (isPowerOf2(width) && isPowerOf2(height)) {
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

