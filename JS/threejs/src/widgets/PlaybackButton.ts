import { asyncCallback, IDisposable, isDefined } from "@juniper-lib/util";
import { FullAudioRecord, IPlayer, MediaPlayerEvents } from "@juniper-lib/audio";
import { keycapDigits } from "@juniper-lib/emoji";
import { TypedEvent } from "@juniper-lib/events";
import { BaseProgress } from "@juniper-lib/progress";
import { Object3D } from "three";
import { Cube } from "../Cube";
import { cleanup } from "../cleanup";
import { BaseEnvironment } from "../environment/BaseEnvironment";
import { solidWhite } from "../materials";
import { ErsatzObject, obj, objGraph } from "../objects";
import { ButtonFactory } from "./ButtonFactory";
import { MeshButton } from "./MeshButton";
import { TextMesh } from "./TextMesh";

const playEvt = new TypedEvent("play");
const stopEvt = new TypedEvent("stop");
const size = 0.1;

type PlaybackButtonEvents = {
    play: TypedEvent<"play">;
    stop: TypedEvent<"stop">;
}

const translations = new Map(
    keycapDigits.map((m, i) => [m.value, i.toString()])
);

export class PlaybackButton<T extends FullAudioRecord>
    extends BaseProgress<PlaybackButtonEvents>
    implements ErsatzObject, IDisposable {

    readonly content3d: Object3D;

    private readonly textLabel: TextMesh;
    private readonly progressBar: Object3D;
    private playButton: MeshButton = null;
    private pauseButton: MeshButton = null;
    private stopButton: MeshButton = null;
    private replayButton: MeshButton = null;

    readonly clickPlay: asyncCallback = null;

    constructor(
        env: BaseEnvironment,
        buttonFactory: ButtonFactory,
        private readonly data: T | string,
        name: string,
        label: string,
        public volume: number,
        private readonly player: IPlayer<T>) {
        super();

        label = translations.get(label) || label || "";

        this.content3d = obj(`playback-${name}`);

        this.textLabel = new TextMesh(env, `playback-${name}-label`, "none", {
            minHeight: size,
            minWidth: 4 * size,
            maxWidth: 4 * size,
            padding: 0.02,
            scale: 1000,
            bgFillColor: buttonFactory.labelFillColor,
            textFillColor: "white"
        });
        this.textLabel.image.value = label;
        this.textLabel.image.addEventListener("redrawn", () =>
            this.repositionLabel());

        this.progressBar = new Cube(4 * size, 0.025, 0.01, solidWhite);
        this.progressBar.visible = false;

        this.clickPlay = async () => {
            if (this.player.data !== this.data) {
                await this.player.load(this.data, this);
            }
            this.player.volume = this.volume;
            await this.player.play();
        };

        this.load(buttonFactory, player);
    }

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            if (this.data === this.player.data) {
                this.player.clear();
            }
            cleanup(this.content3d);
            this.disposed = true;
        }
    }

    private repositionLabel() {
        this.textLabel.position.y = -(size + this.textLabel.objectHeight) / 2;
    }

    private async load(buttonFactory: ButtonFactory, player: IPlayer<T>) {
        const [
            play,
            pause,
            stop,
            replay
        ] = await Promise.all([
            buttonFactory.getMeshButton("media", "play", size),
            buttonFactory.getMeshButton("media", "pause", size),
            buttonFactory.getMeshButton("media", "stop", size),
            buttonFactory.getMeshButton("media", "replay", size)
        ]);

        objGraph(
            this,
            this.playButton = play,
            this.pauseButton = pause,
            this.stopButton = stop,
            this.replayButton = replay,
            this.progressBar,
            this.textLabel
        );

        this.playButton.content3d.position.x = -1.5 * size;
        this.pauseButton.content3d.position.x = -0.5 * size;
        this.stopButton.content3d.position.x = 0.5 * size;
        this.replayButton.content3d.position.x = 1.5 * size;

        this.progressBar.position.y = -size / 2;
        this.progressBar.position.z = 0.01;

        this.repositionLabel();

        const refresh = () => {
            const hasMyData = player.data === this.data;
            this.playButton.disabled = hasMyData
                && (player.playbackState === "loading"
                    || player.playbackState === "playing"
                    || player.playbackState === "errored")
                || !hasMyData
                && player.playbackState === "loading";
            this.pauseButton.disabled = !hasMyData
                || player.playbackState === "loading"
                || player.playbackState !== "playing";
            this.replayButton.disabled
                = this.stopButton.disabled
                = !hasMyData
                || player.playbackState === "loading"
                || player.playbackState === "stopped"
                || player.playbackState === "errored";

            if (!hasMyData
                || player.playbackState === "loading"
                || player.playbackState === "stopped") {
                this.progressBar.visible = false;
            }
        };

        refresh();

        const local = <V extends MediaPlayerEvents<T>[keyof MediaPlayerEvents<T>]>(callback: (evt: V) => void) => (evt: V) => {
            if (evt.source.data === this.data) {
                callback(evt);
            }
        };

        const localRefresh = local(refresh);

        player.addEventListener("loading", refresh);
        player.addEventListener("loaded", localRefresh);
        player.addEventListener("played", localRefresh);
        player.addEventListener("paused", localRefresh);
        player.addEventListener("stopped", refresh);

        player.addEventListener("progress", local((evt) => this.report(evt.value, evt.total)));
        player.addEventListener("played", local(() => this.dispatchEvent(playEvt)));
        player.addEventListener("stopped", local(() => this.dispatchEvent(stopEvt)));

        const onClick = (btn: MeshButton, callback: () => void) => {
            btn.addEventListener("click", callback);
        };

        onClick(this.playButton, this.clickPlay);
        onClick(this.pauseButton, () => player.pause());
        onClick(this.stopButton, () => player.stop());
        onClick(this.replayButton, () => player.restart());
    }

    get label(): string {
        if (isDefined(this.textLabel.image)) {
            return this.textLabel.image.value;
        }

        return null;
    }

    set label(v: string) {
        v = translations.get(v) || v;
        this.textLabel.image.value = v;
    }

    override report(soFar: number, total: number, msg?: string, est?: number) {
        super.report(soFar, total, msg, est);
        const width = this.p * 4 * size;
        this.progressBar.position.x = 0.5 * (width - 4 * size);
        this.progressBar.scale.x = width;
        this.progressBar.visible = soFar > 0;
    }
}
