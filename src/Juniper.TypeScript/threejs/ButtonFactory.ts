import { src, title } from "@juniper/dom/attrs";
import { CanvasImageTypes, CanvasTypes, createUICanvas } from "@juniper/dom/canvas";
import { Img } from "@juniper/dom/tags";
import type { IFetcher } from "@juniper/fetcher";
import { Exception, IProgress, nextPowerOf2, PoppableParentProgressCallback, PriorityMap, progressPopper, Task } from "@juniper/tslib";

interface UVRect {
    u: number;
    v: number;
    du: number;
    dv: number;
}

async function loadIcon(fetcher: IFetcher, setName: string, iconName: string, iconPath: string, popper: PoppableParentProgressCallback): Promise<[string, string, CanvasImageTypes]> {
    const { content } = await fetcher
        .get(iconPath)
        .progress(popper.pop())
        .image();
    return [
        setName,
        iconName,
        content
    ];
}

export class ButtonFactory {
    private readonly uvDescrips = new PriorityMap<string, string, UVRect>();
    private readonly geoms = new PriorityMap<string, string, THREE.BufferGeometry>();

    private readonly readyTask: Task<void>;

    private canvas: CanvasTypes = null;
    private texture: THREE.Texture = null;
    private enabledMaterial: THREE.MeshBasicMaterial = null;
    private disabledMaterial: THREE.MeshBasicMaterial = null;

    constructor(private readonly fetcher: IFetcher, private readonly imagePaths: PriorityMap<string, string, string>, private readonly padding: number) {
        this.readyTask = new Task();
    }

    async load(prog?: IProgress) {
        const popper = progressPopper(prog);
        const imageSets = new PriorityMap(await Promise.all(
            Array.from(this.imagePaths.entries())
                .map(([setName, iconName, path]) =>
                    loadIcon(this.fetcher, setName, iconName, path, popper))));
        const images = Array.from(imageSets.values());
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

        const g = this.canvas.getContext("2d");
        g.fillStyle = "#1e4388";
        g.fillRect(0, 0, canvWidth, canvHeight);

        let i = 0;
        for (const [setName, imgName, img] of imageSets.entries()) {
            const c = i % cols;
            const r = (i - c) / cols;
            const u = widthRatio * (c * iconWidth / width);
            const v = heightRatio * (1 - r / rows) - dv;
            const x = c * iconWidth;
            const y = r * iconHeight + canvHeight - height;
            const w = iconWidth - 2 * this.padding;
            const h = iconHeight - 2 * this.padding;

            g.drawImage(img,
                0, 0, img.width, img.height,
                x + this.padding, y + this.padding, w, h);
            this.uvDescrips.add(setName, imgName, { u, v, du, dv });

            ++i;
        }

        this.texture = new THREE.CanvasTexture(this.canvas as any);
        this.enabledMaterial = new THREE.MeshBasicMaterial({
            map: this.texture,
        });
        this.enabledMaterial.needsUpdate = true;
        this.disabledMaterial = new THREE.MeshBasicMaterial({
            map: this.texture,
            transparent: true,
            opacity: 0.5
        });
        this.disabledMaterial.needsUpdate = true;

        this.readyTask.resolve();
    }

    getSets() {
        return Array.from(this.imagePaths.keys());
    }

    getIcons(setName: string) {
        if (!this.imagePaths.has(setName)) {
            throw new Exception(`Button set ${setName} does not exist`);
        }

        return Array.from(this.imagePaths.get(setName).keys());
    }

    async getMaterial(enabled: boolean) {
        await this.readyTask;
        return enabled
            ? this.enabledMaterial
            : this.disabledMaterial;
    }

    async getGeometry(setName: string, iconName: string) {
        await this.readyTask;
        const uvSet = this.uvDescrips.get(setName);
        const uv = uvSet && uvSet.get(iconName);
        if (!uvSet || !uv) {
            throw new Exception(`Button ${setName}/${iconName} does not exist`, this.uvDescrips);
        }


        let geom = this.geoms.get(setName, iconName);
        if (!geom) {
            geom = new THREE.PlaneBufferGeometry(1, 1, 1, 1)
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
        return geom;
    }

    async getMesh(setName: string, iconName: string, enabled: boolean) {
        const geom = await this.getGeometry(setName, iconName);
        const mesh = new THREE.Mesh(
            geom,
            enabled
                ? this.enabledMaterial
                : this.disabledMaterial);
        mesh.name = `Mesh:${setName}/${iconName}`;
        return mesh;
    }

    async getGeometryAndMaterials(setName: string, iconName: string) {
        const [geometry, enabledMaterial, disabledMaterial] = await Promise.all([
            this.getGeometry(setName, iconName),
            this.getMaterial(true),
            this.getMaterial(false)
        ]);

        return {
            geometry,
            enabledMaterial,
            disabledMaterial
        }
    }

    getImageSrc(setName: string, iconName: string) {
        const imageSet = this.imagePaths.get(setName);
        const imgSrc = imageSet && imageSet.get(iconName);
        if (!imageSet || !imgSrc) {
            throw new Exception(`Button ${setName}/${iconName} does not exist`, this.uvDescrips);
        }

        return imgSrc;
    }

    getImageElement(setName: string, iconName: string) {
        return Img(
            title(setName + " " + iconName),
            src(this.getImageSrc(setName, iconName)));
    }
}
