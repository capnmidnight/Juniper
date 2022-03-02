import { arrayClear, IDisposable, isDefined, isFirefox, isMobileVR } from "juniper-tslib";
import { Blitter } from "./Blitter";
import { ClearBits, FrameAndRenderBuffers, FramebufferType } from "./GLEnum";
import { BaseFrameBuffer, BaseRenderTarget, FrameBufferCanvas, FrameBufferWebXR, FrameBufferWebXRMultisampled, FrameBufferWebXRMultiview, FrameBufferWebXRMultiviewMultisampled } from "./managed/resource/FrameBuffer";

export class RenderTargetManager implements IDisposable {
    private blitChain = new Array<Blitter>();
    private targets = new Array<BaseRenderTarget>();
    private lastLayer: XRWebGLLayer = undefined;
    private lastNumViews: number = null;
    private disposed: boolean = false;
    private mvExtOculus: OCULUS_multiview = null;
    private mvExtOvr: OVR_multiview2 = null;

    constructor(private gl: WebGL2RenderingContext) {
        this.mvExtOculus = this.gl.getExtension("OCULUS_multiview");
        if (!this.mvExtOculus) {
            this.mvExtOvr = this.gl.getExtension("OVR_multiview2");
        }

        this.clearLayers();
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
        const layer = this.lastLayer;
        this.lastLayer = null;
        this.setSession(layer, this.lastNumViews);
    }

    clearLayers() {
        this.setSession(null, 1);
    }

    setSession(layer: XRWebGLLayer, numViews: number) {
        if (layer !== this.lastLayer) {
            this.lastLayer = layer;
            this.lastNumViews = numViews;

            this.destroyTargets();

            if (!isDefined(layer)
                || !isMobileVR()) {
                const target = new FrameBufferCanvas(this.gl);
                this.targets.unshift(target);
            }

            if (isDefined(layer)) {
                let target: BaseFrameBuffer = null;
                if (this.mvMsExt) {
                    target = new FrameBufferWebXRMultiviewMultisampled(this.gl, this.mvMsExt, layer, numViews)
                }
                else if (this.mvExt) {
                    target = new FrameBufferWebXRMultiview(this.gl, this.mvExt, layer, numViews);
                }
                else if (!isFirefox()) {
                    target = new FrameBufferWebXRMultisampled(this.gl, layer);
                }
                else {
                    target = new FrameBufferWebXR(this.gl, layer);
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
    }

    get drawTarget() {
        return this.targets[0];
    }

    beginFrame() {
        this.drawTarget.bind(FramebufferType.DRAW_FRAMEBUFFER);
        this.drawTarget.clear(ClearBits.COLOR_BUFFER_BIT | ClearBits.DEPTH_BUFFER_BIT);
    }

    endFrame() {
        for (const blitter of this.blitChain) {
            blitter.blit();
        }
    }
}
