import { isDefined, once, success } from "@juniper-lib/util";
import { cursor, display, Div, elementSetDisplay, Img, OnClick } from "@juniper-lib/dom";
import { progressSplitWeighted } from "@juniper-lib/progress";
import { BaseVideoPlayer } from "./BaseVideoPlayer";
const loadingCursor = "wait";
const loadedCursor = "pointer";
const errorCursor = "not-allowed";
export class VideoPlayer extends BaseVideoPlayer {
    constructor(context, spatializer) {
        super("video-player", context, spatializer);
        this.element = Div(display("inline-block"), this.thumbnail = Img(cursor(loadingCursor), OnClick(() => {
            if (this.loaded) {
                this.play();
            }
        })), this.video, this.audio);
        this.addEventListener("played", () => this.showVideo(true));
        this.addEventListener("stopped", () => this.showVideo(false));
        this.showVideo(false);
    }
    onDisposing() {
        super.onDisposing();
        if (isDefined(this.element.parentElement)) {
            this.element.remove();
        }
    }
    showVideo(v) {
        elementSetDisplay(this.video, v, "inline-block");
        elementSetDisplay(this.thumbnail, !v, "inline-block");
    }
    async load(data, prog) {
        try {
            this.thumbnail.style.opacity = "0.5";
            this.thumbnail.style.cursor = loadingCursor;
            const progs = progressSplitWeighted(prog, [1, 10]);
            await Promise.all([
                super.load(data, progs.shift()),
                this.loadThumbnail(data, progs.shift())
            ]);
            return this;
        }
        finally {
            this.thumbnail.style.opacity = "1";
            this.thumbnail.style.cursor = this.loaded
                ? loadedCursor
                : errorCursor;
        }
    }
    clear() {
        super.clear();
        this.thumbnail.src = "";
    }
    setTitle(v) {
        super.setTitle(v);
        this.thumbnail.title = v;
    }
    async loadThumbnail(data, prog) {
        prog?.start();
        if (isDefined(data)) {
            this.thumbnail.src = data.thumbnail.url;
            this.thumbnail.style.opacity = "0.5";
            const loading = once(this.thumbnail, "load", "error");
            await success(loading);
        }
        prog?.end();
    }
}
//# sourceMappingURL=VideoPlayer.js.map