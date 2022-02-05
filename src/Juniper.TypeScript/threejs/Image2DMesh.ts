import { arrayCompare, IDisposable, isDefined } from "juniper-tslib";
import { cleanup } from "./cleanup";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { hasWebXRLayers } from "./hasWebXRLayers";
import { solid } from "./materials";
import { objectGetRelativePose } from "./objectGetRelativePose";
import { objectIsFullyVisible } from "./objects";
import { plane } from "./Plane";
import { TexturedMesh } from "./TexturedMesh";

const P = new THREE.Vector4();
const Q = new THREE.Quaternion();
const S = new THREE.Vector3();

let copyCounter = 0;

export class Image2DMesh extends THREE.Object3D implements IDisposable {
    private readonly lastMatrixWorld = new THREE.Matrix4();
    private layer: XRQuadLayer = null;
    private wasVisible = false;
    private webXRLayerEnabled = true;
    private wasWebXRLayerAvailable: boolean = null;
    protected env: BaseEnvironment<unknown> = null;
    mesh: TexturedMesh = null;

    constructor(env: BaseEnvironment<unknown>, name: string, materialOptions: THREE.MeshBasicMaterialParameters = null) {
        super();

        if (env) {
            this.setEnvAndName(env, name);
            this.mesh = new TexturedMesh(this.env, plane, solid(Object.assign(
                { transparent: true, opacity: 1 },
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
        this.webXRLayerEnabled &&= hasWebXRLayers(this.env);
    }

    override copy(source: this, recursive = true) {
        super.copy(source, recursive);
        this.setEnvAndName(source.env, source.name + (++copyCounter));
        this.mesh = source.mesh.clone();
        this.add(this.mesh);
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
                if (this.layer.needsRedraw
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
        }
    }
}

