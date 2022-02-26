import type { IDisposable } from "juniper-tslib";
import { arrayClear, isBoolean, isDefined, isMobileVR, isNullOrUndefined } from "juniper-tslib";
import type { IBlitter } from "./Blitter";
import { Blitter } from "./Blitter";
import { ClearBits, FramebufferType } from "./GLEnum";
import { BaseRenderTarget, RenderTargetCanvas, RenderTargetRenderBufferMultisampled, RenderTargetXRWebGLLayer } from "./RenderTarget";

export class RenderTargetManager implements IDisposable {
    private blitChain = new Array<IBlitter>();
    private targets = new Array<BaseRenderTarget>();
    private lastLayer: XRWebGLLayer = null;
    private lastNumViews: number = null;
    private disposed: boolean = false;
    private mvExtOculus: OCULUS_multiview = null;
    private mvExtOvr: OVR_multiview2 = null;

    constructor(private gl: WebGL2RenderingContext) {
        this.targets.push(new RenderTargetCanvas(gl));
        this.mvExtOculus = this.gl.getExtension("OCULUS_multiview");
        if (!this.mvExtOculus) {
            this.mvExtOvr = this.gl.getExtension("OVR_multiview2");
        }

        if (this.mvExt) {
            const numViews = this.gl.getParameter(this.mvExt.MAX_VIEWS_OVR);
            console.log("NUM VIEWS", numViews);
        }
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
        for (const blitter of this.blitChain) {
            blitter.dispose();
        }

        arrayClear(this.blitChain);

        for (const target of this.targets) {
            target.dispose();
        }

        arrayClear(this.targets);
    }

    resize() {
        const layer = this.lastLayer;
        this.lastLayer = null;
        this.setLayer(layer, this.lastNumViews);
    }

    clearLayers() {
        this.setLayer(null, 1);
    }

    setLayer(layer: XRWebGLLayer, numViews: number) {
        if (layer !== this.lastLayer) {
            this.lastLayer = layer;
            this.lastNumViews = numViews;

            this.destroyTargets();

            // effective for resizing the render targets.
            const canvasTarget = new RenderTargetCanvas(this.gl);
            this.targets.push(canvasTarget);

            if (isBoolean(layer)
                || (isDefined(layer)
                    && isNullOrUndefined(layer.framebuffer))) {
                const offscreenTarget = new RenderTargetRenderBufferMultisampled(this.gl, this.gl.canvas.width, this.gl.canvas.height, numViews);
                this.targets.push(offscreenTarget);
                this.blitChain.push(new Blitter(
                    this.gl,
                    offscreenTarget,
                    canvasTarget,
                    [this.gl.BACK]));
            }
            else if (isDefined(layer)
                && isDefined(layer.framebuffer)) {
                const webXRTarget = new RenderTargetXRWebGLLayer(this.gl, layer, numViews);
                this.targets.push(webXRTarget);

                if (!isMobileVR()) {
                    this.blitChain.push(new Blitter(
                        this.gl,
                        webXRTarget,
                        canvasTarget,
                        [this.gl.BACK]));
                }
            }
            else {
                console.log("Using canvas render target");
            }
        }
    }

    get drawTarget() {
        return this.targets[this.targets.length - 1];
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
