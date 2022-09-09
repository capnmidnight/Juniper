import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { MeshBasicMaterial, Object3D, Quaternion, Vector3 } from "three";
import { solidBlue, solidGreen, solidRed } from "./materials";
import { ErsatzObject, obj, objectIsVisible, objectSetVisible } from "./objects";
import { Translator } from "./Translator";

export class TransformEditorMovingEvent extends TypedEvent<"moving"> {
    constructor() {
        super("moving");
    }
}

export class TransformEditorMovedEvent extends TypedEvent<"moved"> {
    constructor() {
        super("moved");
    }
}

interface TransformEditorEvents {
    moving: TransformEditorMovingEvent;
    moved: TransformEditorMovedEvent;
}

export enum TransformEditorMode {
    Move = "Move",
    Orbit = "Orbit",
    Rotate = "Rotate",
    Resize = "Resize"
}

export class TransformEditor
    extends TypedEventBase<TransformEditorEvents>
    implements ErsatzObject {
    readonly object: Object3D;

    private translators: Translator[];

    private _size: number = 1;

    private readonly movingEvt = new TransformEditorMovingEvent();
    private readonly movedEvt = new TransformEditorMovedEvent();

    private _mode: TransformEditorMode;

    constructor(mode: TransformEditorMode, defaultAvatarHeight: number) {
        super();

        this.object = obj("Translator",
            ...this.translators = [
                this.setTranslator("+X", 1, 0, 0, solidRed, defaultAvatarHeight),
                this.setTranslator("-X", -1, 0, 0, solidRed, defaultAvatarHeight),
                this.setTranslator("+Y", 0, 1, 0, solidGreen, defaultAvatarHeight),
                this.setTranslator("-Y", 0, -1, 0, solidGreen, defaultAvatarHeight),
                this.setTranslator("+Z", 0, 0, 1, solidBlue, defaultAvatarHeight),
                this.setTranslator("-Z", 0, 0, -1, solidBlue, defaultAvatarHeight)
            ]
        );

        this.mode = mode;
    }

    get mode() {
        return this._mode;
    }

    set mode(v) {
        if (v !== this.mode) {
            this._mode = v;
            this.translators[4].object.visible
                = this.translators[5].object.visible
                = this.mode !== TransformEditorMode.Orbit
                && this.mode !== TransformEditorMode.Resize;
        }
    }

    get size(): number {
        return this._size;
    }

    set size(v: number) {
        this._size = v;
        for (const translator of this.translators) {
            translator.size = this.size * 0.5;
        }
    }

    private readonly p = new Vector3();    
    private readonly start = new Vector3();
    private readonly end = new Vector3();
    private readonly up = new Vector3();
    private readonly q = new Quaternion();

    private setTranslator(name: string, sx: number, sy: number, sz: number, color: MeshBasicMaterial, defaultAvatarHeight: number): Translator {
        const translator = new Translator(name, sx, sy, sz, color);
        translator.size = this.size * 0.5;
        translator.addEventListener("dragdir", (evt) => {
            this.object.parent.parent.getWorldPosition(this.p);
            this.object.parent.position.y -= defaultAvatarHeight;

            const dist = this.object.parent.position.length();

            this.start
                .copy(this.object.parent.position)
                .sub(this.p)
                .normalize();

            if (this.mode === TransformEditorMode.Orbit
                || this.mode === TransformEditorMode.Move) {
                this.object
                    .parent
                    .position
                    .add(evt.delta);

                if (this.mode === TransformEditorMode.Orbit) {
                    this.object.parent.position
                        .normalize()
                        .multiplyScalar(dist);

                    this.end
                        .copy(this.object.parent.position)
                        .sub(this.p)
                        .normalize();

                    const d = this.start.dot(this.end);
                    if (-1 <= d && d <= 1) {
                        const a = Math.acos(d);
                        this.up.crossVectors(this.start, this.end).normalize();
                        this.q.setFromAxisAngle(this.up, a);
                        this.object.parent.quaternion.premultiply(this.q);
                    }
                }
            }

            this.object.parent.position.y += defaultAvatarHeight;

            this.dispatchEvent(this.movingEvt);
        });

        translator.addEventListener("up", () =>
            this.dispatchEvent(this.movedEvt));

        return translator;
    }

    get visible() {
        return objectIsVisible(this);
    }

    set visible(v) {
        objectSetVisible(this, v);
    }
}
