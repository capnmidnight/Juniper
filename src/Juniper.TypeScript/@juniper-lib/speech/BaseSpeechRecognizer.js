import { debounce } from "@juniper-lib/events/debounce";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { SpeechRecognizer } from "microsoft-cognitiveservices-speech-sdk";
export class BaseSpeechRecognizer extends TypedEventTarget {
    constructor() {
        super();
        this.endEvt = new TypedEvent("end");
        this.audioEndEvt = new TypedEvent("audioend");
        this.audioStartEvt = new TypedEvent("audiostart");
        this.soundEndEvt = new TypedEvent("soundend");
        this.soundStartEvt = new TypedEvent("soundstart");
        this.speechEndEvt = new TypedEvent("speechdend");
        this.speechStartEvt = new TypedEvent("speechstart");
        this.startEvt = new TypedEvent("start");
        this.refresh = debounce(() => this.onRefresh());
    }
    get speakerCulture() {
        return this._speakerCulture;
    }
    set speakerCulture(v) {
        if (v !== this.speakerCulture) {
            this._speakerCulture = v;
            this.refresh();
        }
    }
    get targetCulture() {
        return this._targetCulture;
    }
    set targetCulture(v) {
        if (v !== this.targetCulture) {
            this._targetCulture = v;
            this.refresh();
        }
    }
    onRefresh() {
    }
}
SpeechRecognizer.enableTelemetry(false);
//# sourceMappingURL=BaseSpeechRecognizer.js.map