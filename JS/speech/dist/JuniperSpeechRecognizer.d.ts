import { AudioRecordingNode } from "@juniper-lib/audio/dist/AudioRecordingNode";
import { IFetcher } from "@juniper-lib/fetcher/dist/IFetcher";
import { IResponse } from "@juniper-lib/fetcher/dist/IResponse";
import { BaseCustomSpeechRecognizer, RecognitionResult } from "./BaseCustomSpeechRecognizer";
export declare class JuniperSpeechRecognizer extends BaseCustomSpeechRecognizer {
    private readonly fetcher;
    private readonly postPath;
    constructor(fetcher: IFetcher, postPath: URL | string, recorder: AudioRecordingNode);
    protected getResult(blob: Blob): Promise<IResponse<RecognitionResult>>;
}
//# sourceMappingURL=JuniperSpeechRecognizer.d.ts.map