import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib";
import { solidBlue, solidGreen, solidRed } from "./materials";
import { ErsatzObject, obj, objectIsVisible, objectSetVisible } from "./objects";
import { Translator } from "./Translator";

const P = new THREE.Vector3();

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

export class TransformEditor extends TypedEventBase<TransformEditorEvents> implements ErsatzObject {
    readonly object: THREE.Object3D;

    private translators: Translator[];

    private _size: number = 1;

    private readonly movingEvt = new TransformEditorMovingEvent();
    private readonly movedEvt = new TransformEditorMovedEvent();

    constructor(orbitTranslate: boolean, defaultAvatarHeight: number) {
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

        this.orbit = orbitTranslate;
    }

    get orbit() {
        return !this.translators[4].visible;
    }

    set orbit(v: boolean) {
        if (v !== this.orbit) {
            this.translators[4].visible
                = this.translators[5].visible
                = !v;
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

    private setTranslator(name: string, sx: number, sy: number, sz: number, color: THREE.MeshBasicMaterial, defaultAvatarHeight: number): Translator {
        const translator = new Translator(name, sx, sy, sz, color);
        translator.size = this.size * 0.5;
        translator.target.addEventListener("dragdir", (evt) => {
            this.object.parent.position.y -= defaultAvatarHeight;

            const dist = this.object.parent.position.length();

            this.object
                .parent
                .position
                .add(evt.delta);

            if (this.orbit) {
                this.object.parent.position
                    .normalize()
                    .multiplyScalar(dist);

                this.object.parent.parent.getWorldPosition(P);
                this.object.parent.lookAt(P);
            }

            this.object.parent.position.y += defaultAvatarHeight;

            this.dispatchEvent(this.movingEvt);
        });

        translator.target.addEventListener("dragend", () =>
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
