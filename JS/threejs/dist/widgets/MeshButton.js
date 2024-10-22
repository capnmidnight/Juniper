import { stringRandom } from "@juniper-lib/tslib/dist/strings/stringRandom";
import { scaleOnHover } from "../animation/scaleOnHover";
import { RayTarget } from "../eventSystem/RayTarget";
import { mesh, obj, objGraph } from "../objects";
export class MeshButton extends RayTarget {
    constructor(name, geometry, enabledMaterial, disabledMaterial, size) {
        name = name + stringRandom(16);
        super(obj(name));
        this.enabledMaterial = enabledMaterial;
        this.disabledMaterial = disabledMaterial;
        this.mesh = mesh(`Mesh-${name}-enabled`, geometry, enabledMaterial);
        this.size = size;
        objGraph(this, this.mesh);
        this.addMesh(this.mesh);
        this.clickable = true;
        this.disabled = this.disabled;
        scaleOnHover(this, true);
    }
    get size() {
        return this.mesh.scale.x;
    }
    set size(v) {
        this.mesh.scale.setScalar(v);
    }
    get disabled() {
        return super.disabled;
    }
    set disabled(v) {
        super.disabled = v;
        this.mesh.material = v
            ? this.disabledMaterial
            : this.enabledMaterial;
    }
}
//# sourceMappingURL=MeshButton.js.map