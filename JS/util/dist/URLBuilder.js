import { isDefined } from "./typeChecks";
function parsePort(portString) {
    if (isDefined(portString) && portString.length > 0) {
        return parseFloat(portString);
    }
    return null;
}
export class URLBuilder {
    #url = null;
    #base = undefined;
    #protocol = null;
    #host = null;
    #hostName = null;
    #userName = null;
    #password = null;
    #port = null;
    #pathName = null;
    #hash = null;
    #query = new Map();
    constructor(url, base) {
        if (url !== undefined) {
            this.#url = new URL(url, base);
            this.#rehydrate();
        }
    }
    #rehydrate() {
        if (isDefined(this.#protocol) && this.#protocol !== this.#url.protocol) {
            this.#url.protocol = this.#protocol;
        }
        if (isDefined(this.#host) && this.#host !== this.#url.host) {
            this.#url.host = this.#host;
        }
        if (isDefined(this.#hostName) && this.#hostName !== this.#url.hostname) {
            this.#url.hostname = this.#hostName;
        }
        if (isDefined(this.#userName) && this.#userName !== this.#url.username) {
            this.#url.username = this.#userName;
        }
        if (isDefined(this.#password) && this.#password !== this.#url.password) {
            this.#url.password = this.#password;
        }
        if (isDefined(this.#port) && this.#port.toFixed(0) !== this.#url.port) {
            this.#url.port = this.#port.toFixed(0);
        }
        if (isDefined(this.#pathName) && this.#pathName !== this.#url.pathname) {
            this.#url.pathname = this.#pathName;
        }
        if (isDefined(this.#hash) && this.#hash !== this.#url.hash) {
            this.#url.hash = this.#hash;
        }
        for (const [k, v] of this.#query) {
            this.#url.searchParams.set(k, v);
        }
        this.#protocol = this.#url.protocol;
        this.#host = this.#url.host;
        this.#hostName = this.#url.hostname;
        this.#userName = this.#url.username;
        this.#password = this.#url.password;
        this.#port = parsePort(this.#url.port);
        this.#pathName = this.#url.pathname;
        this.#hash = this.#url.hash;
        this.#url.searchParams.forEach((v, k) => this.#query.set(k, v));
    }
    #refresh() {
        if (this.#url === null) {
            if (isDefined(this.#protocol)
                && (isDefined(this.#host) || isDefined(this.#hostName))) {
                if (isDefined(this.#host)) {
                    this.#url = new URL(`${this.#protocol}//${this.#host}`, this.#base);
                    this.#port = parsePort(this.#url.port);
                    this.#rehydrate();
                    return false;
                }
                else if (isDefined(this.#hostName)) {
                    this.#url = new URL(`${this.#protocol}//${this.#hostName}`, this.#base);
                    this.#rehydrate();
                    return false;
                }
            }
            else if (isDefined(this.#pathName) && isDefined(this.#base)) {
                this.#url = new URL(this.#pathName, this.#base);
                this.#rehydrate();
                return false;
            }
        }
        return isDefined(this.#url);
    }
    base(base) {
        if (this.#url !== null) {
            throw new Error("Cannot redefine base after defining the protocol and domain");
        }
        this.#base = base;
        this.#refresh();
        return this;
    }
    protocol(protocol) {
        this.#protocol = protocol;
        if (this.#refresh()) {
            this.#url.protocol = protocol;
        }
        return this;
    }
    host(host) {
        this.#host = host;
        if (this.#refresh()) {
            this.#url.host = host;
            this.#hostName = this.#url.hostname;
            this.#port = parsePort(this.#url.port);
        }
        return this;
    }
    hostName(hostName) {
        this.#hostName = hostName;
        if (this.#refresh()) {
            this.#url.hostname = hostName;
            this.#host = `${this.#url.hostname}:${this.#url.port}`;
        }
        return this;
    }
    port(port) {
        this.#port = port;
        if (this.#refresh()) {
            this.#url.port = port.toFixed(0);
            this.#host = `${this.#url.hostname}:${this.#url.port}`;
        }
        return this;
    }
    userName(userName) {
        this.#userName = userName;
        if (this.#refresh()) {
            this.#url.username = userName;
        }
        return this;
    }
    password(password) {
        this.#password = password;
        if (this.#refresh()) {
            this.#url.password = password;
        }
        return this;
    }
    path(path) {
        this.#pathName = path;
        if (this.#refresh()) {
            this.#url.pathname = path;
        }
        return this;
    }
    pathPop(pattern) {
        pattern = pattern || /\/[^/]+\/?$/;
        return this.path(this.#pathName.replace(pattern, ""));
    }
    pathPush(part) {
        let path = this.#pathName;
        if (!path.endsWith("/")) {
            path += "/";
        }
        path += part;
        return this.path(path);
    }
    query(name, value) {
        this.#query.set(name, value);
        if (this.#refresh()) {
            this.#url.searchParams.set(name, value);
        }
        return this;
    }
    hash(hash) {
        this.#hash = hash;
        if (this.#refresh()) {
            this.#url.hash = hash;
        }
        return this;
    }
    toURL() {
        return this.#url;
    }
    toString() {
        return this.#url.href;
    }
    [Symbol.toStringTag]() {
        return this.toString();
    }
}
//# sourceMappingURL=URLBuilder.js.map