import { MediaElementSourceEvent } from "./IPlayable";
class MediaPlayerEvent extends MediaElementSourceEvent {
    constructor(type, source) {
        super(type, source);
    }
}
export class MediaPlayerLoadingEvent extends MediaPlayerEvent {
    constructor(source) {
        super("loading", source);
    }
}
//# sourceMappingURL=IPlayer.js.map