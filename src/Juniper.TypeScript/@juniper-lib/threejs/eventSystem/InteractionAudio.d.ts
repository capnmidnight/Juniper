import type { AudioManager } from "@juniper-lib/audio/AudioManager";
import type { AssetFile } from "@juniper-lib/fetcher/Asset";
import type { EventSystem } from "./EventSystem";
export declare class InteractionAudio {
    private readonly audio;
    private readonly eventSys;
    constructor(audio: AudioManager, eventSys: EventSystem);
    create(type: string, asset: AssetFile, volume: number): Promise<import("@juniper-lib/audio/sources/AudioElementSource").AudioElementSource>;
}
//# sourceMappingURL=InteractionAudio.d.ts.map