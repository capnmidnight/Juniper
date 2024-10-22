import { makeFont } from "@juniper-lib/dom";
import { CanvasImage } from "./CanvasImage";
function isBatteryNavigator(nav) {
    return "getBattery" in nav;
}
const chargeLabels = [
    "",
    "N/A",
    "charging"
];
export class BatteryImage extends CanvasImage {
    static { this.isAvailable = isBatteryNavigator(navigator); }
    constructor() {
        super(256, 128);
        this.battery = null;
        this.lastChargeDirection = null;
        this.lastLevel = null;
        this.chargeDirection = 0;
        this.level = 0.5;
        if (isBatteryNavigator(navigator)) {
            this.readBattery(navigator);
        }
        else {
            this.redraw();
        }
    }
    onRedraw() {
        if (this.battery) {
            this.chargeDirection = this.battery.charging ? 1 : -1;
            this.level = this.battery.level;
        }
        else {
            this.level += 0.1;
            if (this.level > 1) {
                this.level = 0;
            }
        }
        const directionChanged = this.chargeDirection !== this.lastChargeDirection;
        const levelChanged = this.level !== this.lastLevel;
        if (!directionChanged && !levelChanged) {
            return false;
        }
        this.lastChargeDirection = this.chargeDirection;
        this.lastLevel = this.level;
        const levelColor = this.level < 0.10
            ? "red"
            : "silver";
        const padding = 7;
        const scale = 0.7;
        const invScale = (1 - scale) / 2;
        const bodyWidth = this.canvas.width - 2 * padding;
        const width = bodyWidth - 4 * padding;
        const height = this.canvas.height - 4 * padding;
        const midX = bodyWidth / 2;
        const midY = this.canvas.height / 2;
        const label = chargeLabels[this.chargeDirection + 1];
        this.g.clearRect(0, 0, bodyWidth, this.canvas.height);
        this.g.save();
        this.g.translate(invScale * this.canvas.width, invScale * this.canvas.height);
        this.g.globalAlpha = 0.75;
        this.g.scale(scale, scale);
        this.fillRect("silver", 0, 0, bodyWidth, this.canvas.height, 0);
        this.fillRect("silver", bodyWidth, midY - 2 * padding - 10, padding + 10, 4 * padding + 20, 0);
        this.g.clearRect(padding, padding, bodyWidth - 2 * padding, this.canvas.height - 2 * padding);
        this.fillRect("black", padding, padding, bodyWidth - 2 * padding, this.canvas.height - 2 * padding, 0);
        this.g.clearRect(2 * padding, 2 * padding, width * this.level, height);
        this.fillRect(levelColor, 2 * padding, 2 * padding, width * this.level, height, 0);
        this.g.fillStyle = "white";
        this.g.strokeStyle = "black";
        this.g.lineWidth = 4;
        this.g.textBaseline = "middle";
        this.g.font = makeFont({
            fontSize: height / 2,
            fontFamily: "Lato"
        });
        this.drawText(label, midX, midY, "center");
        this.g.restore();
        return true;
    }
    async readBattery(navigator) {
        const redraw = this.redraw.bind(this);
        redraw();
        this.battery = await navigator.getBattery();
        this.battery.addEventListener("chargingchange", redraw);
        this.battery.addEventListener("levelchange", redraw);
        setInterval(redraw, 1000);
        redraw();
    }
}
//# sourceMappingURL=BatteryImage.js.map