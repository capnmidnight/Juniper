import { scaleOnHover } from "../animation/scaleOnHover";
import { makeRayTarget, RayTarget } from "../eventSystem/RayTarget";
import { Image2DMesh } from "../Image2DMesh";
import { IUpdatable } from "../IUpdatable";

export class MenuItem
    extends THREE.Object3D
    implements IUpdatable {
    startX: number = 0;

    useWebXRLayers = false;
    readonly target: RayTarget;

    constructor(width: number, height: number,
        name: string,
        public front: THREE.Object3D,
        public back: Image2DMesh,
        public isClickable: boolean,
        enabled: boolean) {
        super();

        this.name = `MenuItem-${name}`;

        if (this.front) {
            this.front = front;
            this.front.renderOrder = 5;
            this.add(this.front);
        }

        if (this.back) {
            this.back = back;
            this.back.renderOrder = 0;
            this.back.position.z = -0.125;
            this.back.scale.x = width;
            this.back.scale.y = height;
            this.add(this.back);
        }

        this.target = makeRayTarget(this.back.mesh, this);
        this.target.enabled = enabled;

        if (this.isClickable) {
            scaleOnHover(this);
        }
    }

    update(dt: number, frame?: XRFrame) {
        if (this.useWebXRLayers) {
            if (this.back instanceof Image2DMesh) {
                this.back.update(dt, frame);
            }

            if (this.front instanceof Image2DMesh) {
                this.front.update(dt, frame);
            }
        }
    }

    get width() {
        return this.back.scale.x;
    }

    get height() {
        return this.back.scale.y;
    }
}