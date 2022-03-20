import { cursor, display, styles } from "juniper-dom/css";
import { Div, elementSetDisplay, ErsatzElement, Img } from "juniper-dom/tags";
import { IProgress, isDefined, once, progressSplitWeighted } from "juniper-tslib";
import { BaseVideoPlayer } from "./BaseVideoPlayer";
import { FullVideoRecord, ImageRecord } from "./data";

export class VideoPlayer
    extends BaseVideoPlayer
    implements ErsatzElement {

    readonly element: HTMLElement;
    readonly thumbnail: HTMLImageElement;

    constructor() {
        super();

        this.element = Div(
            styles(display("inline-block")),
            this.thumbnail = Img(styles(cursor("pointer"))),
            this.video,
            this.audio
        );

        this.thumbnail.addEventListener("click", () => this.play());
        this.addEventListener("played", () => this.showVideo(true));
        this.addEventListener("stopped", () => this.showVideo(false));

        this.showVideo(false);
    }

    private showVideo(v: boolean) {
        elementSetDisplay(this.video, v, "inline-block");
        elementSetDisplay(this.thumbnail, !v, "inline-block");
    }

    override async load(data: FullVideoRecord, prog?: IProgress): Promise<this> {
        const progs = progressSplitWeighted(prog, [1, 10]);
        await Promise.all([
            this.loadThumbnail(data.thumbnail, progs.shift()),
            super.load(data, progs.shift())
        ]);
        return this;
    }

    protected override setTitle(v: string): void {
        super.setTitle(v);
        this.thumbnail.title = v;
    }

    protected async loadThumbnail(thumbnailFormat: ImageRecord, prog?: IProgress): Promise<void> {
        if (isDefined(prog)) {
            prog.start();
        }
        const task = once<GlobalEventHandlersEventMap, "load">(this.thumbnail, "load");
        this.thumbnail.src = thumbnailFormat.url;
        await task;
        if (isDefined(prog)) {
            prog.end();
        }
    }
}
