import { ActivityDetector } from "@juniper-lib/audio/ActivityDetector";
import { AudioRecordingNode, BlobAvailableEvent } from "@juniper-lib/audio/AudioRecordingNode";
import { LocalUserMicrophone } from "@juniper-lib/audio/LocalUserMicrophone";
import { identity } from "@juniper-lib/tslib/identity";
import { IDisposable } from "@juniper-lib/tslib/using";
import { AudioConfig, AutoDetectSourceLanguageConfig, ProfanityOption, RecognitionEventArgs, Recognizer, ResultReason, SessionEventArgs, SpeechConfig, SpeechRecognitionCanceledEventArgs, SpeechRecognitionEventArgs, SpeechRecognitionResult, SpeechRecognizer } from "microsoft-cognitiveservices-speech-sdk";
import { BaseSpeechRecognizer } from "./BaseSpeechRecognizer";
import { SpeechRecognizerErrorEvent, SpeechRecognizerNoMatchEvent, SpeechRecognizerResultEvent } from "./ISpeechRecognizer";

type EventHandler<T> = (sender: Recognizer, evt: T) => void;

export class AzureSpeechRecognizer
    extends BaseSpeechRecognizer
    implements IDisposable {

    static readonly isAvailable = true;

    private readonly speechConfig: SpeechConfig;
    private readonly audioConfig: AudioConfig;
    private readonly recorder: AudioRecordingNode;

    private readonly onRecognizing: EventHandler<SpeechRecognitionEventArgs>;
    private readonly onRecognized: EventHandler<SpeechRecognitionEventArgs>;
    private readonly onCanceled: EventHandler<SpeechRecognitionCanceledEventArgs>;
    private readonly onSpeechStartDetected: EventHandler<RecognitionEventArgs>;
    private readonly onSpeechEndDetected: EventHandler<RecognitionEventArgs>;
    private readonly onSessionStarted: EventHandler<SessionEventArgs>;
    private readonly onSessionStopped: EventHandler<SessionEventArgs>;

    private recognizer: SpeechRecognizer;
    private counter = 0;
    private curId: number = null;
    private disposed = false;
    private started = false;
    private aborting = false;

    continuous: boolean = false;

    private log(...args: any[]): void {
        console.log(this.curId, this.recorder.state, ...args);
    }

    constructor(subscriptionKey: string, region: string, mics: LocalUserMicrophone, activity: ActivityDetector) {
        super();

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

    protected override onRefresh() {

        const wasStarted = this.started;

        if (this.recognizer) {
            this.recognizer.close();
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

    private onResult(result: SpeechRecognitionResult, isFinal: boolean) {
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
                    this.dispatchEvent(new SpeechRecognizerResultEvent(this.curId, result.language as Culture, result.text, isFinal));
                }
            }
        }
    }

    private onAbort() {
        if (this.curId !== null) {
            this.dispatchEvent(new SpeechRecognizerErrorEvent(this.curId, "aborted", "aborted"));
        }
    }

    dispose() {
        if (!this.disposed) {
            this.disposed = true;

            if (this.recognizer) {
                this.recognizer.close();
            }

            this.audioConfig.close();
            this.speechConfig.close();
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
