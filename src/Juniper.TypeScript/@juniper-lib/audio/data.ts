import { compareBy } from "@juniper-lib/collections/arrays";

export interface MediaRecord {
    url: string;
    contentType: string;
    resolution: number;
}

export interface AudioRecord extends MediaRecord {
    acodec: string;
    abr: number;
    asr: number;
}

export interface FullAudioRecord {
    title: string;
    audios: AudioRecord[];
}


export const audioRecordSorter = compareBy<AudioRecord>("descending", f => f.resolution);