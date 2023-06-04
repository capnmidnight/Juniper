import { AudioRecordingNode } from "@juniper-lib/audio/AudioRecordingNode";
import { IResponse } from "@juniper-lib/fetcher/IResponse";
import { once } from "@juniper-lib/events/once";
import { BaseSpeechRecognizer } from "./BaseSpeechRecognizer";
import { SpeechRecognizerErrorEvent, SpeechRecognizerNoMatchEvent, SpeechRecognizerResultEvent } from "./ISpeechRecognizer";

export interface RecognitionResult {
    culture: Culture;
    text: string;
}

enum RecognitionState {
    Stopped = 0,
    Started = 1,
    AudioStarted = 2,
    SoundStarted = 3,
    SpeechStarted = 4
}

export abstract class BaseCustomSpeechRecognizer extends BaseSpeechRecognizer {

    continuous: boolean;

    private state: RecognitionState = RecognitionState.Stopped;
    private aborting = false;

    constructor(private readonly recorder: AudioRecordingNode) {
        super();

        this.recorder.addEventListener("error", (evt) =>
            this.dispatchEvent(new SpeechRecognizerErrorEvent(null, "audio-capture", evt.error.message)));

        this.recorder.addEventListener("start", () => {
            this.state = RecognitionState.Started;
            this.dispatchEvent(this.startEvt);
        });

        this.recorder.addEventListener("activity", (evt) => {
            if (this.state === RecognitionState.Started && evt.level > 0) {
                this.state = RecognitionState.AudioStarted;
                this.dispatchEvent(this.audioStartEvt);
            }
            else if (this.state === RecognitionState.AudioStarted && evt.level > 0.1) {
                this.state = RecognitionState.SoundStarted;
                this.dispatchEvent(this.soundStartEvt);
            }
            else if (this.state === RecognitionState.SoundStarted && evt.level > 0.1) {
                this.state = RecognitionState.SpeechStarted;
                this.dispatchEvent(this.speechStartEvt);
            }
        });

        this.recorder.addEventListener("stop", () => {
            if (this.state === RecognitionState.SpeechStarted) {
                this.state = RecognitionState.SoundStarted;
                this.dispatchEvent(this.speechEndEvt);
            }

            if (this.state === RecognitionState.SoundStarted) {
                this.state = RecognitionState.AudioStarted;
                this.dispatchEvent(this.soundEndEvt);
            }

            if (this.state === RecognitionState.AudioStarted) {
                this.state = RecognitionState.Started;
                this.dispatchEvent(this.audioEndEvt);
            }
        });

        this.recorder.addEventListener("blobavailable", async (evt) => {
            this.dispatchEvent(evt);

            if (this.aborting) {
                this.aborting = false;
                this.dispatchEvent(new SpeechRecognizerErrorEvent(evt.id, "aborted", "Recording aborted"));
            }
            else {
                const result = await this.getResult(evt.blob);
                if (!result.content
                    || !result.content.text
                    || result.content.text.length === 0) {
                    if (result.status < 400) {
                        this.dispatchEvent(new SpeechRecognizerNoMatchEvent(evt.id));
                    }
                    else {
                        this.dispatchEvent(new SpeechRecognizerErrorEvent(evt.id, "network", result.status.toFixed()));
                    }
                }
                else {
                    this.dispatchEvent(new SpeechRecognizerResultEvent(evt.id, result.content.culture, result.content.text, true));
                    if (!this.continuous) {
                        this.stop();
                    }
                }
            }
        });
    }

    start() {
        this.recorder.start();
    }

    abort() {
        this.aborting = true;
        this.stop();
    }

    stop() {
        const task = once(this.recorder, "stop");
        this.recorder.stop();
        task.then(() => {
            if (this.state === RecognitionState.Started) {
                this.state = RecognitionState.Stopped;
                this.dispatchEvent(this.endEvt);
            }
        });
    }

    protected abstract getResult(blob: Blob): Promise<IResponse<RecognitionResult>>;
}