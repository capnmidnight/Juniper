import { isString } from "@juniper-lib/util";
import { MediaRecord, AudioRecord, FullAudioRecord } from "@juniper-lib/audio";

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
    startTime: number;
}

export function isVideoRecord(obj: MediaRecord): obj is VideoRecord {
    return isString((obj as VideoRecord).vcodec);
}
