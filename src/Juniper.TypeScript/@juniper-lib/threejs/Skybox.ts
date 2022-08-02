import type { CanvasImageTypes, CanvasTypes, Context2D } from "@juniper-lib/dom/canvas";
import { createUtilityCanvas } from "@juniper-lib/dom/canvas";
import { CubeMapFaceIndex } from "@juniper-lib/graphics2d/CubeMapFaceIndex";
import { isArray, isDefined, isGoodNumber, isNumber } from "@juniper-lib/tslib";
import { cleanup } from "./cleanup";
import type { BaseEnvironment } from "./environment/BaseEnvironment";
import { XRTimerTickEvent } from "./environment/XRTimer";
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

export const CUBEMAP_PATTERN = /*@__PURE__*/ {
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
    private readonly canvases = new Array<CanvasTypes>(6);
    private readonly contexts = new Array<Context2D>(6);
    private readonly flipped: CanvasTypes;
    private readonly flipper: Context2D;
    private readonly onNeedsRedraw: () => void;
    private readonly onTick: (evt: XRTimerTickEvent) => void;

    private layerOrientation: DOMPointReadOnly = null;
    private images: CanvasImageTypes[] = null;
    private cube: THREE.CubeTexture;
    private curImagePath: string = null;
    private layer: XRCubeLayer = null;
    private wasVisible = false;
    private stageHeading = 0;
    private rotationNeedsUpdate = false;
    private imageNeedsUpdate = false;

    public useWebXRLayers = true;
    private wasWebXRLayerAvailable: boolean = null;

    visible = true;

    constructor(private readonly env: BaseEnvironment<unknown>) {

        this.onNeedsRedraw = () => this.imageNeedsUpdate = true;
        this.onTick = (evt) => this.checkWebXRLayer(evt.frame);

        this.env.scene.background = black;

        for (let i = 0; i < this.canvases.length; ++i) {
            const f = this.canvases[i] = createUtilityCanvas(FACE_SIZE, FACE_SIZE);
            this.contexts[i] = f.getContext("2d", { alpha: false });
        }

        for (let row = 0; row < CUBEMAP_PATTERN.rows; ++row) {
            const indices = CUBEMAP_PATTERN.indices[row];
            const rotations = CUBEMAP_PATTERN.rotations[row];
            for (let column = 0; column < CUBEMAP_PATTERN.columns; ++column) {
                const i = indices[column];
                if (i > -1) {
                    const g = this.contexts[i];
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

        this.rt.texture.name = "SkyboxOutput";
        this.rtScene.add(this.rtCamera);

        this.flipped = createUtilityCanvas(FACE_SIZE, FACE_SIZE);
        this.flipper = this.flipped.getContext("2d", { alpha: false });
        this.flipper.fillStyle = black.getHexString();
        this.flipper.scale(-1, 1);
        this.flipper.translate(-FACE_SIZE, 0);

        this.setImages("", this.canvases);

        this.env.timer.addTickHandler(this.onTick);

        Object.seal(this);
    }

    setImage(imageID: string, image: CanvasImageTypes) {
        if (imageID !== this.curImagePath) {
            const width = image.width / CUBEMAP_PATTERN.columns;
            const height = image.height / CUBEMAP_PATTERN.rows;
            for (let row = 0; row < CUBEMAP_PATTERN.rows; ++row) {
                const indices = CUBEMAP_PATTERN.indices[row];
                for (let column = 0; column < CUBEMAP_PATTERN.columns; ++column) {
                    const i = indices[column];
                    if (i > -1) {
                        const g = this.contexts[i];
                        g.drawImage(
                            image,
                            column * width, row * height,
                            width, height,
                            0, 0,
                            FACE_SIZE, FACE_SIZE);
                    }
                }
            }

            this.setImages(imageID, this.canvases);
        }
    }

    setImages(imageID: string, images: CanvasImageTypes[]) {
        if (imageID !== this.curImagePath
            || images !== this.images) {

            this.curImagePath = imageID;

            if (images !== this.images) {
                if (isDefined(this.cube)) {
                    cleanup(this.cube);
                }

                this.images = images;

                this.rtScene.background = this.cube = new THREE.CubeTexture(this.images);
                this.cube.name = "SkyboxInput";
            }
        }

        this.updateImages();
    }

    updateImages() {
        this.cube.needsUpdate = true;
        this.imageNeedsUpdate = true;
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

    private checkWebXRLayer(frame: XRFrame) {
        if (this.cube) {
            const isWebXRLayerAvailable = this.useWebXRLayers
                && this.env.hasXRCompositionLayers
                && isDefined(frame)
                && isDefined(this.env.xrBinding);

            const webXRLayerChanged = isWebXRLayerAvailable !== this.wasWebXRLayerAvailable;
            if (webXRLayerChanged) {
                if (isWebXRLayerAvailable) {
                    const space = this.env.renderer.xr.getReferenceSpace();

                    this.layer = this.env.xrBinding.createCubeLayer({
                        space,
                        layout: "mono",
                        isStatic: false,
                        viewPixelWidth: FACE_SIZE,
                        viewPixelHeight: FACE_SIZE,
                        orientation: this.layerOrientation
                    });
                    this.layer.addEventListener("redraw", this.onNeedsRedraw);
                    this.env.addWebXRLayer(this.layer, Number.MAX_VALUE);
                }
                else if (this.layer) {
                    this.env.removeWebXRLayer(this.layer);
                    this.layer.removeEventListener("redraw", this.onNeedsRedraw);
                    this.layer.destroy();
                    this.layer = null;
                }

                this.imageNeedsUpdate = true;
            }

            if (!this.layer || !webXRLayerChanged) {
                const visibleChanged = this.visible !== this.wasVisible;
                const headingChanged = this.env.avatar.heading !== this.stageHeading;

                this.imageNeedsUpdate = this.imageNeedsUpdate
                    || visibleChanged
                    || this.layer && this.layer.needsRedraw;

                this.rotationNeedsUpdate = this.rotationNeedsUpdate
                    || headingChanged;

                this.env.scene.background = this.layer
                    ? null
                    : this.visible
                        ? this.rt.texture
                        : black;

                if (this.rotationNeedsUpdate) {
                    this.layerRotation
                        .copy(this.rotation)
                        .invert();

                    this.stageRotation
                        .setFromAxisAngle(U, this.env.avatar.heading)
                        .premultiply(this.layerRotation);

                    this.layerOrientation = new DOMPointReadOnly(
                        this.stageRotation.x,
                        this.stageRotation.y,
                        this.stageRotation.z,
                        this.stageRotation.w);


                    if (this.layer) {
                        this.layer.orientation = this.layerOrientation;
                    }
                    else {
                        this.rtCamera.quaternion.copy(this.layerRotation);
                        this.imageNeedsUpdate = true;
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

                this.stageHeading = this.env.avatar.heading;
                this.imageNeedsUpdate = false;
                this.rotationNeedsUpdate = false;
                this.wasVisible = this.visible;
            }

            this.wasWebXRLayerAvailable = isWebXRLayerAvailable;
        }
    }
}