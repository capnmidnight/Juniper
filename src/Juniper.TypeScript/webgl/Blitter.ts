import { FramebufferType } from "./GLEnum";
import { BaseFrameBuffer, FrameBufferWebXR } from "./managed/resource/FrameBuffer";

export class Blitter {

    private sourceX0: number;
    private sourceX1: number;
    private sourceY0: number;
    private sourceY1: number;
    private destX0: number;
    private destX1: number;
    private destY0: number;
    private destY1: number;

    constructor(
        private gl: WebGL2RenderingContext,
        private readTarget: BaseFrameBuffer,
        private drawTarget: BaseFrameBuffer,
        private drawBuffers: GLenum[]) {

        const sourceWidth = readTarget instanceof FrameBufferWebXR
            ? readTarget.width / 2
            : readTarget.width;
        const sourceHeight = readTarget.height;
        const sourceXMid = sourceWidth / 2;
        const sourceYMid = sourceHeight / 2;

        const destWidth = drawTarget.width;
        const destHeight = drawTarget.height;
        const destXMid = destWidth / 2;
        const destYMid = destHeight / 2;

        this.sourceX0 = Math.max(0, sourceXMid - destXMid);
        this.sourceX1 = Math.min(sourceWidth, sourceXMid + destXMid);
        this.sourceY0 = Math.max(0, sourceYMid - destYMid);
        this.sourceY1 = Math.min(sourceHeight, sourceYMid + destYMid);

        this.destX0 = Math.max(0, destXMid - sourceXMid);
        this.destX1 = Math.min(destWidth, destXMid + sourceXMid);
        this.destY0 = Math.max(0, destYMid - sourceYMid);
        this.destY1 = Math.min(destHeight, destYMid + sourceYMid);
    }

    blit(): void {
        this.drawTarget.bind(FramebufferType.DRAW_FRAMEBUFFER);
        this.readTarget.bind(FramebufferType.READ_FRAMEBUFFER);
        this.gl.drawBuffers(this.drawBuffers);
        this.gl.readBuffer(this.gl.COLOR_ATTACHMENT0);
        this.gl.blitFramebuffer(
            this.sourceX0, this.sourceY0, this.sourceX1, this.sourceY1,
            this.destX0, this.destY0, this.destX1, this.destY1,
            this.gl.COLOR_BUFFER_BIT,
            this.gl.NEAREST);
    }
}