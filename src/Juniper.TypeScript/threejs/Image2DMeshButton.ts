import { TextImageOptions } from "juniper-2d/TextImage";
import type { IFetcher } from "juniper-fetcher";
import { isNullOrUndefined, stringRandom } from "juniper-tslib";
import { scaleOnHover } from "./animation/scaleOnHover";
import { Image2DMesh } from "./Image2DMesh";
import { TextMeshButton } from "./TextMeshButton";


export class Image2DMeshButton extends TextMeshButton {

    private readonly enabledSubImage: Image2DMesh;
    private readonly disabledSubImage: Image2DMesh;

    constructor(fetcher: IFetcher, name: string, imagePath: string, textButtonStyle: Partial<TextImageOptions>, disabledImagePath: string = null) {
        super(`${name}-button`, "      ", textButtonStyle);

        const id = stringRandom(16);

        this.enabledSubImage = this.createSubImage(fetcher, `${id}-enabled`, 1, imagePath);
        this.enabledImage.add(this.enabledSubImage);

        let disabledOpacity = 1;
        if (isNullOrUndefined(disabledImagePath)) {
            disabledImagePath = imagePath;
            disabledOpacity = 0.75;
        }

        this.disabledSubImage = this.createSubImage(fetcher, `${id}-disabled`, disabledOpacity, disabledImagePath);
        this.disabledImage.add(this.disabledSubImage);

        scaleOnHover(this);
    }

    private createSubImage(fetcher: IFetcher, id: string, opacity: number, path: string) {
        const image = new Image2DMesh(
            `text-${id}`, {
                side: THREE.FrontSide,
            opacity
        });

        image.position.z = 0.01;
        image.scale.setScalar(0.75);

        image.loadImage(path, fetcher);

        return image;
    }
}