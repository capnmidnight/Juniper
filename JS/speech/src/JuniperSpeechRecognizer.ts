import { IResponse } from "@juniper-lib/util";
import { AudioRecordingNode } from "@juniper-lib/audio";
import { IFetcher } from "@juniper-lib/fetcher";
import { BaseCustomSpeechRecognizer, RecognitionResult } from "./BaseCustomSpeechRecognizer";


export class JuniperSpeechRecognizer extends BaseCustomSpeechRecognizer {
    constructor(private readonly fetcher: IFetcher, private readonly postPath: URL | string, recorder: AudioRecordingNode) {
        super(recorder);
    }

    protected async getResult(blob: Blob): Promise<IResponse<RecognitionResult>> {
        return await this.fetcher.post(this.postPath)
            .body({
                formFile: blob,
                speakerCulture: this.speakerCulture,
                targetCulture: this.targetCulture
            })
            .object<RecognitionResult>();
    }
}