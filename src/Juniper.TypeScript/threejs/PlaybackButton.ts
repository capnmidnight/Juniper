import type { AudioElementSource } from "juniper-audio/sources/AudioElementSource";
import { MouseButtons } from "juniper-dom/eventSystem/MouseButton";
import { keycapDigits } from "juniper-emoji/numbers";
import { TypedEvent, TypedEventBase } from "juniper-tslib";
import { ButtonFactory } from "./ButtonFactory";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { EventSystemThreeJSEvent } from "./eventSystem/EventSystemEvent";
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
    private playButton: MeshButton = null;
    private pauseButton: MeshButton = null;
    private stopButton: MeshButton = null;
    private replayButton: MeshButton = null;

    constructor(
        env: BaseEnvironment<unknown>,
        buttonFactory: ButtonFactory,
        name: string,
        label: string,
        clip: AudioElementSource) {
        super();

        label = translations.get(label) || label;

        this.object = obj(`playback-${name}`);

        this.textLabel = new TextMeshLabel(env, `playback-${name}-label`, label, {
            minHeight: size,
            maxHeight: size,
            minWidth: size,
            padding: 0.02,
            scale: 1000,
            bgFillColor: "#1e4388"
        });

        this.load(buttonFactory, clip);
    }

    private async load(buttonFactory: ButtonFactory, clip: AudioElementSource) {
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

        const refresh = () => {
            this.playButton.disabled = clip.playbackState === "playing";
            this.pauseButton.disabled = clip.playbackState !== "playing";
            this.stopButton.disabled = clip.playbackState === "stopped";
            this.replayButton.disabled = clip.playbackState === "stopped";
        }

        refresh();

        clip.addEventListener("played", refresh);
        clip.addEventListener("paused", refresh);
        clip.addEventListener("stopped", refresh);

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
        return this.textLabel.image.value;
    }

    set label(v: string) {
        v = translations.get(v) || v;
        this.textLabel.image.value = v;
    }
}
