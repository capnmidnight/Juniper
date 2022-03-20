import { cursor, display, styles } from "juniper-dom/css";
import { Div, elementSetDisplay, ErsatzElement, Img } from "juniper-dom/tags";
import { IProgress, isDefined, once } from "juniper-tslib";
import { BaseVideoPlayer, ImageRecord } from "./BaseVideoPlayer";

export class PlayableVideo
    extends BaseVideoPlayer
    implements ErsatzElement {

    readonly element: HTMLElement;
    readonly thumbnail: HTMLImageElement;

    constructor() {
        super();

        this.element = Div(
            styles(display("inline-block")),
            this.thumbnail = this.createElement(Img, styles(cursor("pointer"))),
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
