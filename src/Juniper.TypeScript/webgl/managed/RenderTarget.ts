import { CanvasTypes } from "juniper-dom/canvas";
import type { IDisposable } from "juniper-tslib";
import { isDefined } from "juniper-tslib";
import type { ClearBits, FramebufferTypes } from "../GLEnum";
import { FramebufferType } from "../GLEnum";
import { BaseFrameBufferTextureMultiview, FrameBufferTextureMultiview, FrameBufferTextureMultiviewMultisampled } from "./FrameBufferTexture";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import { RenderBufferMultisampled } from "./RenderBuffer";

export interface IRenderTargetAttachment extends IDisposable, ManagedWebGLResource<unknown> {
    attachment: GLenum;
    fbBind(fbType?: GLenum): void;
}

export abstract class RenderTarget extends ManagedWebGLResource<WebGLFramebuffer> {
    public readonly isMainWindow: boolean;

    constructor(
        gl: WebGL2RenderingContext,
        public readonly width: number,
        public readonly height: number,
        buffer?: WebGLFramebuffer | CanvasTypes) {
        const isMainWindow = buffer === gl.canvas
        const handle = isMainWindow
            ? null
            : isDefined(buffer)
                ? buffer
                : gl.createFramebuffer();
        super(gl, handle);
        this.isMainWindow = isMainWindow;
    }

    bind(asType?: FramebufferTypes) {
        this.gl.bindFramebuffer(asType || FramebufferType.DRAW_FRAMEBUFFER, this.handle);
    }

    clear(mask: ClearBits): void {
        this.gl.clear(mask);
    }

    protected onDisposing(): void {
        if (!this.isMainWindow) {
            this.gl.deleteFramebuffer(this.handle);
        }
    }
}

class BaseRenderTargetWithAttachments<RenderTargetAttachmentT extends IRenderTargetAttachment> extends RenderTarget {
    private color: RenderTargetAttachmentT;
    private depth: RenderTargetAttachmentT;
    private attachments = new Array<number>();

    constructor(gl: WebGL2RenderingContext, width: number, height: number, RenderBufferC: (attachment: GLenum) => RenderTargetAttachmentT, frameBuffer?: WebGLFramebuffer | CanvasTypes) {
        super(gl, width, height, frameBuffer);

        this.color = RenderBufferC(gl.COLOR_ATTACHMENT0);
        this.depth = RenderBufferC(gl.DEPTH_ATTACHMENT);
        this.attach(this.color, this.depth);
    }

    invalidate(): void {
        if (!this.isMainWindow) {
            this.gl.invalidateFramebuffer(FramebufferType.DRAW_FRAMEBUFFER, this.attachments);
        }
    }

    attach(...textures: readonly IRenderTargetAttachment[]): void {
        this.bind();
        for (const texture of textures) {
            texture.fbBind(FramebufferType.DRAW_FRAMEBUFFER);
            this.attachments.push(texture.attachment);
        }
    }

    getStatus(): string {
        const code = this.gl.checkFramebufferStatus(FramebufferType.DRAW_FRAMEBUFFER);
        return this.translateStatus(code);
    }

    protected translateStatus(code: number): string {
        switch (code) {
            case this.gl.FRAMEBUFFER_COMPLETE:
                return "Complete";
            case this.gl.FRAMEBUFFER_INCOMPLETE_ATTACHMENT:
                return "One of the buffers enabled for rendering is incomplete";
            case this.gl.FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT:
                return "No buffers are attached to the frame buffer and it is not configured for rendering without attachments";
            case this.gl.FRAMEBUFFER_INCOMPLETE_MULTISAMPLE:
                return "At least one of the attachments does not have the same number of samples as the others";
            case this.gl.FRAMEBUFFER_INCOMPLETE_DIMENSIONS:
                return "At least one of the attachments does not have the same number of layers as the others";
            case this.gl.FRAMEBUFFER_UNSUPPORTED:
                return "The combination of internal buffer formats is not supported";
            default:
                return "Unknown";
        }
    }

    protected override onDisposing() {
        super.onDisposing();
        this.color.dispose();
        this.color = null;
        this.depth.dispose();
        this.depth = null;
    }
}

class BaseRenderTargetWebXRMultiview<X extends OVR_multiview2 | OCULUS_multiview, T extends BaseFrameBufferTextureMultiview<X>> extends BaseRenderTargetWithAttachments<FrameBufferTextureMultiview> {
    constructor(gl: WebGL2RenderingContext, private readonly ext: X, layer: XRWebGLLayer, RenderBufferC: (attachment: number) => T) {
        super(gl, layer.framebufferWidth, layer.framebufferHeight, RenderBufferC, layer.framebuffer);
    }

    protected override translateStatus(code: number) {
        switch (code) {
            case this.ext.FRAMEBUFFER_INCOMPLETE_VIEW_TARGETS_OVR:
                return "Incomplete view targets";
            default:
                return super.translateStatus(code);
        }
    }
}

export class RenderTargetCanvas extends RenderTarget {
    constructor(gl: WebGL2RenderingContext) {
        super(gl, gl.canvas.width, gl.canvas.height, gl.canvas);
    }
}

export class RenderTargetWebXR extends RenderTarget {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer) {
        super(gl, baseLayer.framebufferWidth, baseLayer.framebufferHeight, baseLayer.framebuffer);
    }
}

export class RenderTargetWebXRMultisampled extends BaseRenderTargetWithAttachments<RenderBufferMultisampled> {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, samples?: number) {
        super(gl, baseLayer.framebufferWidth, baseLayer.framebufferHeight, (attachment) => new RenderBufferMultisampled(gl, baseLayer.framebufferWidth, baseLayer.framebufferHeight, attachment, samples), baseLayer.framebuffer);
    }
}

export class RenderTargetWebXRMultiview extends BaseRenderTargetWebXRMultiview<OVR_multiview2, FrameBufferTextureMultiview> {
    constructor(gl: WebGL2RenderingContext, ext: OVR_multiview2, layer: XRWebGLLayer, numViews: number) {
        super(gl, ext, layer, (attachment) => new FrameBufferTextureMultiview(gl, ext, layer.framebufferWidth, layer.framebufferHeight, numViews, attachment));
    }
}

export class RenderTargetWebXRMultiviewMultisampled extends BaseRenderTargetWebXRMultiview<OCULUS_multiview, FrameBufferTextureMultiviewMultisampled> {
    constructor(gl: WebGL2RenderingContext, ext: OCULUS_multiview, layer: XRWebGLLayer, numViews: number, samples?: number) {
        super(gl, ext, layer, (attachment) => new FrameBufferTextureMultiviewMultisampled(gl, ext, layer.framebufferWidth, layer.framebufferHeight, numViews, attachment, samples));
    }
}