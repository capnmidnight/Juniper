import { getMonospaceFonts } from "@juniper-lib/dom/css";
import { TextImage } from "./TextImage";

export class ClockImage extends TextImage {
    constructor() {
        super({
            textFillColor: "#ffffff",
            textStrokeColor: "rgba(0, 0, 0, 0.25)",
            textStrokeSize: 0.05,
            fontFamily: getMonospaceFonts(),
            fontSize: 20,
            minHeight: 1,
            maxHeight: 1,
            padding: 0.3,
            wrapWords: false,
            freezeDimensions: true
        });

        const updater = this.update.bind(this);
        setInterval(updater, 500);
        updater();
    }

    private fps: number = null;
    private drawCalls: number = null;
    private triangles: number = null;

    setStats(fps: number, drawCalls: number, triangles: number): void {
        this.fps = fps;
        this.drawCalls = drawCalls;
        this.triangles = triangles;
    }

    private lastLen: number = 0;

    protected update(): void {
        const time = new Date();
        let value = time.toLocaleTimeString();
        if (this.fps !== null) {
            value += ` ${Math.round(this.fps).toFixed(0)}hz ${this.drawCalls}c ${this.triangles}t`;
        }

        if (value.length !== this.lastLen) {
            this.lastLen = value.length;
            this.unfreeze();
        }

        this.value = value;
    }
}

