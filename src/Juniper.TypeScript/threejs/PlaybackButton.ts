import type { IPlayableSource } from "juniper-audio/sources/IPlayableSource";
import { MouseButtons } from "juniper-dom/eventSystem/MouseButton";
import {
    pauseButton,
    playButton,
    speakerHighVolume,
    speakerLowVolume
} from "juniper-emoji";
import { TypedEvent, TypedEventBase } from "juniper-tslib";
import { EventSystemThreeJSEvent } from "./eventSystem/EventSystemEvent";
import { ErsatzObject, obj } from "./objects";
import { TextMeshButton } from "./TextMeshButton";


function makePauseButtonLabel(label: string): string {
    return label + pauseButton.value + speakerHighVolume.value;
}

function makePlayButtonLabel(label: string): string {
    return label + playButton.value + speakerLowVolume.value;
}

const playEvt = new TypedEvent("play");
const stopEvt = new TypedEvent("stop");
interface PlaybackButtonEvents {
    play: TypedEvent<"play">;
    stop: TypedEvent<"stop">;
}

export class PlaybackButton
    extends TypedEventBase<PlaybackButtonEvents>
    implements ErsatzObject {

    readonly object: THREE.Object3D;
    playButton: TextMeshButton;
    pauseButton: TextMeshButton;

    private _label: string;

    constructor(
        name: string,
        label: string,
        clip: IPlayableSource) {
        super();

        this.object = obj(
            `play-${name}`,

            this.playButton = new TextMeshButton(
                `playbackButtonPlay${label}`,
                makePlayButtonLabel(label)
            ),

            this.pauseButton = new TextMeshButton(
                `playbackButtonPause${label}`,
                makePauseButtonLabel(label)
            )
        );

        let isPlaying = false;

        const onStop = (evt: EventSystemThreeJSEvent<"click">) => {
            if ((!evt || evt.buttons === MouseButtons.Mouse0) && isPlaying) {
                this.playButton.visible = true;
                this.pauseButton.visible = false;
                clip.stop();
                this.dispatchEvent(stopEvt);
                isPlaying = false;
            }
        };

        this._label = label;

        this.playButton.addEventListener("click", async (ev: THREE.Event) => {
            const evt = ev as EventSystemThreeJSEvent<"click">;
            if (evt.buttons === MouseButtons.Mouse0) {
                this.playButton.visible = false;
                this.pauseButton.visible = true;
                this.dispatchEvent(playEvt);
                isPlaying = true;
                await clip.play();
                onStop(evt);
            }
        });

        this.pauseButton.visible = false;
        this.pauseButton.addEventListener("click", (ev: THREE.Event) => {
            const evt = ev as EventSystemThreeJSEvent<"click">;
            onStop(evt);
        });
    }

    get label(): string {
        return this._label;
    }

    set label(v: string) {
        if (v !== this.label) {
            this._label = v;
            this.playButton.image.value = makePlayButtonLabel(v);
            this.pauseButton.image.value = makePauseButtonLabel(v);
        }
    }
}
