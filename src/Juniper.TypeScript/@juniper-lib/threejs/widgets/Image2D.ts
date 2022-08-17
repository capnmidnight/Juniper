import { createUtilityCanvasFromImageBitmap, createUtilityCanvasFromImageData, isImageBitmap, isImageData, isOffscreenCanvas } from "@juniper-lib/dom/canvas";
import { arrayCompare } from "@juniper-lib/tslib/collections/arrayCompare";
import { arrayScan } from "@juniper-lib/tslib/collections/arrayScan";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { inches2Meters, meters2Inches } from "@juniper-lib/tslib/units/length";
import { IDisposable } from "@juniper-lib/tslib/using";
import { BufferGeometry, Matrix4, Mesh, MeshBasicMaterial, MeshBasicMaterialParameters, Object3D, Quaternion, Texture, Vector3, Vector4, VideoTexture } from "three";
import { cleanup } from "../cleanup";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { XRTimerTickEvent } from "../environment/XRTimer";
import { solidTransparent } from "../materials";
import { objectGetRelativePose } from "../objectGetRelativePose";
import { mesh, objectIsFullyVisible, objGraph } from "../objects";
import { plane } from "../Plane";
import { isMesh, isMeshBasicMaterial } from "../typeChecks";
import { StereoLayoutName } from "../VideoPlayer3D";


const P = new Vector4();
const Q = new Quaternion();
const S = new Vector3();

let copyCounter = 0;

export type Image2DObjectSizeMode = "none"
    | "fixed-height"
    | "fixed-width";

export type WebXRLayerType = "none" | "static" | "dynamic";

export class Image2D
    extends Object3D
    implements IDisposable {
    private readonly onTick: (evt: XRTimerTickEvent) => void;
    private readonly lastMatrixWorld = new Matrix4();
    private layer: XRQuadLayer = null;
    private wasUsingLayer = false;
    private _imageWidth: number = 0;
    private _imageHeight: number = 0;
    private curImage: TexImageSource | OffscreenCanvas = null;
    private lastImage: TexImageSource | OffscreenCanvas = null;
    private lastWidth: number = null;
    private lastHeight: number = null;
    protected env: BaseEnvironment = null;

    mesh: Mesh<BufferGeometry, MeshBasicMaterial> = null;
    stereoLayoutName: StereoLayoutName = "mono";
    sizeMode: Image2DObjectSizeMode = "none";

    constructor(env: BaseEnvironment, name: string, public webXRLayerType: WebXRLayerType, materialOrOptions: MeshBasicMaterialParameters | MeshBasicMaterial = null) {
        super();
        this.onTick = (evt) => this.checkWebXRLayer(evt.frame);

        if (env) {
            this.setEnvAndName(env, name);

            let material = isMeshBasicMaterial(materialOrOptions)
                ? materialOrOptions
                : solidTransparent(Object.assign(
                    {},
                    materialOrOptions,
                    { name: this.name }));

            objGraph(this,
                this.mesh = mesh(name + "-Mesh", plane, material));
        }
    }

    override copy(source: this, recursive = true) {
        super.copy(source, recursive);
        this.webXRLayerType = source.webXRLayerType;
        this.setImageSize(source.imageWidth, source.imageHeight);
        this.setEnvAndName(source.env, source.name + (++copyCounter));
        this.mesh = arrayScan(this.children, isMesh) as Mesh<BufferGeometry, MeshBasicMaterial>;
        if (isNullOrUndefined(this.mesh)) {
            this.mesh = source.mesh.clone();
            objGraph(this, this.mesh);
        }

        this.setTextureMap(source.curImage);

        return this;
    }

    dispose(): void {
        this.env.timer.removeTickHandler(this.onTick);
        this.disposeImage();
        cleanup(this.mesh);
    }

    private disposeImage() {
        this.removeWebXRLayer();
        cleanup(this.mesh.material.map);
        this.curImage = null;
    }

    private setImageSize(width: number, height: number) {
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

    private setEnvAndName(env: BaseEnvironment, name: string) {
        this.env = env;
        this.name = name;
        this.env.timer.addTickHandler(this.onTick);
    }

    private get needsLayer(): boolean {
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

            setTimeout(() => layer.destroy(), 100);
        }
    }

    setTextureMap(img: TexImageSource | OffscreenCanvas): void {
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
                img = img as any as HTMLCanvasElement;
            }

            this.curImage = img;

            if (img instanceof HTMLVideoElement) {
                this.setImageSize(img.videoWidth, img.videoHeight);
                this.mesh.material.map = new VideoTexture(img);
            }
            else {
                this.setImageSize(img.width, img.height);
                this.mesh.material.map = new Texture(img);
                this.mesh.material.map.needsUpdate = true;
            }

        }

        this.mesh.material.needsUpdate = true;
    }

    private get isVideo() {
        return this.curImage instanceof HTMLVideoElement;
    }

    updateTexture() {
        if (isDefined(this.curImage)) {
            const curVideo = this.curImage as HTMLVideoElement;
            const newWidth = this.isVideo ? curVideo.videoWidth : this.curImage.width;
            const newHeight = this.isVideo ? curVideo.videoHeight : this.curImage.height;
            if (this.imageWidth !== newWidth
                || this.imageHeight !== newHeight) {

                const img = this.curImage;

                this.disposeImage();

                this.setTextureMap(img);
            }
        }
    }

    private checkWebXRLayer(frame?: XRFrame): void {
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
                || this.mesh.material.map.needsUpdate;
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

                    objectGetRelativePose(this.env.stage, this.mesh, P, Q, S);
                    this.lastMatrixWorld.copy(this.matrixWorld);
                    const transform = new XRRigidTransform(P, Q);
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

                        this.layer = this.env.xrMediaBinding.createQuadLayer(this.curImage as HTMLVideoElement, {
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
                            viewPixelWidth: this.curImage.width,
                            viewPixelHeight: this.curImage.height,
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
                    gl.texSubImage2D(
                        gl.TEXTURE_2D,
                        0,
                        0, 0,
                        gl.RGBA,
                        gl.UNSIGNED_BYTE,
                        this.curImage);
                    gl.generateMipmap(gl.TEXTURE_2D);

                    gl.bindTexture(gl.TEXTURE_2D, null);
                }

                if (arrayCompare(this.matrixWorld.elements, this.lastMatrixWorld.elements) >= 0) {
                    objectGetRelativePose(this.env.stage, this.mesh, P, Q, S);
                    this.lastMatrixWorld.copy(this.matrixWorld);
                    this.layer.transform = new XRRigidTransform(P, Q);
                    this.layer.width = S.x / 2;
                    this.layer.height = S.y / 2;
                }
            }
        }
    }
}

