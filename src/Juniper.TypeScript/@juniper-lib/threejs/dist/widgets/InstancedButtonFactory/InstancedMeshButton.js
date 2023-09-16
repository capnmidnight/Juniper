import { scaleOnHover } from "../../animation/scaleOnHover";
import { RayTarget } from "../../eventSystem/RayTarget";
import { obj } from "../../objects";
export class InstancedMeshButton extends RayTarget {
    get instanceId() { return this._instanceId; }
    set instanceId(v) {
        this.removeMesh(this.parent.enabledInstances); //, this.instanceId);
        this.removeMesh(this.parent.disabledInstances); //, this.instanceId);
        this._instanceId = v;
        this.addMesh(this.parent.enabledInstances); //, this.instanceId);
        this.addMesh(this.parent.disabledInstances); //, this.instanceId);
    }
    constructor(parent, name, size, icon) {
        super(obj(name));
        this.parent = parent;
        this.icon = icon;
        this._instanceId = null;
        this.enabled = true;
        this.clickable = true;
        this.size = size;
        scaleOnHover(this, true);
    }
    precheck(hit) {
        const targetMesh = this.enabled
            ? this.parent.enabledInstances
            : this.parent.disabledInstances;
        console.log(targetMesh.name, hit.object.name);
        return hit.instanceId === this.instanceId
            && hit.object === targetMesh;
    }
    dispose() {
        this.parent.deleteButton(this);
    }
    get size() {
        return this.object.scale.x;
    }
    set size(v) {
        this.object.scale.setScalar(v);
    }
}
//# sourceMappingURL=InstancedMeshButton.js.map