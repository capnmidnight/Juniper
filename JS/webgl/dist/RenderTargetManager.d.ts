import { IDisposable } from "@juniper-lib/util";
import { BaseRenderTarget } from "./managed/resource/FrameBuffer";
export declare class RenderTargetManager implements IDisposable {
    private gl;
    private blitChain;
    private targets;
    private session;
    private webGLLayer;
    private projLayer;
    private readonly views;
    private disposed;
    private mvExtOculus;
    private mvExtOvr;
    binding: XRWebGLBinding;
    constructor(gl: WebGL2RenderingContext);
    get mvMsExt(): OCULUS_multiview;
    get mvExt(): OVR_multiview2;
    dispose(): void;
    private destroyTargets;
    resize(): void;
    setSession(session: null): void;
    setSession(session: XRSession, views: ReadonlyArray<XRView>): void;
    private setLayer;
    get drawTarget(): BaseRenderTarget;
    beginFrame(): void;
    endFrame(): void;
}
//# sourceMappingURL=RenderTargetManager.d.ts.map