import { BlobAvailableEvent } from "@juniper-lib/audio";
import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
export declare class SpeechRecognizerErrorEvent extends TypedEvent<"error"> implements SpeechRecognitionErrorEvent {
    readonly id: number;
    readonly error: SpeechRecognitionError;
    readonly message: string;
    constructor(id: number, error: SpeechRecognitionError, message: string);
}
export declare class SpeechRecognizerResultEvent extends TypedEvent<"result"> {
    readonly id: number;
    readonly culture: Culture;
    readonly results: string;
    readonly isFinal: boolean;
    constructor(id: number, culture: Culture, results: string, isFinal: boolean);
}
export declare class SpeechRecognizerNoMatchEvent extends TypedEvent<"nomatch"> {
    readonly id: number;
    constructor(id: number);
}
export type SpeechRecognizerEvents = {
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
};
export interface ISpeechRecognizer extends TypedEventTarget<SpeechRecognizerEvents> {
    speakerCulture: Culture;
    targetCulture: Culture;
    continuous: boolean;
    abort(): void;
    start(): void;
    stop(): void;
}
//# sourceMappingURL=ISpeechRecognizer.d.ts.map