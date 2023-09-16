import { TypedEvent } from "@juniper-lib/events/TypedEventTarget";
export class MediaElementSourceEvent extends TypedEvent {
    constructor(type, source) {
        super(type);
        this.source = source;
    }
}
export class MediaElementSourceLoadedEvent extends MediaElementSourceEvent {
    constructor(source) {
        super("loaded", source);
    }
}
export class MediaElementSourceErroredEvent extends MediaElementSourceEvent {
    constructor(source, error) {
        super("errored", source);
        this.error = error;
    }
}
export class MediaElementSourcePlayedEvent extends MediaElementSourceEvent {
    constructor(source) {
        super("played", source);
    }
}
export class MediaElementSourcePausedEvent extends MediaElementSourceEvent {
    constructor(source) {
        super("paused", source);
    }
}
export class MediaElementSourceStoppedEvent extends MediaElementSourceEvent {
    constructor(source) {
        super("stopped", source);
    }
}
export class MediaElementSourceProgressEvent extends MediaElementSourceEvent {
    constructor(source) {
        super("progress", source);
        this.value = 0;
        this.total = 0;
    }
}
//# sourceMappingURL=IPlayable.js.map