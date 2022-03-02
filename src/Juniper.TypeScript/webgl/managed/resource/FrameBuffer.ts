import { CanvasTypes } from "juniper-dom/canvas";
import { arrayClear, isDefined, isNullOrUndefined, singleton } from "juniper-tslib";
import { ClearBits, FrameAndRenderBuffers, FramebufferType, FramebufferTypes } from "../../GLEnum";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import { RenderBuffer, RenderBufferMultisampled } from "./RenderBuffer";
import { FrameBufferTexture, FrameBufferTextureMultiview, FrameBufferTextureMultiviewMultisampled } from "./FrameBufferTexture";

export const DefaultRenderBufferFormats = new Map([
    [FrameAndRenderBuffers.COLOR_ATTACHMENT0, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT1, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT2, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT3, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT4, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT5, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT6, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT7, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT8, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT9, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT10, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT11, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT12, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT13, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT14, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.COLOR_ATTACHMENT15, FrameAndRenderBuffers.RGBA8],
    [FrameAndRenderBuffers.DEPTH_ATTACHMENT, FrameAndRenderBuffers.DEPTH_COMPONENT16],
    [FrameAndRenderBuffers.DEPTH_STENCIL_ATTACHMENT, FrameAndRenderBuffers.DEPTH24_STENCIL8]
]);

export interface IRenderTargetAttachment extends ManagedWebGLResource<WebGLRenderbuffer | WebGLTexture> {
    attach(fbType: GLenum, attachment: FrameAndRenderBuffers, format: FrameAndRenderBuffers): void;
}

const lastBound = singleton("Juniper.WebGL.Bindings.FrameBuffer", () => new Map<FramebufferTypes, BaseRenderTarget>());

export abstract class BaseRenderTarget<ExtType extends OVR_multiview2 = OVR_multiview2> extends ManagedWebGLResource<WebGLFramebuffer> {
    private readonly isOwned: boolean;
    constructor(
        gl: WebGL2RenderingContext,
        protected readonly ext: ExtType,
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
        if (lastBound.get(asType) !== this) {
            lastBound.set(asType, this);
            this.gl.bindFramebuffer(asType, this.handle);
        }
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
        if (isDefined(this.ext) && code === this.ext.FRAMEBUFFER_INCOMPLETE_VIEW_TARGETS_OVR) {
            return "Incomplete view targets";
        }

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

export abstract class BaseFrameBuffer<ExtType extends OVR_multiview2 = OVR_multiview2> extends BaseRenderTarget<ExtType> {
    private readonly attachmentsByLocation = new Map<FrameAndRenderBuffers, IRenderTargetAttachment>();
    private readonly attachments = new Array<FrameAndRenderBuffers>();

    constructor(
        gl: WebGL2RenderingContext,
        ext: ExtType,
        width: number,
        height: number,
        buffer?: WebGLFramebuffer | CanvasTypes) {
        super(gl, ext, width, height, buffer);
    }

    override onDisposing() {
        for (const obj of this.attachmentsByLocation.values()) {
            obj.dispose();
        }
        this.attachmentsByLocation.clear();
        arrayClear(this.attachments);
        super.onDisposing();
    }

    invalidate(): void {
        if (this.attachments.length > 0) {
            this.gl.invalidateFramebuffer(FramebufferType.DRAW_FRAMEBUFFER, this.attachments);
        }
    }

    attach(attachment: FrameAndRenderBuffers, object?: IRenderTargetAttachment, format?: FrameAndRenderBuffers): this {
        format = format || DefaultRenderBufferFormats.get(attachment);
        this.bind(FramebufferType.DRAW_FRAMEBUFFER);
        object = object || this.createTextureOrRenderBuffer(attachment);
        object.attach(FramebufferType.DRAW_FRAMEBUFFER, attachment, format);
        this.attachmentsByLocation.set(attachment, object);
        this.attachments.push(attachment);
        return this;
    }

    protected createTextureOrRenderBuffer(attachment: FrameAndRenderBuffers): IRenderTargetAttachment {
        if (FrameAndRenderBuffers.COLOR_ATTACHMENT0 <= attachment && attachment <= FrameAndRenderBuffers.COLOR_ATTACHMENT15) {
            return new FrameBufferTexture(this.gl, this.width, this.height);
        }
        else {
            return new RenderBuffer(this.gl, this.width, this.height);
        }
    }
}


export class FrameBuffer extends BaseFrameBuffer {
    constructor(gl: WebGL2RenderingContext, width: number, height: number) {
        super(gl, null, width, height);
    }
}

export class FrameBufferMultisampled extends BaseFrameBuffer {

    private readonly samples: number;

    constructor(gl: WebGL2RenderingContext, width: number, height: number, samples?: number) {
        super(gl, null, width, height);

        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }

        this.samples = samples;
    }

    protected override createTextureOrRenderBuffer(_attachment: FrameAndRenderBuffers) {
        return new RenderBufferMultisampled(this.gl, this.width, this.height, this.samples);
    }
}

export class FrameBufferCanvas extends BaseRenderTarget {
    constructor(gl: WebGL2RenderingContext) {
        super(gl,
            null,
            gl.canvas.width,
            gl.canvas.height,
            gl.canvas);
    }
}

abstract class BaseFrameBufferWebXR<ExtType extends OVR_multiview2 = OVR_multiview2> extends BaseFrameBuffer<ExtType> {
    constructor(gl: WebGL2RenderingContext, ext: ExtType, baseLayer: XRWebGLLayer) {
        super(gl,
            ext,
            baseLayer.framebufferWidth,
            baseLayer.framebufferHeight,
            baseLayer.framebuffer);
    }
}

export class FrameBufferWebXR extends BaseFrameBufferWebXR {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer) {
        super(gl, null, baseLayer);
    }
}

