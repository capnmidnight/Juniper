import { BaseSpeechRecognizer } from "./BaseSpeechRecognizer";
export declare class WebSpeechRecognizer extends BaseSpeechRecognizer {
    private static readonly Recognition;
    static get isAvailable(): boolean;
    private readonly recognizer;
    private _running;
    get running(): boolean;
    constructor();
    protected onRefresh(): void;
    get continuous(): boolean;
    set continuous(v: boolean);
    abort(): void;
    start(): void;
    stop(): void;
}
//# sourceMappingURL=WebSpeechRecognizer.d.ts.map