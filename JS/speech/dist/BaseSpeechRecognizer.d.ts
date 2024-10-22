import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/dist/TypedEventTarget";
import { ISpeechRecognizer, SpeechRecognizerEvents } from "./ISpeechRecognizer";
export declare abstract class BaseSpeechRecognizer extends TypedEventTarget<SpeechRecognizerEvents> implements ISpeechRecognizer {
    protected readonly endEvt: TypedEvent<"end">;
    protected readonly audioEndEvt: TypedEvent<"audioend">;
    protected readonly audioStartEvt: TypedEvent<"audiostart">;
    protected readonly soundEndEvt: TypedEvent<"soundend">;
    protected readonly soundStartEvt: TypedEvent<"soundstart">;
    protected readonly speechEndEvt: TypedEvent<"speechdend">;
    protected readonly speechStartEvt: TypedEvent<"speechstart">;
    protected readonly startEvt: TypedEvent<"start">;
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