import { singleton } from "@juniper-lib/tslib/singleton";
import { isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
import { Color, LineBasicMaterial, LineBasicMaterialParameters, Material, MaterialParameters, MeshBasicMaterial, MeshBasicMaterialParameters, MeshPhongMaterial, MeshPhongMaterialParameters, MeshStandardMaterial, Object3D, SpriteMaterial, SpriteMaterialParameters } from "three";
import { LineMaterial, LineMaterialParameters } from "./examples/lines/LineMaterial";
import { isMaterial, isMesh } from "./typeChecks";

const materials = singleton("Juniper:Three:Materials", () => new Map<string, Material>());

function del(obj: any, name: string) {
    if (name in obj) {
        delete obj[name];
    }
}

export type SolidMaterial = MeshBasicMaterial | MeshPhongMaterial;
export type SolidMaterialOpts = MeshBasicMaterialParameters | MeshPhongMaterialParameters;

function makeMaterial<MaterialT extends Material, OptionsT extends MaterialParameters>(slug: string, material: (new (opts: OptionsT) => MaterialT), options: OptionsT): MaterialT {

    const key = `${slug}_${Object
        .keys(options)
        .map((k) => `${k}:${(options as any)[k]}`)
        .join(",")}`;

    if (!materials.has(key)) {
        del(options, "name");
        materials.set(key, new material(options));
    }

    return materials.get(key) as MaterialT;
}

function trans<T>(options: T): T {
    return Object.assign(options, {
        transparent: true
    });
}

export function solid(options: MeshBasicMaterialParameters): MeshBasicMaterial {
    return makeMaterial("solid", MeshBasicMaterial, options);
}

export function solidTransparent(options: MeshBasicMaterialParameters): MeshBasicMaterial {
    return makeMaterial("solidTransparent", MeshBasicMaterial, trans(options));
}

export function lit(options: MeshPhongMaterialParameters): MeshPhongMaterial {
    return makeMaterial("lit", MeshPhongMaterial, options);
}

export function litTransparent(options: MeshPhongMaterialParameters): MeshPhongMaterial {
    return makeMaterial("litTransparent", MeshPhongMaterial, trans(options));
}

export function line(options: LineBasicMaterialParameters): LineBasicMaterial {
    return makeMaterial("line", LineBasicMaterial, options);
}

export function lineTransparent(options: LineBasicMaterialParameters): LineBasicMaterial {
    return makeMaterial("lineTransparent", LineBasicMaterial, trans(options));
}

export function line2(options: LineMaterialParameters) {
    return makeMaterial("line2", LineMaterial, options);
}

export function sprite(options: SpriteMaterialParameters): SpriteMaterial {
    return makeMaterial("sprite", SpriteMaterial, options);
}

export function spriteTransparent(options: SpriteMaterialParameters): SpriteMaterial {
    return makeMaterial("spriteTransparent", SpriteMaterial, trans(options));
}

export type MaterialConverter<OldMatT extends Material, NewMatT extends Material> = (oldMat: OldMatT) => NewMatT;
export function convertMaterials<OldMatT extends Material, NewMatT extends Material>(root: Object3D, convertMaterial: MaterialConverter<OldMatT, NewMatT>): void {
    const oldMats = new Set<Material>();
    root.traverse(obj => {
        if (isMesh(obj) && isMaterial(obj.material)) {
            const oldMat = obj.material;
            const newMat = convertMaterial(oldMat as any);
            if (oldMat !== newMat) {
                oldMats.add(oldMat);
                obj.material = newMat;
            }
        }
    });

    for (const oldMat of oldMats) {
        oldMat.dispose();
    }
}

export function materialStandardToBasic(oldMat: MeshStandardMaterial): MeshBasicMaterial {

    if (oldMat.type !== "MeshStandardMaterial") {
        throw new Error("Input material is not MeshStandardMaterial");
    }

    const params: MeshBasicMaterialParameters = {
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
            delete (params as any)[key];
        }
    }
    return new MeshBasicMaterial(params);
}

export function materialStandardToPhong(oldMat: MeshStandardMaterial): MeshPhongMaterial {

    if (oldMat.type !== "MeshStandardMaterial") {
        throw new Error("Input material is not MeshStandardMaterial");
    }

    const params: MeshPhongMaterialParameters = {
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
            delete (params as any)[key];
        }
    }
    return new MeshPhongMaterial(params);
}


export function materialPhongToBasic(oldMat: MeshPhongMaterial): MeshBasicMaterial {

    if (oldMat.type !== "MeshPhongMaterial") {
        throw new Error("Input material is not MeshPhongMaterial");
    }

    const params: MeshBasicMaterialParameters = {
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
            delete (params as any)[key];
        }
    }
    return new MeshBasicMaterial(params);
}

