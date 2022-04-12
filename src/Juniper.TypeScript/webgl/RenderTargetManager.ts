import { arrayClear, arrayReplace, arrayScan, IDisposable, isArray, isDefined, isFirefox, isMobileVR, isNullOrUndefined } from "juniper-tslib";
import { Blitter } from "./Blitter";
import { ClearBits, FrameAndRenderBuffers, FramebufferType } from "./GLEnum";
import { BaseFrameBuffer, BaseRenderTarget, FrameBufferCanvas, FrameBufferWebXR, FrameBufferWebXRMultisampled, FrameBufferWebXRMultiview, FrameBufferWebXRMultiviewMultisampled } from "./managed/resource/FrameBuffer";

export class RenderTargetManager implements IDisposable {
    private blitChain = new Array<Blitter>();
    private targets = new Array<BaseRenderTarget>();
    private session: XRSession = undefined;
    private webGLLayer: XRWebGLLayer = undefined;
    private projLayer: XRProjectionLayer = undefined;
    private readonly views = new Array<XRView>();
    private disposed: boolean = false;
    private mvExtOculus: OCULUS_multiview = null;
    private mvExtOvr: OVR_multiview2 = null;
    binding: XRWebGLBinding = null;

    constructor(private gl: WebGL2RenderingContext) {
        this.mvExtOculus = this.gl.getExtension("OCULUS_multiview");
        if (!this.mvExtOculus) {
            this.mvExtOvr = this.gl.getExtension("OVR_multiview2");
        }

        this.setLayer(null);
    }

    get mvMsExt() {
        return this.mvExtOculus;
    }

    get mvExt() {
        return this.mvExtOculus || this.mvExtOvr;
    }

    dispose() {
        if (!this.disposed) {
            this.destroyTargets();
            this.disposed = true;
        }
    }

    private destroyTargets() {
        arrayClear(this.blitChain);

        for (const target of this.targets) {
            target.dispose();
        }

        arrayClear(this.targets);
    }

    resize() {
        const session = this.session;
        this.session = null;
        this.setSession(session, this.views);
    }

    setSession(session: null): void;
    setSession(session: XRSession, views: XRView[]): void;
    setSession(session: XRSession | null, views?: XRView[]): void {
        if (isNullOrUndefined(session)) {
            views = [];
        }

        console.trace(session, views);

        const renderState = session && session.renderState;
        const webGLLayer = renderState && renderState.baseLayer
            || null;
        const projLayer = renderState
            && isArray(renderState.layers)
            && arrayScan(renderState.layers,
                (l) => l instanceof XRProjectionLayer) as XRProjectionLayer
            || null;

        if (session !== this.session
            || webGLLayer !== this.webGLLayer
            || projLayer !== this.projLayer
            || views.length !== this.views.length) {

            if (session !== this.session) {
                if (isDefined(session)
                    && views.length > 1) {
                    this.binding = new XRWebGLBinding(session, this.gl);
                }
                else {
                    this.binding = null;
                }
            }

            this.session = session;
            this.webGLLayer = webGLLayer;
            this.projLayer = projLayer;
            arrayReplace(this.views, ...views);

            this.destroyTargets();

            this.setLayer(webGLLayer, ...views);
        }
    }

    private setLayer(layer: XRWebGLLayer, ...views: XRView[]) {

        if (!isDefined(layer)
            || !isMobileVR()) {
            const target = new FrameBufferCanvas(this.gl);
            this.targets.unshift(target);
        }

        if (isDefined(layer)) {
            let target: BaseFrameBuffer = null;
            const samples = this.gl.getParameter(this.gl.MAX_SAMPLES);
            if (this.mvMsExt) {
                target = new FrameBufferWebXRMultiviewMultisampled(this.gl, layer, views, samples, this.mvMsExt);
            }
            else if (this.mvExt) {
                target = new FrameBufferWebXRMultiview(this.gl, layer, views, this.mvExt);
            }
            else if (!isFirefox()) {
                target = new FrameBufferWebXRMultisampled(this.gl, layer, views, samples);
            }
            else {
                target = new FrameBufferWebXR(this.gl, layer, views);
            }

            target.attach(FrameAndRenderBuffers.COLOR_ATTACHMENT0);
            target.attach(FrameAndRenderBuffers.DEPTH_ATTACHMENT);
            this.targets.unshift(target);
        }

        for (let i = 1; i < this.targets.length; ++i) {
            const from = this.targets[i - 1];
            const to = this.targets[i];
            this.blitChain.push(new Blitter(
                this.gl,
                from,
                to,
                to instanceof FrameBufferCanvas ? this.gl.BACK : this.gl.COLOR_ATTACHMENT0
            ));
        }
    }

    get drawTarget() {
        return this.targets[0];
    }

    beginFrame() {
        this.drawTarget.bind(FramebufferType.DRAW_FRAMEBUFFER);
        this.drawTarget.clear(ClearBits.COLOR_BUFFER_BIT | ClearBits.DEPTH_BUFFER_BIT | ClearBits.STENCIL_BUFFER_BIT);
    }

    endFrame() {
        for (const blitter of this.blitChain) {
            blitter.blit();
        }
    }
}
