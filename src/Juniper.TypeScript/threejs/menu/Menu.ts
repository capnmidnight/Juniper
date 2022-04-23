import { Animator } from "@juniper/2d/animation/Animator";
import { bump } from "@juniper/2d/animation/tween";
import { TextDirection, TextImageOptions } from "@juniper/2d/TextImage";
import type { FontDescription } from "@juniper/dom/fonts";
import { loadFont } from "@juniper/dom/fonts";
import { arrayReplace, clamp, IProgress, isFunction, isGoodNumber, isString, progressOfArray, progressTasksWeighted, TaskDef } from "@juniper/tslib";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { Image2DMesh } from "../Image2DMesh";
import { TextMesh } from "../TextMesh";
import { MenuItem } from "./MenuItem";

const zero = new THREE.Vector3(0, 0, 0);
const PAGE_SIZE = 5;

export type MenuImagesCollection = {
    backButton: string;
    defaultButton: string;
    title: string,
    logo: {
        front?: string;
        back: string;
    };
};

export interface MenuItemDescription {
    name: string;
    text?: string;
    filePath?: string;
    front?: (Image2DMesh | TextMesh) & {
        width?: number;
    };
    noLabel?: boolean;
    back?: Image2DMesh;
    width?: number;
    height?: number;
    textPosition?: string;
    textDirection?: TextDirection;
    enabled?: boolean;
    clickable?: boolean;
    wrapWords?: boolean;
}

export interface MenuDescription {
    images: MenuImagesCollection;
    font: FontDescription;
}

type menuItemCallback<T extends MenuItemDescription> = (item: T) => void;

export class Menu extends THREE.Object3D {
    private readonly logo: MenuItemDescription = null;
    private readonly backButton: MenuItemDescription;
    private readonly nextButton: MenuItemDescription;
    private readonly prevButton: MenuItemDescription;
    private readonly menuTitle: MenuItemDescription;
    private readonly defaultButtonImage: Image2DMesh;
    private readonly animator = new Animator();
    private readonly lastMenuIndex = new Map<number, number>();
    private readonly buttons = new Array<MenuItem>();

    private menuFont: FontDescription = null;
    private curBlowout: Promise<void> = Promise.resolve();

    constructor(private readonly env: BaseEnvironment<unknown>) {
        super();

        this.name = "Menu";

        this.defaultButtonImage = new Image2DMesh(this.env, "DefaultButton", true);

        this.logo = {
            name: "Logo",
            noLabel: true,
            back: new Image2DMesh(this.env, "LogoBack", true),
            width: 1,
            clickable: false
        };

        this.backButton = {
            name: "Back",
            back: new Image2DMesh(this.env, "BackButton", true)
        };

        this.nextButton = {
            name: "Next",
            back: new Image2DMesh(this.env, "NextButton", true),
            width: 0.25,
            textDirection: "vertical",
            textPosition: "middle",
            wrapWords: false,
            enabled: true
        };

        this.prevButton = {
            name: "Previous",
            back: new Image2DMesh(this.env, "PrevButton", true),
            width: 0.25,
            height: 1,
            textDirection: "vertical",
            textPosition: "top",
            wrapWords: false,
            enabled: true
        };

        this.menuTitle = {
            name: "Menu",
            back: new Image2DMesh(this.env, "MenuTitle", true),
            width: 0.25,
            textDirection: "vertical",
            textPosition: "top",
            clickable: false,
            wrapWords: false
        };
    }

    async load(description: MenuDescription, prog?: IProgress) {
        this.menuFont = description.font;

        let imgs = description.images;

        const tasks: TaskDef[] = [
            [1, (prog) => loadFont(this.menuFont, null, prog)],
            [1, (prog) => this.backButton.back.mesh.loadImage(this.env.fetcher, imgs.backButton, prog)],
            [1, (prog) => this.defaultButtonImage.mesh.loadImage(this.env.fetcher, imgs.defaultButton, prog)],
            [1, (prog) => this.menuTitle.back.mesh.loadImage(this.env.fetcher, imgs.title, prog)],
            [1, (prog) => this.nextButton.back.mesh.loadImage(this.env.fetcher, imgs.title, prog)],
            [1, (prog) => this.prevButton.back.mesh.loadImage(this.env.fetcher, imgs.title, prog)],
            [1, (prog) => this.logo.back.mesh.loadImage(this.env.fetcher, imgs.logo.back, prog)]
        ];

        if (imgs.logo.front) {
            this.logo.front = new Image2DMesh(this.env, "LogoFront", true, { transparent: true });
            tasks.push([1, (prog) => this.logo.front.mesh.loadImage(this.env.fetcher, imgs.logo.front, prog)]);
        }

        await progressTasksWeighted(prog, tasks);

        if (imgs.logo.front) {
            this.logo.front.width = 1;
        }
    }

