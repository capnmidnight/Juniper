import { JuniperAudioContext, Vertex } from "@juniper-lib/audio";
import { BaseGraphDialog } from "./BaseGraphDialog";
export declare class AudioGraphDialogElement extends BaseGraphDialog<Vertex> {
    private readonly context;
    constructor(context: JuniperAudioContext);
    refreshData(): void;
    static install(): import("@juniper-lib/dom").ElementFactory<AudioGraphDialogElement>;
}
export declare function AudioGraphDialog(): AudioGraphDialogElement;
//# sourceMappingURL=AudioGraphDialog.d.ts.map