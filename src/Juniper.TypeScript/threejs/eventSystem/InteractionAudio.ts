import type { AudioManager } from "juniper-audio/AudioManager";
import type { IProgress } from "juniper-tslib";
import type { EventSystem } from "./EventSystem";
import type { EventSystemEvent } from "./EventSystemEvent";
import { isInteractiveObject3D } from "./InteractiveObject3D";

function makeClipName(type: string, isDisabled: boolean) {
    if (type === "click" && isDisabled) {
        type = "error";
    }

    return `InteractionAudio-${type}`;
}

export class InteractionAudio {
    audio: AudioManager;
    eventSys: EventSystem;
    enabled: boolean;

    constructor(audio: AudioManager, eventSys: EventSystem) {
        this.audio = audio;
        this.eventSys = eventSys;
        this.enabled = true;

        const playClip = (evt: EventSystemEvent<"enter" | "exit" | "click">) => {
            const obj = evt.object;
            if (this.enabled
                && isInteractiveObject3D(obj)
                && obj.isClickable) {
                const clipName = makeClipName(evt.type, obj.disabled);
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

    async load(type: string, path: string, volume: number, onProgress?: IProgress) {
        return await this.audio.loadClip(makeClipName(type, false), path, false, false, true, false, volume, [], onProgress);
    }
}