import { BaseTexture } from "./BaseTexture";

export class BaseTextureFramebuffer extends BaseTexture {
    constructor(gl: WebGL2RenderingContext, type: GLenum, private _attachment: GLenum, private _fbBind: (fbType: GLenum, handle: WebGLTexture) => void) {
        super(gl, type);
    }

    fbBind(fbType: GLenum) {
        this._fbBind(fbType, this.handle);
    }

    get attachment() {
        return this._attachment;
    }
}

export class TextureFramebuffer extends BaseTextureFramebuffer {
    constructor(gl: WebGL2RenderingContext, width: number, height: number, attachment: GLenum) {
        super(gl, gl.TEXTURE_2D, attachment, (fbType: GLenum, handle: WebGLTexture) =>
            gl.framebufferTexture2D(fbType, attachment, gl.TEXTURE_2D, handle, 0));

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
}