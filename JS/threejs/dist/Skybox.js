import { isArray, isDefined, isGoodNumber, isNumber, Pi } from "@juniper-lib/util";
import { createUtilityCanvas, dispose } from "@juniper-lib/dom";
import { CubeMapFaceIndex } from "@juniper-lib/graphics2d";
import { Color, CubeCamera, CubeTexture, Quaternion, Scene, Vector3, WebGLCubeRenderTarget } from "three";
import { cleanup } from "./cleanup";
import { isEuler, isQuaternion } from "./typeChecks";
const U = new Vector3(0, 1, 0);
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
        [0, Pi, 0, 0],
        [0, 0, 0, 0],
        [0, Pi, 0, 0]
    ]
};
const black = new Color(0x000000);
export class Skybox {
    get envMap() { return this._cube; }
    constructor(env) {
        this.env = env;
        this.rt = new WebGLCubeRenderTarget(FACE_SIZE);
        this.rtScene = new Scene();
        this.rtCamera = new CubeCamera(0.01, 10, this.rt);
        this._rotation = new Quaternion();
        this.layerRotation = new Quaternion().identity();
        this.stageRotation = new Quaternion().identity();
        this.canvases = new Array(6);
        this.contexts = new Array(6);
        this.layerOrientation = null;
        this.images = null;
        this.curImagePath = null;
        this.layer = null;
        this.wasVisible = false;
        this.stageHeading = 0;
        this.rotationNeedsUpdate = false;
        this.imageNeedsUpdate = false;
        this.useWebXRLayers = true;
        this.wasWebXRLayerAvailable = null;
        this.visible = true;
        this.onNeedsRedraw = () => this.imageNeedsUpdate = true;
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
        this.env.addScopedEventListener(this, "update", (evt) => this.checkWebXRLayer(evt.frame));
        Object.seal(this);
    }
    clear() {
        this.setImage(null, null);
    }
    setImage(imageID, image) {
        if (imageID !== this.curImagePath) {
            if (isDefined(image)) {
                this.sliceImage(image);
                return this.setImages(imageID, this.canvases);
            }
            else {
                return this.setImages(imageID, null);
            }
        }
        return null;
    }
    sliceImage(image) {
        const width = image.width / CUBEMAP_PATTERN.columns;
        const height = image.height / CUBEMAP_PATTERN.rows;
        for (let row = 0; row < CUBEMAP_PATTERN.rows; ++row) {
            const indices = CUBEMAP_PATTERN.indices[row];
            for (let column = 0; column < CUBEMAP_PATTERN.columns; ++column) {
                const i = indices[column];
                if (i > -1) {
                    const g = this.contexts[i];
                    g.drawImage(image, column * width, row * height, width, height, 0, 0, FACE_SIZE, FACE_SIZE);
                }
            }
        }
        return this.canvases;
    }
    setImages(imageID, images) {
        if (imageID !== this.curImagePath
            || images !== this.images) {
            this.curImagePath = imageID;
            if (images !== this.images) {
                if (isDefined(this._cube)) {
                    cleanup(this._cube);
                    this._cube = null;
                }
                if (isDefined(this.images)) {
                    for (const img of this.images) {
                        cleanup(img);
                    }
                }
                this.images = images;
                if (isDefined(this.images)) {
                    this.rtScene.background
                        = this._cube
                            = new CubeTexture(this.images);
                    this._cube.name = "SkyboxInput";
                }
                else {
                    this.rtScene.background = black;
                }
            }
        }
        this.updateImages();
        return this._cube;
    }
    updateImages() {
        this._cube.needsUpdate = true;
        this.imageNeedsUpdate = true;
    }
    get rotation() {
        return this._rotation;
    }
    set rotation(rotation) {
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
    checkWebXRLayer(frame) {
        if (this._cube) {
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
                    dispose(this.layer);
                    this.layer = null;
                }
                this.imageNeedsUpdate = true;
            }
            if (!this.layer || !webXRLayerChanged) {
                const visibleChanged = this.visible !== this.wasVisible;
                const headingChanged = this.env.avatar.headingRadians !== this.stageHeading;
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
                        .setFromAxisAngle(U, this.env.avatar.headingRadians)
                        .premultiply(this.layerRotation);
                    this.layerOrientation = new DOMPointReadOnly(this.stageRotation.x, this.stageRotation.y, this.stageRotation.z, this.stageRotation.w);
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
                        const imgs = this._cube.images;
                        this.flipper.fillRect(0, 0, FACE_SIZE, FACE_SIZE);
                        gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, false);
                        gl.bindTexture(gl.TEXTURE_CUBE_MAP, gLayer.colorTexture);
                        for (let i = 0; i < imgs.length; ++i) {
                            if (this.visible) {
                                const img = imgs[FACES[i]];
                                this.flipper.drawImage(img, 0, 0, img.width, img.height, 0, 0, FACE_SIZE, FACE_SIZE);
                            }
                            gl.texSubImage2D(gl.TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, 0, 0, gl.RGBA, gl.UNSIGNED_BYTE, this.flipped);
                        }
                        gl.bindTexture(gl.TEXTURE_CUBE_MAP, null);
                    }
                    else {
                        this.rtCamera.update(this.env.renderer, this.rtScene);
                    }
                }
                this.stageHeading = this.env.avatar.headingRadians;
                this.imageNeedsUpdate = false;
                this.rotationNeedsUpdate = false;
                this.wasVisible = this.visible;
            }
            this.wasWebXRLayerAvailable = isWebXRLayerAvailable;
        }
    }
}
//# sourceMappingURL=Skybox.js.map