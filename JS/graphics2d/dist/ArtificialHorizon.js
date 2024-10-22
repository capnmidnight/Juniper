import { rgb } from "@juniper-lib/dom/dist/css";
import { deg2rad, Pi, Tau } from "@juniper-lib/tslib/dist/math";
import { CanvasImage } from "./CanvasImage";
export class ArtificialHorizon extends CanvasImage {
    constructor() {
        super(128, 128);
        this._pitchDegrees = 0;
        this._headingDegrees = 0;
        this.redraw();
    }
    get pitchDegrees() {
        return this._pitchDegrees;
    }
    set pitchDegrees(v) {
        if (v !== this.pitchDegrees) {
            this._pitchDegrees = v;
            this.redraw();
        }
    }
    get headingDegrees() {
        return this._headingDegrees;
    }
    set headingDegrees(v) {
        if (v !== this.headingDegrees) {
            this._headingDegrees = v;
            this.redraw();
        }
    }
    setPitchAndHeading(pitchDegrees, headingDegrees) {
        if (pitchDegrees !== this.pitchDegrees
            || headingDegrees !== this.headingDegrees) {
            this._pitchDegrees = pitchDegrees;
            this._headingDegrees = headingDegrees;
            this.redraw();
        }
    }
    onRedraw() {
        const a = deg2rad(this.pitchDegrees);
        const b = deg2rad(this.headingDegrees - 180);
        const p = 5;
        const w = this.canvas.width - 2 * p;
        const h = this.canvas.height - 2 * p;
        const hw = 0.5 * w;
        const hh = 0.5 * h;
        const y = Math.sin(a);
        const g = this.g;
        g.save();
        {
            g.clearRect(0, 0, this.canvas.width, this.canvas.height);
            g.translate(p, p);
            g.scale(hw, hh);
            g.translate(1, 1);
            g.fillStyle = "gray";
            g.beginPath();
            g.arc(0, 0, 1, 0, Tau);
            g.fill();
            g.fillStyle = "lightgrey";
            g.beginPath();
            g.arc(0, 0, 1, 0, Pi, true);
            g.fill();
            g.save();
            {
                g.scale(1, Math.abs(y));
                if (y < 0) {
                    g.fillStyle = "gray";
                }
                g.beginPath();
                g.arc(0, 0, 1, 0, Pi, y < 0);
                g.fill();
            }
            g.restore();
            g.save();
            {
                g.shadowColor = rgb(64, 64, 64);
                g.shadowBlur = 4;
                g.shadowOffsetX = 3;
                g.shadowOffsetY = 3;
                g.rotate(b);
                g.fillStyle = "red";
                g.beginPath();
                g.moveTo(-0.1, 0);
                g.lineTo(0, -0.667);
                g.lineTo(0.1, 0);
                g.closePath();
                g.fill();
                g.fillStyle = "white";
                g.beginPath();
                g.moveTo(-0.1, 0);
                g.lineTo(0, 0.667);
                g.lineTo(0.1, 0);
                g.closePath();
                g.fill();
            }
            g.restore();
            g.beginPath();
            g.strokeStyle = "black";
            g.lineWidth = 0.1;
            g.arc(0, 0, 1, 0, Tau);
            g.stroke();
        }
        g.restore();
        return true;
    }
}
//# sourceMappingURL=ArtificialHorizon.js.map