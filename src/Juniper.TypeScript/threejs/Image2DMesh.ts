import { arrayCompare, IDisposable, isDefined, isNullOrUndefined } from "juniper-tslib";
import { cleanup } from "./cleanup";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { solidTransparent } from "./materials";
import { objectGetRelativePose } from "./objectGetRelativePose";
import { objectIsFullyVisible } from "./objects";
import { plane } from "./Plane";
import { TexturedMesh } from "./TexturedMesh";
import { isMesh } from "./typeChecks";

const P = new THREE.Vector4();
const Q = new THREE.Quaternion();
const S = new THREE.Vector3();

const IMAGE_2D_MESH_SLUG = "XXX_IMAGE_2D_MESH_XXX";

let copyCounter = 0;

export class Image2DMesh extends THREE.Object3D implements IDisposable {
    private readonly lastMatrixWorld = new THREE.Matrix4();
    private layer: XRQuadLayer = null;
    private wasVisible = false;
    private webXRLayerEnabled = true;
    private wasWebXRLayerAvailable: boolean = null;
    private lastImage: any = null;
    protected env: BaseEnvironment<unknown> = null;
    mesh: TexturedMesh = null;

    constructor(env: BaseEnvironment<unknown>, name: string, materialOptions: THREE.MeshBasicMaterialParameters = null) {
        super();

        if (env) {
            this.setEnvAndName(env, name);
            this.mesh = new TexturedMesh(this.env, plane, solidTransparent(Object.assign(
                {},
                materialOptions,
                { name: this.name })));
            this.mesh.name = IMAGE_2D_MESH_SLUG;
            this.add(this.mesh);
        }
    }

    dispose(): void {
        cleanup(this.layer);
    }

    private setEnvAndName(env: BaseEnvironment<unknown>, name: string) {
        this.env = env;
        this.name = name;
        this.webXRLayerEnabled &&= this.env.hasWebXRLayers;
    }

    override copy(source: this, recursive = true) {
        super.copy(source, recursive);
        this.setEnvAndName(source.env, source.name + (++copyCounter));
        for (let i = this.children.length - 1; i >= 0; --i) {
            const child = this.children[i];
            if (isMesh(child)
                && child.name === IMAGE_2D_MESH_SLUG) {
                child.removeFromParent();
                this.mesh = new TexturedMesh(source.env, child.geometry, child.material as any);
            }
        }
        if (isNullOrUndefined(this.mesh)) {
            this.mesh = source.mesh.clone();
            this.add(this.mesh);
        }
        return this;
    }

    checkLayer(frame: XRFrame): void {
        if (this.mesh.material.map.image) {
            const isVisible = objectIsFullyVisible(this);
            const binding = (this.env.renderer.xr as any).getBinding() as XRWebGLBinding;
            const isWebXRLayerAvailable = this.webXRLayerEnabled
                && this.env.renderer.xr.isPresenting
                && isDefined(frame)
                && isDefined(binding);

            const useLayerChanged = isWebXRLayerAvailable !== this.wasWebXRLayerAvailable;
            const visibleChanged = isVisible != this.wasVisible;
            const imageChanged = this.mesh.material.map.image !== this.lastImage;

            if (useLayerChanged || visibleChanged) {
                if (isWebXRLayerAvailable && isVisible) {
                    const space = this.env.renderer.xr.getReferenceSpace();

                    this.layer = binding.createQuadLayer({
                        space,
                        layout: "mono",
                        textureType: "texture",
                        isStatic: false,
                        viewPixelWidth: this.mesh.imageWidth,
                        viewPixelHeight: this.mesh.imageHeight
                    });

                    this.env.addWebXRLayer(this.layer, 500);
                    this.mesh.visible = false;
                }
                else if (this.layer) {
                    this.env.removeWebXRLayer(this.layer);
                    this.layer.destroy();
                    this.layer = null;
                    if (this.visible) {
                        this.mesh.visible = true;
                    }
                }
            }

            if (this.layer) {
                if (imageChanged
                    || this.layer.needsRedraw
                    || this.mesh.material.needsUpdate
                    || this.mesh.material.map.needsUpdate) {
                    const gl = this.env.renderer.getContext();
                    const gLayer = binding.getSubImage(this.layer, frame);

                    gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, true);
                    gl.bindTexture(gl.TEXTURE_2D, gLayer.colorTexture);
                    gl.texSubImage2D(
                        gl.TEXTURE_2D,
                        0,
                        0, 0,
                        gl.RGBA,
                        gl.UNSIGNED_BYTE,
                        this.mesh.material.map.image);

                    gl.bindTexture(gl.TEXTURE_2D, null);
                }

                if (arrayCompare(this.matrixWorld.elements, this.lastMatrixWorld.elements) >= 0) {
                    objectGetRelativePose(this.env.stage, this.mesh, P, Q, S);
                    this.layer.transform = new XRRigidTransform(P, Q);
                    this.layer.width = S.x / 2;
                    this.layer.height = S.y / 2;
                }
            }

            this.wasWebXRLayerAvailable = isWebXRLayerAvailable;
            this.wasVisible = isVisible;
            this.lastImage = this.mesh.material.map.image;
        }
    }
}

