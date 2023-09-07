import { AudioRecordingNode } from "@juniper-lib/audio/AudioRecordingNode";
import { IFetcher } from "@juniper-lib/fetcher/IFetcher";
import { IResponse } from "@juniper-lib/fetcher/IResponse";
import { BaseCustomSpeechRecognizer, RecognitionResult } from "./BaseCustomSpeechRecognizer";
export declare class JuniperSpeechRecognizer extends BaseCustomSpeechRecognizer {
    private readonly fetcher;
    private readonly postPath;
    constructor(fetcher: IFetcher, postPath: URL | string, recorder: AudioRecordingNode);
    protected getResult(blob: Blob): Promise<IResponse<RecognitionResult>>;
}
//# sourceMappingURL=JuniperSpeechRecognizer.d.ts.map