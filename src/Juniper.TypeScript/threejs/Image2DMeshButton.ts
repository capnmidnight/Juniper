import { TextImageOptions } from "juniper-2d/TextImage";
import { IFetcher } from "juniper-fetcher";
import { isNullOrUndefined, stringRandom } from "juniper-tslib";
import { scaleOnHover } from "./animation/scaleOnHover";
import { Image2DMesh } from "./Image2DMesh";
import { IWebXRLayerManager } from "./IWebXRLayerManager";
import { TextMeshButton } from "./TextMeshButton";


export class Image2DMeshButton extends TextMeshButton {

    private readonly enabledSubImage: Image2DMesh;
    private readonly disabledSubImage: Image2DMesh;

    constructor(fetcher: IFetcher, env: IWebXRLayerManager, name: string, imagePath: string, textButtonStyle: Partial<TextImageOptions>, disabledImagePath: string = null) {
        super(fetcher, env, `${name}-button`, "      ", textButtonStyle);

        const id = stringRandom(16);

        this.enabledSubImage = this.createSubImage(`${id}-enabled`, 1, imagePath);
        this.enabledImage.add(this.enabledSubImage);

        let disabledOpacity = 1;
        if (isNullOrUndefined(disabledImagePath)) {
            disabledImagePath = imagePath;
            disabledOpacity = 0.75;
        }

        this.disabledSubImage = this.createSubImage(`${id}-disabled`, disabledOpacity, disabledImagePath);
        this.disabledImage.add(this.disabledSubImage);

        scaleOnHover(this);
    }

    private createSubImage(id: string, opacity: number, path: string) {
        const image = new Image2DMesh(
            this.fetcher,
            this.env,
            `text-${id}`,
            false, {
            side: THREE.FrontSide,
            opacity
        });

        image.position.z = 0.01;
        image.scale.setScalar(0.75);

        image.mesh.loadImage(path);

        return image;
    }
}