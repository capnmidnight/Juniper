import { JuniperAudioContext } from "@juniper-lib/audio/dist/context/JuniperAudioContext";
import { BaseSpatializer } from "@juniper-lib/audio/dist/spatializers/BaseSpatializer";
import { Src } from "@juniper-lib/dom/dist/attrs";
import { cursor, display, opacity } from "@juniper-lib/dom/dist/css";
import { Div, ErsatzElement, Img, HtmlRender, elementSetDisplay } from "@juniper-lib/dom/dist/tags";
import { all } from "@juniper-lib/events/dist/all";
import { once, success } from "@juniper-lib/events/dist/once";
import { IProgress } from "@juniper-lib/progress/dist/IProgress";
import { progressSplitWeighted } from "@juniper-lib/progress/dist/progressSplit";
import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { BaseVideoPlayer } from "./BaseVideoPlayer";
import { FullVideoRecord } from "./data";

const loadingCursor: CssCursorValue = "wait";
const loadedCursor: CssCursorValue = "pointer";
const errorCursor: CssCursorValue = "not-allowed";

export class VideoPlayer
    extends BaseVideoPlayer
    implements ErsatzElement {

    readonly element: HTMLElement;
    readonly thumbnail: HTMLImageElement;

    constructor(context: JuniperAudioContext, spatializer: BaseSpatializer) {
        super("video-player", context, spatializer);

        this.element = Div(
            display("inline-block"),
            this.thumbnail = Img(cursor(loadingCursor)),
            this.video,
            this.audio,
        );

        this.thumbnail.addEventListener("click", () => {
            if (this.loaded) {
                this.play();
            }
        });

        this.addEventListener("played", () => this.showVideo(true));
        this.addEventListener("stopped", () => this.showVideo(false));

        this.showVideo(false);
    }

    protected override onDisposing(): void {
        super.onDisposing();
        if (isDefined(this.element.parentElement)) {
            this.element.remove();
        }
    }

    private showVideo(v: boolean) {
        elementSetDisplay(this.video, v, "inline-block");
        elementSetDisplay(this.thumbnail, !v, "inline-block");
    }

    override async load(data: FullVideoRecord, prog?: IProgress): Promise<this> {
        try {
            HtmlRender(this.thumbnail,
                opacity(0.5),
                cursor(loadingCursor)
            );

            const progs = progressSplitWeighted(prog, [1, 10]);
            await all(
                super.load(data, progs.shift()),
                this.loadThumbnail(data, progs.shift())
            );

            return this;
        }
        finally {
            HtmlRender(this.thumbnail,
                opacity(1),
                cursor(this.loaded
                    ? loadedCursor
                    : errorCursor)
            );
        }
    }

    override clear() {
        super.clear();
        this.thumbnail.src = "";
    }

    protected override setTitle(v: string): void {
        super.setTitle(v);
        this.thumbnail.title = v;
    }

    protected async loadThumbnail(data: FullVideoRecord, prog?: IProgress): Promise<void> {
        prog.start();
        if (isDefined(data)) {
            HtmlRender(this.thumbnail,
                Src(data.thumbnail.url),
                opacity(0.5)
            );
            const loading = once(this.thumbnail, "load", "error");
            await success(loading);
        }
        prog.end();
    }
}