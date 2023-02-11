import { BaseNodeCluster } from "@juniper-lib/audio/BaseNodeCluster";
import { JuniperAudioContext } from "@juniper-lib/audio/context/JuniperAudioContext";
import { JuniperMediaStreamAudioDestinationNode } from "@juniper-lib/audio/context/JuniperMediaStreamAudioDestinationNode";
import { arrayClear } from "@juniper-lib/tslib/collections/arrays";
import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { Exception } from "@juniper-lib/tslib/Exception";
import { ActivityDetector } from "@juniper-lib/webrtc/ActivityDetector";

const ACTIVITY_SENSITIVITY = 0.6;
const PAUSE_LENGTH = 1;

export class AudioRecordingNodeBlobAvailableEvent extends TypedEvent<"blobavailable"> {
    constructor(public readonly blob: Blob) {
        super("blobavailable");
    }
}

interface AudioRecordingNodeEventMap {
    blobavailable: AudioRecordingNodeBlobAvailableEvent;
    dataavailable: BlobEvent;
    error: MediaRecorderErrorEvent;
    pause: Event;
    resume: Event;
    start: Event;
    stop: Event;
}

interface AudioRecordingNodeOptions extends MediaRecorderOptions, AudioNodeOptions {
    enableListening?: boolean;
}

export class AudioRecordingNode
    extends BaseNodeCluster<AudioRecordingNodeEventMap>
    implements MediaRecorderOptions {

    private fwd: (event: Event) => any;

    private readonly input: JuniperMediaStreamAudioDestinationNode;

    listening = false;
    private recording = false;

    private _mimeType: string = undefined;
    private _audioBitsPerSecond: number = undefined;
    private _bitsPerSecond: number = undefined;
    private _audioBitrateMode: BitrateMode = undefined;
    private recorder: MediaRecorder = null;

    public readonly activity: ActivityDetector = null;

    constructor(context: JuniperAudioContext, options?: AudioRecordingNodeOptions) {
        const input = new JuniperMediaStreamAudioDestinationNode(context, options);

        super("audio-recording-node", context, [input]);

        this.fwd = (evt) => this.dispatchEvent(evt);

        this.input = input;

        if (options) {
            this.mimeType = options.mimeType;
            this.audioBitsPerSecond = options.audioBitsPerSecond;
            this.bitsPerSecond = options.bitsPerSecond;
            this.audioBitrateMode = options.audioBitrateMode;
        }

        this.createRecorder();

        const chunks = new Array<Blob>();
        const onStartRecording = () => {
            arrayClear(chunks);
        };

        const onRecordingDataAvailable = (evt: BlobEvent) => {
            chunks.push(evt.data);
        };

        const onStopRecording = () => {
            this.dispatchEvent(new AudioRecordingNodeBlobAvailableEvent(new Blob(chunks, {
                type: this.mimeType
            })));
        };

        this.addEventListener("start", onStartRecording);
        this.addEventListener("dataavailable", onRecordingDataAvailable);
        this.addEventListener("stop", onStopRecording);


        if (options && options.enableListening) {
            this.activity = new ActivityDetector(context);
            let stopRecordingTimer: number = null;
            const checkActivity = () => {
                const level = this.activity.level;

                if (this.listening && level > ACTIVITY_SENSITIVITY) {
                    if (this.state !== "recording") {
                        this.start();
                    }
                    else {
                        if (stopRecordingTimer) {
                            clearTimeout(stopRecordingTimer);
                            stopRecordingTimer = null;
                        }

                        stopRecordingTimer = setTimeout(() => {
                            stopRecordingTimer = null;
                            this.stop();
                        }, PAUSE_LENGTH * 1000);
                    }
                }
            };

            setInterval(checkActivity, 10);
        }
    }

    get stream() {
        if (this.recorder === null) {
            return null;
        }

        return this.recorder.stream;
    }

    get state() {
        if (this.recorder === null) {
            return null;
        }

        return this.recorder.state;
    }

    get mimeType() { return this._mimeType; }
    set mimeType(v) {
        if (this.recording) {
            throw new Exception("Cannot change settings while recording");
        }
        if (v !== this.mimeType) {
            this._mimeType = v;
            this.createRecorder();
        }
    }

    get audioBitsPerSecond() { return this._audioBitsPerSecond; }
    set audioBitsPerSecond(v) {
        if (this.recording) {
            throw new Exception("Cannot change settings while recording");
        }
        if (v !== this.audioBitsPerSecond) {
            this._audioBitsPerSecond = v;
            this.createRecorder();
        }
    }

    get bitsPerSecond() { return this._bitsPerSecond; }
    set bitsPerSecond(v) {
        if (this.recording) {
            throw new Exception("Cannot change settings while recording");
        }
        if (v !== this.bitsPerSecond) {
            this._bitsPerSecond = v;
            this.createRecorder();
        }
    }

    get audioBitrateMode() { return this._audioBitrateMode; }
    set audioBitrateMode(v) {
        if (this.recording) {
            throw new Exception("Cannot change settings while recording");
        }
        if (v !== this.audioBitrateMode) {
            this._audioBitrateMode = v;
            this.createRecorder();
        }
    }

    private createRecorder() {
        if (this.mimeType) {
            if (this.recorder) {
                this.recorder.removeEventListener("dataavailable", this.fwd);
                this.recorder.removeEventListener("error", this.fwd);
                this.recorder.removeEventListener("pause", this.fwd);
                this.recorder.removeEventListener("resume", this.fwd);
                this.recorder.removeEventListener("start", this.fwd);
                this.recorder.removeEventListener("stop", this.fwd);
                this.recorder = null;
            }

            this.recorder = new MediaRecorder(this.input.stream, this);
            this.recorder.addEventListener("dataavailable", this.fwd);
            this.recorder.addEventListener("error", this.fwd);
            this.recorder.addEventListener("pause", this.fwd);
            this.recorder.addEventListener("resume", this.fwd);
            this.recorder.addEventListener("start", this.fwd);
            this.recorder.addEventListener("stop", this.fwd);
        }
    }

    start(timeslice?: number) {
        this.recorder.start(timeslice);
    }

    stop() {
        this.recorder.stop();
    }

    resume() {
        this.recorder.resume();
    }

    pause() {
        this.recorder.pause();
    }

    requestData() {
        this.recorder.requestData();
    }
}
