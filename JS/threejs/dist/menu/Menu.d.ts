import type { FontDescription } from "@juniper-lib/dom";
import { TextDirection } from "@juniper-lib/graphics2d";
import { IProgress } from "@juniper-lib/progress";
import { Object3D } from "three";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { Image2D } from "../widgets/Image2D";
import { TextMesh } from "../widgets/TextMesh";
export type MenuImagesCollection = {
    backButton: string;
    defaultButton: string;
    title: string;
    logo: {
        front?: string;
        back: string;
    };
};
export interface MenuItemDescription {
    name: string;
    text?: string;
    filePath?: string;
    front?: (Image2D | TextMesh) & {
        width?: number;
    };
    noLabel?: boolean;
    back?: Image2D;
    width?: number;
    height?: number;
    textPosition?: string;
    textDirection?: TextDirection;
    enabled?: boolean;
    clickable?: boolean;
}
export interface MenuDescription {
    images: MenuImagesCollection;
    font: FontDescription;
}
type menuItemCallback<T extends MenuItemDescription> = (item: T) => void;
export declare class Menu extends Object3D {
    private readonly env;
    private readonly logo;
    private readonly backButton;
    private readonly nextButton;
    private readonly prevButton;
    private readonly menuTitle;
    private readonly defaultButtonImage;
    private readonly animator;
    private readonly lastMenuIndex;
    private readonly buttons;
    private menuFont;
    private curBlowout;
    constructor(env: BaseEnvironment<unknown>);
    load(description: MenuDescription, prog?: IProgress): Promise<void>;
    showMenu<T extends MenuItemDescription>(menuID: number, title: string, items: MenuItemDescription[], onClick: menuItemCallback<T>, onBack: () => void, prog?: IProgress): Promise<void>;
    private disableAll;
    private showMenuInternal;
    update(dt: number): void;
    private setButtonPosition;
    private createMenuItem;
    private blowOut;
    private blowOutChild;
}
export {};
//# sourceMappingURL=Menu.d.ts.map