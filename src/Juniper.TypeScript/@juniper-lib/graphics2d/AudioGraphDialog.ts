import { IAudioNode, IAudioParam } from "@juniper-lib/audio/context/IAudioNode";
import { JuniperAudioContext } from "@juniper-lib/audio/context/JuniperAudioContext";
import { arrayScan } from "@juniper-lib/tslib/collections/arrays";
import { BaseGraphDialog } from "./BaseGraphDialog";

function getVertexName(n: IAudioNode | IAudioParam) {
    return n.name;
}

export class AudioGraphDialog extends BaseGraphDialog<IAudioNode | IAudioParam>{
    constructor(private readonly context: JuniperAudioContext) {
        super("Audio graph", getVertexName);
    }

    override onShown() {
        const graph = this.context.getAudioGraph();
        this.setGraph(graph);
        this.setOrigin(arrayScan(graph, (g) => getVertexName(g.value) === "final-destination"));
        super.onShown();
    }
}