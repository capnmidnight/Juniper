import { Diff } from "./arrays";
import { isDefined, isNumber } from "./typeChecks";

/**
 * Checks to see if two objects contain the same elements
 * @returns null if the objects match, a diff object, otherwise.
 */

export function objectCompare(a: object, b: object): Diff {
    const inA = [];
    const inB = [];
    const notEquals = [];
    const fields = new Set();
    for (const field in a) {
        fields.add(field);
    }
    for (const field in b) {
        fields.add(field);
    }
    for (const field in fields) {
        if (field in a && field in b) {
            const aValue = (a as any)[field];
            const bValue = (b as any)[field];
            if (aValue !== bValue) {
                notEquals.push(field);
            }
        }
        else if (field in a) {
            inA.push(field);
        }
        else if (field in b) {
            inB.push(field);
        }
    }

    if (inA.length === 0
        && inB.length === 0
        && notEquals.length === 0) {
        return null;
    }
    else {
        return new Diff(inA, inB, notEquals);
    }
}


/**
 * Determines if an object contains a function at the given field name.
 * These are *usually* methods.
 */
export function hasMethod(obj: object, name: string) {
    return obj
        && typeof obj === "object"
        && "name" in obj
        && typeof (obj as any)[name] === "function";
}

/**
 * Attempts to parse a string value to an object, but returns null instead of throwing an error on failure.
 */
export function tryParse<T>(json: string): T {
    try {
        return JSON.parse(json) as T;
    }
    catch {
        return null;
    }
}

/**
 * For use with enumerations: returns the maximum numeric value the enumeration defines.
 * @param x
 * @returns
 */
export function maxEnumValue(x: object) {
    return Math.max(
        ...Object.values(x)
            .filter(v => isNumber(v)) as number[]
    );
}

export function objectSelect(obj: any, fieldDef: string): unknown {
    if(fieldDef.indexOf(',') >= 0) {
        return fieldDef
            .split(',')
            .map(subFieldDef => 
                objectSelect(obj, subFieldDef));
    }
    else {
        const parts = fieldDef.split('.');
        let here = obj;
        while (parts.length > 0 && isDefined(here)) {
            const field = parts.shift();
            here = here[field];
        }

        if (parts.length > 0) {
            return null;
        }

        return here;
    }
}