import { CanvasImageTypes, CanvasTypes, Context2D, createCanvasFromImageBitmap, createUtilityCanvas, isImageBitmap, isOffscreenCanvas, setContextSize } from "juniper-dom/canvas";
import { arrayCompare, hasWebXRLayers, IProgress, isDefined, isNumber } from "juniper-tslib";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { objectGetRelativePose } from "./objectGetRelativePose";
import { objectIsFullyVisible } from "./objects";
import { isTexture } from "./typeChecks";

const inchesPerMeter = 39.3701;

type Material = THREE.Material & {
    map: THREE.Texture;
};

const P = new THREE.Vector4();
const Q = new THREE.Quaternion();
const S = new THREE.Vector3();

export class TexturedMesh extends THREE.Mesh<THREE.BufferGeometry, Material> {
    isVideo: boolean;
    private readonly lastMatrixWorld = new THREE.Matrix4();
    private layerCanvas: CanvasTypes = null;
    private layerCtx: Context2D = null;
    private _imageWidth: number = 0;
    private _imageHeight: number = 0;
    private layer: XRQuadLayer = null;
    private wasVisible = false;
    private webXRLayerEnabled = true;
    private wasWebXRLayerAvailable: boolean = null;

    constructor(protected readonly env: BaseEnvironment<unknown>, geom: THREE.BufferGeometry, mat: Material) {
        super(geom, mat);

        this.webXRLayerEnabled &&= hasWebXRLayers();

        this.isVideo = false;

        this.onBeforeRender = () => {
            if (this.isVideo) {
                this.updateTexture();
            }
        };
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
        this.scale.x = v;
        this.scale.y = v / this.imageAspectRatio;
    }

    get objectHeight() {
        return this.scale.y;
    }

    set objectHeight(v) {
        this.scale.x = this.imageAspectRatio * v;
        this.scale.y = v;
    }

    get pixelDensity() {
        const ppm = (this.imageWidth / this.objectWidth);
        const ppi = ppm / inchesPerMeter;
        return ppi;
    }

    set pixelDensity(ppi) {
        const ppm = ppi * inchesPerMeter;
        this.objectWidth = this.imageWidth / ppm;
    }

    setImage(img: CanvasImageTypes | THREE.Texture): THREE.Texture {
        if (isTexture(img)) {
            if (isNumber(img.image.width)
                && isNumber(img.image.height)) {
                this._imageWidth = img.image.width;
                this._imageHeight = img.image.height;
            }
            this.isVideo = img.image instanceof HTMLVideoElement;
        }
        else {
            this._imageWidth = img.width;
            this._imageHeight = img.height;

            if (isImageBitmap(img)) {
                img = createCanvasFromImageBitmap(img);
            }

            if (isOffscreenCanvas(img)) {
                img = img as any as HTMLCanvasElement;
            }

            img = new THREE.Texture(img);
            img.needsUpdate = true;

            this.isVideo = img instanceof HTMLVideoElement;
        }

        this.layerCanvas = createUtilityCanvas(this.imageWidth, this.imageHeight);
        this.layerCtx = this.layerCanvas.getContext("2d");
        this.material.map = img;
        this.material.needsUpdate = true;

        return img;
    }

    async loadImage(path: string, onProgress?: IProgress): Promise<void> {
        let { content: img } = await this.env.fetcher
            .get(path)
            .progress(onProgress)
            .canvasImage();
        const texture = this.setImage(img);
        texture.name = path;
    }

    checkLayer(frame: XRFrame): void {
        if (this.material.map.image) {
            const isVisible = objectIsFullyVisible(this);
            const binding = (this.env.renderer.xr as any).getBinding() as XRWebGLBinding;
            const isWebXRLayerAvailable = this.webXRLayerEnabled
                && this.env.renderer.xr.isPresenting
                && isDefined(frame)
                && isDefined(binding);

            const useLayerChanged = isWebXRLayerAvailable !== this.wasWebXRLayerAvailable;
            const visibleChanged = isVisible != this.wasVisible;

            if (useLayerChanged || visibleChanged) {
                if (isWebXRLayerAvailable && isVisible) {
                    const space = this.env.renderer.xr.getReferenceSpace();

                    this.layer = binding.createQuadLayer({
                        space,
                        layout: "mono",
                        textureType: "texture",
                        isStatic: false,
                        viewPixelWidth: this.imageWidth,
                        viewPixelHeight: this.imageHeight
                    });

                    this.env.addWebXRLayer(this.layer, 500);
                    this.material.opacity = 0;
                    this.material.needsUpdate = true;
                }
                else if (this.layer) {
                    this.env.removeWebXRLayer(this.layer);
                    this.layer.destroy();
                    this.layer = null;
                    if (this.visible) {
                        this.material.opacity = 1;
                        this.material.needsUpdate = true;
                    }
                }
            }

            if (this.layer) {
                if (this.layer.needsRedraw
                    || this.material.needsUpdate
                    || this.material.map.needsUpdate) {
                    console.log("drawing quad");
                    const gl = this.env.renderer.getContext();
                    const gLayer = binding.getSubImage(this.layer, frame);

                    setContextSize(this.layerCtx, this.imageWidth, this.imageHeight);
                    this.layerCtx.drawImage(this.material.map.image, 0, 0);

                    gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, true);
                    gl.bindTexture(gl.TEXTURE_2D, gLayer.colorTexture);
                    gl.texSubImage2D(
                        gl.TEXTURE_2D,
                        0,
                        0, 0,
                        gl.RGBA,
                        gl.UNSIGNED_BYTE,
                        this.layerCanvas);

                    gl.bindTexture(gl.TEXTURE_2D, null);
                }

                if (arrayCompare(this.matrixWorld.elements, this.lastMatrixWorld.elements) >= 0) {
                    objectGetRelativePose(this.env.stage, this, P, Q, S);
                    this.layer.transform = new XRRigidTransform(P, Q);
                    this.layer.width = S.x;
                    this.layer.height = S.y;
                }
            }

            this.wasWebXRLayerAvailable = isWebXRLayerAvailable;
            this.wasVisible = isVisible;
        }
    }

    updateTexture() {
        const img = this.material.map.image;
        if (isNumber(img.width)
            && isNumber(img.height)
            && (this.imageWidth !== img.width
                || this.imageHeight !== img.height)) {

            this._imageWidth = img.width;
            this._imageHeight = img.height
            this.material.map.dispose();
            this.material.map = new THREE.Texture(img);
            this.material.needsUpdate = true;
        }
        this.material.map.needsUpdate = true;
    }
}
