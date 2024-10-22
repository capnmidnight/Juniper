import { ActivityDetector } from "./ActivityDetector";
import { BaseNodeCluster } from "./BaseNodeCluster";
import { JuniperAudioContext } from "./context/JuniperAudioContext";
import { TypedEvent } from "@juniper-lib/events";
import { MediaType } from "@juniper-lib/mediatypes";
import { ActivityEvent } from "./ActivityEvent";
export declare class BlobAvailableEvent extends TypedEvent<"blobavailable"> {
    readonly id: number;
    readonly blob: Blob;
    constructor(id: number, blob: Blob);
}
export type AudioRecordingNodeEvents = {
    blobavailable: BlobAvailableEvent;
    dataavailable: BlobEvent;
    error: ErrorEvent;
    pause: Event;
    resume: Event;
    start: Event;
    stop: Event;
    activity: ActivityEvent;
};
interface AudioRecordingNodeOptions extends MediaRecorderOptions, AudioNodeOptions {
    audioBitrateMode?: BitrateMode;
}
export declare class AudioRecordingNode extends BaseNodeCluster<AudioRecordingNodeEvents> implements MediaRecorderOptions {
    static getSupportedMediaTypes(): MediaType[];
    private readonly fwd;
    private readonly streamNode;
    private readonly createRecorder;
    private readonly useActiveListening;
    private listening;
    private recording;
    private _mimeType;
    private _audioBitsPerSecond;
    private _bitsPerSecond;
    private _audioBitrateMode;
    private recorder;
    constructor(context: JuniperAudioContext, options?: AudioRecordingNodeOptions);
    constructor(context: JuniperAudioContext, activity: ActivityDetector, options?: AudioRecordingNodeOptions);
    get stream(): MediaStream;
    get state(): RecordingState;
    private checkState;
    get mimeType(): string;
    set mimeType(v: string);
    get audioBitsPerSecond(): number;
    set audioBitsPerSecond(v: number);
    get bitsPerSecond(): number;
    set bitsPerSecond(v: number);
    get audioBitrateMode(): VideoEncoderBitrateMode;
    set audioBitrateMode(v: VideoEncoderBitrateMode);
    private _createRecorder;
    start(): void;
    stop(): void;
    resume(): void;
    pause(): void;
    requestData(): void;
}
export {};
//# sourceMappingURL=AudioRecordingNode.d.ts.map