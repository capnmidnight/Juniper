import { isDefined, isNullOrUndefined, isString } from "@juniper-lib/tslib/typeChecks";
export const typePattern = /([^\/]+)\/(.+)/;
const subTypePattern = /(?:([^\.]+)\.)?([^\+;]+)(?:\+([^;]+))?((?:; *([^=]+)=([^;]+))*)/;
export function mediaTypesToAcceptValue(types) {
    return types
        .flatMap(type => type.extensions.map(ext => "." + ext))
        .sort()
        .join(", ");
}
export class MediaType {
    constructor(_type, _fullSubType, extensions) {
        this._type = _type;
        this._fullSubType = _fullSubType;
        this._primaryExtension = null;
        this.depMessage = null;
        const parameters = new Map();
        this._parameters = parameters;
        const subTypeParts = this._fullSubType.match(subTypePattern);
        this._tree = subTypeParts[1];
        this._subType = subTypeParts[2];
        this._suffix = subTypeParts[3];
        const paramStr = subTypeParts[4];
        this._value = this._fullValue = this._type + "/";
        if (isDefined(this._tree)) {
            this._value = this._fullValue += this._tree + ".";
        }
        this._value = this._fullValue += this._subType;
        if (isDefined(this._suffix)) {
            this._value = this._fullValue += "+" + this._suffix;
        }
        if (isDefined(paramStr)) {
            const pairs = paramStr.split(";")
                .map((p) => p.trim())
                .filter((p) => p.length > 0)
                .map((p) => p.split("="));
            for (const [key, ...values] of pairs) {
                const value = values.join("=");
                parameters.set(key, value);
                const slug = `; ${key}=${value}`;
                this._fullValue += slug;
                if (key !== "q") {
                    this._value += slug;
                }
            }
        }
        this._extensions = extensions || [];
        this._primaryExtension = this._extensions[0] || null;
    }
    static parse(value) {
        if (!value) {
            return null;
        }
        const match = value.match(typePattern);
        if (!match) {
            return null;
        }
        const type = match[1];
        const subType = match[2];
        return new MediaType(type, subType);
    }
    deprecate(message) {
        this.depMessage = message;
        return this;
    }
    check() {
        if (isDefined(this.depMessage)) {
            console.warn(`${this._value} is deprecated ${this.depMessage}`);
        }
    }
    matches(value) {
        if (isNullOrUndefined(value)) {
            return false;
        }
        if (this.typeName === "*" && this.subTypeName === "*") {
            return true;
        }
        let typeName = null;
        let subTypeName = null;
        if (isString(value)) {
            const match = value.match(typePattern);
            if (!match) {
                return false;
            }
            typeName = match[1];
            subTypeName = match[2];
        }
        else {
            typeName = value.typeName;
            subTypeName = value._fullSubType;
        }
        return this.typeName === typeName
            && (this._fullSubType === "*" || this._fullSubType === subTypeName);
    }
    withParameter(key, value) {
        const newSubType = `${this._fullSubType}; ${key}=${value}`;
        return new MediaType(this.typeName, newSubType, this.extensions);
    }
    get typeName() {
        this.check();
        return this._type;
    }
    get tree() {
        this.check();
        return this._tree;
    }
    get suffix() {
        return this._suffix;
    }
    get subTypeName() {
        this.check();
        return this._subType;
    }
    get value() {
        this.check();
        return this._value;
    }
    __getValueUnsafe() {
        return this._value;
    }
    get fullValue() {
        this.check();
        return this._fullValue;
    }
    get parameters() {
        this.check();
        return this._parameters;
    }
    get extensions() {
        this.check();
        return this._extensions;
    }
    __getExtensionsUnsafe() {
        return this._extensions;
    }
    get primaryExtension() {
        this.check();
        return this._primaryExtension;
    }
    toString() {
        if (this.parameters.get("q") === "1") {
            return this.value;
        }
        else {
            return this.fullValue;
        }
    }
    toFileSystemAPIAccepts() {
        return {
            [this.value]: this.extensions.map(v => "." + v)
        };
    }
    addExtension(fileName) {
        if (!fileName) {
            throw new Error("File name is not defined");
        }
        if (this.primaryExtension) {
            fileName = MediaType.removeExtension(fileName);
            fileName = `${fileName}.${this.primaryExtension}`;
        }
        return fileName;
    }
    static removeExtension(fileName) {
        const idx = fileName.lastIndexOf(".");
        if (idx > -1) {
            fileName = fileName.substring(0, idx);
        }
        return fileName;
    }
}
export function create(group, value, ...extensions) {
    return new MediaType(group, value, extensions);
}
export function specialize(group) {
    return create.bind(null, group);
}
//# sourceMappingURL=util.js.map