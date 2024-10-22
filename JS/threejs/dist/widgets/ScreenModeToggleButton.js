import { ScreenMode } from "../ScreenMode";
import { ToggleButton } from "./ToggleButton";
export class ScreenModeToggleButton extends ToggleButton {
    constructor(buttons, mode) {
        const name = ScreenMode[mode];
        super(buttons, name.toLowerCase(), "enter", "exit");
        this.mode = mode;
    }
}
//# sourceMappingURL=ScreenModeToggleButton.js.map