import { IFetcher } from "juniper-fetcher";
import { arrayCompare, IDisposable, isDefined, isNullOrUndefined } from "juniper-tslib";
import { cleanup } from "./cleanup";
import { IUpdatable } from "./IUpdatable";
import { IWebXRLayerManager } from "./IWebXRLayerManager";
import { solidTransparent } from "./materials";
import { objectGetRelativePose } from "./objectGetRelativePose";
import { objectIsFullyVisible } from "./objects";
import { plane } from "./Plane";
import { TexturedMesh } from "./TexturedMesh";
import { isMeshBasicMaterial } from "./typeChecks";
import { StereoLayoutName } from "./VideoPlayer3D";

const P = new THREE.Vector4();
const Q = new THREE.Quaternion();
const S = new THREE.Vector3();

let copyCounter = 0;

export class Image2DMesh
    extends THREE.Object3D
    implements IDisposable, IUpdatable {
    private readonly lastMatrixWorld = new THREE.Matrix4();
    private layer: XRQuadLayer = null;
    private tryWebXRLayers = true;
    private wasUsingLayer = false;
    private lastImage: unknown = null;
    private lastWidth: number = null;
    private lastHeight: number = null;
    stereoLayoutName: StereoLayoutName = "mono";
    protected fetcher: IFetcher = null;
    protected env: IWebXRLayerManager = null;
    mesh: TexturedMesh = null;

    webXRLayersEnabled = true;

    constructor(fetcher: IFetcher, env: IWebXRLayerManager, name: string, private readonly isStatic: boolean, materialOrOptions: THREE.MeshBasicMaterialParameters | THREE.MeshBasicMaterial = null) {
        super();

        if (env) {
            this.setEnvAndName(fetcher, env, name);

            let material = isMeshBasicMaterial(materialOrOptions)
                ? materialOrOptions
                : solidTransparent(Object.assign(
                    {},
                    materialOrOptions,
                    { name: this.name }));

            this.mesh = new TexturedMesh(fetcher, plane, material);
            this.add(this.mesh);
        }
    }

    dispose(): void {
        cleanup(this.layer);
    }

    private setEnvAndName(fetcher: IFetcher, env: IWebXRLayerManager, name: string) {
        this.fetcher = fetcher;
        this.env = env;
        this.name = name;
        this.tryWebXRLayers &&= this.env && this.env.hasXRCompositionLayers;
    }

    override copy(source: this, recursive = true) {
        super.copy(source, recursive);
        this.setEnvAndName(source.fetcher, source.env, source.name + (++copyCounter));
        for (let i = this.children.length - 1; i >= 0; --i) {
            const child = this.children[i];
            if (child.parent instanceof Image2DMesh
                && child instanceof TexturedMesh) {
                child.removeFromParent();
                this.mesh = new TexturedMesh(source.fetcher, child.geometry, child.material as THREE.MeshBasicMaterial);
            }
        }
        if (isNullOrUndefined(this.mesh)) {
            this.mesh = source.mesh.clone();
            this.add(this.mesh);
        }
        return this;
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
            const sizeChanged = this.mesh.imageWidth !== this.lastWidth
                || this.mesh.imageHeight !== this.lastHeight;

            this.wasUsingLayer = useLayer;
            this.lastImage = this.mesh.material.map.image;
            this.lastWidth = this.mesh.imageWidth;
            this.lastHeight = this.mesh.imageHeight;

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
                            viewPixelWidth: this.mesh.imageWidth,
                            viewPixelHeight: this.mesh.imageHeight,
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

