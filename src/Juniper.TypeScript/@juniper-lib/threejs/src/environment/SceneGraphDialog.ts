import { BaseGraphDialog } from "@juniper-lib/graphics2d/dist/BaseGraphDialog";
import { GraphNode } from "@juniper-lib/collections/dist/GraphNode";
import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { Object3D } from "three";
import type { BaseEnvironment } from "./BaseEnvironment";

export class SceneGraphDialog extends BaseGraphDialog<Object3D> {
    constructor(private readonly env: BaseEnvironment<unknown>) {
        super("Scene graph", (obj) => obj.name, () => "grey", () => 1);

        window.addEventListener("keypress", (evt) => {
            if (evt.key === "~") {
                this.showDialog();
            }
        });
    }

    override onShown() {
        const nodes = new Map<Object3D, GraphNode<Object3D>>();

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
