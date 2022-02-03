import type { IPlayableSource } from "juniper-audio/sources/IPlayableSource";
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

    constructor(
        env: BaseEnvironment<unknown>,
        buttonFactory: ButtonFactory,
        name: string,
        label: string,
        clip: IPlayableSource) {
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

    private async load(buttonFactory: ButtonFactory, clip: IPlayableSource) {
        const [
            enabledMaterial,
            disabledMaterial,
            playGeometry,
            pauseGeometry
        ] = await Promise.all([
            buttonFactory.getMaterial(true),
            buttonFactory.getMaterial(false),
            buttonFactory.getGeometry("media", "play"),
            buttonFactory.getGeometry("media", "pause")
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
            )
        );

        this.object.children.forEach((child, i, arr) =>
            child.position.x = (i - arr.length / 2) * size);

        this.isPlaying = false;

        const onStop = (evt: EventSystemThreeJSEvent<"click">) => {
            if ((!evt || evt.buttons === MouseButtons.Mouse0) && this.isPlaying) {
                clip.stop();
                this.dispatchEvent(stopEvt);
                this.isPlaying = false;
            }
        };

        this.playButton.addEventListener("click", async (ev: THREE.Event) => {
            const evt = ev as EventSystemThreeJSEvent<"click">;
            if (evt.buttons === MouseButtons.Mouse0) {
                this.isPlaying = true;
                this.dispatchEvent(playEvt);
                await clip.play();
                onStop(evt);
            }
        });

        this.pauseButton.addEventListener("click", (ev: THREE.Event) => {
            const evt = ev as EventSystemThreeJSEvent<"click">;
            onStop(evt);
        });
    }

    private get isPlaying() {
        return !this.pauseButton.disabled;
    }

    private set isPlaying(v) {
        this.playButton.disabled = v;
        this.pauseButton.disabled = !v;
    }

    get label(): string {
        return this.textLabel.image.value;
    }

    set label(v: string) {
        v = translations.get(v) || v;
        this.textLabel.image.value = v;
    }
}
