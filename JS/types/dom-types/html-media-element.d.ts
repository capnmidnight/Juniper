interface HTMLMediaElement {
    mozHasAudio?: boolean;
    webkitAudioDecodedByteCount?: number;
    audioTracks?: unknown[];
    captureStream?: () => MediaStream;
    mozCaptureStream?: () => MediaStream;
}