    async showMenu<T extends MenuItemDescription>(
        menuID: number,
        title: string,
        items: MenuItemDescription[],
        onClick: menuItemCallback<T>,
        onBack: () => void,
        prog?: IProgress) {

        let pageSize = PAGE_SIZE;
        do {
            const rem = items.length % pageSize;
            if (rem === 0
                || rem === pageSize - 1
                || pageSize === Math.ceil(PAGE_SIZE / 2)) {
                break;
            }
            else {
                --pageSize
            }
        } while (true);

        const index = this.lastMenuIndex.get(menuID) || 0;

        await this.showMenuInternal(menuID, title, items, pageSize, index, onClick, onBack, prog);
    }


    private disableAll(): void {
        setTimeout(() => {
            for (const button of this.buttons) {
                button.disabled = true;
            }
        }, 10);
    }

    private async showMenuInternal<T extends MenuItemDescription>(
        menuID: number,
        title: string,
        items: MenuItemDescription[],
        pageSize: number,
        index: number,
        onClick: menuItemCallback<T>,
        onBack: () => void,
        prog?: IProgress) {

        this.lastMenuIndex.set(menuID, index);

        await this.curBlowout;

        this.clear();

        this.menuTitle.text = title;

        const displayItems = [this.logo, this.menuTitle];

        if (index > 0) {
            displayItems.push(this.prevButton);
        }

        displayItems.push(...items.slice(index, index + Math.min(pageSize, items.length - index)));

        if (index < items.length - pageSize) {
            displayItems.push(this.nextButton);
        }

        const oldOnClick = onClick;
        onClick = (item: T) => {
            this.disableAll();
            oldOnClick(item);
        };

        if (onBack) {
            displayItems.push(this.backButton);

            const oldOnBack = onBack;
            onBack = () => {
                this.disableAll();
                oldOnBack();
            };
        }

        const onPrev = () => {
            this.disableAll();
            this.showMenuInternal(menuID, title, items, pageSize, index - pageSize, onClick, onBack);
        };

        const onNext = () => {
            this.disableAll();
            this.showMenuInternal(menuID, title, items, pageSize, index + pageSize, onClick, onBack);
        };

        for (const button of this.buttons) {
            button.update(0);
        }

        const buttons = await progressOfArray(prog, displayItems, (item: MenuItemDescription, prog: IProgress) => {
            if (item === this.backButton) {
                return this.createMenuItem(item, onBack, prog);
            }
            else if (item === this.prevButton) {
                return this.createMenuItem(item, onPrev, prog);
            }
            else if (item === this.nextButton) {
                return this.createMenuItem(item, onNext, prog);
            }
            else {
                return this.createMenuItem(item, onClick, prog);
            }
        });

        arrayReplace(this.buttons, ...buttons);

        const space = 0.05;
        const radius = 3;
        const midPoint = (this.buttons.length - 1) / 2;
        const l = Math.ceil(midPoint);
        const r = Math.floor(midPoint + 1);
        const left = this.buttons.slice(0, l).reverse();
        const mid = this.buttons.slice(l, r);
        const right = this.buttons.slice(r, this.buttons.length);

        let midWidth = 0;
        let a = 0;
        for (const button of mid) {
            midWidth += button.width + space;
            this.setButtonPosition(button, a, radius);
        }

        midWidth /= 2;

        a = -midWidth / radius;
        for (const button of left) {
            a -= 0.5 * (button.width + space) / radius;
            this.setButtonPosition(button, a, radius);
            a -= 0.5 * (button.width + space) / radius;
        }

        a = midWidth / radius;
        for (const button of right) {
            a += 0.5 * (button.width + space) / radius;
            this.setButtonPosition(button, a, radius);
            a += 0.5 * (button.width + space) / radius;
        }

        for (const button of this.buttons) {
            this.add(button);
        }

        await this.blowOut(false);

        for (const button of this.buttons) {
            button.back.frustumCulled = true;
        }
    }

