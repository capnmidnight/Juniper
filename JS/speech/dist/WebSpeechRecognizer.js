import { isDefined } from "@juniper-lib/util";
import { BaseSpeechRecognizer } from "./BaseSpeechRecognizer";
import { SpeechRecognizerErrorEvent, SpeechRecognizerNoMatchEvent, SpeechRecognizerResultEvent } from "./ISpeechRecognizer";
export class WebSpeechRecognizer extends BaseSpeechRecognizer {
    static { this.Recognition = (window.SpeechRecognition || window.webkitSpeechRecognition); }
    static get isAvailable() {
        return isDefined(WebSpeechRecognizer.Recognition);
    }
    get running() { return this._running; }
    constructor() {
        super();
        this._running = false;
        this.recognizer = new WebSpeechRecognizer.Recognition();
        this.recognizer.addEventListener("end", () => {
            if (this.continuous) {
                this.start();
            }
            else {
                this._running = false;
                this.dispatchEvent(this.endEvt);
            }
        });
        this.recognizer.addEventListener("start", () => this.dispatchEvent(this.startEvt));
        this.recognizer.addEventListener("audioend", () => this.dispatchEvent(this.audioEndEvt));
        this.recognizer.addEventListener("audiostart", () => this.dispatchEvent(this.audioStartEvt));
        this.recognizer.addEventListener("soundend", () => this.dispatchEvent(this.soundEndEvt));
        this.recognizer.addEventListener("soundstart", () => this.dispatchEvent(this.soundStartEvt));
        this.recognizer.addEventListener("speechend", () => this.dispatchEvent(this.speechEndEvt));
        this.recognizer.addEventListener("speechstart", () => this.dispatchEvent(this.speechStartEvt));
        let curId = 0;
        const noMatch = () => this.dispatchEvent(new SpeechRecognizerNoMatchEvent(++curId));
        this.recognizer.addEventListener("nomatch", noMatch);
        this.recognizer.addEventListener("error", (evt) => this.dispatchEvent(new SpeechRecognizerErrorEvent(++curId, evt.error, evt.message)));
        this.recognizer.addEventListener("result", (evt) => {
            if (evt.results.length === 0
                || evt.results[0].length === 0
                || evt.results[0][0].transcript.length === 0) {
                noMatch();
            }
            else {
                const result = evt.results[evt.resultIndex];
                const alternative = result[0];
                console.log("Utterance:", alternative);
                this.dispatchEvent(new SpeechRecognizerResultEvent(++curId, this.targetCulture, alternative.transcript, true));
            }
        });
    }
    onRefresh() {
        this.recognizer.lang = this.targetCulture || this.speakerCulture;
    }
    get continuous() {
        return this.recognizer.continuous;
    }
    set continuous(v) {
        this.recognizer.continuous = v;
    }
    abort() {
        this.recognizer.abort();
    }
    start() {
        this._running = true;
        this.recognizer.start();
    }
    stop() {
        this.recognizer.stop();
    }
}
//# sourceMappingURL=WebSpeechRecognizer.js.map