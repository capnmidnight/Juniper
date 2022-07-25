import type { AudioManager } from "@juniper-lib/audio/AudioManager";
import type { Pointer3DEvent } from "./Pointer3DEvent";
import type { PointerManager } from "./PointerManager";

function makeClipName(type: string, isDisabled: boolean) {
    if (type === "click" && isDisabled) {
        type = "error";
    }

    return `InteractionAudio-${type}`;
}

export class InteractionAudio {
    audio: AudioManager;
    pointers: PointerManager;
    enabled: boolean;

    constructor(audio: AudioManager, pointers: PointerManager) {
        this.audio = audio;
        this.pointers = pointers;
        this.enabled = true;

        const playClip = (evt: Pointer3DEvent<"enter" | "exit" | "click">) => {
            if (this.enabled
                && evt.rayTarget
                && evt.rayTarget.clickable) {
                const clipName = makeClipName(evt.type, !evt.rayTarget.enabled);
                if (this.audio.hasClip(clipName)) {
                    const { x, y, z } = evt.point;
                    this.audio.setClipPosition(clipName, x, y, z);
                    this.audio.playClip(clipName);
                }
            }
        };

        this.pointers.addEventListener("enter", playClip);
        this.pointers.addEventListener("exit", playClip);
        this.pointers.addEventListener("click", playClip);
    }

    create(type: string, element: HTMLAudioElement, volume: number) {
        return this.audio.createClip(makeClipName(type, false), element, false, true, false, volume, []);
    }
}