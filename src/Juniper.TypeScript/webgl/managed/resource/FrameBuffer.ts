import { CanvasTypes } from "juniper-dom/canvas";
import { isDefined, isNullOrUndefined } from "juniper-tslib";
import type { ClearBits, FramebufferTypes } from "../../GLEnum";
import { FramebufferType } from "../../GLEnum";
import { BaseTextureFrameBufferMultiview, TextureFrameBufferMultiview, TextureFrameBufferMultiviewMultisampled } from "./Texture";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import { RenderBufferMultisampled } from "./RenderBuffer";

export interface IRenderTargetAttachment extends ManagedWebGLResource<WebGLRenderbuffer | WebGLTexture> {
    attachment: GLenum;
    fbBind(fbType?: GLenum): void;
}

export abstract class BaseFrameBuffer extends ManagedWebGLResource<WebGLFramebuffer> {
    private readonly isOwned: boolean;


    constructor(
        gl: WebGL2RenderingContext,
        public readonly width: number,
        public readonly height: number,
        buffer?: WebGLFramebuffer | CanvasTypes) {
        const isMainWindow = buffer === gl.canvas;
        const isOwned = !isMainWindow
            && isNullOrUndefined(buffer);
        const handle = isMainWindow
            ? null
            : isDefined(buffer)
                ? buffer
                : gl.createFramebuffer();
        super(gl, handle);

        this.isOwned = isOwned;
    }

    bind(asType: FramebufferTypes) {
        this.gl.bindFramebuffer(asType, this.handle);
    }

    clear(mask: ClearBits): void {
        this.gl.clear(mask);
    }

    protected onDisposing(): void {
        if (this.isOwned) {
            this.gl.deleteFramebuffer(this.handle);
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
}

export class BaseFrameBufferWithAttachments extends BaseFrameBuffer {
    private color: IRenderTargetAttachment;
    private depth: IRenderTargetAttachment;
    private attachments = new Array<number>();

    constructor(gl: WebGL2RenderingContext, width: number, height: number, RenderBufferC: (attachment: GLenum) => IRenderTargetAttachment, frameBuffer?: WebGLFramebuffer | CanvasTypes) {
        super(gl, width, height, frameBuffer);

        this.color = RenderBufferC(gl.COLOR_ATTACHMENT0);
        this.depth = RenderBufferC(gl.DEPTH_ATTACHMENT);
        this.attach(this.color, this.depth);
    }

    invalidate(): void {
        this.gl.invalidateFramebuffer(FramebufferType.DRAW_FRAMEBUFFER, this.attachments);
    }

    attach(...textures: readonly IRenderTargetAttachment[]): void {
        this.bind(FramebufferType.DRAW_FRAMEBUFFER);
        for (const texture of textures) {
            texture.fbBind(FramebufferType.DRAW_FRAMEBUFFER);
            this.attachments.push(texture.attachment);
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

export class FrameBufferCanvas extends BaseFrameBuffer {
    constructor(gl: WebGL2RenderingContext) {
        super(gl,
            gl.canvas.width,
            gl.canvas.height,
            gl.canvas);
    }
}

export class FrameBufferWebXR extends BaseFrameBuffer {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer) {
        super(gl,
            baseLayer.framebufferWidth,
            baseLayer.framebufferHeight,
            baseLayer.framebuffer);
    }
}

export class FrameBufferWebXRMultisampled extends BaseFrameBufferWithAttachments {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, samples?: number) {
        super(gl,
            baseLayer.framebufferWidth,
            baseLayer.framebufferHeight,
            (attachment) => new RenderBufferMultisampled(gl, baseLayer.framebufferWidth, baseLayer.framebufferHeight, attachment, samples)
        );
    }
}

class BaseFrameBufferWebXRMultiview<X extends OVR_multiview2 | OCULUS_multiview, T extends BaseTextureFrameBufferMultiview<X>> extends BaseFrameBufferWithAttachments {
    constructor(gl: WebGL2RenderingContext, private readonly ext: X, baseLayer: XRWebGLLayer, RenderBufferC: (attachment: number) => T) {
        super(gl,
            baseLayer.framebufferWidth,
            baseLayer.framebufferHeight,
            RenderBufferC);
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

export class FrameBufferWebXRMultiview extends BaseFrameBufferWebXRMultiview<OVR_multiview2, TextureFrameBufferMultiview> {
    constructor(gl: WebGL2RenderingContext, ext: OVR_multiview2, baseLayer: XRWebGLLayer, numViews: number) {
        super(gl,
            ext,
            baseLayer,
            (attachment) => new TextureFrameBufferMultiview(gl, ext, baseLayer.framebufferWidth, baseLayer.framebufferHeight, numViews, attachment)
        );
    }
}

export class FrameBufferWebXRMultiviewMultisampled extends BaseFrameBufferWebXRMultiview<OCULUS_multiview, TextureFrameBufferMultiviewMultisampled> {
    constructor(gl: WebGL2RenderingContext, ext: OCULUS_multiview, baseLayer: XRWebGLLayer, numViews: number, samples?: number) {
        super(gl,
            ext,
            baseLayer,
            (attachment) => new TextureFrameBufferMultiviewMultisampled(gl, ext, baseLayer.framebufferWidth, baseLayer.framebufferHeight, numViews, attachment, samples)
        );
    }
}