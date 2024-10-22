import { BaseCustomSpeechRecognizer } from "./BaseCustomSpeechRecognizer";
export class JuniperSpeechRecognizer extends BaseCustomSpeechRecognizer {
    constructor(fetcher, postPath, recorder) {
        super(recorder);
        this.fetcher = fetcher;
        this.postPath = postPath;
    }
    async getResult(blob) {
        return await this.fetcher.post(this.postPath)
            .body({
            formFile: blob,
            speakerCulture: this.speakerCulture,
            targetCulture: this.targetCulture
        })
            .object();
    }
}
//# sourceMappingURL=JuniperSpeechRecognizer.js.map