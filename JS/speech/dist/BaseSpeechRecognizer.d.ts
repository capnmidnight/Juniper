import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import { ISpeechRecognizer, SpeechRecognizerEvents } from "./ISpeechRecognizer";
export declare abstract class BaseSpeechRecognizer extends TypedEventTarget<SpeechRecognizerEvents> implements ISpeechRecognizer {
    protected readonly endEvt: TypedEvent<"end", EventTarget>;
    protected readonly audioEndEvt: TypedEvent<"audioend", EventTarget>;
    protected readonly audioStartEvt: TypedEvent<"audiostart", EventTarget>;
    protected readonly soundEndEvt: TypedEvent<"soundend", EventTarget>;
    protected readonly soundStartEvt: TypedEvent<"soundstart", EventTarget>;
    protected readonly speechEndEvt: TypedEvent<"speechdend", EventTarget>;
    protected readonly speechStartEvt: TypedEvent<"speechstart", EventTarget>;
    protected readonly startEvt: TypedEvent<"start", EventTarget>;
    private refresh;
    constructor();
    private _speakerCulture;
    get speakerCulture(): Culture;
    set speakerCulture(v: Culture);
    private _targetCulture;
    get targetCulture(): Culture;
    set targetCulture(v: Culture);
    protected onRefresh(): void;
    abstract continuous: boolean;
    abstract abort(): void;
    abstract start(): void;
    abstract stop(): void;
}
//# sourceMappingURL=BaseSpeechRecognizer.d.ts.map