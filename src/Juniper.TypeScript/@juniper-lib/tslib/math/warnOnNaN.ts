import { isGoodNumber, isNumber } from "../typeChecks";

interface Vec2 { x: number; y: number; }
interface Vec3 extends Vec2 { z: number }
interface Vec4 extends Vec3 { w: number }
type Vec = Vec2 | Vec3 | Vec4 | ArrayLike<number>;

export function warnOnNaN(val: number | Vec, msg?: string): void {
    let type: string = null;
    let isBad = false;

    if (isNumber(val)) {
        type = "Value is"
        isBad = !isGoodNumber(val);
    }
    else if ("length" in val) {
        type = "Array contains";
        for (let i = 0; i < val.length; ++i) {
            if (!isGoodNumber(val[i])) {
                isBad = true;
                break;
            }
        }
    }
    else {
        type = "Vector component";
        if ("w" in val) {
            isBad = isBad || !isGoodNumber(val.w);
        }

        if ("z" in val) {
            isBad = isBad || !isGoodNumber(val.z);
        }

        isBad = isBad || !isGoodNumber(val.y);
        isBad = isBad || !isGoodNumber(val.x);
    }


    if (isBad) {
        if (msg) {
            msg = `[${msg}] `
        }
        else {
            msg = "";
        }

        console.warn(`${msg}${type} not-a-number`);
    }
}