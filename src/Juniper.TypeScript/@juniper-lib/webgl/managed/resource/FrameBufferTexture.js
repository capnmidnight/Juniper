import { BaseTexture } from "./Texture";
class BaseFrameBufferTexture extends BaseTexture {
    constructor(gl, type, width, height) {
        super(gl, type);
        this.width = width;
        this.height = height;
    }
}
export class FrameBufferTexture extends BaseFrameBufferTexture {
    constructor(gl, format, width, height) {
        super(gl, gl.TEXTURE_2D, width, height);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_MAG_FILTER, this.gl.NEAREST);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_MIN_FILTER, this.gl.NEAREST);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_WRAP_S, this.gl.CLAMP_TO_EDGE);
        this.gl.texParameteri(this.type, this.gl.TEXTURE_WRAP_T, this.gl.CLAMP_TO_EDGE);
        this.gl.texStorage2D(this.type, 1, format, this.width, this.height);
    }
    attach(fbType, attachment) {
        this.gl.framebufferTexture2D(fbType, attachment, this.gl.TEXTURE_2D, this.handle, 0);
    }
}
class BaseFrameBufferTextureMultiview extends BaseFrameBufferTexture {
    constructor(gl, ext, format, width, height, views) {
        super(gl, gl.TEXTURE_2D_ARRAY, width, height);
        this.ext = ext;
        this.views = views;
        this.bind();
        this.gl.texStorage3D(this.gl.TEXTURE_2D_ARRAY, 1, format, this.width, this.height, this.views.length);
    }
}
export class FrameBufferTextureMultiview extends BaseFrameBufferTextureMultiview {
    constructor(gl, ext, format, width, height, views) {
        super(gl, ext, format, width, height, views);
    }
    attach(fbType, attachment) {
        this.ext.framebufferTextureMultiviewOVR(fbType, attachment, this.handle, 0, 0, this.views.length);
    }
}
export class FrameBufferTextureMultiviewMultisampled extends BaseFrameBufferTextureMultiview {
    constructor(gl, ext, format, width, height, samples, views) {
        super(gl, ext, format, width, height, views);
        this.samples = samples;
    }
    attach(fbType, attachment) {
        this.ext.framebufferTextureMultisampleMultiviewOVR(fbType, attachment, this.handle, 0, this.samples, 0, this.views.length);
    }
}
//# sourceMappingURL=FrameBufferTexture.js.map