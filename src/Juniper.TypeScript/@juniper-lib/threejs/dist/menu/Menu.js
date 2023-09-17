import { loadFont } from "@juniper-lib/dom/dist/fonts";
import { AssetImage } from "@juniper-lib/fetcher/dist/Asset";
import { unwrapResponse } from "@juniper-lib/fetcher/dist/unwrapResponse";
import { Animator } from "@juniper-lib/graphics2d/dist/animation/Animator";
import { bump } from "@juniper-lib/graphics2d/dist/animation/tween";
import { Image_Jpeg, Image_Png } from "@juniper-lib/mediatypes";
import { arrayReplace } from "@juniper-lib/collections/dist/arrays";
import { clamp } from "@juniper-lib/tslib/dist/math";
import { progressOfArray } from "@juniper-lib/progress/dist/progressOfArray";
import { progressTasksWeighted } from "@juniper-lib/progress/dist/progressTasks";
import { isFunction, isGoodNumber, isString } from "@juniper-lib/tslib/dist/typeChecks";
import { Object3D, Vector3 } from "three";
import { objGraph } from "../objects";
import { Image2D } from "../widgets/Image2D";
import { TextMesh } from "../widgets/TextMesh";
import { MenuItem } from "./MenuItem";
const zero = new Vector3(0, 0, 0);
const PAGE_SIZE = 5;
export class Menu extends Object3D {
    constructor(env) {
        super();
        this.env = env;
        this.logo = null;
        this.animator = new Animator();
        this.lastMenuIndex = new Map();
        this.buttons = new Array();
        this.menuFont = null;
        this.curBlowout = Promise.resolve();
        this.name = "Menu";
        this.defaultButtonImage = new Image2D(this.env, "DefaultButton", "none");
        this.logo = {
            name: "Logo",
            noLabel: true,
            back: new Image2D(this.env, "LogoBack", "none"),
            width: 1,
            clickable: false
        };
        this.backButton = {
            name: "Back",
            back: new Image2D(this.env, "BackButton", "none")
        };
        this.nextButton = {
            name: "Next",
            back: new Image2D(this.env, "NextButton", "none"),
            width: 0.25,
            textDirection: "vertical",
            textPosition: "middle",
            enabled: true
        };
        this.prevButton = {
            name: "Previous",
            back: new Image2D(this.env, "PrevButton", "none"),
            width: 0.25,
            height: 1,
            textDirection: "vertical",
            textPosition: "top",
            enabled: true
        };
        this.menuTitle = {
            name: "Menu",
            back: new Image2D(this.env, "MenuTitle", "none"),
            width: 0.25,
            textDirection: "vertical",
            textPosition: "top",
            clickable: false
        };
    }
    async load(description, prog) {
        this.menuFont = description.font;
        const imgs = description.images;
        const backButtonAsset = new AssetImage(imgs.backButton, Image_Jpeg, !this.env.DEBUG);
        const defaultButtonAsset = new AssetImage(imgs.defaultButton, Image_Jpeg, !this.env.DEBUG);
        const titleAsset = new AssetImage(imgs.title, Image_Jpeg, !this.env.DEBUG);
        const logoBackAsset = new AssetImage(imgs.logo.back, Image_Png, !this.env.DEBUG);
        let logoFrontAsset = null;
        if (imgs.logo.front) {
            logoFrontAsset = new AssetImage(imgs.logo.front, Image_Png, !this.env.DEBUG);
            this.logo.front = new Image2D(this.env, "LogoFront", "none", { transparent: true });
        }
        await progressTasksWeighted(prog, [
            [1, (prog) => loadFont(this.menuFont, null, prog)],
            [5, (prog) => this.env.fetcher.assets(prog, backButtonAsset, defaultButtonAsset, titleAsset, logoBackAsset, logoFrontAsset)]
        ]);
        this.backButton.back.setTextureMap(backButtonAsset.result);
        this.defaultButtonImage.setTextureMap(defaultButtonAsset.result);
        this.menuTitle.back.setTextureMap(titleAsset.result);
        this.nextButton.back.setTextureMap(titleAsset.result);
        this.prevButton.back.setTextureMap(titleAsset.result);
        this.logo.back.setTextureMap(logoBackAsset.result);
        if (imgs.logo.front) {
            this.logo.front.setTextureMap(logoFrontAsset.result);
            this.logo.front.width = 1;
        }
    }
    async showMenu(menuID, title, items, onClick, onBack, prog) {
        let pageSize = PAGE_SIZE;
        do {
            const rem = items.length % pageSize;
            if (rem === 0
                || rem === pageSize - 1
                || pageSize === Math.ceil(PAGE_SIZE / 2)) {
                break;
            }
            else {
                --pageSize;
            }
        } while (true);
        const index = this.lastMenuIndex.get(menuID) || 0;
        await this.showMenuInternal(menuID, title, items, pageSize, index, onClick, onBack, prog);
    }
    disableAll() {
        setTimeout(() => {
            for (const button of this.buttons) {
                button.disabled = true;
            }
        }, 10);
    }
    async showMenuInternal(menuID, title, items, pageSize, index, onClick, onBack, prog) {
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
        onClick = (item) => {
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
        const buttons = await progressOfArray(prog, displayItems, (item, prog) => {
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
        objGraph(this, ...this.buttons);
        await this.blowOut(false);
        for (const button of this.buttons) {
            button.back.frustumCulled = true;
        }
    }
    update(dt) {
        this.animator.update(dt);
    }
    setButtonPosition(button, a, radius) {
        const x = radius * Math.sin(a);
        const z = -radius * Math.cos(a);
        button.object.position.set(x, 0, z);
        button.object.lookAt(zero);
        button.startX = x;
        button.object.position.x = x - 10;
    }
    async createMenuItem(item, onClick, prog) {
        if (!item.back) {
            if (item.filePath) {
                item.back = new Image2D(this.env, `${item.name}-Background`, "none");
                const img = await this.env.fetcher
                    .get(item.filePath)
                    .progress(prog)
                    .image()
                    .then(unwrapResponse);
                item.back.setTextureMap(img);
            }
            else {
                item.back = this.defaultButtonImage.clone();
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
        const enabled = item.enabled !== false;
        if (item.text.length > 0) {
            if (!item.front && !item.noLabel) {
                const options = {
                    textFillColor: enabled ? "white" : "dimgray",
                    textDirection: item.textDirection,
                    padding: {
                        top: 0.025,
                        right: 0.05,
                        bottom: 0.025,
                        left: 0.05
                    },
                    scale: 400,
                    fontFamily: this.menuFont.fontFamily,
                    fontSize: this.menuFont.fontSize,
                    fontStyle: this.menuFont.fontStyle,
                    fontVariant: this.menuFont.fontVariant,
                    fontWeight: this.menuFont.fontWeight,
                    maxWidth: item.width,
                    maxHeight: item.height
                };
                const img = new TextMesh(this.env, item.text, "none", options);
                img.mesh.renderOrder = 1;
                img.addEventListener("redrawn", () => {
                    const y = (img.image.height - item.height + 0.025) / 2;
                    if (item.textPosition === "bottom") {
                        img.position.y = y;
                    }
                    else {
                        img.position.y = -y;
                    }
                });
                img.position.z = -0.01;
                item.front = img;
            }
            if (item.front) {
                item.front.frustumCulled = false;
                if (item.front instanceof TextMesh) {
                    item.front.image.value = item.text;
                }
            }
        }
        const button = new MenuItem(item.width, item.height, item.name, item.front, item.back);
        button.clickable = item.clickable !== false;
        button.enabled = enabled;
        if (button.clickable
            && isFunction(onClick)) {
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
    async blowOut(d) {
        await Promise.all(this.buttons.map((child, i) => this.blowOutChild(child, i, d)));
        this.animator.clear();
    }
    async blowOutChild(child, i, d) {
        const wasDisabled = child.disabled;
        child.disabled = true;
        await this.animator.start(0.125 * i, 0.5, (t) => {
            const st = clamp(d ? t : (1 - t), 0, 1);
            child.object.position.x = child.startX - 10 * bump(st, 0.15);
        });
        child.disabled = wasDisabled;
    }
}
//# sourceMappingURL=Menu.js.map