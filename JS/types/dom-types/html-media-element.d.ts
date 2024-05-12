interface HTMLMediaElement {
    sinkId: string;
    setSinkId(id: string): Promise<void>;
    mozHasAudio?: boolean;
    webkitAudioDecodedByteCount?: number;
    audioTracks?: unknown[];
    captureStream?: () => MediaStream;
    mozCaptureStream?: () => MediaStream;
}