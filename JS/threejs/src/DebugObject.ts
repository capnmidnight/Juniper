import { isDefined } from "@juniper-lib/util";
import { ColorRepresentation, Object3D } from "three";
import { Cube } from "./Cube";
import { lit, solidBlue, solidGreen, solidRed } from "./materials";
import { objGraph } from "./objects";

export class DebugObject extends Object3D {
    private center: Cube = null;
    private xp: Cube;
    private yp: Cube;
    private zn: Cube;

    constructor(private color?: ColorRepresentation) {
        super();

        if (isDefined(this.color)) {
            this.center = new Cube(0.5, 0.5, 0.5, lit({ color: this.color }));
            objGraph(this, this.center);
        }

        this.xp = new Cube(1.0, 0.1, 0.1, solidRed);
        this.yp = new Cube(0.1, 1.0, 0.1, solidGreen);
        this.zn = new Cube(0.1, 0.1, 1.0, solidBlue);

        this.xp.position.x = 1;
        this.yp.position.y = 1;
        this.zn.position.z = -1;

        objGraph(this, this.xp, this.yp, this.zn);
    }

    override copy(source: this, recursive = true): this {
        super.copy(source, recursive);
        this.color = source.color;
        this.center = new Cube(0.5, 0.5, 0.5, lit({ color: source.color }));
        return this;
    }

    get size(): number {
        return this.xp.scale.x;
    }

    set size(v: number) {
        if (isDefined(this.center)) {
            this.center.scale.setScalar(v);
        }

        this.xp.scale.setScalar(0.1 * v);
        this.yp.scale.setScalar(0.1 * v);
        this.zn.scale.setScalar(0.1 * v);

        this.xp.scale.x
            = this.yp.scale.y
            = this.zn.scale.z
            = v;

        this.xp.position.x
            = this.yp.position.y
            = v;

        this.zn.position.z
            = -v;
    }
}