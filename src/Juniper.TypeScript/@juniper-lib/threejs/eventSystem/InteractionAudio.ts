import type { AudioManager } from "@juniper-lib/audio/AudioManager";
import type { AssetFile } from "@juniper-lib/fetcher/Asset";
import type { Pointer3DEvent } from "./devices/Pointer3DEvent";
import type { EventSystem } from "./EventSystem";

function makeClipName(type: string, isDisabled: boolean) {
    if (type === "click" && isDisabled) {
        type = "error";
    }

    return `InteractionAudio-${type}`;
}

export class InteractionAudio {
    constructor(
        private readonly audio: AudioManager,
        private readonly eventSys: EventSystem) {

        const playClip = (evt: Pointer3DEvent<"enter" | "exit" | "click">) => {
            if (evt.pointer.type !== "nose"
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

        this.eventSys.addEventListener("enter", playClip);
        this.eventSys.addEventListener("exit", playClip);
        this.eventSys.addEventListener("click", playClip);
    }

    create(type: string, asset: AssetFile, volume: number) {
        return this.audio.createClip(makeClipName(type, false), asset, false, true, false, true, volume, []);
    }
}