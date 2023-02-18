import { debounce } from "@juniper-lib/tslib/events/debounce";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { SpeechRecognizer } from "microsoft-cognitiveservices-speech-sdk";
import { ISpeechRecognizer, SpeechRecognizerEvents } from "./ISpeechRecognizer";

export abstract class BaseSpeechRecognizer
    extends TypedEventBase<SpeechRecognizerEvents>
    implements ISpeechRecognizer {

    protected readonly endEvt = new TypedEvent("end");
    protected readonly audioEndEvt = new TypedEvent("audioend");
    protected readonly audioStartEvt = new TypedEvent("audiostart");
    protected readonly soundEndEvt = new TypedEvent("soundend");
    protected readonly soundStartEvt = new TypedEvent("soundstart");
    protected readonly speechEndEvt = new TypedEvent("speechdend");
    protected readonly speechStartEvt = new TypedEvent("speechstart");
    protected readonly startEvt = new TypedEvent("start");

    private refresh: () => void;

    constructor() {
        super();
        this.refresh = debounce(() => this.onRefresh());
    }

    private _speakerCulture: Culture;

    get speakerCulture(): Culture {
        return this._speakerCulture;
    }

    set speakerCulture(v: Culture) {
        if (v !== this.speakerCulture) {
            this._speakerCulture = v;
            this.refresh();
        }
    }

    private _targetCulture: Culture;

    get targetCulture(): Culture {
        return this._targetCulture;
    }

    set targetCulture(v: Culture) {
        if (v !== this.targetCulture) {
            this._targetCulture = v;
            this.refresh();
        }
    }

    protected onRefresh(): void {
    }
    
    abstract continuous: boolean;
    abstract abort(): void;
    abstract start(): void;
    abstract stop(): void;
}

SpeechRecognizer.enableTelemetry(false);

