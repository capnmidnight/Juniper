import { arrayClear, arrayReplace, arrayScan } from "@juniper-lib/collections/arrays";
import { isFirefox, isMobileVR } from "@juniper-lib/tslib/flags";
import { isArray, isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { Blitter } from "./Blitter";
import { ClearBits, FrameAndRenderBuffers, FramebufferType } from "./GLEnum";
import { FrameBufferCanvas, FrameBufferWebXR, FrameBufferWebXRMultisampled, FrameBufferWebXRMultiview, FrameBufferWebXRMultiviewMultisampled } from "./managed/resource/FrameBuffer";
import { dispose } from "@juniper-lib/dom/canvas";
export class RenderTargetManager {
    constructor(gl) {
        this.gl = gl;
        this.blitChain = new Array();
        this.targets = new Array();
        this.session = undefined;
        this.webGLLayer = undefined;
        this.projLayer = undefined;
        this.views = new Array();
        this.disposed = false;
        this.mvExtOculus = null;
        this.mvExtOvr = null;
        this.binding = null;
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
    destroyTargets() {
        arrayClear(this.blitChain);
        this.targets.forEach(dispose);
        arrayClear(this.targets);
    }
    resize() {
        const session = this.session;
        this.session = null;
        this.setSession(session, this.views);
    }
    setSession(session, views) {
        if (isNullOrUndefined(session)) {
            views = [];
        }
        console.trace(session, views);
        const renderState = session && session.renderState;
        const webGLLayer = renderState && renderState.baseLayer
            || null;
        const projLayer = renderState
            && isArray(renderState.layers)
            && arrayScan(renderState.layers, (l) => l instanceof XRProjectionLayer)
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
    setLayer(layer, ...views) {
        if (!isDefined(layer)
            || !isMobileVR()) {
            const target = new FrameBufferCanvas(this.gl);
            this.targets.unshift(target);
        }
        if (isDefined(layer)) {
            let target = null;
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
            this.blitChain.push(new Blitter(this.gl, from, to, to instanceof FrameBufferCanvas
                ? this.gl.BACK
                : this.gl.COLOR_ATTACHMENT0));
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
//# sourceMappingURL=RenderTargetManager.js.map