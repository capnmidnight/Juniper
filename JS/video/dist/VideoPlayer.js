import { Src } from "@juniper-lib/dom/dist/attrs";
import { cursor, display, opacity } from "@juniper-lib/dom/dist/css";
import { Div, Img, HtmlRender, elementSetDisplay } from "@juniper-lib/dom/dist/tags";
import { all } from "@juniper-lib/events/dist/all";
import { once, success } from "@juniper-lib/events/dist/once";
import { progressSplitWeighted } from "@juniper-lib/progress/dist/progressSplit";
import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { BaseVideoPlayer } from "./BaseVideoPlayer";
const loadingCursor = "wait";
const loadedCursor = "pointer";
const errorCursor = "not-allowed";
export class VideoPlayer extends BaseVideoPlayer {
    constructor(context, spatializer) {
        super("video-player", context, spatializer);
        this.element = Div(display("inline-block"), this.thumbnail = Img(cursor(loadingCursor)), this.video, this.audio);
        this.thumbnail.addEventListener("click", () => {
            if (this.loaded) {
                this.play();
            }
        });
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
            HtmlRender(this.thumbnail, opacity(0.5), cursor(loadingCursor));
            const progs = progressSplitWeighted(prog, [1, 10]);
            await all(super.load(data, progs.shift()), this.loadThumbnail(data, progs.shift()));
            return this;
        }
        finally {
            HtmlRender(this.thumbnail, opacity(1), cursor(this.loaded
                ? loadedCursor
                : errorCursor));
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
        prog.start();
        if (isDefined(data)) {
            HtmlRender(this.thumbnail, Src(data.thumbnail.url), opacity(0.5));
            const loading = once(this.thumbnail, "load", "error");
            await success(loading);
        }
        prog.end();
    }
}
//# sourceMappingURL=VideoPlayer.js.map