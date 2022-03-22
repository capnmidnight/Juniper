import { CanvasImageTypes, createCanvasFromImageBitmap, isImageBitmap, isOffscreenCanvas } from "juniper-dom/canvas";
import { IFetcher } from "juniper-fetcher";
import { IProgress, isNumber } from "juniper-tslib";

const inchesPerMeter = 39.3701;

type Material = THREE.Material & {
    map: THREE.Texture;
};

export class TexturedMesh extends THREE.Mesh<THREE.BufferGeometry, Material> {
    isVideo: boolean;
    private _imageWidth: number = 0;
    private _imageHeight: number = 0;

    constructor(protected fetcher: IFetcher, geom: THREE.BufferGeometry, mat: Material) {
        super(geom, mat);

        this.isVideo = false;

        this.onBeforeRender = () => {
            if (this.isVideo) {
                this.updateTexture();
            }
        };
    }

    override copy(source: this, recursive = true): this {
        super.copy(source, recursive);
        this.fetcher = source.fetcher;
        this.isVideo = source.isVideo;
        this._imageWidth = source.imageWidth;
        this._imageHeight = source.imageHeight;
        return this;
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

    setImage(img: CanvasImageTypes): THREE.Texture {
        this._imageWidth = img.width;
        this._imageHeight = img.height;

        if (isImageBitmap(img)) {
            img = createCanvasFromImageBitmap(img);
        }

        if (isOffscreenCanvas(img)) {
            img = img as any as HTMLCanvasElement;
        }

        this.isVideo = img instanceof HTMLVideoElement;

        this.material.map = img instanceof HTMLVideoElement
            ? new THREE.VideoTexture(img)
            : new THREE.Texture(img);

        this.material.map.needsUpdate = true;
        this.material.needsUpdate = true;

        return this.material.map;
    }

    async loadImage(path: string, prog?: IProgress): Promise<void> {
        let { content: img } = await this.fetcher
            .get(path)
            .progress(prog)
            .image();
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
