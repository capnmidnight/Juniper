import { LineMaterial, LineMaterialParameters } from "./examples/lines/LineMaterial";

const materials = new Map<string, THREE.Material>();

function del(obj: any, name: string) {
    if (name in obj) {
        delete obj[name];
    }
}

export type SolidMaterial = THREE.MeshBasicMaterial | THREE.MeshStandardMaterial;
export type SolidMaterialOpts = THREE.MeshBasicMaterialParameters | THREE.MeshStandardMaterialParameters;

function makeMaterial<MaterialT extends THREE.Material, OptionsT extends THREE.MaterialParameters>(slug: string, material: (new (opts: OptionsT) => MaterialT), options: OptionsT): MaterialT {

    const key = `${slug}_${Object
        .keys(options)
        .map(k => `${k}:${(options as any)[k]}`)
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

export function lit(options: THREE.MeshStandardMaterialParameters): THREE.MeshStandardMaterial {
    return makeMaterial("lit", THREE.MeshStandardMaterial, options);
}

export function litTransparent(options: THREE.MeshStandardMaterialParameters): THREE.MeshStandardMaterial {
    return makeMaterial("litTransparent", THREE.MeshStandardMaterial, trans(options));
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

export type ColorOpts = (THREE.Color | number | string);

export const black = 0x000000;
export const blue = 0x0000ff;
export const green = 0x00ff00;
export const cyan = 0x00ffff;
export const red = 0xff0000;
export const magenta = 0xff00ff;
export const yellow = 0xffff00;
export const grey = 0xc0c0c0;
export const white = 0xffffff;

export const solidBlack = solid({ color: black });
export const solidBlue = solid({ color: blue });
export const solidGreen = solid({ color: green });
export const solidCyan = solid({ color: cyan });
export const solidRed = solid({ color: red });
export const solidMagenta = solid({ color: magenta });
export const solidYellow = solid({ color: yellow });
export const solidGrey = solid({ color: grey });
export const solidWhite = solid({ color: white });
export function solidTransparentBlack(opacity: number) { return solidTransparent({ color: black, opacity }); }
export function solidTransparentBlue(opacity: number) { return solidTransparent({ color: blue, opacity }); }
export function solidTransparentGreen(opacity: number) { return solidTransparent({ color: green, opacity }); }
export function solidTransparentCyan(opacity: number) { return solidTransparent({ color: cyan, opacity }); }
export function solidTransparentRed(opacity: number) { return solidTransparent({ color: red, opacity }); }
export function solidTransparentMagenta(opacity: number) { return solidTransparent({ color: magenta, opacity }); }
export function solidTransparentYellow(opacity: number) { return solidTransparent({ color: yellow, opacity }); }
export function solidTransparentGrey(opacity: number) { return solidTransparent({ color: grey, opacity }); }
export function solidTransparentWhite(opacity: number) { return solidTransparent({ color: white, opacity }); }

export const litBlack = lit({ color: black });
export const litBlue = lit({ color: blue });
export const litGreen = lit({ color: green });
export const litCyan = lit({ color: cyan });
export const litRed = lit({ color: red });
export const litMagenta = lit({ color: magenta });
export const litYellow = lit({ color: yellow });
export const litGrey = lit({ color: grey });
export const litWhite = lit({ color: white });
export function litTransparentBlack(opacity: number) { return litTransparent({ color: black, opacity }); }
export function litTransparentBlue(opacity: number) { return litTransparent({ color: blue, opacity }); }
export function litTransparentGreen(opacity: number) { return litTransparent({ color: green, opacity }); }
export function litTransparentCyan(opacity: number) { return litTransparent({ color: cyan, opacity }); }
export function litTransparentRed(opacity: number) { return litTransparent({ color: red, opacity }); }
export function litTransparentMagenta(opacity: number) { return litTransparent({ color: magenta, opacity }); }
export function litTransparentYellow(opacity: number) { return litTransparent({ color: yellow, opacity }); }
export function litTransparentGrey(opacity: number) { return litTransparent({ color: grey, opacity }); }
export function litTransparentWhite(opacity: number) { return litTransparent({ color: white, opacity }); }

export const lineBlack = line({ color: black });
export const lineBlue = line({ color: blue });
export const lineGreen = line({ color: green });
export const lineCyan = line({ color: cyan });
export const lineRed = line({ color: red });
export const lineMagenta = line({ color: magenta });
export const lineYellow = line({ color: yellow });
export const lineGrey = line({ color: grey });
export const lineWhite = line({ color: white });
export function lineTransparentBlack(opacity: number) { return lineTransparent({ color: black, opacity }); }
export function lineTransparentBlue(opacity: number) { return lineTransparent({ color: blue, opacity }); }
export function lineTransparentGreen(opacity: number) { return lineTransparent({ color: green, opacity }); }
export function lineTransparentCyan(opacity: number) { return lineTransparent({ color: cyan, opacity }); }
export function lineTransparentRed(opacity: number) { return lineTransparent({ color: red, opacity }); }
export function lineTransparentMagenta(opacity: number) { return lineTransparent({ color: magenta, opacity }); }
export function lineTransparentYellow(opacity: number) { return lineTransparent({ color: yellow, opacity }); }
export function lineTransparentGrey(opacity: number) { return lineTransparent({ color: grey, opacity }); }
export function lineTransparentWhite(opacity: number) { return lineTransparent({ color: white, opacity }); }

export const spriteBlack = sprite({ color: black });
export const spriteBlue = sprite({ color: blue });
export const spriteGreen = sprite({ color: green });
export const spriteCyan = sprite({ color: cyan });
export const spriteRed = sprite({ color: red });
export const spriteMagenta = sprite({ color: magenta });
export const spriteYellow = sprite({ color: yellow });
export const spriteGrey = sprite({ color: grey });
export const spriteWhite = sprite({ color: white });
export function spriteTransparentBlack(opacity: number) { return spriteTransparent({ color: black, opacity }); }
export function spriteTransparentBlue(opacity: number) { return spriteTransparent({ color: blue, opacity }); }
export function spriteTransparentGreen(opacity: number) { return spriteTransparent({ color: green, opacity }); }
export function spriteTransparentCyan(opacity: number) { return spriteTransparent({ color: cyan, opacity }); }
export function spriteTransparentRed(opacity: number) { return spriteTransparent({ color: red, opacity }); }
export function spriteTransparentMagenta(opacity: number) { return spriteTransparent({ color: magenta, opacity }); }
export function spriteTransparentYellow(opacity: number) { return spriteTransparent({ color: yellow, opacity }); }
export function spriteTransparentGrey(opacity: number) { return spriteTransparent({ color: grey, opacity }); }
export function spriteTransparentWhite(opacity: number) { return spriteTransparent({ color: white, opacity }); }