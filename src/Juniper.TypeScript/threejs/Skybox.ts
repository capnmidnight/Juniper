import { CubeMapFaceIndex } from "juniper-2d/CubeMapFaceIndex";
import type { CanvasImageTypes, CanvasTypes, Context2D } from "juniper-dom/canvas";
import { createUtilityCanvas, isImageBitmap } from "juniper-dom/canvas";
import {
    Exception,
    IProgress,
    isArray,
    isDefined,
    isGoodNumber,
    isNumber,
    isString,
    LRUCache
} from "juniper-tslib";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { isEuler, isQuaternion } from "./typeChecks";

type SkyboxRotation = THREE.Quaternion | THREE.Euler | number[] | number;

const U = new THREE.Vector3(0, 1, 0);
const FACE_SIZE = 2048;
const FACE_SIZE_HALF = FACE_SIZE / 2;
const FACES = [1,
    0,
    2,
    3,
    4,
    5
];

const CUBEMAP_PATTERN = {
    rows: 3,
    columns: 4,
    indices: [
        [CubeMapFaceIndex.None, CubeMapFaceIndex.Up, CubeMapFaceIndex.None, CubeMapFaceIndex.None],
        [CubeMapFaceIndex.Left, CubeMapFaceIndex.Front, CubeMapFaceIndex.Right, CubeMapFaceIndex.Back],
        [CubeMapFaceIndex.None, CubeMapFaceIndex.Down, CubeMapFaceIndex.None, CubeMapFaceIndex.None]
    ],
    rotations: [
        [0, Math.PI, 0, 0],
        [0, 0, 0, 0],
        [0, Math.PI, 0, 0]
    ]
};

const black = new THREE.Color(0x000000);

export class Skybox {

    private readonly rt = new THREE.WebGLCubeRenderTarget(FACE_SIZE);
    private readonly rtScene = new THREE.Scene();
    private readonly rtCamera = new THREE.CubeCamera(0.01, 10, this.rt);
    private readonly _rotation = new THREE.Quaternion();
    private readonly layerRotation = new THREE.Quaternion().identity();
    private readonly stageRotation = new THREE.Quaternion().identity();
    private readonly imgCache = new LRUCache<string, CanvasImageTypes>(10);
    private readonly imgTasks = new Map<string, Promise<CanvasImageTypes>>();
    private readonly frames = new Array<CanvasTypes>(6);
    private readonly frameContexts = new Array<Context2D>(6);

    private readonly cube: THREE.CubeTexture;
    private readonly flipped: CanvasTypes;
    private readonly flipper: Context2D;
    
    private curImagePath: string = null;
    private layer: XRCubeLayer = null;
    private wasVisible = false;
    private wasWebXRLayerAvailable: boolean = null;
    private stageHeading = 0;
    private rotationNeedsUpdate = false;
    private imageNeedsUpdate = false;
    private webXRLayerEnabled = true;

    visible = true;

    constructor(private readonly env: BaseEnvironment<unknown>) {

        this.webXRLayerEnabled &&= this.env.hasXRCompositionLayers;

        this.env.scene.background = black;

        this.imgCache.addEventListener("itemevicted", (evt) => {
            if (isImageBitmap(evt.value)) {
                evt.value.close();
            }
        });

        for (let i = 0; i < this.frames.length; ++i) {
            const f = this.frames[i] = createUtilityCanvas(FACE_SIZE, FACE_SIZE);
            this.frameContexts[i] = f.getContext("2d");
        }

        for (let row = 0; row < CUBEMAP_PATTERN.rows; ++row) {
            const indices = CUBEMAP_PATTERN.indices[row];
            const rotations = CUBEMAP_PATTERN.rotations[row];
            for (let column = 0; column < CUBEMAP_PATTERN.columns; ++column) {
                const i = indices[column];
                if (i > -1) {
                    const g = this.frameContexts[i];
                    const rotation = rotations[column];
                    if (rotation > 0) {
                        if ((rotation % 2) === 0) {
                            g.translate(FACE_SIZE_HALF, FACE_SIZE_HALF);
                        }
                        else {
                            g.translate(FACE_SIZE_HALF, FACE_SIZE_HALF);
                        }
                        g.rotate(rotation);
                        g.translate(-FACE_SIZE_HALF, -FACE_SIZE_HALF);
                    }
                }
            }
        }

        this.cube = new THREE.CubeTexture(this.frames);
        this.cube.name = "SkyboxInput";

        this.rt.texture.name = "SkyboxOutput";
        this.rtScene.add(this.rtCamera);
        this.rtScene.background = this.cube;

        this.flipped = createUtilityCanvas(FACE_SIZE, FACE_SIZE);
        this.flipper = this.flipped.getContext("2d");
        this.flipper.fillStyle = black.getHexString();
        this.flipper.scale(-1, 1);
        this.flipper.translate(-FACE_SIZE, 0);

        Object.seal(this);
    }

