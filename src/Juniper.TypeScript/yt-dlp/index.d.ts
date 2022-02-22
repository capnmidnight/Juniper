interface YTMetadataFormatFragment {
    path: string;
    duration: number;
}

interface YTMetadataFormat {
    format_id: string;
    format_note: string;
    ext: string;
    protocol: string;
    acodec: string;
    vcodec: string;
    url: string;
    width?: number;
    height?: number;
    fragments: YTMetadataFormatFragment[];
    audio_ext: string;
    video_ext: string;
    format: string;
    resolution: string;
    http_headers: {
        [key: string]: string;
    },
    asr?: number;
    filesize?: number;
    source_preference?: number;
    fps?: number;
    quality?: number;
    tbr?: number;
    language?: string;
    language_preference?: number;
    dynamic_range?: string;
    abr?: number;
    downloader_options?: {
        http_chunk_size: number
    };
    container?: string;
    vbr?: number;
    filesize_approx?: number;
}

interface YTMetadataThumbnail {
    id: string;
    url: string;
    preference: number;
    width?: number;
    height?: number;
    resolution?: string;
}

interface YTMetadataPlaylist {
    id: string;
    ext: string;
    title: string;
    description: string;
    duration: number;
    upload_date: string;
    uploader: string;
    uploader_id: string;
    uploader_url: string;
}

interface YTMetadata {
    id: string;
    title: string;
    formats: YTMetadataFormat[];
    thumbnails: YTMetadataThumbnail[];
    thumbnail: string;
    description: string;
    upload_date: string;
    uploader: string;
    uploader_id: string;
    uploader_url: string;
    channel_id: string;
    channel_url: string;
    duration: number;
    view_count: number;
    average_rating?: number;
    age_limit: number;
    webpage_url: string;
    categories: string[];
    tags: [];
    playable_in_embed: boolean;
    is_live: boolean;
    was_live: boolean;
    live_status: string;
    release_timestamp?: number;
    chapters: unknown;
    like_count: number;
    channel: string;
    channel_follower_count: number;
    track: string;
    artist: string;
    creator: string;
    alt_title: string;
    availability: string;
    original_url: string;
    webpage_url_basename: string;
    webpage_url_domain: string;
    extractor: string;
    extractor_key: string;
    playlist?: YTMetadataPlaylist[];
    playlist_index?: number;
    display_id: string;
    duration_string: string;
    requested_subtitles?: boolean;
    __has_drm: boolean;
    fulltitle: string;
    requested_formats: YTMetadataFormat[];
    format: string;
    format_id: string;
    ext: string;
    protocol: string;
    language?: string;
    format_note: string;
    filesize_approx: number;
    tbr: number;
    width: number;
    height: number;
    resolution: string;
    fps: number;
    dynamic_range: string;
    vcodec: string;
    vbr: number;
    stretched_ratio?: number;
    acodec: string;
    abr: number;
    asr: number;
    epoch: number;
    _filename: string;
    filename: string;
    urls: string
}


interface YTMediaEntry {
    contentType: string;
    url: string;
}

interface YTBasicResult {
    video?: YTMediaEntry;
    audio?: YTMediaEntry;
}