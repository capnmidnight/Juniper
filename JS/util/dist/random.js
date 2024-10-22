/**
 * Generates a random number on a given range
 * @param min (inclusive) start of the range
 * @param max (inclusive) end of the range
 * @returns a number X that is min <= X <= max
 */
export function randomRange(min, max) {
    return (max - min) * Math.random() + min;
}
//# sourceMappingURL=random.js.map