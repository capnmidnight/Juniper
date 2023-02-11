interface HTMLMediaElement {
    sinkId: string;
    setSinkId(id: string): Promise<void>;
    mozHasAudio?: boolean;
    webkitAudioDecodedByteCount?: number;
    audioTracks?: unknown[];
    preservesPitch?: boolean;
    mozPreservesPitch?: boolean;
    webkitPreservesPitch?: boolean;
    captureStream?: () => MediaStream;
    mozCaptureStream?: () => MediaStream;
}

type Data = ArrayBuffer | TypedArray | DataView;

type AudioDataFormat =
    | "u8"
    | "s16"
    | "s32"
    | "u8-planar"
    | "s16-planar"
    | "s32-planar"
    | "f32-planar";

interface AudioDataInit {
    format: AudioDataFormat;
    sampleRate: number;
    numberOfFrames: number;
    numberOfChannels: number;
    timestamp: number;
    data: Data;
}

interface AudioDataOptions {
    planeIndex: number;
    frameOffset?: number;
    frameCount?: number;
}

interface AudioData {
    readonly format: AudioDataFormat;
    readonly sampleRate: number;
    readonly numberOfFrames: number;
    readonly numberOfChannels: number;
    readonly duration: number;
    readonly timestamp: number;

    allocationSize(options: AudioDataOptions): number;
    copyTo(destination: Data, options: AudioDataOptions): void;
    clone(): AudioData;
    close(): void;
}

class AudioData {
    prototype: AudioData;
    new(init: AudioDataInit): AudioData;
}

type EncodedAudioChunkType = "key" | "delta";

interface EncodedAudioChunkOptions {
    type: EncodedAudioChunkType;
    timestamp: number;
    duration: number;
    data: Data;
}

interface EncodedAudioChunk {
    readonly type: EncodedAudioChunkType;
    readonly timestamp: number;
    readonly duration: number;
    readonly byteLength: number;

    copyTo(destination: Data): void;
}

class EncodedAudioChunk {
    prototype: EncodedAudioChunk;
    new(optionss: EncodedAudioChunkOptions);
}

interface AudioEncoderOutputMetadata {
    readonly decoderConfig: {
        readonly codec: string;
        readonly sampleRate: number;
        readonly numberOfChannels: number;
        readonly description?: Data;
    }
}

interface AudioEncoderConfig {
    codec: string;
    sampleRate: number;
    numberOfChannels: number;
    bitrate?: number;
}

interface AudioEncoderEventMap {
    dequeue: Event;
}

interface AudioEncoderOptions {
    output: (chunk: EncodedAudioChunk, metadata?: AudioEncoderOutputMetadata) => void;
    error: (err: Error) => void;
}

type AudioEncoderState =
    | "unconfigured"
    | "configured"
    | "closed";

interface AudioEncoderConfigurationSupportInfo {
    supported: boolean;
    config: AudioEncoderConfig;
}

interface AudioEncoder extends EventTarget {
    static isConfigSupported(config: AudioEncoderConfig): Promise<AudioEncoderConfigurationSupportInfo>;

    readonly encodeQueueSize: number;
    readonly state: AudioEncoderState;

    configure(config: AudioEncoderConfig): void;
    encode(data: AudioData): void;

    flush(): Promise<void>;
    reset(): void;
    close(): void;

    addEventListener<K extends keyof AudioEncoderEventMap>(type: K, listener: (this: AudioEncoder, ev: AudioEncoderEventMap[K]) => any, options?: boolean | AddEventListenerOptions): void;
    addEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | AddEventListenerOptions): void;
    removeEventListener<K extends keyof AudioEncoderEventMap>(type: K, listener: (this: AudioEncoder, ev: AudioEncoderEventMap[K]) => any, options?: boolean | EventListenerOptions): void;
    removeEventListener(type: string, listener: EventListenerOrEventListenerObject, options?: boolean | EventListenerOptions): void;
}

class AudioEncoder extends AudioEncoder {
    constructor(init: AudioEncoderOptions);
}