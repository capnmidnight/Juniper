import { isString } from "juniper-tslib";

export interface MediaRecord {
    url: string;
    contentType: string;
    resolution: number;
}

export interface ImageRecord extends MediaRecord {
    width: number;
    height: number;
}

export interface AudioRecord extends MediaRecord {
    acodec: string;
    abr: number;
    asr: number;
}

export interface VideoRecord extends AudioRecord, ImageRecord {
    vcodec: string;
    fps: number;
    vbr: number;
}

export interface FullVideoRecord {
    title: string;
    thumbnail: ImageRecord;
    audios: AudioRecord[];
    videos: VideoRecord[];
}

export function isVideoRecord(obj: MediaRecord): obj is VideoRecord {
    return isString((obj as VideoRecord).vcodec);
}
