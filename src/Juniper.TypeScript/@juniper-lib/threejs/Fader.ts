import { clamp, once, TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import { BackSide, Mesh, MeshBasicMaterial } from "three";
import { cube as geom } from "./Cube";
import { solidTransparent } from "./materials";
import { ErsatzObject, mesh } from "./objects";

interface FaderEvents {
    fadecomplete: TypedEvent<"fadecomplete">;
}

const completeEvt = new TypedEvent("fadecomplete");

export class Fader extends TypedEventBase<FaderEvents>
    implements ErsatzObject {
    opacity = 1;
    direction = 0;

    speed: number;

    readonly object: Mesh;
    private readonly material: MeshBasicMaterial;

    constructor(name: string, t = 0.15) {
        super();

        this.material = solidTransparent({
            name: "FaderMaterial",
            color: 0x000000,
            side: BackSide
        });
        this.object = mesh(name, geom, this.material);
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

        this.material.opacity = this.opacity;
        this.material.transparent = this.opacity < 1;
        this.material.needsUpdate = true;
    }
}
