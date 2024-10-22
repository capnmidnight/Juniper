import { isDefined } from "@juniper-lib/util";
import { Mat4, Quat } from "gl-matrix";
const origin = { x: 0, y: 0, z: 0, w: 1 };
const direction = { x: 0, y: 0, z: 0, w: 0 };
const dir = new Quat();
export class Mesh {
    constructor(gl, geom, texture, material) {
        this.gl = gl;
        this.geom = geom;
        this.texture = texture;
        this.material = material;
        this.visible = true;
        this.model = new Mat4().identity();
    }
    render(cam, frame, baseRefSpace) {
        if (this.visible) {
            const material = this.material;
            material.setModel(this.model);
            material.setGeometry(this.geom);
            material.setTexture(this.texture);
            if (isDefined(frame)) {
                origin.x = cam.view[12];
                origin.y = cam.view[13];
                origin.z = cam.view[14];
                Mat4.getRotation(dir, cam.view);
                direction.x = dir[0];
                direction.y = dir[1];
                direction.z = dir[2];
                direction.w = dir[3];
                const originOffset = new XRRigidTransform(origin, direction);
                const refSpace = baseRefSpace.getOffsetReferenceSpace(originOffset);
                const pose = frame.getViewerPose(refSpace);
                for (const view of pose.views) {
                    material.setEye(view.eye, this.texture.isStereo);
                    material.setView(view.transform.inverse.matrix);
                    material.setProjection(view.projectionMatrix);
                    const viewport = frame.session.renderState.baseLayer.getViewport(view);
                    this.gl.viewport(viewport.x, viewport.y, viewport.width, viewport.height);
                    this.geom.drawElements();
                }
            }
            else {
                material.setEye("none", false);
                material.setView(cam.view);
                material.setProjection(cam.projection);
                this.gl.viewport(0, 0, this.gl.canvas.width, this.gl.canvas.height);
                this.geom.drawElements();
            }
        }
    }
}
//# sourceMappingURL=Mesh.js.map