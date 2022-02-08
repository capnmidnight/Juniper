import { specialize } from "./util";
const chemical = specialize("chemical");
export const anyChemical = chemical("*");
export const Chemical_X_Cdx = chemical("x-cdx", "cdx");
export const Chemical_X_Cif = chemical("x-cif", "cif");
export const Chemical_X_Cmdf = chemical("x-cmdf", "cmdf");
export const Chemical_X_Cml = chemical("x-cml", "cml");
export const Chemical_X_Csml = chemical("x-csml", "csml");
export const Chemical_X_Pdb = chemical("x-pdb");
export const Chemical_X_Xyz = chemical("x-xyz", "xyz");
export const allChemical = [
    Chemical_X_Cdx,
    Chemical_X_Cif,
    Chemical_X_Cmdf,
    Chemical_X_Cml,
    Chemical_X_Csml,
    Chemical_X_Pdb,
    Chemical_X_Xyz
];
