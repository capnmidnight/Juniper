import { TypedEvent } from "@juniper-lib/events";
export class SpeechRecognizerErrorEvent extends TypedEvent {
    constructor(id, error, message) {
        super("error");
        this.id = id;
        this.error = error;
        this.message = message;
    }
}
export class SpeechRecognizerResultEvent extends TypedEvent {
    constructor(id, culture, results, isFinal) {
        super("result");
        this.id = id;
        this.culture = culture;
        this.results = results;
        this.isFinal = isFinal;
    }
}
export class SpeechRecognizerNoMatchEvent extends TypedEvent {
    constructor(id) {
        super("nomatch");
        this.id = id;
    }
}
//# sourceMappingURL=ISpeechRecognizer.js.map