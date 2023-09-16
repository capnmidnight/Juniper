import { CanvasImage } from "./CanvasImage";
export declare class ArtificialHorizon extends CanvasImage {
    private _pitchDegrees;
    private _headingDegrees;
    constructor();
    get pitchDegrees(): number;
    set pitchDegrees(v: number);
    get headingDegrees(): number;
    set headingDegrees(v: number);
    setPitchAndHeading(pitchDegrees: number, headingDegrees: number): void;
    protected onRedraw(): boolean;
}
//# sourceMappingURL=ArtificialHorizon.d.ts.map