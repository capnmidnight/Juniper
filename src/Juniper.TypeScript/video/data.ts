import { AudioRecord, FullAudioRecord, MediaRecord } from "@juniper/audio/data";
import { isString } from "@juniper/tslib";

export interface ImageRecord extends MediaRecord {
    width: number;
    height: number;
}

export interface VideoRecord extends AudioRecord, ImageRecord {
    vcodec: string;
    fps: number;
    vbr: number;
}

export interface FullVideoRecord extends FullAudioRecord {
    thumbnail: ImageRecord;
    videos: VideoRecord[];
}

export function isVideoRecord(obj: MediaRecord): obj is VideoRecord {
    return isString((obj as VideoRecord).vcodec);
}
