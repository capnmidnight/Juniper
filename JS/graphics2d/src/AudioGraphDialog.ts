import { JuniperAudioContext, Vertex } from "@juniper-lib/audio/dist/context/JuniperAudioContext";
import { assertNever } from "@juniper-lib/tslib/dist/typeChecks";
import { BaseGraphDialog } from "./BaseGraphDialog";

function getVertexName(n: Vertex) {
    return n.name;
}

function getVertexColor(n: Vertex): CssColorValue {
    if (n.nodeClass === "node") {
        return "lightgreen";
    }
    else if (n.nodeClass === "param") {
        return "lightyellow";
    }
    else if (n.nodeClass === "unknown") {
        return "pink";
    }
    else {
        assertNever(n.nodeClass);
    }
}

function getWeightMod(a: Vertex, b: Vertex, connected: boolean): number {
    return !connected || a.nodeClass === b.nodeClass
        ? 1
        : 1.8;
}

export class AudioGraphDialog extends BaseGraphDialog<Vertex>{
    constructor(private readonly context: JuniperAudioContext) {
        super("Audio graph", getVertexName, getVertexColor, getWeightMod);
    }

    override refreshData() {
        const graph = this.context.getAudioGraph(false);
        this.setGraph(graph);
        super.refreshData();
    }
}