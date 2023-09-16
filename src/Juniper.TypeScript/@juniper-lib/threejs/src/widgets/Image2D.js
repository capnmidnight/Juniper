import { arrayCompare, arrayScan } from "@juniper-lib/collections/arrays";
import { createUtilityCanvasFromImageBitmap, createUtilityCanvasFromImageData, dispose, isImageBitmap, isImageData, isOffscreenCanvas } from "@juniper-lib/dom/canvas";
import { getHeight, getWidth } from "@juniper-lib/tslib/images";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { inches2Meters, meters2Inches } from "@juniper-lib/tslib/units/length";
import { Matrix4, Object3D, Texture, Vector3, VideoTexture } from "three";
import { plane } from "../Plane";
import { cleanup } from "../cleanup";
import { getRelativeXRRigidTransform } from "../getRelativeXRRigidTransform";
import { solidTransparent } from "../materials";
import { mesh, objGraph, objectIsFullyVisible } from "../objects";
import { isMesh, isMeshBasicMaterial } from "../typeChecks";
const S = new Vector3();
let copyCounter = 0;
export class Image2D extends Object3D {
    constructor(env, name, webXRLayerType, materialOrOptions = null) {
        super();
        this.webXRLayerType = webXRLayerType;
        this.lastMatrixWorld = new Matrix4();
        this._imageWidth = 0;
        this._imageHeight = 0;
        this.forceUpdate = false;
        this.wasUsingLayer = false;
        this.layer = null;
        this.curImage = null;
        this.lastImage = null;
        this.lastWidth = null;
        this.lastHeight = null;
        this.env = null;
        this.mesh = null;
        this.stereoLayoutName = "mono";
        this.sizeMode = "none";
        if (env) {
            this.setEnvAndName(env, name);
            const material = isMeshBasicMaterial(materialOrOptions)
                ? materialOrOptions
                : solidTransparent(Object.assign({}, materialOrOptions, { name: this.name }));
            objGraph(this, this.mesh = mesh(name + "-Mesh", plane, material));
        }
    }
    copy(source, recursive = true) {
        super.copy(source, recursive);
        this.webXRLayerType = source.webXRLayerType;
        this.setImageSize(source.imageWidth, source.imageHeight);
        this.setEnvAndName(source.env, source.name + (++copyCounter));
        this.mesh = arrayScan(this.children, isMesh);
        if (isNullOrUndefined(this.mesh)) {
            this.mesh = source.mesh.clone();
            objGraph(this, this.mesh);
        }
        this.setTextureMap(source.curImage);
        return this;
    }
    dispose() {
        this.env.removeScope(this);
        this.disposeImage();
        cleanup(this.mesh);
    }
    disposeImage() {
        this.removeWebXRLayer();
        cleanup(this.mesh.material.map);
        this.curImage = null;
    }
    setImageSize(width, height) {
        if (width !== this.imageWidth
            || height !== this.imageHeight) {
            const { objectWidth, objectHeight } = this;
            this._imageWidth = width;
            this._imageHeight = height;
            if (this.sizeMode !== "none") {
                if (this.sizeMode === "fixed-width") {
                    this.objectWidth = objectWidth;
                }
                else {
                    this.objectHeight = objectHeight;
                }
            }
        }
    }
    get imageWidth() {
        return this._imageWidth;
    }
    get imageHeight() {
        return this._imageHeight;
    }
    get imageAspectRatio() {
        return this.imageWidth / this.imageHeight;
    }
    get objectWidth() {
        return this.scale.x;
    }
    set objectWidth(v) {
        this.scale.set(v, this.scale.y = v / this.imageAspectRatio, 1);
    }
    get objectHeight() {
        return this.scale.y;
    }
    set objectHeight(v) {
        this.scale.set(this.imageAspectRatio * v, v, 1);
    }
    get pixelDensity() {
        const inches = meters2Inches(this.objectWidth);
        const ppi = this.imageWidth / inches;
        return ppi;
    }
    set pixelDensity(ppi) {
        const inches = this.imageWidth / ppi;
        const meters = inches2Meters(inches);
        this.objectWidth = meters;
    }
    setEnvAndName(env, name) {
        this.env = env;
        this.name = name;
        this.env.addScopedEventListener(this, "update", (evt) => this.checkWebXRLayer(evt.frame));
    }
    get needsLayer() {
        if (!objectIsFullyVisible(this)
            || isNullOrUndefined(this.mesh.material.map)
            || isNullOrUndefined(this.curImage)) {
            return false;
        }
        if (!(this.curImage instanceof HTMLVideoElement)) {
            return true;
        }
        return !this.curImage.paused || this.curImage.currentTime > 0;
    }
    removeWebXRLayer() {
        if (isDefined(this.layer)) {
            this.wasUsingLayer = false;
            this.env.removeWebXRLayer(this.layer);
            this.mesh.visible = true;
            const layer = this.layer;
            this.layer = null;
            setTimeout(() => dispose(layer), 100);
        }
    }
    setTextureMap(img) {
        if (this.curImage) {
            this.disposeImage();
        }
        if (img) {
            if (isImageBitmap(img)) {
                img = createUtilityCanvasFromImageBitmap(img);
            }
            else if (isImageData(img)) {
                img = createUtilityCanvasFromImageData(img);
            }
            if (isOffscreenCanvas(img)) {
                img = img;
            }
            this.curImage = img;
            this.setImageSize(getWidth(img), getHeight(img));
            if (img instanceof HTMLVideoElement) {
                this.mesh.material.map = new VideoTexture(img);
            }
            else {
                this.mesh.material.map = new Texture(img);
                this.mesh.material.map.needsUpdate = true;
            }
        }
        this.mesh.material.needsUpdate = true;
    }
    get isVideo() {
        return this.curImage instanceof HTMLVideoElement;
    }
    updateTexture() {
        if (isDefined(this.curImage)) {
            const newWidth = getWidth(this.curImage);
            const newHeight = getHeight(this.curImage);
            ;
            if (this.imageWidth !== newWidth
                || this.imageHeight !== newHeight) {
                const img = this.curImage;
                this.disposeImage();
                this.setTextureMap(img);
            }
            else {
                this.mesh.material.map.needsUpdate
                    = this.forceUpdate
                        = true;
            }
        }
    }
    checkWebXRLayer(frame) {
        if (this.mesh.material.map && this.curImage) {
            const isLayersAvailable = this.webXRLayerType !== "none"
                && this.env.hasXRCompositionLayers
                && this.env.showWebXRLayers
                && isDefined(frame)
                && (this.isVideo && isDefined(this.env.xrMediaBinding)
                    || !this.isVideo && isDefined(this.env.xrBinding));
            const useLayer = isLayersAvailable && this.needsLayer;
            const useLayerChanged = useLayer !== this.wasUsingLayer;
            const imageChanged = this.curImage !== this.lastImage
                || this.mesh.material.needsUpdate
                || this.mesh.material.map.needsUpdate
                || this.forceUpdate;
            const sizeChanged = this.imageWidth !== this.lastWidth
                || this.imageHeight !== this.lastHeight;
            this.wasUsingLayer = useLayer;
            this.lastImage = this.curImage;
            this.lastWidth = this.imageWidth;
            this.lastHeight = this.imageHeight;
            if (useLayerChanged || sizeChanged) {
                if ((!useLayer || sizeChanged) && this.layer) {
                    this.removeWebXRLayer();
                }
                if (useLayer) {
                    const space = this.env.referenceSpace;
                    const transform = getRelativeXRRigidTransform(this.env.stage, this.mesh, S);
                    this.lastMatrixWorld.copy(this.matrixWorld);
                    const width = S.x / 2;
                    const height = S.y / 2;
                    const layout = this.stereoLayoutName === "mono"
                        ? "mono"
                        : this.stereoLayoutName === "left-right" || this.stereoLayoutName === "right-left"
                            ? "stereo-left-right"
                            : "stereo-top-bottom";
                    if (this.isVideo) {
                        const invertStereo = this.stereoLayoutName === "right-left"
                            || this.stereoLayoutName === "bottom-top";
                        this.layer = this.env.xrMediaBinding.createQuadLayer(this.curImage, {
                            space,
                            layout,
                            invertStereo,
                            transform,
                            width,
                            height
                        });
                    }
                    else {
                        this.layer = this.env.xrBinding.createQuadLayer({
                            space,
                            layout,
                            textureType: "texture",
                            isStatic: this.webXRLayerType === "static",
                            viewPixelWidth: getWidth(this.curImage),
                            viewPixelHeight: getHeight(this.curImage),
                            transform,
                            width,
                            height
                        });
                    }
                    this.env.addWebXRLayer(this.layer, 500);
                    this.mesh.visible = false;
                }
            }
            if (this.layer) {
                if (imageChanged || this.layer.needsRedraw) {
                    const gl = this.env.gl;
                    const gLayer = this.env.xrBinding.getSubImage(this.layer, frame);
                    gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, true);
                    gl.bindTexture(gl.TEXTURE_2D, gLayer.colorTexture);
                    gl.texSubImage2D(gl.TEXTURE_2D, 0, 0, 0, gl.RGBA, gl.UNSIGNED_BYTE, this.curImage);
                    gl.generateMipmap(gl.TEXTURE_2D);
                    gl.bindTexture(gl.TEXTURE_2D, null);
                    this.forceUpdate = false;
                }
                if (arrayCompare(this.matrixWorld.elements, this.lastMatrixWorld.elements) >= 0) {
                    this.layer.transform = getRelativeXRRigidTransform(this.env.stage, this.mesh, S);
                    this.lastMatrixWorld.copy(this.matrixWorld);
                    this.layer.width = S.x / 2;
                    this.layer.height = S.y / 2;
                }
            }
            else {
                this.forceUpdate = false;
            }
        }
    }
}
//# sourceMappingURL=Image2D.js.map