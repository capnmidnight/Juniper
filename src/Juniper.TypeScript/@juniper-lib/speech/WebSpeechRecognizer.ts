import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { BaseSpeechRecognizer } from "./BaseSpeechRecognizer";
import { SpeechRecognizerErrorEvent, SpeechRecognizerNoMatchEvent, SpeechRecognizerResultEvent } from "./ISpeechRecognizer";


export class WebSpeechRecognizer extends BaseSpeechRecognizer {

    private static Recognition = ((window as any).SpeechRecognition || (window as any).webkitSpeechRecognition) as Constructor<SpeechRecognition, typeof SpeechRecognition>;
    public static get isAvailable() {
        return isDefined(WebSpeechRecognizer.Recognition);
    }

    private readonly recognizer: SpeechRecognition;

    private _running = false;
    get running() { return this._running; }

    constructor() {
        super();
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

        this.recognizer.addEventListener("nomatch", () =>
            this.dispatchEvent(new SpeechRecognizerNoMatchEvent(++curId)));

        this.recognizer.addEventListener("error", (evt) =>
            this.dispatchEvent(new SpeechRecognizerErrorEvent(++curId, evt.error, evt.message)));

        this.recognizer.addEventListener("result", (evt) => {
            const result = evt.results[evt.resultIndex];
            const alternative = result[0];
            console.log("Utterance:", alternative);
            this.dispatchEvent(new SpeechRecognizerResultEvent(++curId, this.targetCulture, alternative.transcript));
        });
    }

    get targetCulture(): Culture {
        return this.recognizer.lang as Culture;
    }

    set targetCulture(v: Culture) {
        this.recognizer.lang = v;
    }

    get continuous(): boolean {
        return this.recognizer.continuous;
    }

    set continuous(v: boolean) {
        this.recognizer.continuous = v;
    }

    abort(): void {
        this.recognizer.abort();
    }

    start(): void {
        this._running = true;
        this.recognizer.start();
    }

    stop(): void {
        this.recognizer.stop();
    }
}

