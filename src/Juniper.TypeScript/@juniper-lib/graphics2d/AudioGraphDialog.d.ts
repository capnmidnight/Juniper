import { JuniperAudioContext, Vertex } from "@juniper-lib/audio/context/JuniperAudioContext";
import { BaseGraphDialog } from "./BaseGraphDialog";
export declare class AudioGraphDialog extends BaseGraphDialog<Vertex> {
    private readonly context;
    constructor(context: JuniperAudioContext);
    refreshData(): void;
}
//# sourceMappingURL=AudioGraphDialog.d.ts.map