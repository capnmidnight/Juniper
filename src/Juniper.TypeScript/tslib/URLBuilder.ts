export class URLBuilder {
    static get(url: string | URL, base?: string | URL) {
        return new URLBuilder(url, base);
    }

    private readonly _url: URL;
    constructor(url: string | URL, base?: string | URL) {
        this._url = new URL(url, base);
    }

    protocol(protocol: string) {
        this._url.protocol = protocol;
        return this;
    }

    userName(userName: string) {
        this._url.username = userName;
        return this;
    }

    password(password: string) {
        this._url.password = password;
        return this;
    }

    host(host: string) {
        this._url.host = host;
        return this;
    }

    hostName(hostName: string) {
        this._url.hostname = hostName;
    }

    port(port: number) {
        this._url.port = port.toString();
        return this;
    }

    path(pathName: string) {
        this._url.pathname = pathName;
        return this;
    }

    pathPop(pattern?: RegExp) {
        pattern = pattern || /\/[^\/]+\/?$/;
        this._url.pathname = this._url.pathname.replace(pattern, "");
        return this;
    }

    pathPush(part: string) {
        if (!this._url.pathname.endsWith('/')) {
            this._url.pathname += '/';
        }

        this._url.pathname += part;
        return this;
    }

    query(name: string, value: string) {
        this._url.searchParams.set(name, value);
        return this;
    }

    hash(hash: string) {
        this._url.hash = hash;
        return this;
    }

    get url() {
        return this._url;
    }

    toString() {
        return this._url.toString();
    }
}