import { isDefined, isNullOrUndefined, isString } from "@juniper-lib/util";
export const typePattern = /*@__PURE__*/ (function () { return /([^\/]+)\/(.+)/; })();
const subTypePattern = /*@__PURE__*/ (function () { return /(?:([^\.]+)\.)?([^\+;]+)(?:\+([^;]+))?((?:; *([^=]+)=([^;]+))*)/; })();
export function mediaTypesToAcceptValue(types) {
    return types
        .flatMap(type => type.extensions)
        .sort()
        .join(", ");
}
export class MediaType {
    #type;
    #fullSubType;
    #tree;
    #subType;
    #suffix;
    #parameters;
    #value;
    #fullValue;
    #extensions;
    #primaryExtension = null;
    #depMessage = null;
    constructor(type, fullSubType, extensions) {
        this.#type = type;
        this.#fullSubType = fullSubType;
        const parameters = new Map();
        this.#parameters = parameters;
        const subTypeParts = this.#fullSubType.match(subTypePattern);
        this.#tree = subTypeParts[1];
        this.#subType = subTypeParts[2];
        this.#suffix = subTypeParts[3];
        const paramStr = subTypeParts[4];
        let subType = "";
        if (isDefined(this.#tree)) {
            subType += this.#tree + ".";
        }
        subType += this.#subType;
        if (isDefined(this.#suffix)) {
            subType += "+" + this.#suffix;
        }
        this.#fullValue = this.#value = `${this.#type}/${subType}`;
        if (isDefined(paramStr)) {
            const pairs = paramStr.split(";")
                .map((p) => p.trim())
                .filter((p) => p.length > 0)
                .map((p) => p.split("="));
            for (const [key, ...values] of pairs) {
                const value = values.join("=");
                parameters.set(key, value);
                const slug = `; ${key}=${value}`;
                this.#fullValue += slug;
                if (key !== "q") {
                    this.#value += slug;
                }
            }
        }
        this.#extensions = (extensions || [])
            .map(ext => ext.startsWith(".") ? ext : `.${ext}`);
        this.#primaryExtension = this.#extensions[0] || null;
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
        this.#depMessage = message;
        return this;
    }
    #check() {
        if (isDefined(this.#depMessage)) {
            console.warn(`${this.#value} is deprecated ${this.#depMessage}`);
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
            subTypeName = value.#fullSubType;
        }
        return this.typeName === typeName
            && (this.#fullSubType === "*" || this.#fullSubType === subTypeName);
    }
    withParameter(key, value) {
        const newSubType = `${this.#fullSubType}; ${key}=${value}`;
        return new MediaType(this.typeName, newSubType, this.extensions);
    }
    get typeName() {
        this.#check();
        return this.#type;
    }
    get tree() {
        this.#check();
        return this.#tree;
    }
    get suffix() {
        return this.#suffix;
    }
    get subTypeName() {
        this.#check();
        return this.#subType;
    }
    get value() {
        this.#check();
        return this.#value;
    }
    __getValueUnsafe() {
        return this.#value;
    }
    get fullValue() {
        this.#check();
        return this.#fullValue;
    }
    get parameters() {
        this.#check();
        return this.#parameters;
    }
    get extensions() {
        this.#check();
        return this.#extensions;
    }
    __getExtensionsUnsafe() {
        return this.#extensions;
    }
    get primaryExtension() {
        this.#check();
        return this.#primaryExtension;
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
            [this.value]: Array.from(this.extensions)
        };
    }
    addExtension(fileName) {
        if (!fileName) {
            throw new Error("File name is not defined");
        }
        if (this.primaryExtension) {
            fileName = MediaType.removeExtension(fileName);
            fileName += this.primaryExtension;
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