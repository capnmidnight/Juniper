type ProtocolType = `${string}:`;
export declare class URLBuilder {
    private _url;
    private _base;
    private _protocol;
    private _host;
    private _hostName;
    private _userName;
    private _password;
    private _port;
    private _pathName;
    private _hash;
    private readonly _query;
    constructor(url?: string | URL, base?: string | URL);
    private rehydrate;
    private refresh;
    base(base: string | URL): this;
    protocol(protocol: ProtocolType): this;
    host(host: string): this;
    hostName(hostName: string): this;
    port(port: number): this;
    userName(userName: string): this;
    password(password: string): this;
    path(path: string): this;
    pathPop(pattern?: RegExp): this;
    pathPush(part: string): this;
    query(name: string, value: string): this;
    hash(hash: string): this;
    toURL(): URL;
    toString(): string;
    [Symbol.toStringTag](): string;
}
export {};
//# sourceMappingURL=URLBuilder.d.ts.map