import type { IDisposable } from "juniper-tslib";
import { arrayClear, isDefined, isMobileVR, isNullOrUndefined } from "juniper-tslib";
import type { IBlitter } from "./Blitter";
import { Blitter } from "./Blitter";
import { ClearBits, FramebufferType } from "./GLEnum";
import { BaseRenderTarget, RenderTargeFrameBufferTextureMultiviewMultisampled, RenderTargetCanvas, RenderTargetFrameBufferTextureMultiview, RenderTargetRenderBuffer, RenderTargetRenderBufferMultisampled, RenderTargetXRWebGLLayer } from "./RenderTarget";

export class RenderTargetManager implements IDisposable {
    private blitChain = new Array<IBlitter>();
    private targets = new Array<BaseRenderTarget>();
    private lastLayer: XRWebGLLayer = undefined;
    private lastNumViews: number = null;
    private disposed: boolean = false;
    private mvExtOculus: OCULUS_multiview = null;
    private mvExtOvr: OVR_multiview2 = null;

    constructor(private gl: WebGL2RenderingContext) {
        this.targets.unshift(new RenderTargetCanvas(gl));
        this.mvExtOculus = this.gl.getExtension("OCULUS_multiview");
        if (!this.mvExtOculus) {
            this.mvExtOvr = this.gl.getExtension("OVR_multiview2");
        }

        if (this.mvExt) {
            const numViews = this.gl.getParameter(this.mvExt.MAX_VIEWS_OVR);
            console.log("NUM VIEWS", numViews);
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
        if (layer !== this.lastLayer) {
            const attrib = this.gl.getContextAttributes();

            this.lastLayer = layer;
            this.lastNumViews = numViews;

            this.destroyTargets();

            // effective for resizing the render targets.
            const canvasTarget = new RenderTargetCanvas(this.gl);
            this.targets.unshift(canvasTarget);

            if (isNullOrUndefined(layer)) {
                console.log("Using canvas render target");
            }
            else if (isDefined(layer.framebuffer)) {
                let target: BaseRenderTarget = null;
                if (this.mvMsExt && attrib.antialias) {
                    target = new RenderTargeFrameBufferTextureMultiviewMultisampled(this.gl, layer, numViews)
                }
                else if (this.mvExt && attrib.antialias) {
                    target = new RenderTargetFrameBufferTextureMultiview(this.gl, layer, numViews);
                }
                else {
                    target = new RenderTargetXRWebGLLayer(this.gl, layer);
                }

                this.targets.unshift(target);

                if (!isMobileVR()) {
                    this.blitChain.push(new Blitter(
                        this.gl,
                        target,
                        canvasTarget,
                        [this.gl.BACK]));
                }
            }
            else {
                let target: BaseRenderTarget = null;
                if (attrib.antialias) {
                    target = new RenderTargetRenderBufferMultisampled(this.gl, this.gl.canvas.width, this.gl.canvas.height);
                }
                else {
                    target = new RenderTargetRenderBuffer(this.gl, this.gl.canvas.width, this.gl.canvas.height);
                }
                this.targets.unshift(target);
                this.blitChain.push(new Blitter(
                    this.gl,
                    target,
                    canvasTarget,
                    [this.gl.BACK]));
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
