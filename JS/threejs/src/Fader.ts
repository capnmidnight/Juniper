import { Task } from "@juniper-lib/events";
import { clamp } from "@juniper-lib/util";
import { BackSide, Mesh, MeshBasicMaterial } from "three";
import { cube } from "./Cube";
import { solidTransparent } from "./materials";
import { ErsatzObject } from "./objects";

export class Fader implements ErsatzObject {
    opacity = 1;
    direction = 0;

    speed: number;

    readonly content3d: Mesh;
    private readonly material: MeshBasicMaterial;
    private readonly task = new Task(false);

    constructor(name: string, t = 0.15) {
        this.material = solidTransparent({
            name: "FaderMaterial",
            color: 0x000000,
            side: BackSide
        });
        this.content3d = cube(name, 1, 1, 1, this.material);
        this.content3d.renderOrder = Number.MAX_SAFE_INTEGER;
        this.speed = 1 / t;
        this.content3d.layers.enableAll();
    }

    private async start(direction: number) {
        this.direction = direction;
        this.task.restart();
        await this.task;
    }

    async fadeOut() {
        if (this.direction != 1) {
            await this.start(1);
        }
    }

    async fadeIn() {
        if (this.direction != -1) {
            await this.start(-1);
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
                this.task.resolve();
            }
        }

        this.material.opacity = this.opacity;
        this.material.transparent = this.opacity < 1;
        this.material.needsUpdate = true;
    }
}
