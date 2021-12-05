import { getMonospaceFonts } from "juniper-dom";
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
    _fps = null;
    get fps() {
        return this._fps;
    }
    set fps(v) {
        if (v !== this.fps) {
            this._fps = v;
            this.update();
        }
    }
    lastLen = 0;
    update() {
        const time = new Date();
        let value = time.toLocaleTimeString();
        if (this.fps !== null) {
            value += ` ${Math.round(this.fps).toFixed(0)}hz`;
        }
        if (value.length !== this.lastLen) {
            this.lastLen = value.length;
            this.unfreeze();
        }
        this.value = value;
    }
}
