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
export declare const audioRecordSorter: import("@juniper-lib/collections/arrays").CompareFunction<AudioRecord>;
//# sourceMappingURL=data.d.ts.map