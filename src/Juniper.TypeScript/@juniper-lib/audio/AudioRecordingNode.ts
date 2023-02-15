import { MediaType } from "@juniper-lib/mediatypes";
import * as allAudioTypes from "@juniper-lib/mediatypes/audio";
import { arrayClear, arrayScan } from "@juniper-lib/tslib/collections/arrays";
import { debounce } from "@juniper-lib/tslib/events/debounce";
import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import { Exception } from "@juniper-lib/tslib/Exception";
import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { ActivityDetector } from "./ActivityDetector";
import { BaseNodeCluster } from "./BaseNodeCluster";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
import { JuniperMediaStreamAudioDestinationNode } from "./context/JuniperMediaStreamAudioDestinationNode";

const RECORDING_DELAY = .25;
const ACTIVITY_SENSITIVITY = 0.6;
const PAUSE_LENGTH = 1;

export class AudioRecordingNodeBlobAvailableEvent extends TypedEvent<"blobavailable"> {
    constructor(public readonly blob: Blob) {
        super("blobavailable");
    }
}

export class ActivityEvent extends TypedEvent<"activity"> {
    public level = 0;
    constructor() {
        super("activity");
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
    activity: ActivityEvent;
}

interface AudioRecordingNodeOptions extends MediaRecorderOptions, AudioNodeOptions {
    enableListening?: boolean;
}

export class AudioRecordingNode
    extends BaseNodeCluster<AudioRecordingNodeEventMap>
    implements MediaRecorderOptions {

    static getSupportedMediaTypes(): MediaType[] {
        return Object
            .values(allAudioTypes)
            .filter(v => MediaRecorder.isTypeSupported(v.value));
    }

    private fwd: (event: Event) => any;

    private readonly streamNode: JuniperMediaStreamAudioDestinationNode;

    listening = false;
    private recording = false;

    private _mimeType: string = undefined;
    private _audioBitsPerSecond: number = undefined;
    private _bitsPerSecond: number = undefined;
    private _audioBitrateMode: BitrateMode = undefined;
    private recorder: MediaRecorder = null;

    private readonly createRecorder: () => void;

    constructor(context: JuniperAudioContext, options?: AudioRecordingNodeOptions) {

        const input = context.createGain();

        const delay = context.createDelay(1);
        delay.delayTime.value = RECORDING_DELAY;

        const streamNode = new JuniperMediaStreamAudioDestinationNode(context, options);

        input.connect(delay).connect(streamNode);

        super("audio-recording-node", context, [input], null, [delay, streamNode]);

        this.fwd = (evt) => this.dispatchEvent(evt);

        this.streamNode = streamNode;

        this.createRecorder = debounce(this._createRecorder.bind(this));

        options = options || {};

        if (isNullOrUndefined(options.mimeType)) {
            const supportedTypes = AudioRecordingNode.getSupportedMediaTypes();
            const recordingMimeType = arrayScan(supportedTypes,
                t => t === allAudioTypes.Audio_WebMOpus,
                t => t === allAudioTypes.Audio_Mpeg,
                isDefined);
            options.mimeType = recordingMimeType.value;
        }

        this.mimeType = options.mimeType;
        this.audioBitsPerSecond = options.audioBitsPerSecond;
        this.bitsPerSecond = options.bitsPerSecond;
        this.audioBitrateMode = options.audioBitrateMode;

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
            const activityEvt = new ActivityEvent();
            const activity = new ActivityDetector(context);

            let stopRecordingTimer: number = null;
            const checkActivity = () => {
                activityEvt.level = activity.level;
                this.dispatchEvent(activityEvt);

                if (this.listening && activityEvt.level > ACTIVITY_SENSITIVITY) {
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
                        }, PAUSE_LENGTH * 1000) as any;
                    }
                }
            };

            setInterval(checkActivity, 10);
            input.connect(activity);
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

    private _createRecorder() {
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

            this.recorder = new MediaRecorder(this.streamNode.stream, this);
            this.recorder.addEventListener("dataavailable", this.fwd);
            this.recorder.addEventListener("error", this.fwd);
            this.recorder.addEventListener("pause", this.fwd);
            this.recorder.addEventListener("resume", this.fwd);
            this.recorder.addEventListener("start", this.fwd);
            this.recorder.addEventListener("stop", this.fwd);
        }
    }

    start(timeslice?: number) {
        if (this.recorder.state === "inactive") {
            this.recorder.start(timeslice);
        }
    }

    stop() {
        if (this.recorder.state !== "inactive") {
            this.recorder.stop();
        }
    }

    resume() {
        if (this.recorder.state === "paused") {
            this.recorder.resume();
        }
    }

    pause() {
        if (this.recorder.state !== "paused") {
            this.recorder.pause();
        }
    }

    requestData() {
        this.recorder.requestData();
    }
}
