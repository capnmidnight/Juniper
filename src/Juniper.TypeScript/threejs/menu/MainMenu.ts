import { CanvasImageTypes } from "juniper-dom/canvas";
import { TimerTickEvent } from "juniper-timers";
import {
    buildTree,
    dispose,
    IProgress,
    isDefined,
    isNullOrUndefined,
    progressOfArray,
    progressSplitWeighted,
    progressTasksWeighted,
    TreeNode,
    TypedEventBase
} from "juniper-tslib";
import { cleanup } from "../cleanup";
import type { Application, ApplicationEvents } from "../environment/Application";
import { ApplicationJoinRoomEvent } from "../environment/ApplicationEvents";
import type { Environment } from "../environment/Environment";
import { Image2DMesh } from "../Image2DMesh";
import { Menu, MenuDescription, MenuItemDescription } from "./Menu";
import { MenuItemData } from "./models";

type Node = TreeNode<MenuItemData>;


export class MainMenu
    extends TypedEventBase<ApplicationEvents>
    implements Application {
    private readonly menu: Menu;
    private readonly node2MenuItem = new Map<Node, MenuItemDescription>();
    private readonly menuItem2Node = new Map<MenuItemDescription, Node>();
    private readonly images = new Map<string, CanvasImageTypes>();

    private menuDesc: MenuDescription = null;
    private curMenu: Node = null;
    private menuRoot: Node = null;

    constructor(public readonly env: Environment) {
        super();

        this.menu = new Menu(this.env);
        this.env.worldUISpace.add(this.menu);

        Object.seal(this);
    }

    async init(params: Map<string, unknown>): Promise<void> {
        this.menuDesc = params.get("config") as MenuDescription;
    }

    async load(onProgress?: IProgress) {
        await progressTasksWeighted(onProgress, [
            [5, prog => this.menu.load(this.menuDesc, prog)],
            [1, prog => this.loadMenuData(prog)]
        ]);
    }

    private async loadMenuData(onProgress?: IProgress): Promise<void> {
        this.destroyMenuItems();

        const menuItems = await this.env.fetcher
            .get("/VR/Menu" + (location.search.includes("all") ? "/1" : ""))
            .progress(onProgress)
            .object<MenuItemData[]>();

        this.menuRoot = buildTree(
            menuItems,
            v => v.id,
            v => v.parentID,
            v => v.order
        );

        for (const node of this.menuRoot.breadthFirst()) {
            const value = node.value;
            if (value !== null) {
                const item: MenuItemDescription = {
                    name: value.label,
                    filePath: value.filePath,
                    enabled: value.enabled
                };
                this.node2MenuItem.set(node, item);
                this.menuItem2Node.set(item, node);
            }
        }
    }

    private async loadMenuItems(parent: Node, onProgress?: IProgress) {
        const imageSet = Array.from(new Set(parent.children.map(m => m.value.filePath)))
            .filter(f => isDefined(f) && !this.images.has(f));

        await progressOfArray(
            onProgress,
            imageSet,
            async (f, prog) => {
                let { content: img } = await this.env.fetcher
                    .get(f)
                    .progress(prog)
                    .canvasImage();
                this.images.set(f, img);
            });

        for (const child of parent.children) {
            const item = this.node2MenuItem.get(child);
            if (isNullOrUndefined(item.back)
                && this.images.has(item.filePath)) {
                const imgMesh = new Image2DMesh(this.env, item.filePath + item.name);
                imgMesh.mesh.setImage(this.images.get(item.filePath));
                imgMesh.frustumCulled = false;
                item.back = imgMesh;
            }
        }
    }

    async show(onProgress?: IProgress) {
        this.dispatchEvent(new ApplicationJoinRoomEvent(this, "lobby"));
        const selection = this.curMenu || this.menuRoot;
        await progressTasksWeighted(onProgress, [
            [10, prog => {
                this.env.skybox.rotation = null;
                return this.env.skybox.setImagePath("/VR/LandingPageImage", prog);
            }],
            [1, prog => this.showMenuSelection(selection, prog)]
        ]);
        this.menu.visible = true;
    }

    update(evt: TimerTickEvent) {
        this.menu.update(evt.dt);
    }

    dispose() {
        this.env.worldUISpace.remove(this.menu);
        cleanup(this.menu);
        this.menu.removeFromParent();
        this.destroyMenuItems();
    }

    private destroyMenuItems() {
        for (const img of this.images.values()) {
            dispose(img);
        }

        this.images.clear();
        this.node2MenuItem.clear();
        this.menuItem2Node.clear();
    }

    private async showMenuSelection(selection: Node, onProgress?: IProgress) {
        this.curMenu = selection;
        const [imgProg, showProg] = progressSplitWeighted(onProgress, [10, 1]);
        await this.loadMenuItems(selection, imgProg);

        const menuItems = selection
            .children
            .map(child => this.node2MenuItem.get(child));

        const onBack = selection.isRoot
            ? null
            : () => this.selectMenuItem(selection.parent);

        let menuID = 0;
        let label = "Menu";

        if (selection.value) {
            menuID = selection.value.id;
            label = selection.value.label;
        }

        await this.menu.showMenu(
            menuID,
            label,
            menuItems,
            (item) => {
                const node = this.menuItem2Node.get(item);
                this.selectMenuItem(node);
            },
            onBack,
            showProg);
    }

    private async selectMenuItem(selection: Node, onProgress?: IProgress) {
        if (selection.isLeaf) {
            this.menu.visible = false;
            const { actionApp, actionParam } = selection.value;
            const req = this.env.apps.app(actionApp);
            for (const part of actionParam.split(",")) {
                const [a, ...bs] = part.split(":");
                req.param(a, bs.join(":"));
            }
            await req.load();
        }
        else {
            await this.showMenuSelection(selection, onProgress);
        }
    }
}
