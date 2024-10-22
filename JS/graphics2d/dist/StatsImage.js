import { isNullOrUndefined } from "@juniper-lib/util";
import { getMonospaceFonts, rgb } from "@juniper-lib/dom";
import { TextImage } from "./TextImage";
export class StatsImage extends TextImage {
    constructor() {
        super({
            textFillColor: "white",
            textStrokeColor: rgb(0, 0, 0, 0.5),
            textStrokeSize: 0.025,
            fontFamily: getMonospaceFonts(),
            fontSize: 20,
            minHeight: 1,
            maxHeight: 1,
            padding: 0.3,
            freezeDimensions: true
        });
        this.fps = null;
        this.drawCalls = null;
        this.triangles = null;
    }
    setStats(fps, drawCalls, triangles) {
        this.fps = fps;
        this.drawCalls = drawCalls;
        this.triangles = triangles;
        const value = ` ${Math.round(this.fps).toFixed(0)}hz ${this.drawCalls}c ${this.triangles}t`;
        if (isNullOrUndefined(this.value)
            || value.length !== this.value.length) {
            this.unfreeze();
        }
        this.value = value;
    }
}
//# sourceMappingURL=StatsImage.js.map