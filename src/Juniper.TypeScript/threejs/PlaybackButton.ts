import { FullAudioRecord } from "@juniper/audio/data";
import { MediaElementSourceEvent } from "@juniper/audio/sources/IPlayable";
import { IPlayer } from "@juniper/audio/sources/IPlayer";
import { keycapDigits } from "@juniper/emoji/numbers";
import { MouseButtons } from "@juniper/event-system/MouseButton";
import { IFetcher } from "@juniper/fetcher";
import { AsyncCallback, BaseProgress, IDisposable, isDefined, TypedEvent } from "@juniper/tslib";
import { ButtonFactory } from "./ButtonFactory";
import { cleanup } from "./cleanup";
import { Cube } from "./Cube";
import { EventSystemThreeJSEvent } from "./eventSystem/EventSystemEvent";
import { IWebXRLayerManager } from "./IWebXRLayerManager";
import { solidWhite } from "./materials";
import { MeshButton } from "./MeshButton";
import { ErsatzObject, obj, objGraph } from "./objects";
import { TextMeshLabel } from "./TextMeshLabel";


const playEvt = new TypedEvent("play");
const stopEvt = new TypedEvent("stop");
const size = 0.1;
interface PlaybackButtonEvents {
    play: TypedEvent<"play">;
    stop: TypedEvent<"stop">;
}

const translations = new Map(
    keycapDigits.alts.map((m, i) => [m.value, i.toString()])
);

export class PlaybackButton<T extends FullAudioRecord>
    extends BaseProgress<PlaybackButtonEvents>
    implements ErsatzObject, IDisposable {

    readonly object: THREE.Object3D;

    private readonly textLabel: TextMeshLabel;
    private readonly progressBar: THREE.Object3D;
    private playButton: MeshButton = null;
    private pauseButton: MeshButton = null;
    private stopButton: MeshButton = null;
    private replayButton: MeshButton = null;

    clickPlay: AsyncCallback = null;

    constructor(
        fetcher: IFetcher,
        env: IWebXRLayerManager,
        buttonFactory: ButtonFactory,
        private readonly data: T | string,
        name: string,
        label: string,
        player: IPlayer) {
        super();

        label = translations.get(label) || label || "";

        this.object = obj(`playback-${name}`);

        this.textLabel = new TextMeshLabel(fetcher, env, `playback-${name}-label`, label, {
            minHeight: size,
            maxHeight: size,
            minWidth: size,
            padding: 0.02,
            scale: 1000,
            bgFillColor: "#1e4388"
        });

        this.progressBar = new Cube(1, 0.025, 0.01, solidWhite);
        this.progressBar.position.y = -size / 2;
        this.progressBar.position.z = 0.01;
        this.progressBar.visible = false;

        this.load(buttonFactory, player);
    }

    private disposed = false;
    dispose(): void {
        if (!this.disposed) {
            cleanup(this.object);
            this.disposed = true;
        }
    }

    private get progBarWidth() {
        return (isDefined(this.label) && this.label.length > 0
            ? 5
            : 4) * size;
    }

    private get progBarOffsetX() {
        return (isDefined(this.label) && this.label.length > 0
            ? 1
            : 0) * size;
    }

    private async load(buttonFactory: ButtonFactory, player: IPlayer) {
        const [
            enabledMaterial,
            disabledMaterial,
            playGeometry,
            pauseGeometry,
            stopGeometry,
            replayGeometry
        ] = await Promise.all([
            buttonFactory.getMaterial(true),
            buttonFactory.getMaterial(false),
            buttonFactory.getGeometry("media", "play"),
            buttonFactory.getGeometry("media", "pause"),
            buttonFactory.getGeometry("media", "stop"),
            buttonFactory.getGeometry("media", "replay")
        ]);

        objGraph(
            this.object,
            this.textLabel,
            this.playButton = new MeshButton(
                "PlayButton",
                playGeometry,
                enabledMaterial,
                disabledMaterial,
                size),
            this.pauseButton = new MeshButton(
                "PauseButton",
                pauseGeometry,
                enabledMaterial,
                disabledMaterial,
                size
            ),
            this.stopButton = new MeshButton(
                "StopButton",
                stopGeometry,
                enabledMaterial,
                disabledMaterial,
                size
            ),
            this.replayButton = new MeshButton(
                "ReplayButton",
                replayGeometry,
                enabledMaterial,
                disabledMaterial,
                size
            )
        );

        this.object.children.forEach((child, i, arr) =>
            child.position.x = (i - arr.length / 2) * size);

        this.object.add(this.progressBar);

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
        }

        refresh();

        const local = <T extends MediaElementSourceEvent<string, IPlayer>>(callback: (evt: T) => void) => (evt: T) => {
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
            btn.addEventListener("click", async (ev: THREE.Event) => {
                const evt = ev as EventSystemThreeJSEvent<"click">;
                if (evt.buttons === MouseButtons.Mouse0) {
                    callback();
                }
            });
        }

        this.clickPlay = async () => {
            if (player.data !== this.data) {
                await player.load(this.data, this);
            }
            await player.play();
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
        const width = this.p * this.progBarWidth;
        this.progressBar.position.x = 0.5 * (width - this.progBarWidth - this.progBarOffsetX);
        this.progressBar.scale.x = width;
        this.progressBar.visible = soFar > 0;
    }
}
