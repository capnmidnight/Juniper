import { IPlayable } from "juniper-audio/sources/IPlayable";
import { MouseButtons } from "juniper-dom/eventSystem/MouseButton";
import { keycapDigits } from "juniper-emoji/numbers";
import { IFetcher } from "juniper-fetcher";
import { isDefined, TypedEvent, TypedEventBase } from "juniper-tslib";
import { ButtonFactory } from "./ButtonFactory";
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

export class PlaybackButton
    extends TypedEventBase<PlaybackButtonEvents>
    implements ErsatzObject {

    readonly object: THREE.Object3D;

    private readonly textLabel: TextMeshLabel;
    private readonly progressBar: THREE.Object3D;
    private playButton: MeshButton = null;
    private pauseButton: MeshButton = null;
    private stopButton: MeshButton = null;
    private replayButton: MeshButton = null;

    constructor(
        fetcher: IFetcher,
        env: IWebXRLayerManager,
        buttonFactory: ButtonFactory,
        name: string,
        label: string,
        clip: IPlayable) {
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

        this.load(buttonFactory, clip);
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

    private async load(buttonFactory: ButtonFactory, clip: IPlayable) {
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
            this.playButton.disabled = clip.playbackState === "loading"
                || clip.playbackState === "playing"
                || clip.playbackState === "errored";
            this.pauseButton.disabled = clip.playbackState === "loading"
                || clip.playbackState !== "playing";
            this.replayButton.disabled
                = this.stopButton.disabled
                = clip.playbackState === "loading"
                || clip.playbackState === "stopped"
                || clip.playbackState === "errored";

            if (clip.playbackState === "loading"
                || clip.playbackState === "stopped") {
                this.progressBar.visible = false;
            }
        }

        refresh();

        clip.addEventListener("loaded", refresh);
        clip.addEventListener("played", refresh);
        clip.addEventListener("paused", refresh);
        clip.addEventListener("stopped", refresh);

        clip.addEventListener("progress", (evt) => {
            const width = this.progBarWidth * evt.value / evt.total;
            this.progressBar.position.x = 0.5 * (width - this.progBarWidth - this.progBarOffsetX);
            this.progressBar.scale.x = width;
            this.progressBar.visible = evt.value > 0;
        });

        clip.addEventListener("played", () => this.dispatchEvent(playEvt));
        clip.addEventListener("stopped", () => this.dispatchEvent(stopEvt));

        const onClick = (btn: MeshButton, callback: () => void) => {
            btn.addEventListener("click", async (ev: THREE.Event) => {
                const evt = ev as EventSystemThreeJSEvent<"click">;
                if (evt.buttons === MouseButtons.Mouse0) {
                    callback();
                }
            });
        }

        onClick(this.playButton, () => clip.play());
        onClick(this.pauseButton, () => clip.pause());
        onClick(this.stopButton, () => clip.stop());
        onClick(this.replayButton, () => clip.restart());
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
}
