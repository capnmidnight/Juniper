import { clamp, isArray, once, TypedEvent, TypedEventBase } from "juniper-tslib";
import { cube as geom } from "./Cube";
import { solid } from "./materials";
import { ErsatzObject } from "./objects";

interface FaderEvents {
    fadecomplete: TypedEvent<"fadecomplete">;
}

const completeEvt = new TypedEvent("fadecomplete");

export class Fader extends TypedEventBase<FaderEvents>
    implements ErsatzObject {
    opacity = 1;
    direction = 0;

    speed: number;

    readonly object: THREE.Mesh;

    constructor(name: string, t = 0.15, color: (THREE.Color | number) = 0x000000) {
        super();

        this.object = new THREE.Mesh(geom, solid({
            name: "FaderMaterial",
            color,
            side: THREE.BackSide
        }));

        this.object.name = name;
        this.object.renderOrder = Number.MAX_VALUE;
        this.speed = 1 / t;
        this.object.layers.enableAll();
    }

    async fadeOut() {
        if (this.direction != 1) {
            this.direction = 1;
            await once(this, "fadecomplete");
        }
    }

    async fadeIn() {
        if (this.direction != -1) {
            this.direction = -1;
            await once(this, "fadecomplete");
        }
    }

    update(dt: number) {
        if (this.direction !== 0) {
            const dOpacity = this.direction * this.speed * dt / 1000;
            if (0 <= this.opacity && this.opacity <= 1) {
                this.opacity += dOpacity;
            }

            if (this.direction === 1 && this.opacity >= 1
                || this.direction === -1 && this.opacity <= 0) {
                this.opacity = clamp(this.opacity, 0, 1);
                this.direction = 0;
                this.dispatchEvent(completeEvt);
            }
        }

        if (!isArray(this.object.material)) {
            this.object.material.opacity = this.opacity;
            this.object.material.transparent = this.opacity < 1;
        }
    }
}