    async prefetch(path: string, prog: IProgress): Promise<void> {
        if (!this.imgCache.has(path)) {
            await this.getImage(path, prog);
        }
        else if (prog) {
            prog.end("skip");
        }
    }

    private getImage(path: string, prog: IProgress): Promise<CanvasImageTypes> {
        if (this.imgCache.has(path)) {
            return Promise.resolve(this.imgCache.get(path));
        }

        if (this.imgTasks.has(path)) {
            return this.imgTasks.get(path);
        }

        const task = this.env.fetcher
            .get(path)
            .progress(prog)
            .image()
            .then((response) => {
                this.imgCache.set(path, response.content);
                this.imgTasks.delete(path);
                return response.content;
            });

        this.imgTasks.set(path, task);
        return task;
    }

    async isCached(path: string): Promise<boolean> {
        return await this.imgCache.has(path);
    }

    clearCache(): void {
        this.imgCache.clear();
    }

    async setImagePath(path: string, prog?: IProgress) {
        if (!isString(path)) {
            throw new Exception("path must be defined");
        }

        if (path !== this.curImagePath) {
            this.curImagePath = path;

            const image = await this.getImage(path, prog);
            const width = image.width / CUBEMAP_PATTERN.columns;
            const height = image.height / CUBEMAP_PATTERN.rows;
            for (let row = 0; row < CUBEMAP_PATTERN.rows; ++row) {
                const indices = CUBEMAP_PATTERN.indices[row];
                for (let column = 0; column < CUBEMAP_PATTERN.columns; ++column) {
                    const i = indices[column];
                    if (i > -1) {
                        const g = this.frameContexts[i];
                        g.drawImage(
                            image,
                            column * width, row * height,
                            width, height,
                            0, 0,
                            FACE_SIZE, FACE_SIZE);
                    }
                }
            }

            this.cube.needsUpdate = true;
            this.imageNeedsUpdate = true;
        }
        else if (prog) {
            prog.end();
        }
    }

    get rotation(): THREE.Quaternion {
        return this._rotation;
    }

    set rotation(rotation: SkyboxRotation) {
        const { x, y, z, w } = this._rotation;

        if (isQuaternion(rotation)) {
            this._rotation.copy(rotation);
        }
        else if (isEuler(rotation)) {
            this._rotation.setFromEuler(rotation);
        }
        else if (isArray(rotation)) {
            if (rotation.length === 4
                && isNumber(rotation[0])
                && isNumber(rotation[1])
                && isNumber(rotation[2])
                && isNumber(rotation[3])) {
                this._rotation.fromArray(rotation);
            }
            else {
                throw new Error("Skybox rotation was not a valid array format. Needs an array of 4 numbers.");
            }
        }
        else if (isGoodNumber(rotation)) {
            this._rotation.setFromAxisAngle(U, rotation);
        }
        else {
            if (isDefined(rotation)) {
                console.warn("Skybox rotation must be a THREE.Quaternion, THREE.Euler, number[] (representing a Quaternion), or a number (representing rotation about the Y-axis).");
            }
            this._rotation.identity();
        }

        this.rotationNeedsUpdate = this._rotation.x !== x
            || this._rotation.y !== y
            || this._rotation.z !== z
            || this._rotation.w !== w;
    }

