import { CanvasImageTypes, createCanvasFromImageBitmap, isImageBitmap, isOffscreenCanvas } from "@juniper-lib/dom/canvas";
import { IFetcher } from "@juniper-lib/fetcher";
import { arrayCompare, arrayScan, IDisposable, inches2Meters, IProgress, isDefined, isNullOrUndefined, isNumber, meters2Inches } from "@juniper-lib/tslib";
import { cleanup } from "../cleanup";
import { IUpdatable } from "../IUpdatable";
import { IWebXRLayerManager } from "../IWebXRLayerManager";
import { solidTransparent } from "../materials";
import { objectGetRelativePose } from "../objectGetRelativePose";
import { objectIsFullyVisible, objGraph } from "../objects";
import { plane } from "../Plane";
import { isMesh, isMeshBasicMaterial } from "../typeChecks";
import { StereoLayoutName } from "../VideoPlayer3D";

const P = new THREE.Vector4();
const Q = new THREE.Quaternion();
const S = new THREE.Vector3();

let copyCounter = 0;

export type Image2DObjectSizeMode = "none"
    | "fixed-height"
    | "fixed-width";

export class Image2D
    extends THREE.Object3D
    implements IDisposable, IUpdatable {
    private readonly lastMatrixWorld = new THREE.Matrix4();
    private layer: XRQuadLayer = null;
    private tryWebXRLayers = true;
    private wasUsingLayer = false;
    private _imageWidth: number = 0;
    private _imageHeight: number = 0;
    private lastImage: unknown = null;
    private lastWidth: number = null;
    private lastHeight: number = null;
    stereoLayoutName: StereoLayoutName = "mono";
    protected env: IWebXRLayerManager = null;
    mesh: THREE.Mesh<THREE.BufferGeometry, THREE.MeshBasicMaterial> = null;

    webXRLayersEnabled = true;

    sizeMode: Image2DObjectSizeMode = "none";

    constructor(env: IWebXRLayerManager, name: string, private readonly isStatic: boolean, materialOrOptions: THREE.MeshBasicMaterialParameters | THREE.MeshBasicMaterial = null) {
        super();

        if (env) {
            this.setEnvAndName(env, name);

            let material = isMeshBasicMaterial(materialOrOptions)
                ? materialOrOptions
                : solidTransparent(Object.assign(
                    {},
                    materialOrOptions,
                    { name: this.name }));

            this.mesh = new THREE.Mesh(plane, material);
            objGraph(this, this.mesh);
        }
    }

    override copy(source: this, recursive = true) {
        super.copy(source, recursive);
        this.setImageSize(source.imageWidth, source.imageHeight);
        this.setEnvAndName(source.env, source.name + (++copyCounter));
        this.mesh = arrayScan(this.children, isMesh) as THREE.Mesh<THREE.BufferGeometry, THREE.MeshBasicMaterial>;
        if (isNullOrUndefined(this.mesh)) {
            this.mesh = source.mesh.clone();
        }

        objGraph(this, this.mesh);

        return this;
    }

    dispose(): void {
        cleanup(this.layer);
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

    private setEnvAndName(env: IWebXRLayerManager, name: string) {
        this.env = env;
        this.name = name;
        this.tryWebXRLayers &&= this.env && this.env.hasXRCompositionLayers;
    }

    private get needsLayer(): boolean {
        if (!objectIsFullyVisible(this)
            || isNullOrUndefined(this.mesh.material.map)
            || isNullOrUndefined(this.mesh.material.map.image)) {
            return false;
        }

        const img = this.mesh.material.map.image;
        if (!(img instanceof HTMLVideoElement)) {
            return true;
        }

        return !img.paused || img.currentTime > 0;
    }

    removeWebXRLayer() {
        if (isDefined(this.layer)) {
            this.wasUsingLayer = false;
            this.env.removeWebXRLayer(this.layer);
            const layer = this.layer;
            this.layer = null;

            setTimeout(() => {
                layer.destroy();
                this.mesh.visible = true;
            }, 100);
        }
    }

    setTextureMap(img: CanvasImageTypes | HTMLVideoElement): THREE.Texture {
        if (isImageBitmap(img)) {
            img = createCanvasFromImageBitmap(img);
        }

        if (isOffscreenCanvas(img)) {
            img = img as any as HTMLCanvasElement;
        }

        if (img instanceof HTMLVideoElement) {
            this.mesh.material.map = new THREE.VideoTexture(img);
            this.setImageSize(img.videoWidth, img.videoHeight);
        }
        else {
            this.mesh.material.map = new THREE.Texture(img);
            this.setImageSize(img.width, img.height);
            this.mesh.material.map.needsUpdate = true;
        }

        this.mesh.material.needsUpdate = true;

        return this.mesh.material.map;
    }

    async loadTextureMap(fetcher: IFetcher, path: string, prog?: IProgress): Promise<void> {
        let { content: img } = await fetcher
            .get(path)
            .progress(prog)
            .image();
        const texture = this.setTextureMap(img);
        texture.name = path;
    }

    updateTexture() {
        const img = this.mesh.material.map.image;
        if (isNumber(img.width)
            && isNumber(img.height)
            && (this.imageWidth !== img.width
                || this.imageHeight !== img.height)) {
            this.mesh.material.map.dispose();
            this.mesh.material.map = new THREE.Texture(img);
            this.mesh.material.needsUpdate = true;
            this.setImageSize(img.width, img.height);
        }
        this.mesh.material.map.needsUpdate = true;
    }

    update(_dt: number, frame?: XRFrame): void {
        if (this.mesh.material.map && this.mesh.material.map.image) {
            const isVideo = this.mesh.material.map instanceof THREE.VideoTexture;
            const isLayersAvailable = this.tryWebXRLayers
                && this.webXRLayersEnabled
                && isDefined(frame)
                && (isVideo && isDefined(this.env.xrMediaBinding)
                    || !isVideo && isDefined(this.env.xrBinding));
            const useLayer = isLayersAvailable && this.needsLayer;

            const useLayerChanged = useLayer !== this.wasUsingLayer;
            const imageChanged = this.mesh.material.map.image !== this.lastImage
                || this.mesh.material.needsUpdate
                || this.mesh.material.map.needsUpdate;
            const sizeChanged = this.imageWidth !== this.lastWidth
                || this.imageHeight !== this.lastHeight;

            this.wasUsingLayer = useLayer;
            this.lastImage = this.mesh.material.map.image;
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

                    if (isVideo) {
                        const invertStereo = this.stereoLayoutName === "right-left"
                            || this.stereoLayoutName === "bottom-top";

                        this.layer = this.env.xrMediaBinding.createQuadLayer(this.mesh.material.map.image, {
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
                            isStatic: this.isStatic,
                            viewPixelWidth: this.imageWidth,
                            viewPixelHeight: this.imageHeight,
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
                        this.mesh.material.map.image);
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