export type ColorOpts = (Color | number | string);

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
export function solidTransparentBlack(opacity: number) { return solidTransparent({ color: black, opacity }); }
export function solidTransparentBlue(opacity: number) { return solidTransparent({ color: blue, opacity }); }
export function solidTransparentGreen(opacity: number) { return solidTransparent({ color: green, opacity }); }
export function solidTransparentCyan(opacity: number) { return solidTransparent({ color: cyan, opacity }); }
export function solidTransparentRed(opacity: number) { return solidTransparent({ color: red, opacity }); }
export function solidTransparentMagenta(opacity: number) { return solidTransparent({ color: magenta, opacity }); }
export function solidTransparentYellow(opacity: number) { return solidTransparent({ color: yellow, opacity }); }
export function solidTransparentGrey(opacity: number) { return solidTransparent({ color: grey, opacity }); }
export function solidTransparentWhite(opacity: number) { return solidTransparent({ color: white, opacity }); }

export const litBlack = /*@__PURE__*/ lit({ color: black });
export const litBlue = /*@__PURE__*/ lit({ color: blue });
export const litGreen = /*@__PURE__*/ lit({ color: green });
export const litCyan = /*@__PURE__*/ lit({ color: cyan });
export const litRed = /*@__PURE__*/ lit({ color: red });
export const litMagenta = /*@__PURE__*/ lit({ color: magenta });
export const litYellow = /*@__PURE__*/ lit({ color: yellow });
export const litGrey = /*@__PURE__*/ lit({ color: grey });
export const litWhite = /*@__PURE__*/ lit({ color: white });
export function litTransparentBlack(opacity: number) { return litTransparent({ color: black, opacity }); }
export function litTransparentBlue(opacity: number) { return litTransparent({ color: blue, opacity }); }
export function litTransparentGreen(opacity: number) { return litTransparent({ color: green, opacity }); }
export function litTransparentCyan(opacity: number) { return litTransparent({ color: cyan, opacity }); }
export function litTransparentRed(opacity: number) { return litTransparent({ color: red, opacity }); }
export function litTransparentMagenta(opacity: number) { return litTransparent({ color: magenta, opacity }); }
export function litTransparentYellow(opacity: number) { return litTransparent({ color: yellow, opacity }); }
export function litTransparentGrey(opacity: number) { return litTransparent({ color: grey, opacity }); }
export function litTransparentWhite(opacity: number) { return litTransparent({ color: white, opacity }); }

export const lineBlack = /*@__PURE__*/ line({ color: black });
export const lineBlue = /*@__PURE__*/ line({ color: blue });
export const lineGreen = /*@__PURE__*/ line({ color: green });
export const lineCyan = /*@__PURE__*/ line({ color: cyan });
export const lineRed = /*@__PURE__*/ line({ color: red });
export const lineMagenta = /*@__PURE__*/ line({ color: magenta });
export const lineYellow = /*@__PURE__*/ line({ color: yellow });
export const lineGrey = /*@__PURE__*/ line({ color: grey });
export const lineWhite = /*@__PURE__*/ line({ color: white });
export function lineTransparentBlack(opacity: number) { return lineTransparent({ color: black, opacity }); }
export function lineTransparentBlue(opacity: number) { return lineTransparent({ color: blue, opacity }); }
export function lineTransparentGreen(opacity: number) { return lineTransparent({ color: green, opacity }); }
export function lineTransparentCyan(opacity: number) { return lineTransparent({ color: cyan, opacity }); }
export function lineTransparentRed(opacity: number) { return lineTransparent({ color: red, opacity }); }
export function lineTransparentMagenta(opacity: number) { return lineTransparent({ color: magenta, opacity }); }
export function lineTransparentYellow(opacity: number) { return lineTransparent({ color: yellow, opacity }); }
export function lineTransparentGrey(opacity: number) { return lineTransparent({ color: grey, opacity }); }
export function lineTransparentWhite(opacity: number) { return lineTransparent({ color: white, opacity }); }

export const spriteBlack = /*@__PURE__*/ sprite({ color: black });
export const spriteBlue = /*@__PURE__*/ sprite({ color: blue });
export const spriteGreen = /*@__PURE__*/ sprite({ color: green });
export const spriteCyan = /*@__PURE__*/ sprite({ color: cyan });
export const spriteRed = /*@__PURE__*/ sprite({ color: red });
export const spriteMagenta = /*@__PURE__*/ sprite({ color: magenta });
export const spriteYellow = /*@__PURE__*/ sprite({ color: yellow });
export const spriteGrey = /*@__PURE__*/ sprite({ color: grey });
export const spriteWhite = /*@__PURE__*/ sprite({ color: white });
export function spriteTransparentBlack(opacity: number) { return spriteTransparent({ color: black, opacity }); }
export function spriteTransparentBlue(opacity: number) { return spriteTransparent({ color: blue, opacity }); }
export function spriteTransparentGreen(opacity: number) { return spriteTransparent({ color: green, opacity }); }
export function spriteTransparentCyan(opacity: number) { return spriteTransparent({ color: cyan, opacity }); }
export function spriteTransparentRed(opacity: number) { return spriteTransparent({ color: red, opacity }); }
export function spriteTransparentMagenta(opacity: number) { return spriteTransparent({ color: magenta, opacity }); }
export function spriteTransparentYellow(opacity: number) { return spriteTransparent({ color: yellow, opacity }); }
export function spriteTransparentGrey(opacity: number) { return spriteTransparent({ color: grey, opacity }); }
export function spriteTransparentWhite(opacity: number) { return spriteTransparent({ color: white, opacity }); }