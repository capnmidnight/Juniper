type Data = ArrayBuffer | Int8Array | Uint8Array | Uint8ClampedArray | Int16Array | Uint16Array | Int32Array | Uint32Array | Float32Array | Float64Array | DataView;

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

declare class AudioData implements AudioData {
    constructor(init: AudioDataInit);
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

declare class EncodedAudioChunk implements EncodedAudioChunk {
    constructor(optionss: EncodedAudioChunkOptions);
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

declare class AudioEncoder implements AudioEncoder {
    static isConfigSupported(config: AudioEncoderConfig): Promise<AudioEncoderConfigurationSupportInfo>;
    constructor(init: AudioEncoderOptions);
}