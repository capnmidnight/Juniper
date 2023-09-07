import { AudioRecordingNode } from "@juniper-lib/audio/AudioRecordingNode";
import { IResponse } from "@juniper-lib/fetcher/IResponse";
import { BaseSpeechRecognizer } from "./BaseSpeechRecognizer";
export interface RecognitionResult {
    culture: Culture;
    text: string;
}
export declare abstract class BaseCustomSpeechRecognizer extends BaseSpeechRecognizer {
    private readonly recorder;
    continuous: boolean;
    private state;
    private aborting;
    constructor(recorder: AudioRecordingNode);
    start(): void;
    abort(): void;
    stop(): void;
    protected abstract getResult(blob: Blob): Promise<IResponse<RecognitionResult>>;
}
//# sourceMappingURL=BaseCustomSpeechRecognizer.d.ts.map