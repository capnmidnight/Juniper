/**
 * A class decorator for mixing multiple implementations.
 * @param constructors
 * @returns
 */
export function mixin(...constructors: any[]) {

    return function (baseCtor: any) {
        constructors.forEach((derivedCtor) => {
            Object.getOwnPropertyNames(baseCtor.prototype).forEach((name) => {
                Object.defineProperty(
                    derivedCtor.prototype,
                    name,
                    Object.getOwnPropertyDescriptor(baseCtor.prototype, name) ||
                    Object.create(null)
                );
            });
        });
    }
}