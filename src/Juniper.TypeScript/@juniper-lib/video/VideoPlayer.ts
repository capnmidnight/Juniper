import { src } from "@juniper-lib/dom/attrs";
import { cursor, display, opacity, styles } from "@juniper-lib/dom/css";
import { Div, elementApply, elementSetDisplay, ErsatzElement, Img } from "@juniper-lib/dom/tags";
import { once, success } from "@juniper-lib/tslib/events/once";
import { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import { progressSplitWeighted } from "@juniper-lib/tslib/progress/progressSplit";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { BaseVideoPlayer } from "./BaseVideoPlayer";
import { FullVideoRecord } from "./data";

const loadingCursor: CSSCursorValue = "wait";
const loadedCursor: CSSCursorValue = "pointer";
const errorCursor: CSSCursorValue = "not-allowed";

export class VideoPlayer
    extends BaseVideoPlayer
    implements ErsatzElement {

    readonly element: HTMLElement;
    readonly thumbnail: HTMLImageElement;

    constructor(audioCtx: AudioContext) {
        super(audioCtx);

        this.element = Div(
            styles(display("inline-block")),
            this.thumbnail = Img(styles(cursor(loadingCursor))),
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
            elementApply(this.thumbnail,
                opacity(0.5),
                cursor(loadingCursor)
            );

            const progs = progressSplitWeighted(prog, [1, 10]);
            await Promise.all([
                super.load(data, progs.shift()),
                this.loadThumbnail(data, progs.shift())
            ]);

            return this;
        }
        finally {
            elementApply(this.thumbnail,
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
            elementApply(this.thumbnail,
                src(data.thumbnail.url),
                opacity(0.5)
            );
            const loading = once<GlobalEventHandlersEventMap>(this.thumbnail, "load", "error");
            await success(loading);
        }
        prog.end();
    }
}