export class FrameBufferWebXRMultisampled extends BaseFrameBufferWebXR {

    private readonly samples: number;

    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, samples?: number) {
        super(gl, null, baseLayer);

        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }

        this.samples = samples;
    }

    protected override createTextureOrRenderBuffer(_attachment: FrameAndRenderBuffers) {
        return new RenderBufferMultisampled(this.gl, this.width, this.height, this.samples);
    }
}

abstract class BaseFrameBufferWebXRMultiview<ExtType extends OVR_multiview2 = OVR_multiview2> extends BaseFrameBufferWebXR<ExtType> {
    constructor(gl: WebGL2RenderingContext, ext: ExtType, baseLayer: XRWebGLLayer, protected readonly numViews: number) {
        super(gl, ext, baseLayer);
    }
}

export class FrameBufferWebXRMultiview extends BaseFrameBufferWebXRMultiview {
    constructor(gl: WebGL2RenderingContext, ext: OVR_multiview2, baseLayer: XRWebGLLayer, numViews: number) {
        super(gl, ext, baseLayer, numViews);
    }

    protected override createTextureOrRenderBuffer(_attachment: FrameAndRenderBuffers) {
        return new FrameBufferTextureMultiview(this.gl, this.ext, this.width, this.height, this.numViews)
    }
}

export class FrameBufferWebXRMultiviewMultisampled extends BaseFrameBufferWebXRMultiview<OCULUS_multiview> {

    private readonly samples: number;

    constructor(gl: WebGL2RenderingContext, ext: OCULUS_multiview, baseLayer: XRWebGLLayer, numViews: number, samples?: number) {
        super(gl, ext, baseLayer, numViews);

        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }

        this.samples = samples;
    }

    protected override createTextureOrRenderBuffer(_attachment: FrameAndRenderBuffers) {
        return new FrameBufferTextureMultiviewMultisampled(this.gl, this.ext, this.width, this.height, this.numViews, this.samples);
    }
}