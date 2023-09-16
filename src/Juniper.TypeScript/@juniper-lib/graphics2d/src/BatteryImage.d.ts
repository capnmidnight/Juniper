import { CanvasImage } from "./CanvasImage";
export declare class BatteryImage extends CanvasImage {
    static readonly isAvailable: boolean;
    private battery;
    private lastChargeDirection;
    private lastLevel;
    private chargeDirection;
    private level;
    constructor();
    protected onRedraw(): boolean;
    private readBattery;
}
//# sourceMappingURL=BatteryImage.d.ts.map