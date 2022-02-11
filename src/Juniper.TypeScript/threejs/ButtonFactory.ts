import { src, title } from "juniper-dom/attrs";
import { CanvasImageTypes, CanvasTypes, createUICanvas } from "juniper-dom/canvas";
import { Img } from "juniper-dom/tags";
import type { IFetcher } from "juniper-fetcher";
import { Exception, IProgress, nextPowerOf2, PoppableParentProgressCallback, progressPopper } from "juniper-tslib";

interface UVRect {
    u: number;
    v: number;
    du: number;
    dv: number;
}

async function loadIcons(fetcher: IFetcher, setName: string, iconNames: Map<string, string>, popper: PoppableParentProgressCallback): Promise<[string, Map<string, CanvasImageTypes>]> {
    return [
        setName,
        new Map(await Promise.all(
            Array.from(iconNames.entries()).map((pair) =>
                loadIcon(fetcher, pair[0], pair[1], popper))))
    ];
}

async function loadIcon(fetcher: IFetcher, iconName: string, iconPath: string, popper: PoppableParentProgressCallback): Promise<[string, CanvasImageTypes]> {
    const { content } = await fetcher
        .get(iconPath)
        .progress(popper.pop())
        .image();
    return [
        iconName,
        content
    ];
}

export class ButtonFactory {
    private readonly uvDescrips = new Map<string, Map<string, UVRect>>();
    private readonly geoms = new Map<string, Map<string, THREE.BufferGeometry>>();

    private readonly readyTask: Promise<void>;

    private onLoadComplete: () => void = null;
    private iconWidth: number = null;
    private iconHeight: number = null;
    private canvas: CanvasTypes = null;
    private texture: THREE.Texture = null;
    private enabledMaterial: THREE.MeshBasicMaterial = null;
    private disabledMaterial: THREE.MeshBasicMaterial = null;

    constructor(private readonly fetcher: IFetcher, private readonly imagePaths: Map<string, Map<string, string>>, private readonly padding: number) {
        this.readyTask = new Promise((resolve) => {
            this.onLoadComplete = resolve;
        });
    }

    async load(prog?: IProgress) {
        const popper = progressPopper(prog);
        const images = new Map(await Promise.all(
            Array.from(this.imagePaths.entries())
                .map((kv) =>
                    loadIcons(this.fetcher, kv[0], kv[1], popper))));

        const rows = images.size;
        const cols = Math.max(...Array.from(images.values())
            .map(arr => arr.size));

        const width = Math.max(...Array.from(images.values())
            .map(arr => Array.from(arr.values()).map(img => img.width)
                .reduce((a, b) => a + b, 0)));

        const height = Array.from(images.values())
            .map(arr => Math.max(...Array.from(arr.values()).map(img => img.height)))
            .reduce((a, b) => a + b, 0);

        const canvWidth = nextPowerOf2(width);
        const canvHeight = nextPowerOf2(height);

        const widthRatio = width / canvWidth;
        const heightRatio = height / canvHeight;

        this.iconWidth = width / cols;
        this.iconHeight = height / rows;
        const du = this.iconWidth / canvWidth;
        const dv = this.iconHeight / canvHeight;
        this.canvas = createUICanvas(canvWidth, canvHeight);

        const g = this.canvas.getContext("2d");
        g.fillStyle = "#1e4388";
        g.fillRect(0, 0, canvWidth, canvHeight);

        let r = 0;
        for (const [name, imgRows] of images) {
            this.uvDescrips.set(name, new Map<string, UVRect>());

            let c = 0;
            for (const [imgName, img] of imgRows) {
                const u = widthRatio * (c * this.iconWidth / width);
                const v = heightRatio * (1 - r / rows) - dv;
                const x = c * this.iconWidth;
                const y = r * this.iconHeight + canvHeight - height;
                const w = this.iconWidth - 2 * this.padding;
                const h = this.iconHeight - 2 * this.padding;

                g.drawImage(img,
                    0, 0, img.width, img.height,
                    x + this.padding, y + this.padding, w, h);
                this.uvDescrips.get(name).set(imgName, { u, v, du, dv });

                ++c;
            }

            ++r;
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

        this.onLoadComplete();
        this.onLoadComplete = null;
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

        let geomSet = this.geoms.get(setName);
        let geom = geomSet && geomSet.get(iconName);
        if (!geom) {
            if (!geomSet) {
                this.geoms.set(setName, geomSet = new Map<string, THREE.BufferGeometry>());
            }
            geomSet.set(iconName, geom = new THREE.PlaneBufferGeometry(1, 1, 1, 1));

            const uvBuffer = geom.getAttribute("uv");
            for (let i = 0; i < uvBuffer.count; ++i) {
                const u = uvBuffer.getX(i) * uv.du + uv.u;
                const v = uvBuffer.getY(i) * uv.dv + uv.v;
                uvBuffer.setX(i, u);
                uvBuffer.setY(i, v);
            }
        }
        geom.name = `Geometry:${setName}/${iconName}`;
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
