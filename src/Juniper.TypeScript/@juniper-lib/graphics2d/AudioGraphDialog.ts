import { JuniperAudioContext, Vertex } from "@juniper-lib/audio/context/JuniperAudioContext";
import { arrayScan } from "@juniper-lib/tslib/collections/arrays";
import { assertNever } from "@juniper-lib/tslib/typeChecks";
import { BaseGraphDialog } from "./BaseGraphDialog";

function getVertexName(n: Vertex) {
    return n.name;
}

function getVertexColor(n: Vertex): CSSColorValue {
    if (n.classID === "node") {
        return "lightgreen";
    }
    else if (n.classID === "param") {
        return "lightyellow";
    }
    else if (n.classID === "unknown") {
        return "pink";
    }
    else {
        assertNever(n.classID);
    }
}

function getWeightMod(a: Vertex, b: Vertex, connected: boolean): number {
    return !connected || a.classID === b.classID
        ? 1
        : 1.8;
}

export class AudioGraphDialog extends BaseGraphDialog<Vertex>{
    constructor(private readonly context: JuniperAudioContext) {
        super("Audio graph", getVertexName, getVertexColor, getWeightMod);
    }

    override refreshData() {
        const graph = this.context.getAudioGraph();
        this.setGraph(graph);
        this.setOrigin(arrayScan(graph, (g) =>
            g.value.type === "media-stream-audio-destination"));
        super.refreshData();
    }
}