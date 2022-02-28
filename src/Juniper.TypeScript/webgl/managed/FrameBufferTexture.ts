import { isNullOrUndefined } from "juniper-tslib";
import { IRenderTargetBuffer } from "../RenderTarget";
import { BaseTexture } from "./BaseTexture";

export abstract class BaseFrameBufferTexture extends BaseTexture implements IRenderTargetBuffer {
    constructor(gl: WebGL2RenderingContext, type: GLenum, private _attachment: GLenum) {
        super(gl, type);
    }

    abstract fbBind(fbType: GLenum): void;

    get attachment() {
        return this._attachment;
    }
}


export abstract class BaseFrameBufferTextureMultiview<MVExtT extends OVR_multiview2 | OCULUS_multiview> extends BaseFrameBufferTexture {
    protected readonly ext: MVExtT;

    constructor(gl: WebGL2RenderingContext, extName: "OVR_multiview2" | "OCULUS_multiview", width: number, height: number, protected readonly numViews: number, attachment: GLenum) {
        super(gl, gl.TEXTURE_2D_ARRAY, attachment);

        this.ext = gl.getExtension(extName);
        const internalFormat = attachment === gl.DEPTH_ATTACHMENT
            ? gl.DEPTH_COMPONENT16
            : gl.RGB8;

        gl.texStorage3D(gl.TEXTURE_2D_ARRAY, 1, internalFormat, width, height, this.numViews);
    }
}



export class FrameBufferTexture extends BaseFrameBufferTexture {
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


export class FrameBufferTextureMultiview extends BaseFrameBufferTextureMultiview<OVR_multiview2> {
    constructor(gl: WebGL2RenderingContext, width: number, height: number, numViews: number, attachment: GLenum) {
        super(gl, "OVR_multiview2", width, height, numViews, attachment);
    }

    override fbBind(fbType: GLenum) {
        this.ext.framebufferTextureMultiviewOVR(fbType, this.attachment, this.handle, 0, 0, this.numViews);
    }
}


export class FrameBufferTextureMultiviewMultisampled extends BaseFrameBufferTextureMultiview<OCULUS_multiview> {
    private readonly samples: number;

    constructor(gl: WebGL2RenderingContext, width: number, height: number, numViews: number, attachment: GLenum, samples?: number) {
        super(gl, "OCULUS_multiview", width, height, numViews, attachment);

        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }

        this.samples = samples;
    }

    override fbBind(fbType: GLenum) {
        this.ext.framebufferTextureMultisampleMultiviewOVR(fbType, this.attachment, this.handle, 0, this.samples, 0, this.numViews);
    }
}