import type { IDisposable } from "juniper-tslib";
import type { ClearBits } from "./GLEnum";
import { FramebufferType } from "./GLEnum";
import { FrameBuffer } from "./managed/FrameBuffer";
import { RenderBuffer, RenderBufferMultisampled } from "./managed/RenderBuffer";


export abstract class BaseRenderTarget implements IDisposable {
    private disposed;

    constructor(private buffer: FrameBuffer,
        private _width: number,
        private _height: number,
        public readonly numViews: number) {
        this.disposed = false;
    }

    get width() {
        return this._width;
    }

    get height() {
        return this._height;
    }

    bind(type?: FramebufferType) {
        this.buffer.bind(type);
    }

    clear(mask: ClearBits) {
        this.buffer.clear(mask);
    }

    invalidate() {
        this.buffer.invalidate();
    }

    dispose(): void {
        if (!this.disposed) {
            this.onDisposing();
            this.buffer.dispose();
            this.buffer = null;
            this._width = -1;
            this._height = -1;
            this.disposed = true;
        }
    }

    protected onDisposing() { }
}

export class RenderTargetCanvas extends BaseRenderTarget {
    constructor(gl: WebGL2RenderingContext) {
        super(new FrameBuffer(gl, FramebufferType.DRAW_FRAMEBUFFER, null), gl.canvas.width, gl.canvas.height, 1);
    }
}

export class RenderTargetXRWebGLLayer extends BaseRenderTarget {
    constructor(gl: WebGL2RenderingContext, baseLayer: XRWebGLLayer, numViews: number) {
        super(new FrameBuffer(gl, FramebufferType.DRAW_FRAMEBUFFER, baseLayer.framebuffer), baseLayer.framebufferWidth, baseLayer.framebufferHeight, numViews);
    }
}

export class RenderTargetRenderBuffer extends BaseRenderTarget {
    private color: RenderBuffer;
    private depth: RenderBuffer;

    constructor(gl: WebGL2RenderingContext, width: number, height: number, frameBuffer?: WebGLFramebuffer) {
        const buffer = new FrameBuffer(gl, FramebufferType.DRAW_FRAMEBUFFER, frameBuffer);
        super(buffer, width, height, 1);
        this.color = new RenderBuffer(gl, width, height, gl.COLOR_ATTACHMENT0);
        this.depth = new RenderBuffer(gl, width, height, gl.DEPTH_ATTACHMENT);
        buffer.attach(this.color, this.depth);
    }

    protected override onDisposing() {
        super.onDisposing();
        this.color.dispose();
        this.color = null;
        this.depth.dispose();
        this.depth = null;
    }
}

export class RenderTargetRenderBufferMultisampled extends BaseRenderTarget {
    private color: RenderBuffer;
    private depth: RenderBuffer;

    constructor(gl: WebGL2RenderingContext, width: number, height: number, numViews: number, frameBuffer?: WebGLFramebuffer) {
        const buffer = new FrameBuffer(gl, FramebufferType.DRAW_FRAMEBUFFER, frameBuffer);
        super(buffer, width, height, numViews);

        this.color = new RenderBufferMultisampled(gl, width, height, gl.COLOR_ATTACHMENT0);
        this.depth = new RenderBufferMultisampled(gl, width, height, gl.DEPTH_ATTACHMENT);
        buffer.attach(this.color, this.depth);
    }

    protected override onDisposing() {
        super.onDisposing();
        this.color.dispose();
        this.color = null;
        this.depth.dispose();
        this.depth = null;
    }
}