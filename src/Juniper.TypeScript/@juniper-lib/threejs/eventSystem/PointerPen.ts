import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";
import type { EventSystem } from "./EventSystem";

export class PointerPen extends BaseScreenPointerSinglePoint {
    constructor(evtSys: EventSystem, renderer: THREE.WebGLRenderer, camera: THREE.PerspectiveCamera) {
        super("pen", PointerName.Pen, evtSys, renderer, camera);
        Object.seal(this);
    }
}
