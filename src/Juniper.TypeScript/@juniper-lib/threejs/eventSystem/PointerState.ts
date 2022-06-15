export class PointerState {
    buttons = 0;
    moveDistance = 0;
    dragDistance = 0;
    readonly position = new THREE.Vector2();
    readonly motion = new THREE.Vector2();
    dz = 0;
    readonly uv = new THREE.Vector2();
    readonly duv = new THREE.Vector2();
    canClick = false;
    dragging = false;
    ctrl = false;
    alt = false;
    shift = false;
    meta = false;

    constructor() {
        Object.seal(this);
    }

    copy(ptr: PointerState) {
        this.buttons = ptr.buttons;
        this.moveDistance = ptr.moveDistance;
        this.dragDistance = ptr.dragDistance;
        this.position.copy(ptr.position);
        this.motion.copy(ptr.motion);
        this.dz = ptr.dz;
        this.uv.copy(ptr.uv);
        this.duv.copy(ptr.duv);
        this.canClick = ptr.canClick;
        this.dragging = ptr.dragging;
        this.ctrl = ptr.ctrl;
        this.alt = ptr.alt;
        this.shift = ptr.shift;
        this.meta = ptr.meta;
    }
}
