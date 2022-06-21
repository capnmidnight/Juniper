import { TextImage } from "@juniper-lib/graphics2d/TextImage";
import { Image2D } from "./Image2D";
import { IWebXRLayerManager } from "./IWebXRLayerManager";

const redrawnEvt = { type: "redrawn" };

export class TextMesh extends Image2D {
    private _textImage: TextImage = null;

    private _onRedrawn: () => void;

    constructor(env: IWebXRLayerManager, name: string, materialOptions?: THREE.MeshBasicMaterialParameters) {
        super(env, name, false, materialOptions);

        this._onRedrawn = this.onRedrawn.bind(this);
    }

    private onRedrawn() {
        this.updateTexture();
        this.scale.set(this._textImage.width, this._textImage.height, 0.01);
        this.dispatchEvent(redrawnEvt);
    }


    get textImage() {
        return this._textImage;
    }

    set textImage(v) {
        if (v !== this.textImage) {
            if (this.textImage) {
                this.textImage.clearEventListeners();
            }

            this._textImage = v;

            if (this.textImage) {
                this.textImage.addEventListener("redrawn", this._onRedrawn);
                this.setImage(this.textImage.canvas);
                this._onRedrawn();
            }
        }
    }
}