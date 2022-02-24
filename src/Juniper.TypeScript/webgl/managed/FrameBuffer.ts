import type { ClearBits, FramebufferTypes } from "../GLEnum";
import { ManagedWebGLResource } from "./ManagedWebGLResource";
import type { RenderBuffer } from "./RenderBuffer";
import type { BaseTextureFramebuffer } from "./TextureFramebuffer";

export class FrameBuffer extends ManagedWebGLResource<WebGLFramebuffer> {
    private _isMainWindow = false;
    private attachments = new Array<number>();

    constructor(
        gl: WebGL2RenderingContext,
        private type: FramebufferTypes,
        buffer?: WebGLFramebuffer) {
        super(gl, buffer === undefined
            ? gl.createFramebuffer()
            : buffer);
        this._isMainWindow = buffer === undefined;
    }

    get isMainWindow() {
        return this._isMainWindow;
    }

    bind(asType?: FramebufferTypes) {
        this.gl.bindFramebuffer(asType || this.type, this.handle);
    }

    invalidate(): void {
        this.gl.invalidateFramebuffer(this.type, this.attachments);
    }

    clear(mask: ClearBits): void {
        this.gl.clear(mask);
    }

    onDisposing(): void {
        if (this._isMainWindow) {
            this.gl.deleteFramebuffer(this.handle);
        }
    }

    attach(...textures: readonly (BaseTextureFramebuffer | RenderBuffer)[]): void {
        this.bind();
        for (const texture of textures) {
            texture.fbBind(this.type);
            this.attachments.push(texture.attachment);
        }
    }

    getStatus(): string {
        const code = this.gl.checkFramebufferStatus(this.type);
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