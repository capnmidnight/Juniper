import { dispose } from "@juniper-lib/dom/dist/canvas";
import { arrayClear } from "@juniper-lib/collections/dist/arrays";
import { singleton } from "@juniper-lib/tslib/dist/singleton";
import { isDefined, isFunction, isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
import { FrameAndRenderBuffers, FramebufferType } from "../../GLEnum";
import { FrameBufferTexture, FrameBufferTextureMultiview, FrameBufferTextureMultiviewMultisampled } from "./FrameBufferTexture";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import { RenderBufferMultisampled } from "./RenderBuffer";
export const DefaultRenderBufferFormats = /*@__PURE__*/ new Map([
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
function isIRenderTargetAttachment(obj) {
    return isDefined(obj)
        && isFunction(obj.attach);
}
const lastBound = singleton("Juniper.WebGL.Bindings.FrameBuffer", () => new Map());
export class BaseRenderTarget extends ManagedWebGLResource {
    constructor(gl, width, height, buffer) {
        const isMainWindow = buffer === gl.canvas;
        const isOwned = !isMainWindow
            && isNullOrUndefined(buffer);
        const handle = isMainWindow
            ? null
            : isDefined(buffer)
                ? buffer
                : gl.createFramebuffer();
        super(gl, handle);
        this.width = width;
        this.height = height;
        this.isOwned = isOwned;
    }
    bind(asType) {
        if (lastBound.get(asType) !== this) {
            lastBound.set(asType, this);
            this.gl.bindFramebuffer(asType, this.handle);
        }
    }
    clear(mask) {
        this.gl.clear(mask);
    }
    onDisposing() {
        if (this.isOwned) {
            this.gl.deleteFramebuffer(this.handle);
        }
    }
    getStatus() {
        const code = this.gl.checkFramebufferStatus(FramebufferType.DRAW_FRAMEBUFFER);
        return this.translateStatus(code);
    }
    translateStatus(code) {
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
export class BaseFrameBuffer extends BaseRenderTarget {
    constructor(gl, width, height, buffer) {
        super(gl, width, height, buffer);
        this.attachmentsByLocation = new Map();
        this.attachments = new Array();
    }
    onDisposing() {
        for (const obj of this.attachmentsByLocation.values()) {
            dispose(obj);
        }
        this.attachmentsByLocation.clear();
        arrayClear(this.attachments);
        super.onDisposing();
    }
    invalidate() {
        if (this.attachments.length > 0) {
            this.gl.invalidateFramebuffer(FramebufferType.DRAW_FRAMEBUFFER, this.attachments);
        }
    }
    attach(attachment, formatOrObject) {
        this.bind(FramebufferType.DRAW_FRAMEBUFFER);
        let object = null;
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
    createAttachment(format) {
        return new FrameBufferTexture(this.gl, format, this.width, this.height);
    }
}
export class FrameBuffer extends BaseFrameBuffer {
    constructor(gl, width, height) {
        super(gl, width, height);
    }
}
export class FrameBufferMultisampled extends BaseFrameBuffer {
    constructor(gl, width, height, samples) {
        super(gl, null, width, height);
        if (isNullOrUndefined(samples)) {
            samples = gl.getParameter(gl.MAX_SAMPLES);
        }
        this.samples = samples;
    }
    createAttachment(format) {
        return new RenderBufferMultisampled(this.gl, format, this.width, this.height, this.samples);
    }
}
export class FrameBufferCanvas extends BaseRenderTarget {
    constructor(gl) {
        super(gl, gl.canvas.width, gl.canvas.height, gl.canvas);
    }
}
class BaseFrameBufferWebXR extends BaseFrameBuffer {
    constructor(gl, baseLayer, views) {
        super(gl, baseLayer.framebufferWidth, baseLayer.framebufferHeight, baseLayer.framebuffer);
        this.views = views;
    }
    createAttachment(format) {
        return super.createAttachment(format);
    }
}
export class FrameBufferWebXR extends BaseFrameBufferWebXR {
    constructor(gl, baseLayer, views) {
        super(gl, baseLayer, views);
    }
}
export class FrameBufferWebXRMultisampled extends BaseFrameBufferWebXR {
    constructor(gl, baseLayer, views, samples) {
        super(gl, baseLayer, views);
        this.samples = samples;
    }
    createAttachment(format) {
        return new RenderBufferMultisampled(this.gl, format, this.width, this.height, this.samples);
    }
}
class BaseFrameBufferWebXRMultiview extends BaseFrameBufferWebXR {
    constructor(gl, baseLayer, views, ext) {
        super(gl, baseLayer, views);
        this.ext = ext;
    }
    translateStatus(code) {
        if (code === this.ext.FRAMEBUFFER_INCOMPLETE_VIEW_TARGETS_OVR) {
            return "Incomplete view targets";
        }
        return super.translateStatus(code);
    }
}
export class FrameBufferWebXRMultiview extends BaseFrameBufferWebXRMultiview {
    constructor(gl, baseLayer, views, ext) {
        super(gl, baseLayer, views, ext);
    }
    createAttachment(format) {
        return new FrameBufferTextureMultiview(this.gl, this.ext, format, this.width, this.height, this.views);
    }
}
export class FrameBufferWebXRMultiviewMultisampled extends BaseFrameBufferWebXRMultiview {
    constructor(gl, baseLayer, views, samples, ext) {
        super(gl, baseLayer, views, ext);
        this.samples = samples;
    }
    createAttachment(format) {
        return new FrameBufferTextureMultiviewMultisampled(this.gl, this.ext, format, this.width, this.height, this.samples, this.views);
    }
}
//# sourceMappingURL=FrameBuffer.js.map