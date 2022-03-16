import { arrayCompare, IDisposable, isDefined, isNullOrUndefined } from "juniper-tslib";
import { cleanup } from "./cleanup";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { IUpdatable } from "./IUpdatable";
import { solidTransparent } from "./materials";
import { objectGetRelativePose } from "./objectGetRelativePose";
import { objectIsFullyVisible } from "./objects";
import { plane } from "./Plane";
import { TexturedMesh } from "./TexturedMesh";

const P = new THREE.Vector4();
const Q = new THREE.Quaternion();
const S = new THREE.Vector3();

let copyCounter = 0;

export class Image2DMesh
    extends THREE.Object3D
    implements IDisposable, IUpdatable {
    private readonly lastMatrixWorld = new THREE.Matrix4();
    private layer: XRQuadLayer = null;
    private wasVisible = false;
    private webXRLayerEnabled = true;
    private wasLayersAvailable: boolean = null;
    private lastImage: unknown = null;
    private lastWidth: number = null;
    private lastHeight: number = null;
    protected env: BaseEnvironment<unknown> = null;
    mesh: TexturedMesh = null;

    constructor(env: BaseEnvironment<unknown>, name: string, private readonly isStatic: boolean, materialOptions: THREE.MeshBasicMaterialParameters = null) {
        super();

        if (env) {
            this.setEnvAndName(env, name);
            this.mesh = new TexturedMesh(this.env, plane, solidTransparent(Object.assign(
                {},
                materialOptions,
                { name: this.name })));
            this.add(this.mesh);
        }
    }

    dispose(): void {
        cleanup(this.layer);
    }

    private setEnvAndName(env: BaseEnvironment<unknown>, name: string) {
        this.env = env;
        this.name = name;
        this.webXRLayerEnabled &&= this.env.hasXRCompositionLayers;
    }

    override copy(source: this, recursive = true) {
        super.copy(source, recursive);
        this.setEnvAndName(source.env, source.name + (++copyCounter));
        for (let i = this.children.length - 1; i >= 0; --i) {
            const child = this.children[i];
            if (child.parent instanceof Image2DMesh
                && child instanceof TexturedMesh) {
                child.removeFromParent();
                this.mesh = new TexturedMesh(source.env, child.geometry, child.material as THREE.MeshBasicMaterial);
            }
        }
        if (isNullOrUndefined(this.mesh)) {
            this.mesh = source.mesh.clone();
            this.add(this.mesh);
        }
        return this;
    }

    update(_dt: number, frame: XRFrame): void {
        if (this.mesh.material.map.image) {
            const isVisible = objectIsFullyVisible(this);
            const isLayersAvailable = this.webXRLayerEnabled
                && this.env.renderer.xr.isPresenting
                && isDefined(frame)
                && isDefined(this.env.xrBinding);
            const useLayer = isLayersAvailable && isVisible;

            const layersAvailableChanged = isLayersAvailable !== this.wasLayersAvailable;
            const visibleChanged = isVisible != this.wasVisible;
            const imageChanged = this.mesh.material.map.image !== this.lastImage
                || this.mesh.material.needsUpdate
                || this.mesh.material.map.needsUpdate;
            const sizeChanged = this.mesh.imageWidth !== this.lastWidth
                || this.mesh.imageHeight !== this.lastHeight;


            this.wasLayersAvailable = isLayersAvailable;
            this.wasVisible = isVisible;
            this.lastImage = this.mesh.material.map.image;
            this.lastWidth = this.mesh.imageWidth;
            this.lastHeight = this.mesh.imageHeight;


            if (layersAvailableChanged || visibleChanged || sizeChanged) {
                if ((!useLayer || sizeChanged) && this.layer) {
                    this.env.removeWebXRLayer(this.layer);
                    this.layer.destroy();
                    this.layer = null;
                    if (this.visible) {
                        this.mesh.visible = true;
                    }
                }

                if (useLayer) {
                    const space = this.env.renderer.xr.getReferenceSpace();

                    this.layer = this.env.xrBinding.createQuadLayer({
                        space,
                        layout: "mono",
                        textureType: "texture",
                        isStatic: this.isStatic,
                        viewPixelWidth: this.mesh.imageWidth,
                        viewPixelHeight: this.mesh.imageHeight
                    });

                    this.env.addWebXRLayer(this.layer, 500);
                    this.mesh.visible = false;
                }
            }

            if (this.layer) {
                if (imageChanged || this.layer.needsRedraw) {
                    const gl = this.env.renderer.getContext();
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
                    this.layer.transform = new XRRigidTransform(P, Q);
                    this.layer.width = S.x / 2;
                    this.layer.height = S.y / 2;
                }
            }
        }
    }
}

