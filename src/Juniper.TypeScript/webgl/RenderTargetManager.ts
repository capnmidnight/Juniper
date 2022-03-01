import { arrayClear, IDisposable, isDefined, isMobileVR } from "juniper-tslib";
import { Blitter } from "./Blitter";
import { ClearBits, FramebufferType } from "./GLEnum";
import { BaseFrameBuffer, FrameBufferCanvas, FrameBufferWebXR, FrameBufferWebXRMultisampled, FrameBufferWebXRMultiview, FrameBufferWebXRMultiviewMultisampled } from "./managed/resource/FrameBuffer";

export class RenderTargetManager implements IDisposable {
    private blitChain = new Array<Blitter>();
    private targets = new Array<BaseFrameBuffer>();
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

            const canvasTarget = new FrameBufferCanvas(this.gl);
            this.targets.unshift(canvasTarget);

            if (isDefined(layer)
                && isDefined(layer.framebuffer)) {
                const webXRTarget = new FrameBufferWebXR(this.gl, layer);
                this.targets.unshift(webXRTarget);

                let target: BaseFrameBuffer = null;
                if (this.mvMsExt) {
                    target = new FrameBufferWebXRMultiviewMultisampled(this.gl, this.mvMsExt, layer, numViews)
                }
                else if (this.mvExt) {
                    target = new FrameBufferWebXRMultiview(this.gl, this.mvExt, layer, numViews);
                }
                else {
                    target = new FrameBufferWebXRMultisampled(this.gl, layer);
                }

                this.targets.unshift(target);
                this.blitChain.push(new Blitter(
                    this.gl,
                    target,
                    webXRTarget,
                    [this.gl.COLOR_ATTACHMENT0]));

                if (!isMobileVR()) {
                    this.blitChain.push(new Blitter(
                        this.gl,
                        webXRTarget,
                        canvasTarget,
                        [this.gl.BACK]));
                }
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
