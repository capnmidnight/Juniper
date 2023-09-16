import { Vector3 } from "three";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { PointerID } from "../Pointers";
import { BasePointer } from "./BasePointer";

export class PointerNose extends BasePointer {
    private readonly point = new Vector3();
    private readonly lastPoint = new Vector3();

    constructor(env: BaseEnvironment) {
        super("nose", PointerID.Nose, env, null);

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

    override get canSend() {
        return false;
    }

    protected updatePointerOrientation() {
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