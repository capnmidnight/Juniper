import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
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

    abstract lang: string;
    abstract continuous: boolean;
    abstract abort(): void;
    abstract start(): void;
    abstract stop(): void;
}
