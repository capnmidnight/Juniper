export function all<T extends readonly unknown[] | []>(...tasks: T): Promise<{ -readonly [P in keyof T]: Awaited<T[P]> }> {
    return Promise.all(tasks);
}