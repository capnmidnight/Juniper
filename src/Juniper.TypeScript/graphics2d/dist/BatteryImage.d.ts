import { CanvasImage } from "juniper-dom";
export declare class BatteryImage extends CanvasImage<void> {
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
