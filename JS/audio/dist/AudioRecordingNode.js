import * as allAudioTypes from "@juniper-lib/mediatypes/dist/audio";
import { ActivityDetector } from "./ActivityDetector";
import { BaseNodeCluster } from "./BaseNodeCluster";
import { JuniperMediaStreamAudioDestinationNode } from "./context/JuniperMediaStreamAudioDestinationNode";
import { debounce, isNullOrUndefined, arrayScan, isDefined, arrayClear } from "@juniper-lib/util";
import { TypedEvent } from "@juniper-lib/events";
const RECORDING_DELAY = .25;
const ACTIVITY_SENSITIVITY = 0.6;
const PAUSE_LENGTH = 1;
export class BlobAvailableEvent extends TypedEvent {
    constructor(id, blob) {
        super("blobavailable");
        this.id = id;
        this.blob = blob;
    }
}
export class AudioRecordingNode extends BaseNodeCluster {
    static getSupportedMediaTypes() {
        return Object
            .values(allAudioTypes)
            .filter(v => MediaRecorder.isTypeSupported(v.value));
    }
    constructor(context, activityOrOptions, options) {
        let activity = null;
        if (activityOrOptions instanceof ActivityDetector) {
            activity = activityOrOptions;
        }
        const input = context.createGain();
        const delay = context.createDelay(1);
        delay.delayTime.value = RECORDING_DELAY;
        const streamNode = new JuniperMediaStreamAudioDestinationNode(context, options);
        input.connect(delay).connect(streamNode);
        super("audio-recording-node", context, [input], null, [delay, streamNode]);
        this.useActiveListening = false;
        this.listening = false;
        this.recording = false;
        this._mimeType = undefined;
        this._audioBitsPerSecond = undefined;
        this._bitsPerSecond = undefined;
        this._audioBitrateMode = undefined;
        this.recorder = null;
        this.fwd = (evt) => this.dispatchEvent(evt);
        this.streamNode = streamNode;
        this.createRecorder = debounce(this._createRecorder.bind(this));
        options = options || {};
        if (isNullOrUndefined(options.mimeType)) {
            const supportedTypes = AudioRecordingNode.getSupportedMediaTypes();
            const recordingMimeType = arrayScan(supportedTypes, t => t === allAudioTypes.Audio_WebMOpus, t => t === allAudioTypes.Audio_Mpeg, isDefined);
            options.mimeType = recordingMimeType.value;
        }
        this.mimeType = options.mimeType;
        this.audioBitsPerSecond = options.audioBitsPerSecond;
        this.bitsPerSecond = options.bitsPerSecond;
        this.audioBitrateMode = options.audioBitrateMode;
        this.createRecorder();
        const chunks = new Array();
        const onStartRecording = () => {
            arrayClear(chunks);
        };
        const onRecordingDataAvailable = (evt) => {
            chunks.push(evt.data);
        };
        let counter = 0;
        const onStopRecording = () => {
            this.dispatchEvent(new BlobAvailableEvent(++counter, new Blob(chunks, {
                type: this.mimeType
            })));
        };
        this.addEventListener("start", onStartRecording);
        this.addEventListener("dataavailable", onRecordingDataAvailable);
        this.addEventListener("stop", onStopRecording);
        let stopRecordingTimer = null;
        const start = () => {
            this.recording = true;
            this.recorder.start();
        };
        const stop = () => {
            stopRecordingTimer = null;
            this.recording = false;
            this.recorder.stop();
        };
        if (activity) {
            this.useActiveListening = true;
            activity.addEventListener("activity", (evt) => {
                if (this.listening && evt.level > ACTIVITY_SENSITIVITY) {
                    if (this.recording) {
                        if (stopRecordingTimer) {
                            clearTimeout(stopRecordingTimer);
                            stopRecordingTimer = null;
                        }
                        stopRecordingTimer = setTimeout(stop, PAUSE_LENGTH * 1000);
                    }
                    else {
                        start();
                    }
                }
                else if (!this.listening && stopRecordingTimer) {
                    clearTimeout(stopRecordingTimer);
                    stop();
                }
            });
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
            return "inactive";
        }
        return this.recorder.state;
    }
    checkState() {
        if (this.state !== "inactive") {
            throw new Error("Cannot change settings while recording. State: " + this.state);
        }
    }
    get mimeType() { return this._mimeType; }
    set mimeType(v) {
        this.checkState();
        if (v !== this.mimeType) {
            this._mimeType = v;
            this.createRecorder();
        }
    }
    get audioBitsPerSecond() { return this._audioBitsPerSecond; }
    set audioBitsPerSecond(v) {
        this.checkState();
        if (v !== this.audioBitsPerSecond) {
            this._audioBitsPerSecond = v;
            this.createRecorder();
        }
    }
    get bitsPerSecond() { return this._bitsPerSecond; }
    set bitsPerSecond(v) {
        this.checkState();
        if (v !== this.bitsPerSecond) {
            this._bitsPerSecond = v;
            this.createRecorder();
        }
    }
    get audioBitrateMode() { return this._audioBitrateMode; }
    set audioBitrateMode(v) {
        this.checkState();
        if (v !== this.audioBitrateMode) {
            this._audioBitrateMode = v;
            this.createRecorder();
        }
    }
    _createRecorder() {
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
    start() {
        if (this.useActiveListening) {
            this.listening = true;
        }
        else if (this.recorder != null && this.recorder.state === "inactive") {
            this.recorder.start();
        }
    }
    stop() {
        if (this.useActiveListening) {
            this.listening = false;
        }
        else if (this.recorder != null && this.recorder.state !== "inactive") {
            this.recorder.stop();
        }
    }
    resume() {
        if (this.recorder != null && this.recorder.state === "paused") {
            this.recorder.resume();
        }
    }
    pause() {
        if (this.recorder != null && this.recorder.state !== "paused") {
            this.recorder.pause();
        }
    }
    requestData() {
        if (this.recorder != null) {
            this.recorder.requestData();
        }
    }
}
//# sourceMappingURL=AudioRecordingNode.js.map