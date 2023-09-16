import { Vector3 } from "three";
import { PointerID } from "../Pointers";
import { BasePointer } from "./BasePointer";
export class PointerNose extends BasePointer {
    constructor(env) {
        super("nose", PointerID.Nose, env, null);
        this.point = new Vector3();
        this.lastPoint = new Vector3();
        this.mayTeleport = false;
        this.enabled = true;
        this._isActive = true;
        this.updatePointerOrientation();
        this.moveDistance = 0;
        Object.seal(this);
    }
    vibrate() {
        // do nothing
    }
    get canSend() {
        return false;
    }
    updatePointerOrientation() {
        const camera = this.env.camera;
        camera.getWorldPosition(this.origin);
        camera.getWorldDirection(this.direction);
        this.up.copy(camera.up);
        this.point.copy(this.direction)
            .add(this.origin);
        this.moveDistance = this.point.distanceTo(this.lastPoint);
        this.lastPoint.copy(this.point);
    }
}
//# sourceMappingURL=PointerNose.js.map