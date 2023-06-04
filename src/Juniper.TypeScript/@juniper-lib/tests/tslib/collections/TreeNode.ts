import { TestCase } from "@juniper-lib/testing/tdd/TestCase";
import { TreeNode } from "@juniper-lib/collections/TreeNode";

export class TreeNodeTests extends TestCase {

    private tree: TreeNode<number>;

    override setup() {
        this.tree = new TreeNode(0);
        const one = new TreeNode(1);
        const two = new TreeNode(2);
        this.tree.connectTo(one);
        this.tree.connectTo(two);
        one.connectTo(new TreeNode(3));
        one.connectTo(new TreeNode(4));
        two.connectTo(new TreeNode(5));
        two.connectTo(new TreeNode(6));
    }

    test_BreadthFirst() {
        const values = Array.from(this.tree.breadthFirst())
            .map(node => node.value);
        this.areExact(values[0], 0);
        this.areExact(values[1], 1);
        this.areExact(values[2], 2);
        this.areExact(values[3], 3);
        this.areExact(values[4], 4);
        this.areExact(values[5], 5);
        this.areExact(values[6], 6);
    }

    test_DepthFirst() {
        const values = Array.from(this.tree.depthFirst())
            .map(node => node.value);
        this.areExact(values[0], 6);
        this.areExact(values[1], 5);
        this.areExact(values[2], 2);
        this.areExact(values[3], 4);
        this.areExact(values[4], 3);
        this.areExact(values[5], 1);
        this.areExact(values[6], 0);
    }
}
