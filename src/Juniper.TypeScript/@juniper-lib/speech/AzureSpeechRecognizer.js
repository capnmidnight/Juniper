import { AudioRecordingNode, BlobAvailableEvent } from "@juniper-lib/audio/AudioRecordingNode";
import { identity } from "@juniper-lib/tslib/identity";
import { dispose } from "@juniper-lib/tslib/using";
import { AudioConfig, AutoDetectSourceLanguageConfig, ProfanityOption, ResultReason, SpeechConfig, SpeechRecognizer } from "microsoft-cognitiveservices-speech-sdk";
import { BaseSpeechRecognizer } from "./BaseSpeechRecognizer";
import { SpeechRecognizerErrorEvent, SpeechRecognizerNoMatchEvent, SpeechRecognizerResultEvent } from "./ISpeechRecognizer";
export class AzureSpeechRecognizer extends BaseSpeechRecognizer {
    log(...args) {
        console.log(this.curId, this.recorder.state, ...args);
    }
    constructor(subscriptionKey, region, mics, activity) {
        super();
        this.counter = 0;
        this.curId = null;
        this.disposed = false;
        this.started = false;
        this.aborting = false;
        this.continuous = false;
        this.speechConfig = SpeechConfig.fromSubscription(subscriptionKey, region);
        this.speechConfig.setProfanity(ProfanityOption.Raw);
        this.audioConfig = AudioConfig.fromStreamInput(mics.outStream);
        this.recorder = new AudioRecordingNode(mics.autoGainNode.context, activity);
        mics.connect(this.recorder);
        this.recorder.addEventListener("start", () => {
            this.log("recorder start");
            if (this.started) {
                this.curId = ++this.counter;
            }
        });
        this.recorder.addEventListener("blobavailable", (evt) => {
            this.log("blobavailable", evt.id);
            if (this.started && this.curId !== null) {
                this.dispatchEvent(new BlobAvailableEvent(this.curId, evt.blob));
            }
        });
        this.onSpeechStartDetected = (_, evt) => {
            this.log("speechStartDetected", evt);
            this.dispatchEvent(this.speechStartEvt);
        };
        this.onRecognizing = (_, evt) => {
            this.log("recognizing", evt);
            this.onResult(evt.result, false);
        };
        this.onRecognized = (_, evt) => {
            this.log("recognized", evt);
            this.onResult(evt.result, true);
            this.curId = null;
        };
        this.onCanceled = (_, evt) => {
            this.log("canceled", evt);
            if (this.curId !== null) {
                this.dispatchEvent(new SpeechRecognizerNoMatchEvent(this.curId));
            }
        };
        this.onSpeechEndDetected = (_, evt) => {
            this.log("speechEndDetected", evt);
            this.dispatchEvent(this.speechEndEvt);
        };
        this.onSessionStarted = (_, evt) => {
            this.log("sessionStarted", evt);
            this.dispatchEvent(this.audioStartEvt);
        };
        this.onSessionStopped = (_, evt) => {
            this.log("sessionStopped", evt);
            this.dispatchEvent(this.audioEndEvt);
        };
        this.onRefresh();
    }
    onRefresh() {
        const wasStarted = this.started;
        if (this.recognizer) {
            dispose(this.recognizer);
            this.recognizer = null;
        }
        const languages = [this.targetCulture, this.speakerCulture].filter(identity);
        if (languages.length === 0) {
            this.recognizer = new SpeechRecognizer(this.speechConfig, this.audioConfig);
        }
        else {
            const langConfig = AutoDetectSourceLanguageConfig.fromLanguages(languages);
            this.recognizer = SpeechRecognizer.FromConfig(this.speechConfig, langConfig, this.audioConfig);
        }
        this.recognizer.recognizing = this.onRecognizing;
        this.recognizer.recognized = this.onRecognized;
        this.recognizer.canceled = this.onCanceled;
        this.recognizer.sessionStarted = this.onSessionStarted;
        this.recognizer.sessionStopped = this.onSessionStopped;
        this.recognizer.speechStartDetected = this.onSpeechStartDetected;
        this.recognizer.speechEndDetected = this.onSpeechEndDetected;
        if (wasStarted) {
            this.start();
        }
    }
    onResult(result, isFinal) {
        this.log("result", result.reason, result.language, result.text);
        if (this.curId !== null) {
            if (result.reason === ResultReason.NoMatch) {
                this.dispatchEvent(new SpeechRecognizerNoMatchEvent(this.curId));
            }
            else if (result.reason === ResultReason.Canceled) {
                this.onAbort();
            }
            else if (result.reason === ResultReason.RecognizedSpeech
                || result.reason === ResultReason.RecognizingSpeech) {
                if (result.text && result.text.length > 0) {
                    this.dispatchEvent(new SpeechRecognizerResultEvent(this.curId, result.language, result.text, isFinal));
                }
            }
        }
    }
    onAbort() {
        if (this.curId !== null) {
            this.dispatchEvent(new SpeechRecognizerErrorEvent(this.curId, "aborted", "aborted"));
        }
    }
    dispose() {
        if (!this.disposed) {
            this.disposed = true;
            if (this.recognizer) {
                dispose(this.recognizer);
            }
            dispose(this.audioConfig);
            dispose(this.speechConfig);
        }
    }
    start() {
        this.log("starting");
        this.started = true;
        this.aborting = false;
        this.recorder.start();
        if (this.continuous) {
            this.recognizer.startContinuousRecognitionAsync(() => {
                this.log("started continuous");
                this.dispatchEvent(this.startEvt);
            });
        }
        else {
            this.recognizer.recognizeOnceAsync((result) => {
                this.log("started");
                this.started = false;
                this.onResult(result, true);
            });
        }
    }
    stop() {
        this.started = false;
        this.recognizer.stopContinuousRecognitionAsync(() => {
            if (this.aborting) {
                this.onAbort();
            }
            else {
                this.dispatchEvent(this.endEvt);
            }
        });
    }
    abort() {
        this.aborting = true;
        this.stop();
    }
}
AzureSpeechRecognizer.isAvailable = true;
//# sourceMappingURL=AzureSpeechRecognizer.js.map