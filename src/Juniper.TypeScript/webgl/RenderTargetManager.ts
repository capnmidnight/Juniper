import { arrayClear, IDisposable, isDefined, isMobileVR } from "juniper-tslib";
import { Blitter, IBlitter } from "./Blitter";
import { ClearBits, FramebufferType } from "./GLEnum";
import {
    RenderTarget,
    RenderTargetCanvas,
    RenderTargetWebXRMultiview,
    RenderTargetWebXRMultiviewMultisampled,
    RenderTargetWebXRMultisampled,
    RenderTargetWebXR
} from "./managed/RenderTarget";

export class RenderTargetManager implements IDisposable {
    private blitChain = new Array<IBlitter>();
    private targets = new Array<RenderTarget>();
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
        console.log("setLayer", layer, numViews, this.lastLayer);
        if (layer !== this.lastLayer) {
            const attrib = this.gl.getContextAttributes();

            this.lastLayer = layer;
            this.lastNumViews = numViews;

            this.destroyTargets();

            const canvasTarget = new RenderTargetCanvas(this.gl);
            this.targets.unshift(canvasTarget);

            if (isDefined(layer)
                && isDefined(layer.framebuffer)) {
                let target: RenderTarget = null;
                if (this.mvMsExt && attrib.antialias) {
                    target = new RenderTargetWebXRMultiviewMultisampled(this.gl, this.mvMsExt, layer, numViews, 4)
                }
                else if (this.mvExt && !attrib.antialias) {
                    target = new RenderTargetWebXRMultiview(this.gl, this.mvExt, layer, numViews);
                }
                else if (attrib.antialias) {
                    target = new RenderTargetWebXRMultisampled(this.gl, layer, 4);
                }
                else {
                    target = new RenderTargetWebXR(this.gl, layer);
                }

                console.log("target", target);

                this.targets.unshift(target);

                if (!isMobileVR()) {
                    this.blitChain.push(new Blitter(
                        this.gl,
                        target,
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
