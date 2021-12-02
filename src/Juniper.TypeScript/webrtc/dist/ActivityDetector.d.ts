import { ErsatzAudioNode } from "juniper-audio";
export declare class ActivityDetector implements ErsatzAudioNode {
    private readonly name;
    private _level;
    private maxLevel;
    private analyzer;
    private buffer;
    constructor(name: string, audioCtx: AudioContext);
    dispose(): void;
    get level(): number;
    get input(): AnalyserNode;
    get output(): AnalyserNode;
}
