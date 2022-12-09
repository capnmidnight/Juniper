import { JuniperAudioContext, Vertex } from "@juniper-lib/audio/context/JuniperAudioContext";
import { arrayScan } from "@juniper-lib/tslib/collections/arrays";
import { assertNever } from "@juniper-lib/tslib/typeChecks";
import { BaseGraphDialog } from "./BaseGraphDialog";

function getVertexName(n: Vertex) {
    return n.name;
}

function getVertexColor(n: Vertex): CSSColorValue {
    if (n.classID === "jnode") {
        return "green";
    }
    else if (n.classID === "node") {
        return "lightgreen";
    }
    else if (n.classID === "jparam") {
        return "yellow";
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

export class AudioGraphDialog extends BaseGraphDialog<Vertex>{
    constructor(private readonly context: JuniperAudioContext) {
        super("Audio graph", getVertexName, getVertexColor);
    }

    override onShown() {
        const graph = this.context.getAudioGraph();
        this.setGraph(graph);
        this.setOrigin(arrayScan(graph, (g) =>
            getVertexName(g.value) === "destination"));
        super.onShown();
    }
}