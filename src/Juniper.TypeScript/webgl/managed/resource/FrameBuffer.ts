import { CanvasTypes } from "@juniper/dom/canvas";
import { arrayClear, isDefined, isFunction, isNullOrUndefined, singleton } from "@juniper/tslib";
import { ClearBits, FrameAndRenderBuffers, FramebufferType, FramebufferTypes } from "../../GLEnum";
import { FrameBufferTexture, FrameBufferTextureMultiview, FrameBufferTextureMultiviewMultisampled } from "./FrameBufferTexture";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import { RenderBufferMultisampled } from "./RenderBuffer";

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
    attach(fbType: GLenum, attachment: FrameAndRenderBuffers): void;
}

function isIRenderTargetAttachment(obj: IRenderTargetAttachment | FrameAndRenderBuffers): obj is IRenderTargetAttachment {
    return isDefined(obj)
        && isFunction((obj as IRenderTargetAttachment).attach);
}

const lastBound = singleton("Juniper.WebGL.Bindings.FrameBuffer", () => new Map<FramebufferTypes, BaseRenderTarget>());

export abstract class BaseRenderTarget extends ManagedWebGLResource<WebGLFramebuffer> {
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

export abstract class BaseFrameBuffer extends BaseRenderTarget {
    private readonly attachmentsByLocation = new Map<FrameAndRenderBuffers, IRenderTargetAttachment>();
    private readonly attachments = new Array<FrameAndRenderBuffers>();

    constructor(
        gl: WebGL2RenderingContext,
        width: number,
        height: number,
        buffer?: WebGLFramebuffer | CanvasTypes) {
        super(gl, width, height, buffer);
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

    attach(attachment: FrameAndRenderBuffers): this
    attach(attachment: FrameAndRenderBuffers, format: FrameAndRenderBuffers): this
    attach(attachment: FrameAndRenderBuffers, object: IRenderTargetAttachment): this
    attach(attachment: FrameAndRenderBuffers, formatOrObject?: FrameAndRenderBuffers | IRenderTargetAttachment): this {
        this.bind(FramebufferType.DRAW_FRAMEBUFFER);
        let object: IRenderTargetAttachment = null;
        if (isIRenderTargetAttachment(formatOrObject)) {
            object = formatOrObject;
        }
        else {
            const format = formatOrObject || DefaultRenderBufferFormats.get(attachment);
            object = this.createAttachment(format);
        }

        object.attach(FramebufferType.DRAW_FRAMEBUFFER, attachment);
        this.attachmentsByLocation.set(attachment, object);
        this.attachments.push(attachment);
        return this;
    }

    protected createAttachment(format: FrameAndRenderBuffers): IRenderTargetAttachment {
        return new FrameBufferTexture(this.gl, format, this.width, this.height);
    }
}


export class FrameBuffer extends BaseFrameBuffer {
    constructor(gl: WebGL2RenderingContext, width: number, height: number) {
        super(gl, width, height);
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

    protected override createAttachment(format: FrameAndRenderBuffers) {
        return new RenderBufferMultisampled(this.gl, format, this.width, this.height, this.samples);
    }
}

export class FrameBufferCanvas extends BaseRenderTarget {
    constructor(gl: WebGL2RenderingContext) {
        super(gl,
            gl.canvas.width,
            gl.canvas.height,
            gl.canvas);
    }
}

abstract class BaseFrameBufferWebXR extends BaseFrameBuffer {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, public readonly views: XRView[]) {
        super(gl,
            baseLayer.framebufferWidth,
            baseLayer.framebufferHeight,
            baseLayer.framebuffer);
    }

    protected override createAttachment(format: FrameAndRenderBuffers) {
        return super.createAttachment(format);
    }
}

export class FrameBufferWebXR extends BaseFrameBufferWebXR {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[]) {
        super(gl, baseLayer, views);
    }
}

export class FrameBufferWebXRMultisampled extends BaseFrameBufferWebXR {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[], private readonly samples: number) {
        super(gl, baseLayer, views);
    }

    protected override createAttachment(format: FrameAndRenderBuffers) {
        return new RenderBufferMultisampled(this.gl, format, this.width, this.height, this.samples);
    }
}

abstract class BaseFrameBufferWebXRMultiview<ExtType extends OVR_multiview2> extends BaseFrameBufferWebXR {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[], protected readonly ext: ExtType) {
        super(gl, baseLayer, views);
    }

    protected override translateStatus(code: number) {
        if (code === this.ext.FRAMEBUFFER_INCOMPLETE_VIEW_TARGETS_OVR) {
            return "Incomplete view targets";
        }

        return super.translateStatus(code);
    }
}

export class FrameBufferWebXRMultiview extends BaseFrameBufferWebXRMultiview<OVR_multiview2> {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[], ext: OVR_multiview2) {
        super(gl, baseLayer, views, ext);
    }

    protected override createAttachment(format: FrameAndRenderBuffers) {
        return new FrameBufferTextureMultiview(this.gl, this.ext, format, this.width, this.height, this.views)
    }
}

export class FrameBufferWebXRMultiviewMultisampled extends BaseFrameBufferWebXRMultiview<OCULUS_multiview> {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, views: XRView[], private readonly samples: number, ext: OCULUS_multiview) {
        super(gl, baseLayer, views, ext);
    }

    protected override createAttachment(format: FrameAndRenderBuffers) {
        return new FrameBufferTextureMultiviewMultisampled(this.gl, this.ext, format, this.width, this.height, this.samples, this.views);
    }
}