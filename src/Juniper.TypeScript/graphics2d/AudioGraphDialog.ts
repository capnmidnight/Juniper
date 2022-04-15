import { getAudioGraph, getVertexName } from "@juniper/audio/nodes";
import { arrayScan } from "@juniper/collections";
import { BaseGraphDialog } from "./BaseGraphDialog";

export class AudioGraphDialog extends BaseGraphDialog<AudioNode | AudioParam>{
    constructor() {
        super("Audio graph", getVertexName);
    }

    override onShown() {
        const graph = getAudioGraph();
        this.setGraph(graph);
        this.setOrigin(arrayScan(graph, (g) => getVertexName(g.value) === "final-destination"));
        super.onShown();
    }
}