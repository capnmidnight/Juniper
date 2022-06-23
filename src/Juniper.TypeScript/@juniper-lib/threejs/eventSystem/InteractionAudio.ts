import type { AudioManager } from "@juniper-lib/audio/AudioManager";
import type { IProgress } from "@juniper-lib/tslib";
import type { EventSystem } from "./EventSystem";
import type { EventSystemEvent } from "./EventSystemEvent";

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

        this.eventSys.addEventListener("enter", playClip);
        this.eventSys.addEventListener("exit", playClip);
        this.eventSys.addEventListener("click", playClip);
    }

    async load(type: string, path: string, volume: number, prog?: IProgress) {
        return await this.audio.loadClip(makeClipName(type, false), path, false, false, true, false, volume, [], prog);
    }

    create(type: string, element: HTMLAudioElement, volume: number) {
        return this.audio.createClip(makeClipName(type, false), element, false, true, false, volume, []);
    }
}