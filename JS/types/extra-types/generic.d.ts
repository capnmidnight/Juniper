interface Constructor<T, C extends abstract new (...args: any) => T> {
    new(...params: ConstructorParameters<C>): T;
    prototype: T;
}