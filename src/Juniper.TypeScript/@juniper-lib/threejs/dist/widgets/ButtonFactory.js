import { PriorityMap } from "@juniper-lib/collections/dist/PriorityMap";
import { createUICanvas } from "@juniper-lib/dom/dist/canvas";
import { AssetImage } from "@juniper-lib/fetcher/dist/Asset";
import { Image_Png } from "@juniper-lib/mediatypes";
import { Exception } from "@juniper-lib/tslib/dist/Exception";
import { nextPowerOf2 } from "@juniper-lib/tslib/dist/math";
import { CanvasTexture, MeshBasicMaterial, PlaneGeometry } from "three";
import { MeshButton } from "./MeshButton";
export class ButtonFactory {
    constructor(imagePaths, padding, buttonFillColor, labelFillColor, debug) {
        this.imagePaths = imagePaths;
        this.padding = padding;
        this.buttonFillColor = buttonFillColor;
        this.labelFillColor = labelFillColor;
        this.uvDescrips = new PriorityMap();
        this.geoms = new PriorityMap();
        this.canvas = null;
        this.texture = null;
        this.enabledMaterial = null;
        this.disabledMaterial = null;
        this.assetSets = new PriorityMap(Array.from(this.imagePaths.entries())
            .map(([setName, iconName, path]) => [
            setName,
            iconName,
            new AssetImage(path, Image_Png, !debug)
        ]));
        this.assets = Array.from(this.assetSets.values());
        this.ready = Promise.all(this.assets)
            .then(() => {
            const images = this.assets.map(asset => asset.result);
            const iconWidth = Math.max(...images.map((img) => img.width));
            const iconHeight = Math.max(...images.map((img) => img.height));
            const area = iconWidth * iconHeight * images.length;
            const squareDim = Math.sqrt(area);
            const cols = Math.floor(squareDim / iconWidth);
            const rows = Math.ceil(images.length / cols);
            const width = cols * iconWidth;
            const height = rows * iconHeight;
            const canvWidth = nextPowerOf2(width);
            const canvHeight = nextPowerOf2(height);
            const widthRatio = width / canvWidth;
            const heightRatio = height / canvHeight;
            const du = iconWidth / canvWidth;
            const dv = iconHeight / canvHeight;
            this.canvas = createUICanvas(canvWidth, canvHeight);
            const g = this.canvas.getContext("2d", { alpha: false });
            g.fillStyle = this.buttonFillColor;
            g.fillRect(0, 0, canvWidth, canvHeight);
            let i = 0;
            for (const [setName, imgName, asset] of this.assetSets.entries()) {
                const img = asset.result;
                const c = i % cols;
                const r = (i - c) / cols;
                const u = widthRatio * (c * iconWidth / width);
                const v = heightRatio * (1 - r / rows) - dv;
                const x = c * iconWidth;
                const y = r * iconHeight + canvHeight - height;
                const w = iconWidth - 2 * this.padding;
                const h = iconHeight - 2 * this.padding;
                g.drawImage(img, 0, 0, img.width, img.height, x + this.padding, y + this.padding, w, h);
                this.uvDescrips.add(setName, imgName, { u, v, du, dv });
                ++i;
            }
            this.texture = new CanvasTexture(this.canvas);
            this.enabledMaterial = new MeshBasicMaterial({
                map: this.texture,
            });
            this.enabledMaterial.needsUpdate = true;
            this.disabledMaterial = new MeshBasicMaterial({
                map: this.texture,
                transparent: true,
                opacity: 0.5
            });
            this.disabledMaterial.needsUpdate = true;
        });
    }
    getImageSrc(setName, iconName) {
        const imageSet = this.imagePaths.get(setName);
        const imgSrc = imageSet && imageSet.get(iconName);
        if (!imgSrc) {
            throw new Exception(`Button ${setName}/${iconName} does not exist`, this.uvDescrips);
        }
        return imgSrc;
    }
    async getMeshButton(setName, iconName, size) {
        await this.ready;
        const uvSet = this.uvDescrips.get(setName);
        const uv = uvSet && uvSet.get(iconName);
        if (!uvSet || !uv) {
            throw new Exception(`Button ${setName}/${iconName} does not exist`, this.uvDescrips);
        }
        let geom = this.geoms.get(setName, iconName);
        if (!geom) {
            geom = new PlaneGeometry(1, 1, 1, 1);
            geom.name = `Geometry:${setName}/${iconName}`;
            this.geoms.add(setName, iconName, geom);
            const uvBuffer = geom.getAttribute("uv");
            for (let i = 0; i < uvBuffer.count; ++i) {
                const u = uvBuffer.getX(i) * uv.du + uv.u;
                const v = uvBuffer.getY(i) * uv.dv + uv.v;
                uvBuffer.setX(i, u);
                uvBuffer.setY(i, v);
            }
        }
        const mesh = new MeshButton(iconName, geom, this.enabledMaterial, this.disabledMaterial, size);
        return mesh;
    }
}
//# sourceMappingURL=ButtonFactory.js.map