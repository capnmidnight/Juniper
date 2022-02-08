import { isDefined, isString, singleton } from "juniper-tslib";
const byValue = singleton("Juniper:MediaTypes:byValue", () => new Map());
const byExtension = singleton("Juniper:MediaTypes:byExtension", () => new Map());
const depMessages = singleton("Juniper:MediaTypes:depMessages", () => new WeakMap());
function register(type) {
    let isNew = false;
    type = singleton("Juniper.MediaTypes:" + type.value, () => {
        isNew = true;
        return type;
    });
    if (isNew) {
        byValue.set(type.value, type);
        for (const ext of type.extensions) {
            if (!byExtension.has(ext)) {
                byExtension.set(ext, new Array());
            }
            const byExts = byExtension.get(ext);
            if (byExts.indexOf(type) < 0) {
                byExts.push(type);
            }
        }
    }
    return type;
}
export function deprecate(type, msg) {
    depMessages.set(type, msg);
    return type;
}
const subTypePattern = /(?:([^\.]+)\.)?([^\+;]+)(?:\+([^;]+))?((?:; *([^=]+)=([^;]+))*)/;
class InternalMediaType {
    _type;
    _fullSubType;
    _tree;
    _subType;
    _suffix;
    _parameters;
    _value;
    _fullValue;
    _extensions;
    _primaryExtension = null;
    constructor(_type, _fullSubType, extensions) {
        this._type = _type;
        this._fullSubType = _fullSubType;
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
            const pairs = paramStr.split(';')
                .map(p => p.trim())
                .filter(p => p.length > 0)
                .map(p => p.split('='));
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
    check() {
        const msg = depMessages.get(this);
        if (msg) {
            console.warn(`${this._value} is deprecated ${msg}`);
        }
    }
    withParameter(key, value) {
        const newSubType = `${this._fullSubType}; ${key}=${value}`;
        const type = new InternalMediaType(this.typeName, newSubType, this.extensions);
        const msg = depMessages.get(this);
        if (msg) {
            depMessages.set(type, msg);
        }
        return type;
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
    addExtension(fileName) {
        if (!fileName) {
            throw new Error("File name is not defined");
        }
        if (this.primaryExtension) {
            const idx = fileName.lastIndexOf(".");
            if (idx > -1) {
                const currentExtension = fileName.substring(idx + 1);
                ;
                if (this.extensions.indexOf(currentExtension) > -1) {
                    fileName = fileName.substring(0, idx);
                }
            }
            fileName = `${fileName}.${this.primaryExtension}`;
        }
        return fileName;
    }
    matches(value) {
        if (isString(value)) {
            value = mediaTypeParse(value);
        }
        return (this.typeName === "*" && this.subTypeName === "*")
            || (this.typeName === value.typeName && (this.subTypeName === "*" || this.subTypeName === value.subTypeName));
    }
    matchesFileName(fileName) {
        if (!fileName) {
            return false;
        }
        const types = mediaTypeGuessByFileName(fileName);
        for (const type of types) {
            if (mediaTypesMatch(type, this)) {
                return true;
            }
        }
        return false;
    }
}
const typePattern = /([^\/]+)\/(.+)/;
export function mediaTypeParse(value) {
    if (!value) {
        return null;
    }
    const match = value.match(typePattern);
    if (!match) {
        return null;
    }
    const type = match[1];
    const subType = match[2];
    const parsedType = new InternalMediaType(type, subType);
    const weight = parsedType.parameters.get("q");
    const basicType = byValue.get(parsedType.value) || new InternalMediaType(type, subType, []);
    if (isDefined(weight)) {
        return basicType.withParameter("q", weight);
    }
    else {
        return basicType;
    }
}
export function mediaTypesMatch(value, pattern) {
    if (isString(value)) {
        value = mediaTypeParse(value);
    }
    if (isString(pattern)) {
        pattern = mediaTypeParse(pattern);
    }
    return pattern.matches(value);
}
export function mediaTypeGuessByFileName(fileName) {
    if (!fileName) {
        console.warn("Couldn't guess media type. Must provide a valid fileName.");
        return [];
    }
    const idx = fileName.lastIndexOf(".");
    if (idx === -1) {
        console.warn("Couldn't guess media type. FileName has no extension.");
        return [];
    }
    const ext = fileName.substring(idx);
    return mediaTypeGuessByExtension(ext);
}
export function mediaTypeGuessByExtension(ext) {
    if (!ext) {
        ext = "unknown";
    }
    else if (ext[0] == '.') {
        ext = ext.substring(1);
    }
    if (byExtension.has(ext)) {
        return byExtension.get(ext);
    }
    else {
        return [new InternalMediaType("unknown", ext, [ext])];
    }
}
export function mediaTypeNormalizeFileName(fileName, fileType) {
    if (!fileType && fileName.indexOf(".") > -1) {
        const guesses = mediaTypeGuessByFileName(fileName);
        if (guesses.length > 0) {
            fileType = guesses[0].value;
        }
    }
    return fileType;
}
export function create(group, value, ...extensions) {
    return register(new InternalMediaType(group, value, extensions));
}
export function specialize(group) {
    return function (value, ...extensions) {
        return create(group, value, ...extensions);
    };
}
