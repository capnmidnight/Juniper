
export interface IPoseable {
    setPosition(px: number, py: number, pz: number): void;

    setOrientation(fx: number, fy: number, fz: number): void;
    setOrientation(fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;

    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number): void;
    set(px: number, py: number, pz: number, fx: number, fy: number, fz: number, ux: number, uy: number, uz: number): void;
}

