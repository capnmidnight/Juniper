export function logValue<T>(msg: string, v: T): T {
    console.log(msg, v);
    return v;
}
