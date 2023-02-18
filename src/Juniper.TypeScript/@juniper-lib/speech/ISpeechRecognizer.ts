import { BlobAvailableEvent } from "@juniper-lib/audio/AudioRecordingNode";
import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";

export class SpeechRecognizerErrorEvent
    extends TypedEvent<"error">
    implements SpeechRecognitionErrorEvent {
    constructor(public readonly id: number, public readonly error: SpeechRecognitionError, public readonly message: string) {
        super("error");
    }
}

export class SpeechRecognizerResultEvent
    extends TypedEvent<"result"> {
    constructor(
        public readonly id: number,
        public readonly culture: Culture,
        public readonly results: string,
        public readonly isFinal: boolean) {
        super("result");
    }
}

export class SpeechRecognizerNoMatchEvent
    extends TypedEvent<"nomatch"> {
    constructor(public readonly id: number) {
        super("nomatch");
    }
}

export interface SpeechRecognizerEvents {
    end: TypedEvent<"end">;
    error: SpeechRecognizerErrorEvent;
    nomatch: SpeechRecognizerNoMatchEvent;
    result: SpeechRecognizerResultEvent;
    audioend: TypedEvent<"audioend">;
    audiostart: TypedEvent<"audiostart">;
    soundend: TypedEvent<"soundend">;
    soundstart: TypedEvent<"soundstart">;
    speechend: TypedEvent<"speechend">;
    speechstart: TypedEvent<"speechstart">;
    start: TypedEvent<"start">;
    blobavailable: BlobAvailableEvent;
}


export interface ISpeechRecognizer extends TypedEventBase<SpeechRecognizerEvents> {
    speakerCulture: Culture;
    targetCulture: Culture;
    continuous: boolean;

    abort(): void;
    start(): void;
    stop(): void;
}


