import { IDisposable } from "@juniper-lib/util";
import { Color, Intersection } from "three";
import { InstancedButtonFactory } from ".";
import { scaleOnHover } from "../../animation/scaleOnHover";
import { RayTarget } from "../../eventSystem/RayTarget";
import { obj } from "../../objects";

export class InstancedMeshButton extends RayTarget
    implements IDisposable {

    private _instanceId: number = null;
    get instanceId() { return this._instanceId; }
    set instanceId(v) {
        this.removeMesh(this.parent.enabledInstances);//, this.instanceId);
        this.removeMesh(this.parent.disabledInstances);//, this.instanceId);
        this._instanceId = v;
        this.addMesh(this.parent.enabledInstances);//, this.instanceId);
        this.addMesh(this.parent.disabledInstances);//, this.instanceId);
    }

    constructor(private readonly parent: InstancedButtonFactory, name: string, size: number, readonly icon: Color) {
        super(obj(name));

        this.enabled = true;
        this.clickable = true;
        this.size = size;

        scaleOnHover(this, true);
    }

    precheck(hit: Intersection) {
        const targetMesh = this.enabled
            ? this.parent.enabledInstances
            : this.parent.disabledInstances;

        console.log(targetMesh.name, hit.object.name);

        return hit.instanceId === this.instanceId
            && hit.object === targetMesh;
    }

    dispose(): void {
        this.parent.deleteButton(this);
    }

    get size(): number {
        return this.content3d.scale.x;
    }

    set size(v: number) {
        this.content3d.scale.setScalar(v);
    }
}