import { GraphNode, isDefined } from "juniper-tslib";
import { BaseGraphDialog } from "juniper-dom/BaseGraphDialog";
import { BaseEnvironment } from "./BaseEnvironment";

export class SceneGraphDialog extends BaseGraphDialog<THREE.Object3D> {
    constructor(private readonly env: BaseEnvironment<unknown>) {
        super("Scene graph", obj => obj.name);

        window.addEventListener("keypress", (evt) => {
            if (evt.key === '~') {
                this.showDialog();
            }
        });
    }

    override onShown() {
        const nodes = new Map<THREE.Object3D, GraphNode<THREE.Object3D>>();

        this.env.scene.traverse(obj => {
            nodes.set(obj, new GraphNode(obj));
        });

        for (const [obj, node2] of nodes) {
            const node1 = nodes.get(obj.parent);
            if (isDefined(node1)) {
                node1.connectTo(node2);
            }
        }

        this.setGraph(Array.from(nodes.values()));
        this.setOrigin(nodes.get(this.env.scene));

        super.onShown();
    }
}
