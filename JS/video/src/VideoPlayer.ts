import { isDefined, once, success } from "@juniper-lib/util";
import { BaseSpatializer, JuniperAudioContext } from "@juniper-lib/audio";
import { CssCursorValue, cursor, display, Div, elementSetDisplay, Img, OnClick } from "@juniper-lib/dom";
import { IProgress, progressSplitWeighted } from "@juniper-lib/progress";
import { BaseVideoPlayer } from "./BaseVideoPlayer";
import { FullVideoRecord } from "./data";

const loadingCursor: CssCursorValue = "wait";
const loadedCursor: CssCursorValue = "pointer";
const errorCursor: CssCursorValue = "not-allowed";

export class VideoPlayer
    extends BaseVideoPlayer {

    readonly element: HTMLElement;
    readonly thumbnail: HTMLImageElement;

    constructor(context: JuniperAudioContext, spatializer: BaseSpatializer) {
        super("video-player", context, spatializer);

        this.element = Div(
            display("inline-block"),
            this.thumbnail = Img(
                cursor(loadingCursor),
                OnClick(() => {
                    if (this.loaded) {
                        this.play();
                    }
                })),
            this.video,
            this.audio,
        );

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

    override clear() {
        super.clear();
        this.thumbnail.src = "";
    }

    protected override setTitle(v: string): void {
        super.setTitle(v);
        this.thumbnail.title = v;
    }

    protected async loadThumbnail(data: FullVideoRecord, prog?: IProgress): Promise<void> {
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
