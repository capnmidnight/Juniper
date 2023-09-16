import { assertNever } from "@juniper-lib/tslib/dist/typeChecks";
import { BaseGraphDialog } from "./BaseGraphDialog";
function getVertexName(n) {
    return n.name;
}
function getVertexColor(n) {
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
function getWeightMod(a, b, connected) {
    return !connected || a.nodeClass === b.nodeClass
        ? 1
        : 1.8;
}
export class AudioGraphDialog extends BaseGraphDialog {
    constructor(context) {
        super("Audio graph", getVertexName, getVertexColor, getWeightMod);
        this.context = context;
    }
    refreshData() {
        const graph = this.context.getAudioGraph(false);
        this.setGraph(graph);
        super.refreshData();
    }
}
//# sourceMappingURL=AudioGraphDialog.js.map