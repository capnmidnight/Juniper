import { PointerName } from "@juniper-lib/tslib/events/PointerName";
import { BaseScreenPointerSinglePoint } from "./BaseScreenPointerSinglePoint";
import type { EventSystem } from "./EventSystem";

export class PointerSingleTouch extends BaseScreenPointerSinglePoint {
    constructor(evtSys: EventSystem, renderer: THREE.WebGLRenderer, camera: THREE.PerspectiveCamera) {
        super("touch", PointerName.Touch0, evtSys, renderer, camera);
        Object.seal(this);
    }
}
