import { isDefined } from "./typeChecks";
function parsePort(portString) {
    if (isDefined(portString) && portString.length > 0) {
        return parseFloat(portString);
    }
    return null;
}
export class URLBuilder {
    constructor(url, base) {
        this._url = null;
        this._base = undefined;
        this._protocol = null;
        this._host = null;
        this._hostName = null;
        this._userName = null;
        this._password = null;
        this._port = null;
        this._pathName = null;
        this._hash = null;
        this._query = new Map();
        if (url !== undefined) {
            this._url = new URL(url, base);
            this.rehydrate();
        }
    }
    rehydrate() {
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
        this._url.searchParams.forEach((v, k) => this._query.set(k, v));
    }
    refresh() {
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
    base(base) {
        if (this._url !== null) {
            throw new Error("Cannot redefine base after defining the protocol and domain");
        }
        this._base = base;
        this.refresh();
        return this;
    }
    protocol(protocol) {
        this._protocol = protocol;
        if (this.refresh()) {
            this._url.protocol = protocol;
        }
        return this;
    }
    host(host) {
        this._host = host;
        if (this.refresh()) {
            this._url.host = host;
            this._hostName = this._url.hostname;
            this._port = parsePort(this._url.port);
        }
        return this;
    }
    hostName(hostName) {
        this._hostName = hostName;
        if (this.refresh()) {
            this._url.hostname = hostName;
            this._host = `${this._url.hostname}:${this._url.port}`;
        }
        return this;
    }
    port(port) {
        this._port = port;
        if (this.refresh()) {
            this._url.port = port.toFixed(0);
            this._host = `${this._url.hostname}:${this._url.port}`;
        }
        return this;
    }
    userName(userName) {
        this._userName = userName;
        if (this.refresh()) {
            this._url.username = userName;
        }
        return this;
    }
    password(password) {
        this._password = password;
        if (this.refresh()) {
            this._url.password = password;
        }
        return this;
    }
    path(path) {
        this._pathName = path;
        if (this.refresh()) {
            this._url.pathname = path;
        }
        return this;
    }
    pathPop(pattern) {
        pattern = pattern || /\/[^/]+\/?$/;
        return this.path(this._pathName.replace(pattern, ""));
    }
    pathPush(part) {
        let path = this._pathName;
        if (!path.endsWith("/")) {
            path += "/";
        }
        path += part;
        return this.path(path);
    }
    query(name, value) {
        this._query.set(name, value);
        if (this.refresh()) {
            this._url.searchParams.set(name, value);
        }
        return this;
    }
    hash(hash) {
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
//# sourceMappingURL=URLBuilder.js.map