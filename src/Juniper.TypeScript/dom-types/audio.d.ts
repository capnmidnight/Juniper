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