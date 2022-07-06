import { isNullOrUndefined, singleton } from "@juniper-lib/tslib";
import { LineMaterial, LineMaterialParameters } from "./examples/lines/LineMaterial";

const materials = singleton("Juniper:Three:Materials", () => new Map<string, THREE.Material>());

function del(obj: any, name: string) {
    if (name in obj) {
        delete obj[name];
    }
}

export type SolidMaterial = THREE.MeshBasicMaterial | THREE.MeshPhongMaterial;
export type SolidMaterialOpts = THREE.MeshBasicMaterialParameters | THREE.MeshPhongMaterialParameters;

function makeMaterial<MaterialT extends THREE.Material, OptionsT extends THREE.MaterialParameters>(slug: string, material: (new (opts: OptionsT) => MaterialT), options: OptionsT): MaterialT {

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

export function solid(options: THREE.MeshBasicMaterialParameters): THREE.MeshBasicMaterial {
    return makeMaterial("solid", THREE.MeshBasicMaterial, options);
}

export function solidTransparent(options: THREE.MeshBasicMaterialParameters): THREE.MeshBasicMaterial {
    return makeMaterial("solidTransparent", THREE.MeshBasicMaterial, trans(options));
}

export function lit(options: THREE.MeshPhongMaterialParameters): THREE.MeshPhongMaterial {
    return makeMaterial("lit", THREE.MeshPhongMaterial, options);
}

export function litTransparent(options: THREE.MeshPhongMaterialParameters): THREE.MeshPhongMaterial {
    return makeMaterial("litTransparent", THREE.MeshPhongMaterial, trans(options));
}

export function line(options: THREE.LineBasicMaterialParameters): THREE.LineBasicMaterial {
    return makeMaterial("line", THREE.LineBasicMaterial, options);
}

export function lineTransparent(options: THREE.LineBasicMaterialParameters): THREE.LineBasicMaterial {
    return makeMaterial("lineTransparent", THREE.LineBasicMaterial, trans(options));
}

export function line2(options: LineMaterialParameters) {
    return makeMaterial("line2", LineMaterial, options);
}

export function sprite(options: THREE.SpriteMaterialParameters): THREE.SpriteMaterial {
    return makeMaterial("sprite", THREE.SpriteMaterial, options);
}

export function spriteTransparent(options: THREE.SpriteMaterialParameters): THREE.SpriteMaterial {
    return makeMaterial("spriteTransparent", THREE.SpriteMaterial, trans(options));
}

export function materialStandardToBasic(oldMat: THREE.MeshStandardMaterial, transparent?: boolean): THREE.MeshBasicMaterial {

    const params: THREE.MeshBasicMaterialParameters = {
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
        map: oldMat.map,
        name: oldMat.name + "-Basic",
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
        transparent: isNullOrUndefined(transparent) ? oldMat.transparent : transparent,
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
    return new THREE.MeshBasicMaterial(params);
}

export function materialStandardToPhong(oldMat: THREE.MeshStandardMaterial, transparent?: boolean): THREE.MeshPhongMaterial {

    const params: THREE.MeshPhongMaterialParameters = {
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
        name: oldMat.name + "-Basic",
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
        transparent: isNullOrUndefined(transparent) ? oldMat.transparent : transparent,
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
    return new THREE.MeshPhongMaterial(params);
}


export function materialPhongToBasic(oldMat: THREE.MeshPhongMaterial, transparent?: boolean): THREE.MeshBasicMaterial {

    const params: THREE.MeshBasicMaterialParameters = {
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
        map: oldMat.map,
        name: oldMat.name + "-Basic",
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
        transparent: isNullOrUndefined(transparent) ? oldMat.transparent : transparent,
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
    return new THREE.MeshBasicMaterial(params);
}

export type ColorOpts = (THREE.Color | number | string);

export const black = /*@__PURE__*/ 0x000000;
export const blue = /*@__PURE__*/ 0x0000ff;
export const green = /*@__PURE__*/ 0x00ff00;
export const cyan = /*@__PURE__*/ 0x00ffff;
export const red = /*@__PURE__*/ 0xff0000;
export const magenta = /*@__PURE__*/ 0xff00ff;
export const yellow = /*@__PURE__*/ 0xffff00;
export const grey = /*@__PURE__*/ 0xc0c0c0;
export const white = /*@__PURE__*/ 0xffffff;

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