    update(dt: number, frame: XRFrame): void {
        this.animator.update(dt);
        for (const button of this.buttons) {
            button.update(dt, frame);
        }
    }

    private setButtonPosition(button: MenuItem, a: number, radius: number) {
        const x = radius * Math.sin(a);
        const z = -radius * Math.cos(a);
        button.position.set(x, 0, z);
        button.lookAt(zero);
        button.startX = x;
        button.position.x = x - 10;
    }

    private async createMenuItem(item: MenuItemDescription, onClick?: menuItemCallback<MenuItemDescription> | Function, prog?: IProgress): Promise<MenuItem> {

        if (!item.back) {
            if (item.filePath) {
                item.back = new Image2DMesh(this.env, `${item.name}-Background`, true);
                await item.back.mesh.loadImage(this.env.fetcher, item.filePath, prog);
            }
            else {
                item.back = this.defaultButtonImage.clone() as Image2DMesh;
            }
            item.back.frustumCulled = false;
        }

        if (!isGoodNumber(item.width)) {
            item.width = 0.5;
        }

        if (!isGoodNumber(item.height)) {
            item.height = 1;
        }

        if (!isString(item.textDirection)) {
            item.textDirection = "horizontal";
        }

        if (!isString(item.textPosition)) {
            item.textPosition = "bottom";
        }

        if (!isString(item.text)) {
            item.text = item.name;
        }

        item.wrapWords = item.wrapWords !== false;

        const enabled = item.enabled !== false;

        if (item.text.length > 0) {
            if (!item.front && !item.noLabel) {
                const options: Partial<TextImageOptions> = {
                    textFillColor: enabled ? "#ffffff" : "#505050",
                    textDirection: item.textDirection,
                    padding: {
                        top: 0.025,
                        right: 0.05,
                        bottom: 0.025,
                        left: 0.05
                    },
                    scale: 400,
                    wrapWords: item.wrapWords,
                    fontFamily: this.menuFont.fontFamily,
                    fontSize: this.menuFont.fontSize,
                    fontStyle: this.menuFont.fontStyle,
                    fontVariant: this.menuFont.fontVariant,
                    fontWeight: this.menuFont.fontWeight,
                    maxWidth: item.width,
                    maxHeight: item.height
                };
                const img = new TextMesh(this.env, item.text);
                img.mesh.renderOrder = 1;

                img.addEventListener("redrawn", () => {
                    const y = (img.textHeight - item.height + 0.025) / 2;
                    if (item.textPosition === "bottom") {
                        img.position.y = y;
                    }
                    else {
                        img.position.y = -y;
                    }
                });

                img.createTextImage(options);

                img.position.z = -0.01;

                item.front = img;
            }

            if (item.front) {
                item.front.frustumCulled = false;

                if (item.front instanceof TextMesh) {
                    item.front.value = item.text;
                }
            }
        }

        const button = new MenuItem(
            item.width,
            item.height,
            item.name,
            item.front,
            item.back,
            item.clickable !== false,
            enabled);

        if (!button.disabled && isFunction(onClick)) {
            button.addEventListener("click", () => {
                this.curBlowout = this.blowOut(true);
                onClick(item);
            });
        }

        if (prog) {
            prog.end("button loaded");
        }

        return button;
    }

    private async blowOut(d: boolean) {
        await Promise.all(this.children.map((child, i) => {
            if (child instanceof MenuItem) {
                return this.blowOutChild(child, i, d);
            }
            else {
                return Promise.resolve();
            }
        }));
        this.animator.clear();
    }

    private async blowOutChild(child: MenuItem, i: number, d: boolean): Promise<void> {
        const wasDisabled = child.disabled;
        child.disabled = true;

        await this.animator.start(0.125 * i, 0.5, (t: number) => {
            const st = clamp(d ? t : (1 - t), 0, 1);
            child.position.x = child.startX - 10 * bump(st, 0.15);
        });

        child.disabled = wasDisabled;
    }
}