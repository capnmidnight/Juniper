import { BaseGraphDialog } from "@juniper-lib/graphics2d/BaseGraphDialog";
import { GraphNode } from "@juniper-lib/collections/GraphNode";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
export class SceneGraphDialog extends BaseGraphDialog {
    constructor(env) {
        super("Scene graph", (obj) => obj.name, () => "grey", () => 1);
        this.env = env;
        window.addEventListener("keypress", (evt) => {
            if (evt.key === "~") {
                this.showDialog();
            }
        });
    }
    onShown() {
        const nodes = new Map();
        this.env.scene.traverse((obj) => {
            nodes.set(obj, new GraphNode(obj));
        });
        for (const [obj, node2] of nodes) {
            const node1 = nodes.get(obj.parent);
            if (isDefined(node1)) {
                node1.connectTo(node2);
            }
        }
        this.setGraph(Array.from(nodes.values()));
        super.onShown();
    }
}
//# sourceMappingURL=SceneGraphDialog.js.map