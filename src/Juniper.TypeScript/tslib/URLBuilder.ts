import { isDefined } from "./typeChecks";

type ProtocolType = `${string}:`;

function parsePort(portString: string): number {
    if (isDefined(portString) && portString.length > 0) {
        return parseFloat(portString);
    }
    return null;
}

export class URLBuilder {
    private _url: URL = null;
    private _base: string | URL = undefined;
    private _protocol: string = null
    private _host: string = null;
    private _hostName: string = null;
    private _userName: string = null;
    private _password: string = null;
    private _port: number = null;
    private _pathName: string = null;
    private _hash: string = null;
    private readonly _query = new Map<string, string>();

    constructor(url?: string | URL, base?: string | URL) {
        if (url !== undefined) {
            this._url = new URL(url, base);
            this.rehydrate();
        }
    }

    private rehydrate(): void {
        if (isDefined(this._protocol) && this._protocol !== this._url.protocol) {
            this._url.protocol = this._protocol;
        }

        if (isDefined(this._host) && this._host !== this._url.host) {
            this._url.host = this._host;
        }

        if (isDefined(this._hostName) && this._hostName !== this._url.hostname) {
            this._url.hostname = this._hostName;
        }

        if (isDefined(this._userName) && this._userName !== this._url.username) {
            this._url.username = this._userName;
        }

        if (isDefined(this._password) && this._password !== this._url.password) {
            this._url.password = this._password;
        }

        if (isDefined(this._port) && this._port.toFixed(0) !== this._url.port) {
            this._url.port = this._port.toFixed(0);
        }

        if (isDefined(this._pathName) && this._pathName !== this._url.pathname) {
            this._url.pathname = this._pathName;
        }

        if (isDefined(this._hash) && this._hash !== this._url.hash) {
            this._url.hash = this._hash;
        }

        for (const [k, v] of this._query) {
            this._url.searchParams.set(k, v);
        }

        this._protocol = this._url.protocol;
        this._host = this._url.host;
        this._hostName = this._url.hostname;
        this._userName = this._url.username;
        this._password = this._url.password;
        this._port = parsePort(this._url.port);
        this._pathName = this._url.pathname;
        this._hash = this._url.hash;
        this._url.searchParams.forEach((v, k) =>
            this._query.set(k, v));
    }

    private refresh(): boolean {
        if (this._url === null) {
            if (isDefined(this._protocol)
                && (isDefined(this._host) || isDefined(this._hostName))) {
                if (isDefined(this._host)) {
                    this._url = new URL(`${this._protocol}//${this._host}`, this._base);
                    this._port = parsePort(this._url.port);
                    this.rehydrate();
                    return false;
                }
                else if (isDefined(this._hostName)) {
                    this._url = new URL(`${this._protocol}//${this._hostName}`, this._base);
                    this.rehydrate();
                    return false;
                }
            }
            else if (isDefined(this._pathName) && isDefined(this._base)) {
                this._url = new URL(this._pathName, this._base);
                this.rehydrate();
                return false;
            }
        }

        return isDefined(this._url);
    }

    base(base: string | URL) {
        if (this._url !== null) {
            throw new Error("Cannot redefine base after defining the protocol and domain")
        }

        this._base = base;
        this.refresh();
        return this;
    }

    protocol(protocol: ProtocolType) {
        this._protocol = protocol;
        if (this.refresh()) {
            this._url.protocol = protocol;
        }
        return this;
    }

    host(host: string) {
        this._host = host;
        if (this.refresh()) {
            this._url.host = host;
            this._hostName = this._url.hostname;
            this._port = parsePort(this._url.port);
        }
        return this;
    }

    hostName(hostName: string) {
        this._hostName = hostName;
        if (this.refresh()) {
            this._url.hostname = hostName;
            this._host = `${this._url.hostname}:${this._url.port}`;
        }
        return this;
    }

    port(port: number) {
        this._port = port;
        if (this.refresh()) {
            this._url.port = port.toFixed(0);
            this._host = `${this._url.hostname}:${this._url.port}`;
        }
        return this;
    }

    userName(userName: string) {
        this._userName = userName;
        if (this.refresh()) {
            this._url.username = userName;
        }
        return this;
    }

    password(password: string) {
        this._password = password;
        if (this.refresh()) {
            this._url.password = password;
        }
        return this;
    }

    path(path: string) {
        this._pathName = path;
        if (this.refresh()) {
            this._url.pathname = path;
        }
        return this;
    }

    pathPop(pattern?: RegExp) {
        pattern = pattern || /\/[^\/]+\/?$/;
        return this.path(this._pathName.replace(pattern, ""));
    }

    pathPush(part: string) {
        let path = this._pathName;
        if (!path.endsWith('/')) {
            path += '/';
        }

        path += part;

        return this.path(path);
    }

    query(name: string, value: string) {
        this._query.set(name, value);
        if (this.refresh()) {
            this._url.searchParams.set(name, value);
        }
        return this;
    }

    hash(hash: string) {
        this._hash = hash;
        if (this.refresh()) {
            this._url.hash = hash;
        }
        return this;
    }

    toURL() {
        return this._url;
    }

    toString() {
        return this._url.href;
    }

    [Symbol.toStringTag]() {
        return this.toString();
    }
}