import { IDisposable } from "@juniper-lib/util";
import { ActivityDetector, LocalUserMicrophone } from "@juniper-lib/audio";
import { BaseSpeechRecognizer } from "./BaseSpeechRecognizer";
export declare class AzureSpeechRecognizer extends BaseSpeechRecognizer implements IDisposable {
    static readonly isAvailable = true;
    private readonly speechConfig;
    private readonly audioConfig;
    private readonly recorder;
    private readonly onRecognizing;
    private readonly onRecognized;
    private readonly onCanceled;
    private readonly onSpeechStartDetected;
    private readonly onSpeechEndDetected;
    private readonly onSessionStarted;
    private readonly onSessionStopped;
    private recognizer;
    private counter;
    private curId;
    private disposed;
    private started;
    private aborting;
    continuous: boolean;
    private log;
    constructor(subscriptionKey: string, region: string, mics: LocalUserMicrophone, activity: ActivityDetector);
    protected onRefresh(): void;
    private onResult;
    private onAbort;
    dispose(): void;
    start(): void;
    stop(): void;
    abort(): void;
}
//# sourceMappingURL=AzureSpeechRecognizer.d.ts.map