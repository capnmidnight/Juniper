import type { IDisposable } from "juniper-tslib";
import type { FramebufferTypes } from "./GLEnum";
import { FramebufferType } from "./GLEnum";
import { FrameBuffer } from "./managed/FrameBuffer";
import { FrameBufferTexture, FrameBufferTextureMultiview, FrameBufferTextureMultiviewMultisampled } from "./managed/FrameBufferTexture";
import { ManagedWebGLResource } from "./managed/ManagedWebGLResource";
import { RenderBuffer, RenderBufferMultisampled } from "./managed/RenderBuffer";


export interface IRenderTargetBuffer extends IDisposable, ManagedWebGLResource<unknown> {
    attachment: GLenum;
    fbBind(fbType?: GLenum): void;
}


export abstract class BaseRenderTarget extends FrameBuffer {
    constructor(gl: WebGL2RenderingContext, type: FramebufferTypes, public readonly width: number, public readonly height: number, buffer?: WebGLFramebuffer) {
        super(gl, type, buffer);
    }
}

export class BaseRenderTargetBuffered<RenderBufferT extends IRenderTargetBuffer> extends BaseRenderTarget {
    private color: RenderBufferT;
    private depth: RenderBufferT;

    constructor(gl: WebGL2RenderingContext, width: number, height: number, RenderBufferC: (attachment: GLenum) => RenderBufferT, frameBuffer?: WebGLFramebuffer) {
        super(gl, FramebufferType.DRAW_FRAMEBUFFER, width, height, frameBuffer);

        this.color = RenderBufferC(gl.COLOR_ATTACHMENT0);
        this.depth = RenderBufferC(gl.DEPTH_ATTACHMENT);
        this.attach(this.color, this.depth);
    }

    protected override onDisposing() {
        super.onDisposing();
        this.color.dispose();
        this.color = null;
        this.depth.dispose();
        this.depth = null;
    }
}

export class RenderTargetCanvas extends BaseRenderTarget {
    constructor(gl: WebGL2RenderingContext) {
        super(gl, FramebufferType.DRAW_FRAMEBUFFER, gl.canvas.width, gl.canvas.height);
    }
}

export class RenderTargetXRWebGLLayer extends BaseRenderTarget {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer) {
        super(gl, FramebufferType.DRAW_FRAMEBUFFER, baseLayer.framebufferWidth, baseLayer.framebufferHeight, baseLayer.framebuffer);
    }
}

export class RenderTargetRenderBuffer extends BaseRenderTargetBuffered<RenderBuffer> {
    constructor(gl: WebGL2RenderingContext, width: number, height: number, frameBuffer?: WebGLFramebuffer) {
        super(gl, width, height, (attachment) => new RenderBuffer(gl, width, height, attachment), frameBuffer);
    }
}

export class RenderTargetRenderBufferMultisampled extends BaseRenderTargetBuffered<RenderBufferMultisampled> {
    constructor(gl: WebGL2RenderingContext, width: number, height: number, samples?: number, frameBuffer?: WebGLFramebuffer) {
        super(gl, width, height, (attachment) => new RenderBufferMultisampled(gl, width, height, attachment, samples), frameBuffer);
    }
}

export class RenderTargetFrameBufferTexture extends BaseRenderTargetBuffered<FrameBufferTexture> {
    constructor(gl: WebGL2RenderingContext, width: number, height: number, frameBuffer?: WebGLFramebuffer) {
        super(gl, width, height, (attachment) => new FrameBufferTexture(gl, width, height, attachment), frameBuffer);
    }
}

export class RenderTargetFrameBufferTextureMultiview extends BaseRenderTargetBuffered<FrameBufferTextureMultiview> {
    constructor(gl: WebGL2RenderingContext, layer: XRWebGLLayer, numViews: number) {
        super(gl, layer.framebufferWidth, layer.framebufferHeight, (attachment) => new FrameBufferTextureMultiview(gl, layer.framebufferWidth, layer.framebufferHeight, numViews, attachment), layer.framebuffer);
    }
}

export class RenderTargeFrameBufferTextureMultiviewMultisampled extends BaseRenderTargetBuffered<FrameBufferTextureMultiviewMultisampled> {
    constructor(gl: WebGL2RenderingContext, layer: XRWebGLLayer, numViews: number, samples?: number) {
        super(gl, layer.framebufferWidth, layer.framebufferHeight, (attachment) => new FrameBufferTextureMultiviewMultisampled(gl, layer.framebufferWidth, layer.framebufferHeight, numViews, attachment, samples), layer.framebuffer);
    }
}