import { getMonospaceFonts, rgb } from "@juniper-lib/dom/dist/css";
import { isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
import { TextImage } from "./TextImage";
export class ClockImage extends TextImage {
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
        const updater = this.update.bind(this);
        setInterval(updater, 500);
        updater();
    }
    update() {
        const time = new Date();
        const value = time.toLocaleTimeString();
        if (isNullOrUndefined(this.value)
            || value.length !== this.value.length) {
            this.unfreeze();
        }
        this.value = value;
    }
}
//# sourceMappingURL=ClockImage.js.map