    update(frame: XRFrame) {
        if (this.cube) {
            const isWebXRLayerAvailable = this.webXRLayerEnabled
                && this.env.renderer.xr.isPresenting
                && isDefined(frame)
                && isDefined(this.env.xrBinding);

            if (isWebXRLayerAvailable !== this.wasWebXRLayerAvailable) {
                if (isWebXRLayerAvailable) {
                    const space = this.env.renderer.xr.getReferenceSpace();

                    this.layer = this.env.xrBinding.createCubeLayer({
                        space,
                        layout: "mono",
                        isStatic: false,
                        viewPixelWidth: FACE_SIZE,
                        viewPixelHeight: FACE_SIZE
                    });

                    this.env.addWebXRLayer(this.layer, Number.MAX_VALUE);
                }
                else if (this.layer) {
                    this.env.removeWebXRLayer(this.layer);
                    this.layer.destroy();
                    this.layer = null;
                }

                this.imageNeedsUpdate
                    = this.rotationNeedsUpdate
                    = true;
            }

            this.env.scene.background = this.layer
                ? null
                : this.visible
                    ? this.rt.texture
                    : black;

            if (this.layer) {
                if (this.visible !== this.wasVisible
                    || this.layer.needsRedraw) {
                    this.imageNeedsUpdate = true;
                }

                if (this.env.avatar.heading !== this.stageHeading) {
                    this.rotationNeedsUpdate = true;
                    this.stageHeading = this.env.avatar.heading;
                    this.stageRotation.setFromAxisAngle(U, this.env.avatar.heading);
                }
            }
            else {
                this.rotationNeedsUpdate
                    = this.imageNeedsUpdate
                    = this.imageNeedsUpdate
                    || this.rotationNeedsUpdate;
            }

            if (this.rotationNeedsUpdate) {
                this.layerRotation
                    .copy(this.rotation)
                    .invert();

                if (this.layer) {
                    this.layerRotation.multiply(this.stageRotation);
                    this.layer.orientation = new DOMPointReadOnly(
                        this.layerRotation.x,
                        this.layerRotation.y,
                        this.layerRotation.z,
                        this.layerRotation.w);
                }
                else {
                    this.rtCamera.quaternion.copy(this.layerRotation);
                }
            }


            if (this.imageNeedsUpdate) {
                if (this.layer) {
                    const gl = this.env.renderer.getContext();
                    const gLayer = this.env.xrBinding.getSubImage(this.layer, frame);
                    const imgs = this.cube.images as CanvasImageTypes[];

                    this.flipper.fillRect(0, 0, FACE_SIZE, FACE_SIZE);

                    gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
                    gl.bindTexture(gl.TEXTURE_CUBE_MAP, gLayer.colorTexture);

                    for (let i = 0; i < imgs.length; ++i) {
                        if (this.visible) {
                            const img = imgs[FACES[i]];
                            this.flipper.drawImage(
                                img,
                                0, 0, img.width, img.height,
                                0, 0, FACE_SIZE, FACE_SIZE);
                        }

                        gl.texSubImage2D(
                            gl.TEXTURE_CUBE_MAP_POSITIVE_X + i,
                            0,
                            0, 0,
                            gl.RGBA,
                            gl.UNSIGNED_BYTE,
                            this.flipped);
                    }

                    gl.bindTexture(gl.TEXTURE_CUBE_MAP, null);
                }
                else {
                    this.rtCamera.update(this.env.renderer, this.rtScene);
                }
            }

            this.imageNeedsUpdate = false;
            this.rotationNeedsUpdate = false;
            this.wasVisible = this.visible;
            this.wasWebXRLayerAvailable = isWebXRLayerAvailable;
        }
    }
}