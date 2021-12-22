import { ButtonFactory } from "../ButtonFactory";
import { ScreenMode } from "../ScreenMode";
import { ToggleButton } from "./ToggleButton";

export class ScreenModeToggleButton extends ToggleButton {
    constructor(buttons: ButtonFactory, public readonly mode: ScreenMode) {
        const name = ScreenMode[mode];
        super(buttons, name.toLowerCase(), "enter", "exit");
    }
}
