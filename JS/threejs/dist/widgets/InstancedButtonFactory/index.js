import { Exception, arrayCompare, arrayRemove, arrayReplace, nextPowerOf2 } from "@juniper-lib/util";
import { PriorityMap } from "@juniper-lib/collections";
import { createUICanvas } from "@juniper-lib/dom";
import { AssetImage } from "@juniper-lib/fetcher";
import { Image_Png } from "@juniper-lib/mediatypes";
import { CanvasTexture, Color, DoubleSide, DynamicDrawUsage, InstancedMesh, ShaderMaterial, Uniform } from "three";
import { plane } from "../../Plane";
import { obj, objGraph, objectIsFullyVisible } from "../../objects";
import { InstancedMeshButton } from "./InstancedMeshButton";
import fragShader from "./fragment.glsl";
import vertShader from "./vertex.glsl";
const vertexShader = vertShader.replace("#define THREEZ", "");
const fragmentShader = fragShader.replace("#define THREEZ", "");
export class InstancedButtonFactory {
    get enabledInstances() { return this._enabledInstances; }
    get disabledInstances() { return this._disabledInstances; }
    constructor(imagePaths, padding, buttonFillColor, labelFillColor, debug) {
        this.imagePaths = imagePaths;
        this.padding = padding;
        this.buttonFillColor = buttonFillColor;
        this.labelFillColor = labelFillColor;
        this.uvDescrips = new PriorityMap();
        this.canvas = null;
        this._enabledInstances = null;
        this._disabledInstances = null;
        this.buttons = new Array();
        this.content3d = obj("InstancedButtonFactory");
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
            const duv = iconWidth / canvWidth;
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
                const v = heightRatio * (1 - r / rows) - duv;
                const x = c * iconWidth;
                const y = r * iconHeight + canvHeight - height;
                const w = iconWidth - 2 * this.padding;
                const h = iconHeight - 2 * this.padding;
                g.drawImage(img, 0, 0, img.width, img.height, x + this.padding, y + this.padding, w, h);
                this.uvDescrips.add(setName, imgName, new Color(u, v, duv));
                ++i;
            }
            const texture = new CanvasTexture(this.canvas);
            const map = new Uniform(texture);
            const enabledMaterial = new ShaderMaterial({
                vertexShader,
                fragmentShader,
                side: DoubleSide,
                uniforms: {
                    map
                }
            });
            enabledMaterial.needsUpdate = true;
            const disabledMaterial = enabledMaterial.clone();
            disabledMaterial.transparent = true;
            disabledMaterial.uniforms.opacity = new Uniform(0.5);
            disabledMaterial.needsUpdate = true;
            objGraph(this, this._enabledInstances = new InstancedMesh(plane, enabledMaterial, 100), this._disabledInstances = new InstancedMesh(plane, disabledMaterial, 100));
            this._enabledInstances.setColorAt(0, new Color(0, 0, 0));
            this._enabledInstances.name = "EnabledInstances";
            this._enabledInstances.instanceMatrix.setUsage(DynamicDrawUsage);
            this._disabledInstances.setColorAt(0, new Color(0, 0, 0));
            this._disabledInstances.name = "DisabledInstances";
            this._disabledInstances.instanceMatrix.setUsage(DynamicDrawUsage);
            const enableds = new Array();
            const disableds = new Array();
            const colors = new PriorityMap();
            const matrices = new PriorityMap();
            const rebuild = (instances, buttons, filter) => () => {
                arrayReplace(buttons, this.buttons.filter(b => objectIsFullyVisible(b) && filter(b)));
                let colorsChanged = false;
                let matricesChanged = false;
                for (let i = 0; i < buttons.length; ++i) {
                    const btn = buttons[i];
                    btn.instanceId = i;
                    instances.setColorAt(i, btn.icon);
                    colorsChanged = colorsChanged
                        || btn.icon !== colors.get(instances, i);
                    colors.add(instances, i, btn.icon);
                    instances.setMatrixAt(i, btn.content3d.matrixWorld);
                    matricesChanged = matricesChanged
                        || !matrices.has(instances, i)
                        || arrayCompare(matrices.get(instances, i), btn.content3d.matrixWorld.elements) !== -1;
                    if (!matrices.has(instances, i)) {
                        matrices.add(instances, i, btn.content3d.matrix.toArray());
                    }
                    else {
                        btn.content3d.matrix.toArray(matrices.get(instances, i), 0);
                    }
                }
                instances.count = buttons.length;
                instances.instanceColor.needsUpdate = colorsChanged;
                instances.instanceMatrix.needsUpdate = matricesChanged;
            };
            this._enabledInstances.onBeforeRender = rebuild(this._enabledInstances, enableds, b => b.enabled);
            this._disabledInstances.onBeforeRender = rebuild(this._disabledInstances, disableds, b => b.disabled);
        });
    }
    async getCanvas() {
        await this.ready;
        return this.canvas;
    }
    async getInstancedMeshButton(setName, iconName, size) {
        await this.ready;
        const uvSet = this.uvDescrips.get(setName);
        const uv = uvSet && uvSet.get(iconName);
        if (!uvSet || !uv) {
            throw new Exception(`Button ${setName}/${iconName} does not exist`, this.uvDescrips);
        }
        const button = new InstancedMeshButton(this, `btn.${setName}.${iconName}`, size, uv);
        this.buttons.push(button);
        return button;
    }
    deleteButton(btn) {
        arrayRemove(this.buttons, btn);
    }
    getImageSrc(setName, iconName) {
        const imageSet = this.imagePaths.get(setName);
        const imgSrc = imageSet && imageSet.get(iconName);
        if (!imgSrc) {
            throw new Exception(`Button ${setName}/${iconName} does not exist`, this.uvDescrips);
        }
        return imgSrc;
    }
}
//# sourceMappingURL=index.js.map