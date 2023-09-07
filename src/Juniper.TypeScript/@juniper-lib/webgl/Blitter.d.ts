import { FrameAndRenderBuffers } from "./GLEnum";
import { BaseRenderTarget } from "./managed/resource/FrameBuffer";
export declare class Blitter {
    private gl;
    private readTarget;
    private drawTarget;
    private readonly sourceX0;
    private readonly sourceX1;
    private readonly sourceY0;
    private readonly sourceY1;
    private readonly destX0;
    private readonly destX1;
    private readonly destY0;
    private readonly destY1;
    private readonly drawBuffers;
    constructor(gl: WebGL2RenderingContext, readTarget: BaseRenderTarget, drawTarget: BaseRenderTarget, drawBuffer: FrameAndRenderBuffers);
    blit(): void;
}
//# sourceMappingURL=Blitter.d.ts.map