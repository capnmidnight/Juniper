export function stringRepeat(str: string, count: number, sep = ""): string {
    if (count < 0) {
        throw new Error("Can't repeat negative times: " + count);
    }

    let sb = "";
    for (let i = 0; i < count; ++i) {
        sb += str;
        if (i < count - 1) {
            sb += sep;
        }
    }

    return sb;
}
