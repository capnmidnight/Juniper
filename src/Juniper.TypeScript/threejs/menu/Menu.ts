import { Animator } from "juniper-2d/animation/Animator";
import { bump } from "juniper-2d/animation/tween";
import { TextDirection, TextImageOptions } from "juniper-2d/TextImage";
import type { FontDescription } from "juniper-dom/fonts";
import { loadFont } from "juniper-dom/fonts";
import type { IFetcher } from "juniper-fetcher";
import type { IProgress } from "juniper-tslib";
import {
    clamp,
    isFunction,
    isGoodNumber,
    isString,
    progressOfArray,
    progressTasksWeighted,
    TaskDef
} from "juniper-tslib";
import { cleanup } from "../cleanup";
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

    private menuFont: FontDescription = null;
    private curBlowout: Promise<void> = Promise.resolve();

    constructor(private readonly fetcher: IFetcher) {
        super();

        this.name = "Menu";

        this.defaultButtonImage = new Image2DMesh("DefaultButton");

        this.logo = {
            name: "Logo",
            noLabel: true,
            back: new Image2DMesh("LogoBack"),
            width: 1,
            clickable: false
        };

        this.backButton = {
            name: "Back",
            back: new Image2DMesh("BackButton")
        };

        this.nextButton = {
            name: "Next",
            back: new Image2DMesh("NextButton"),
            width: 0.25,
            textDirection: "vertical",
            textPosition: "middle",
            wrapWords: false,
            enabled: true
        };

        this.prevButton = {
            name: "Previous",
            back: new Image2DMesh("PrevButton"),
            width: 0.25,
            height: 1,
            textDirection: "vertical",
            textPosition: "top",
            wrapWords: false,
            enabled: true
        };

        this.menuTitle = {
            name: "Menu",
            back: new Image2DMesh("MenuTitle"),
            width: 0.25,
            textDirection: "vertical",
            textPosition: "top",
            clickable: false,
            wrapWords: false
        };
    }

    async load(description: MenuDescription, onProgress?: IProgress) {
        this.menuFont = description.font;

        let imgs = description.images;

        const tasks: TaskDef[] = [
            [1, (prog) => loadFont(this.menuFont, null, prog)],
            [1, (prog) => this.backButton.back.loadImage(imgs.backButton, this.fetcher, prog)],
            [1, (prog) => this.defaultButtonImage.loadImage(imgs.defaultButton, this.fetcher, prog)],
            [1, (prog) => this.menuTitle.back.loadImage(imgs.title, this.fetcher, prog)],
            [1, (prog) => this.nextButton.back.loadImage(imgs.title, this.fetcher, prog)],
            [1, (prog) => this.prevButton.back.loadImage(imgs.title, this.fetcher, prog)],
            [1, (prog) => this.logo.back.loadImage(imgs.logo.back, this.fetcher, prog)]
        ];

        if (imgs.logo.front) {
            this.logo.front = new Image2DMesh("LogoFront", { transparent: true });
            tasks.push([1, (prog) => this.logo.front.loadImage(imgs.logo.front, this.fetcher, prog)]);
        }

        await progressTasksWeighted(onProgress, tasks);

        if (imgs.logo.front) {
            this.logo.front.width = 1;
        }
    }

    clearMenu() {
        cleanup(this);
    }

    async showMenu<T extends MenuItemDescription>(
        menuID: number,
        title: string,
        items: MenuItemDescription[],
        onClick: menuItemCallback<T>,
        onBack: () => void,
        onProgress?: IProgress) {

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

        await this.showMenuInternal(menuID, title, items, pageSize, index, onClick, onBack, onProgress);
    }

    private async showMenuInternal<T extends MenuItemDescription>(
        menuID: number,
        title: string,
        items: MenuItemDescription[],
        pageSize: number,
        index: number,
        onClick: menuItemCallback<T>,
        onBack: () => void,
        onProgress?: IProgress) {

        this.lastMenuIndex.set(menuID, index);

        await this.curBlowout;

        this.clearMenu();

        this.menuTitle.text = title;

        const displayItems = [this.logo, this.menuTitle];

        if (index > 0) {
            displayItems.push(this.prevButton);
        }

        displayItems.push(...items.slice(index, index + Math.min(pageSize, items.length - index)));

        if (index < items.length - pageSize) {
            displayItems.push(this.nextButton);
        }

        const disableAll = () => {
            setTimeout(() => {
                for (const button of buttons) {
                    button.disabled = true;
                }
            }, 10);
        };

        const oldOnClick = onClick;
        onClick = (item: T) => {
            disableAll();
            oldOnClick(item);
        };

        if (onBack) {
            displayItems.push(this.backButton);

            const oldOnBack = onBack;
            onBack = () => {
                disableAll();
                oldOnBack();
            };
        }

        const onPrev = () => {
            disableAll();
            this.showMenuInternal(menuID, title, items, pageSize, index - pageSize, onClick, onBack);
        };

        const onNext = () => {
            disableAll();
            this.showMenuInternal(menuID, title, items, pageSize, index + pageSize, onClick, onBack);
        };

        const buttons = await progressOfArray(onProgress, displayItems, (item: MenuItemDescription, onProgress: IProgress) => {
            if (item === this.backButton) {
                return this.createMenuItem(item, onBack, onProgress);
            }
            else if (item === this.prevButton) {
                return this.createMenuItem(item, onPrev, onProgress);
            }
            else if (item === this.nextButton) {
                return this.createMenuItem(item, onNext, onProgress);
            }
            else {
                return this.createMenuItem(item, onClick, onProgress);
            }
        });

        const space = 0.05;
        const radius = 3;
        const midPoint = (buttons.length - 1) / 2;
        const l = Math.ceil(midPoint);
        const r = Math.floor(midPoint + 1);
        const left = buttons.slice(0, l).reverse();
        const mid = buttons.slice(l, r);
        const right = buttons.slice(r, buttons.length);

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

        for (const button of buttons) {
            this.add(button);
        }

        await this.blowOut(false);
    }

    update(dt: number): void {
        this.animator.update(dt);
    }

    private setButtonPosition(button: MenuItem, a: number, radius: number) {
        const x = radius * Math.sin(a);
        const z = -radius * Math.cos(a);
        button.position.set(x, 0, z);
        button.lookAt(zero);
        button.startX = x;
        button.position.x = x - 10;
    }

    private async createMenuItem(item: MenuItemDescription, onClick?: menuItemCallback<MenuItemDescription> | Function, onProgress?: IProgress): Promise<MenuItem> {

        if (!item.back) {
            if (item.filePath) {
                item.back = new Image2DMesh(`${item.name}-Background`);
                await item.back.loadImage(item.filePath, this.fetcher, onProgress);
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
                const img = new TextMesh(item.text);

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

        if (onProgress) {
            onProgress.report(1, 1, "button loaded");
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