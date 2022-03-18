interface HTMLAudioElement {
    sinkId: string;
    setSinkId(id: string): Promise<void>;
}

interface HTMLVideoElement {
    mozHasAudio?: boolean;
    webkitAudioDecodedByteCount?: number;
    audioTracks?: unknown[];
    mozPreservesPitch?: boolean;
    webkitPreservesPitch?: boolean;
}