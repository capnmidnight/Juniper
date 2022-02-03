import type { CanvasImageTypes } from "juniper-dom/canvas";
import { createCanvasFromImageBitmap, isImageBitmap, isOffscreenCanvas } from "juniper-dom/canvas";
import type { IProgress } from "juniper-tslib";
import { isNumber } from "juniper-tslib";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { isTexture } from "./typeChecks";

const inchesPerMeter = 39.3701;

type Material = THREE.Material & {
    map: THREE.Texture;
};

export class TexturedMesh extends THREE.Mesh<THREE.BufferGeometry, Material> {
    isVideo: boolean;
    private _imageWidth: number = 0;
    private _imageHeight: number = 0;

    constructor(protected readonly env: BaseEnvironment<unknown>, geom: THREE.BufferGeometry, mat: Material) {
        super(geom, mat);
        this.isVideo = false;

        this.onBeforeRender = () => {
            if (this.isVideo) {
                this.updateTexture();
            }
        };
    }

    get imageWidth() {
        return this._imageWidth;
    }

    get imageHeight() {
        return this._imageHeight;
    }

    get imageAspectRatio() {
        return this.imageWidth / this.imageHeight;
    }

    get objectWidth() {
        return this.scale.x;
    }

    set objectWidth(v) {
        this.scale.x = v;
        this.scale.y = v / this.imageAspectRatio;
    }

    get objectHeight() {
        return this.scale.y;
    }

    set objectHeight(v) {
        this.scale.x = this.imageAspectRatio * v;
        this.scale.y = v;
    }

    get pixelDensity() {
        const ppm = (this.imageWidth / this.objectWidth);
        const ppi = ppm / inchesPerMeter;
        return ppi;
    }

    set pixelDensity(ppi) {
        const ppm = ppi * inchesPerMeter;
        this.objectWidth = this.imageWidth / ppm;
    }

    setImage(img: CanvasImageTypes | THREE.Texture): THREE.Texture {
        if (isTexture(img)) {
            if (isNumber(img.image.width)
                && isNumber(img.image.height)) {
                this._imageWidth = img.image.width;
                this._imageHeight = img.image.height;
            }
            this.isVideo = img.image instanceof HTMLVideoElement;
        }
        else {
            this._imageWidth = img.width;
            this._imageHeight = img.height;

            if (isImageBitmap(img)) {
                img = createCanvasFromImageBitmap(img);
            }

            if (isOffscreenCanvas(img)) {
                img = img as any as HTMLCanvasElement;
            }

            img = new THREE.Texture(img);
            img.needsUpdate = true;

            this.isVideo = img instanceof HTMLVideoElement;
        }

        this.material.map = img;
        this.material.needsUpdate = true;

        return img;
    }

    async loadImage(path: string, onProgress?: IProgress): Promise<void> {
        let { content: img } = await this.env.fetcher
            .get(path)
            .progress(onProgress)
            .canvasImage();
        const texture = this.setImage(img);
        texture.name = path;
    }

    updateTexture() {
        const img = this.material.map.image;
        if (isNumber(img.width)
            && isNumber(img.height)
            && (this.imageWidth !== img.width
                || this.imageHeight !== img.height)) {
            this._imageWidth = img.width;
            this._imageHeight = img.height
            this.material.map.dispose();
            this.material.map = new THREE.Texture(img);
            this.material.needsUpdate = true;
        }
        this.material.map.needsUpdate = true;
    }
}
