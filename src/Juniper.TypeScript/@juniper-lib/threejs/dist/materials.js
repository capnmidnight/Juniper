import { dispose } from "@juniper-lib/dom/dist/canvas";
import { singleton } from "@juniper-lib/tslib/dist/singleton";
import { isNullOrUndefined } from "@juniper-lib/tslib/dist/typeChecks";
import { Color, LineBasicMaterial, MeshBasicMaterial, MeshPhongMaterial, SpriteMaterial } from "three";
import { LineMaterial } from "./examples/lines/LineMaterial";
import { isMaterial, isMesh } from "./typeChecks";
const materials = singleton("Juniper:Three:Materials", () => new Map());
function del(obj, name) {
    if (name in obj) {
        delete obj[name];
    }
}
function makeMaterial(slug, material, options) {
    const key = `${slug}_${Object
        .keys(options)
        .map((k) => `${k}:${options[k]}`)
        .join(",")}`;
    if (!materials.has(key)) {
        del(options, "name");
        materials.set(key, new material(options));
    }
    return materials.get(key);
}
function trans(options) {
    return Object.assign(options, {
        transparent: true
    });
}
export function solid(options) {
    return makeMaterial("solid", MeshBasicMaterial, options);
}
export function solidTransparent(options) {
    return makeMaterial("solidTransparent", MeshBasicMaterial, trans(options));
}
export function lit(options) {
    return makeMaterial("lit", MeshPhongMaterial, options);
}
export function litTransparent(options) {
    return makeMaterial("litTransparent", MeshPhongMaterial, trans(options));
}
export function line(options) {
    return makeMaterial("line", LineBasicMaterial, options);
}
export function lineTransparent(options) {
    return makeMaterial("lineTransparent", LineBasicMaterial, trans(options));
}
export function line2(options) {
    return makeMaterial("line2", LineMaterial, options);
}
export function sprite(options) {
    return makeMaterial("sprite", SpriteMaterial, options);
}
export function spriteTransparent(options) {
    return makeMaterial("spriteTransparent", SpriteMaterial, trans(options));
}
export function convertMaterials(root, convertMaterial) {
    const oldMats = new Set();
    root.traverse(obj => {
        if (isMesh(obj) && isMaterial(obj.material)) {
            const oldMat = obj.material;
            const newMat = convertMaterial(oldMat);
            if (oldMat !== newMat) {
                oldMats.add(oldMat);
                obj.material = newMat;
            }
        }
    });
    for (const oldMat of oldMats) {
        dispose(oldMat);
    }
}
export function materialStandardToBasic(oldMat) {
    if (oldMat.type !== "MeshStandardMaterial") {
        throw new Error("Input material is not MeshStandardMaterial");
    }
    const params = {
        alphaMap: oldMat.alphaMap,
        alphaTest: oldMat.alphaTest,
        alphaToCoverage: oldMat.alphaToCoverage,
        aoMap: oldMat.aoMap,
        aoMapIntensity: oldMat.aoMapIntensity,
        blendDst: oldMat.blendDst,
        blendDstAlpha: oldMat.blendDstAlpha,
        blendEquation: oldMat.blendEquation,
        blendEquationAlpha: oldMat.blendEquationAlpha,
        blending: oldMat.blending,
        blendSrc: oldMat.blendSrc,
        blendSrcAlpha: oldMat.blendSrcAlpha,
        clipIntersection: oldMat.clipIntersection,
        clippingPlanes: oldMat.clippingPlanes,
        clipShadows: oldMat.clipShadows,
        color: oldMat.color,
        colorWrite: oldMat.colorWrite,
        depthFunc: oldMat.depthFunc,
        depthTest: oldMat.depthTest,
        depthWrite: oldMat.depthWrite,
        dithering: oldMat.dithering,
        envMap: oldMat.envMap,
        fog: oldMat.fog,
        lightMap: oldMat.lightMap,
        lightMapIntensity: oldMat.lightMapIntensity,
        map: oldMat.emissiveMap || oldMat.map,
        name: oldMat.name + "-Standard-To-Basic",
        opacity: oldMat.opacity,
        polygonOffset: oldMat.polygonOffset,
        polygonOffsetFactor: oldMat.polygonOffsetFactor,
        polygonOffsetUnits: oldMat.polygonOffsetUnits,
        precision: oldMat.precision,
        premultipliedAlpha: oldMat.premultipliedAlpha,
        shadowSide: oldMat.shadowSide,
        side: oldMat.side,
        stencilFail: oldMat.stencilFail,
        stencilFunc: oldMat.stencilFunc,
        stencilFuncMask: oldMat.stencilFuncMask,
        stencilRef: oldMat.stencilRef,
        stencilWrite: oldMat.stencilWrite,
        stencilWriteMask: oldMat.stencilWriteMask,
        stencilZFail: oldMat.stencilZFail,
        stencilZPass: oldMat.stencilZPass,
        toneMapped: oldMat.toneMapped,
        transparent: oldMat.transparent,
        userData: oldMat.userData,
        vertexColors: oldMat.vertexColors,
        visible: oldMat.visible,
        wireframe: oldMat.wireframe,
        wireframeLinecap: oldMat.wireframeLinecap,
        wireframeLinejoin: oldMat.wireframeLinejoin,
        wireframeLinewidth: oldMat.wireframeLinewidth
    };
    for (const [key, value] of Object.entries(params)) {
        if (isNullOrUndefined(value)) {
            delete params[key];
        }
    }
    return new MeshBasicMaterial(params);
}
export function materialStandardToPhong(oldMat) {
    if (oldMat.type !== "MeshStandardMaterial") {
        throw new Error("Input material is not MeshStandardMaterial");
    }
    const params = {
        alphaMap: oldMat.alphaMap,
        alphaTest: oldMat.alphaTest,
        alphaToCoverage: oldMat.alphaToCoverage,
        aoMap: oldMat.aoMap,
        aoMapIntensity: oldMat.aoMapIntensity,
        blendDst: oldMat.blendDst,
        blendDstAlpha: oldMat.blendDstAlpha,
        blendEquation: oldMat.blendEquation,
        blendEquationAlpha: oldMat.blendEquationAlpha,
        blending: oldMat.blending,
        blendSrc: oldMat.blendSrc,
        blendSrcAlpha: oldMat.blendSrcAlpha,
        bumpMap: oldMat.bumpMap,
        bumpScale: oldMat.bumpScale,
        clipIntersection: oldMat.clipIntersection,
        clippingPlanes: oldMat.clippingPlanes,
        clipShadows: oldMat.clipShadows,
        color: oldMat.color,
        colorWrite: oldMat.colorWrite,
        depthFunc: oldMat.depthFunc,
        depthTest: oldMat.depthTest,
        depthWrite: oldMat.depthWrite,
        displacementBias: oldMat.displacementBias,
        displacementMap: oldMat.displacementMap,
        displacementScale: oldMat.displacementScale,
        dithering: oldMat.dithering,
        emissive: oldMat.emissive,
        emissiveIntensity: oldMat.emissiveIntensity,
        emissiveMap: oldMat.emissiveMap,
        envMap: oldMat.envMap,
        flatShading: oldMat.flatShading,
        fog: oldMat.fog,
        lightMap: oldMat.lightMap,
        lightMapIntensity: oldMat.lightMapIntensity,
        map: oldMat.map,
        name: oldMat.name + "-Standard-To-Phong",
        normalMap: oldMat.normalMap,
        normalMapType: oldMat.normalMapType,
        normalScale: oldMat.normalScale,
        opacity: oldMat.opacity,
        polygonOffset: oldMat.polygonOffset,
        polygonOffsetFactor: oldMat.polygonOffsetFactor,
        polygonOffsetUnits: oldMat.polygonOffsetUnits,
        precision: oldMat.precision,
        premultipliedAlpha: oldMat.premultipliedAlpha,
        shadowSide: oldMat.shadowSide,
        side: oldMat.side,
        stencilFail: oldMat.stencilFail,
        stencilFunc: oldMat.stencilFunc,
        stencilFuncMask: oldMat.stencilFuncMask,
        stencilRef: oldMat.stencilRef,
        stencilWrite: oldMat.stencilWrite,
        stencilWriteMask: oldMat.stencilWriteMask,
        stencilZFail: oldMat.stencilZFail,
        stencilZPass: oldMat.stencilZPass,
        toneMapped: oldMat.toneMapped,
        transparent: oldMat.transparent,
        userData: oldMat.userData,
        vertexColors: oldMat.vertexColors,
        visible: oldMat.visible,
        wireframe: oldMat.wireframe,
        wireframeLinecap: oldMat.wireframeLinecap,
        wireframeLinejoin: oldMat.wireframeLinejoin,
        wireframeLinewidth: oldMat.wireframeLinewidth
    };
    for (const [key, value] of Object.entries(params)) {
        if (isNullOrUndefined(value)) {
            delete params[key];
        }
    }
    return new MeshPhongMaterial(params);
}
export function materialPhongToBasic(oldMat) {
    if (oldMat.type !== "MeshPhongMaterial") {
        throw new Error("Input material is not MeshPhongMaterial");
    }
    const params = {
        alphaMap: oldMat.alphaMap,
        alphaTest: oldMat.alphaTest,
        alphaToCoverage: oldMat.alphaToCoverage,
        aoMap: oldMat.aoMap,
        aoMapIntensity: oldMat.aoMapIntensity,
        blendDst: oldMat.blendDst,
        blendDstAlpha: oldMat.blendDstAlpha,
        blendEquation: oldMat.blendEquation,
        blendEquationAlpha: oldMat.blendEquationAlpha,
        blending: oldMat.blending,
        blendSrc: oldMat.blendSrc,
        blendSrcAlpha: oldMat.blendSrcAlpha,
        clipIntersection: oldMat.clipIntersection,
        clippingPlanes: oldMat.clippingPlanes,
        clipShadows: oldMat.clipShadows,
        color: oldMat.color,
        colorWrite: oldMat.colorWrite,
        depthFunc: oldMat.depthFunc,
        depthTest: oldMat.depthTest,
        depthWrite: oldMat.depthWrite,
        dithering: oldMat.dithering,
        envMap: oldMat.envMap,
        fog: oldMat.fog,
        lightMap: oldMat.lightMap,
        lightMapIntensity: oldMat.lightMapIntensity,
        map: oldMat.emissiveMap || oldMat.map,
        name: oldMat.name + "-Phong-To-Basic",
        opacity: oldMat.opacity,
        polygonOffset: oldMat.polygonOffset,
        polygonOffsetFactor: oldMat.polygonOffsetFactor,
        polygonOffsetUnits: oldMat.polygonOffsetUnits,
        precision: oldMat.precision,
        premultipliedAlpha: oldMat.premultipliedAlpha,
        reflectivity: oldMat.reflectivity,
        refractionRatio: oldMat.refractionRatio,
        shadowSide: oldMat.shadowSide,
        side: oldMat.side,
        specularMap: oldMat.specularMap,
        stencilFail: oldMat.stencilFail,
        stencilFunc: oldMat.stencilFunc,
        stencilFuncMask: oldMat.stencilFuncMask,
        stencilRef: oldMat.stencilRef,
        stencilWrite: oldMat.stencilWrite,
        stencilWriteMask: oldMat.stencilWriteMask,
        stencilZFail: oldMat.stencilZFail,
        stencilZPass: oldMat.stencilZPass,
        toneMapped: oldMat.toneMapped,
        transparent: oldMat.transparent,
        userData: oldMat.userData,
        vertexColors: oldMat.vertexColors,
        visible: oldMat.visible,
        wireframe: oldMat.wireframe,
        wireframeLinecap: oldMat.wireframeLinecap,
        wireframeLinejoin: oldMat.wireframeLinejoin,
        wireframeLinewidth: oldMat.wireframeLinewidth
    };
    for (const [key, value] of Object.entries(params)) {
        if (isNullOrUndefined(value)) {
            delete params[key];
        }
    }
    return new MeshBasicMaterial(params);
}
export function materialPhysicalToPhong(oldMat) {
    if (oldMat.type !== "MeshPhysicalMaterial") {
        throw new Error("Input material is not MeshPhysicalMaterial");
    }
    const params = {
        alphaMap: oldMat.alphaMap,
        alphaTest: oldMat.alphaTest,
        alphaToCoverage: oldMat.alphaToCoverage,
        aoMap: oldMat.aoMap,
        aoMapIntensity: oldMat.aoMapIntensity,
        blendDst: oldMat.blendDst,
        blendDstAlpha: oldMat.blendDstAlpha,
        blendEquation: oldMat.blendEquation,
        blendEquationAlpha: oldMat.blendEquationAlpha,
        blending: oldMat.blending,
        blendSrc: oldMat.blendSrc,
        blendSrcAlpha: oldMat.blendSrcAlpha,
        bumpMap: oldMat.bumpMap,
        bumpScale: oldMat.bumpScale,
        clipIntersection: oldMat.clipIntersection,
        clippingPlanes: oldMat.clippingPlanes,
        clipShadows: oldMat.clipShadows,
        color: oldMat.color,
        colorWrite: oldMat.colorWrite,
        depthFunc: oldMat.depthFunc,
        depthTest: oldMat.depthTest,
        depthWrite: oldMat.depthWrite,
        displacementBias: oldMat.displacementBias,
        displacementMap: oldMat.displacementMap,
        displacementScale: oldMat.displacementScale,
        dithering: oldMat.dithering,
        emissive: oldMat.emissive,
        emissiveIntensity: oldMat.emissiveIntensity,
        emissiveMap: oldMat.emissiveMap,
        envMap: oldMat.envMap,
        flatShading: oldMat.flatShading,
        fog: oldMat.fog,
        lightMap: oldMat.lightMap,
        lightMapIntensity: oldMat.lightMapIntensity,
        map: oldMat.map,
        name: oldMat.name + "-Standard-To-Phong",
        normalMap: oldMat.normalMap,
        normalMapType: oldMat.normalMapType,
        normalScale: oldMat.normalScale,
        opacity: oldMat.opacity,
        polygonOffset: oldMat.polygonOffset,
        polygonOffsetFactor: oldMat.polygonOffsetFactor,
        polygonOffsetUnits: oldMat.polygonOffsetUnits,
        precision: oldMat.precision,
        premultipliedAlpha: oldMat.premultipliedAlpha,
        reflectivity: oldMat.reflectivity,
        shadowSide: oldMat.shadowSide,
        shininess: oldMat.sheen,
        side: oldMat.side,
        specular: oldMat.specularColor,
        specularMap: oldMat.specularColorMap,
        stencilFail: oldMat.stencilFail,
        stencilFunc: oldMat.stencilFunc,
        stencilFuncMask: oldMat.stencilFuncMask,
        stencilRef: oldMat.stencilRef,
        stencilWrite: oldMat.stencilWrite,
        stencilWriteMask: oldMat.stencilWriteMask,
        stencilZFail: oldMat.stencilZFail,
        stencilZPass: oldMat.stencilZPass,
        toneMapped: oldMat.toneMapped,
        transparent: oldMat.transparent,
        userData: oldMat.userData,
        vertexColors: oldMat.vertexColors,
        visible: oldMat.visible,
        wireframe: oldMat.wireframe,
        wireframeLinecap: oldMat.wireframeLinecap,
        wireframeLinejoin: oldMat.wireframeLinejoin,
        wireframeLinewidth: oldMat.wireframeLinewidth
    };
    for (const [key, value] of Object.entries(params)) {
        if (isNullOrUndefined(value)) {
            delete params[key];
        }
    }
    return new MeshPhongMaterial(params);
}
export const black = /*@__PURE__*/ 0x000000;
export const blue = /*@__PURE__*/ 0x0000ff;
export const green = /*@__PURE__*/ 0x00ff00;
export const cyan = /*@__PURE__*/ 0x00ffff;
export const red = /*@__PURE__*/ 0xff0000;
export const magenta = /*@__PURE__*/ 0xff00ff;
export const yellow = /*@__PURE__*/ 0xffff00;
export const grey = /*@__PURE__*/ 0xc0c0c0;
export const white = /*@__PURE__*/ 0xffffff;
export const colorBlack = /*@__PURE__*/ new Color(black);
export const colorBlue = /*@__PURE__*/ new Color(blue);
export const colorGreen = /*@__PURE__*/ new Color(green);
export const colorCyan = /*@__PURE__*/ new Color(cyan);
export const colorRed = /*@__PURE__*/ new Color(red);
export const colorMagenta = /*@__PURE__*/ new Color(magenta);
export const colorYellow = /*@__PURE__*/ new Color(yellow);
export const colorGrey = /*@__PURE__*/ new Color(grey);
export const colorWhite = /*@__PURE__*/ new Color(white);
export const solidBlack = /*@__PURE__*/ solid({ color: black });
export const solidBlue = /*@__PURE__*/ solid({ color: blue });
export const solidGreen = /*@__PURE__*/ solid({ color: green });
export const solidCyan = /*@__PURE__*/ solid({ color: cyan });
export const solidRed = /*@__PURE__*/ solid({ color: red });
export const solidMagenta = /*@__PURE__*/ solid({ color: magenta });
export const solidYellow = /*@__PURE__*/ solid({ color: yellow });
export const solidGrey = /*@__PURE__*/ solid({ color: grey });
export const solidWhite = /*@__PURE__*/ solid({ color: white });
export function solidTransparentBlack(opacity) { return solidTransparent({ color: black, opacity }); }
export function solidTransparentBlue(opacity) { return solidTransparent({ color: blue, opacity }); }
export function solidTransparentGreen(opacity) { return solidTransparent({ color: green, opacity }); }
export function solidTransparentCyan(opacity) { return solidTransparent({ color: cyan, opacity }); }
export function solidTransparentRed(opacity) { return solidTransparent({ color: red, opacity }); }
export function solidTransparentMagenta(opacity) { return solidTransparent({ color: magenta, opacity }); }
export function solidTransparentYellow(opacity) { return solidTransparent({ color: yellow, opacity }); }
export function solidTransparentGrey(opacity) { return solidTransparent({ color: grey, opacity }); }
export function solidTransparentWhite(opacity) { return solidTransparent({ color: white, opacity }); }
export const litBlack = /*@__PURE__*/ lit({ color: black });
export const litBlue = /*@__PURE__*/ lit({ color: blue });
export const litGreen = /*@__PURE__*/ lit({ color: green });
export const litCyan = /*@__PURE__*/ lit({ color: cyan });
export const litRed = /*@__PURE__*/ lit({ color: red });
export const litMagenta = /*@__PURE__*/ lit({ color: magenta });
export const litYellow = /*@__PURE__*/ lit({ color: yellow });
export const litGrey = /*@__PURE__*/ lit({ color: grey });
export const litWhite = /*@__PURE__*/ lit({ color: white });
export function litTransparentBlack(opacity) { return litTransparent({ color: black, opacity }); }
export function litTransparentBlue(opacity) { return litTransparent({ color: blue, opacity }); }
export function litTransparentGreen(opacity) { return litTransparent({ color: green, opacity }); }
export function litTransparentCyan(opacity) { return litTransparent({ color: cyan, opacity }); }
export function litTransparentRed(opacity) { return litTransparent({ color: red, opacity }); }
export function litTransparentMagenta(opacity) { return litTransparent({ color: magenta, opacity }); }
export function litTransparentYellow(opacity) { return litTransparent({ color: yellow, opacity }); }
export function litTransparentGrey(opacity) { return litTransparent({ color: grey, opacity }); }
export function litTransparentWhite(opacity) { return litTransparent({ color: white, opacity }); }
export const lineBlack = /*@__PURE__*/ line({ color: black });
export const lineBlue = /*@__PURE__*/ line({ color: blue });
export const lineGreen = /*@__PURE__*/ line({ color: green });
export const lineCyan = /*@__PURE__*/ line({ color: cyan });
export const lineRed = /*@__PURE__*/ line({ color: red });
export const lineMagenta = /*@__PURE__*/ line({ color: magenta });
export const lineYellow = /*@__PURE__*/ line({ color: yellow });
export const lineGrey = /*@__PURE__*/ line({ color: grey });
export const lineWhite = /*@__PURE__*/ line({ color: white });
export function lineTransparentBlack(opacity) { return lineTransparent({ color: black, opacity }); }
export function lineTransparentBlue(opacity) { return lineTransparent({ color: blue, opacity }); }
export function lineTransparentGreen(opacity) { return lineTransparent({ color: green, opacity }); }
export function lineTransparentCyan(opacity) { return lineTransparent({ color: cyan, opacity }); }
export function lineTransparentRed(opacity) { return lineTransparent({ color: red, opacity }); }
export function lineTransparentMagenta(opacity) { return lineTransparent({ color: magenta, opacity }); }
export function lineTransparentYellow(opacity) { return lineTransparent({ color: yellow, opacity }); }
export function lineTransparentGrey(opacity) { return lineTransparent({ color: grey, opacity }); }
export function lineTransparentWhite(opacity) { return lineTransparent({ color: white, opacity }); }
export const spriteBlack = /*@__PURE__*/ sprite({ color: black });
export const spriteBlue = /*@__PURE__*/ sprite({ color: blue });
export const spriteGreen = /*@__PURE__*/ sprite({ color: green });
export const spriteCyan = /*@__PURE__*/ sprite({ color: cyan });
export const spriteRed = /*@__PURE__*/ sprite({ color: red });
export const spriteMagenta = /*@__PURE__*/ sprite({ color: magenta });
export const spriteYellow = /*@__PURE__*/ sprite({ color: yellow });
export const spriteGrey = /*@__PURE__*/ sprite({ color: grey });
export const spriteWhite = /*@__PURE__*/ sprite({ color: white });
export function spriteTransparentBlack(opacity) { return spriteTransparent({ color: black, opacity }); }
export function spriteTransparentBlue(opacity) { return spriteTransparent({ color: blue, opacity }); }
export function spriteTransparentGreen(opacity) { return spriteTransparent({ color: green, opacity }); }
export function spriteTransparentCyan(opacity) { return spriteTransparent({ color: cyan, opacity }); }
export function spriteTransparentRed(opacity) { return spriteTransparent({ color: red, opacity }); }
export function spriteTransparentMagenta(opacity) { return spriteTransparent({ color: magenta, opacity }); }
export function spriteTransparentYellow(opacity) { return spriteTransparent({ color: yellow, opacity }); }
export function spriteTransparentGrey(opacity) { return spriteTransparent({ color: grey, opacity }); }
export function spriteTransparentWhite(opacity) { return spriteTransparent({ color: white, opacity }); }
//# sourceMappingURL=materials.js.map