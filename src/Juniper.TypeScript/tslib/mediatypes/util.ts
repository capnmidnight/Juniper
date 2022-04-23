import { isDefined, isNullOrUndefined, isString, singleton } from "../";

export interface MediaType {
    typeName: string;
    subTypeName: string;
    value: string;
    fullValue: string;
    extensions: ReadonlyArray<string>;
    primaryExtension: string;

    withParameter(key: string, value: string): MediaType;

    toString(): string;
    addExtension(fileName: string): string;
    matches(value: MediaType | string): boolean;
    matchesFileName(fileName: string): boolean;
}


const byValue = singleton("Juniper:MediaTypes:byValue", () => new Map<string, MediaType>());
const byExtension = singleton("Juniper:MediaTypes:byExtension", () => new Map<string, MediaType[]>());
const depMessages = singleton("Juniper:MediaTypes:depMessages", () => new WeakMap<MediaType, string>());
const comments = singleton("Juniper:MediaTypes:comments", () => new WeakMap<MediaType, string>());

function register(type: MediaType): MediaType {
    let isNew = false;
    type = singleton("Juniper.MediaTypes:" + type.value, () => {
        isNew = true;
        return type;
    });

    if (isNew) {
        byValue.set(type.value, type);

        for (const ext of type.extensions) {
            if (!byExtension.has(ext)) {
                byExtension.set(ext, new Array<MediaType>());
            }

            const byExts = byExtension.get(ext);
            if (byExts.indexOf(type) < 0) {
                byExts.push(type);
            }
        }
    }

    return type;
}

export function deprecate(type: MediaType, msg: string): MediaType {
    depMessages.set(type, msg);
    return type;
}

export function comment(type: MediaType, msg: string): MediaType {
    comments.set(type, msg);
    return type;
}

const subTypePattern = /(?:([^\.]+)\.)?([^\+;]+)(?:\+([^;]+))?((?:; *([^=]+)=([^;]+))*)/;
class InternalMediaType implements MediaType {
    private readonly _tree: string;
    private readonly _subType: string;
    private readonly _suffix: string;
    private readonly _parameters: ReadonlyMap<string, string>;

    private readonly _value: string;
    private readonly _fullValue: string;

    private readonly _extensions: ReadonlyArray<string>;
    private readonly _primaryExtension: string = null;


    constructor(
        private readonly _type: string,
        private readonly _fullSubType: string,
        extensions?: ReadonlyArray<string>) {

        const parameters = new Map<string, string>();
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
                .map((p) => p.trim())
                .filter((p) => p.length > 0)
                .map((p) => p.split('='));
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

    private check() {
        const msg = depMessages.get(this);
        if (msg) {
            console.warn(`${this._value} is deprecated ${msg}`);
        }
    }

    withParameter(key: string, value: string): MediaType {
        const newSubType = `${this._fullSubType}; ${key}=${value}`;
        return new InternalMediaType(this.typeName, newSubType, this.extensions);
    }

    get typeName(): string {
        this.check();
        return this._type;
    }

    get tree(): string {
        this.check();
        return this._tree;
    }

    get suffix(): string {
        return this._suffix;
    }

    get subTypeName(): string {
        this.check();
        return this._subType;
    }

    get value(): string {
        this.check();
        return this._value;
    }

    get fullValue(): string {
        this.check();
        return this._fullValue;
    }

    get parameters(): ReadonlyMap<string, string> {
        this.check();
        return this._parameters;
    }

    get extensions(): ReadonlyArray<string> {
        this.check();
        return this._extensions;
    }

    get primaryExtension(): string {
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

    addExtension(fileName: string): string {
        if (!fileName) {
            throw new Error("File name is not defined");
        }

        if (this.primaryExtension) {
            const idx = fileName.lastIndexOf(".");
            if (idx > -1) {
                const currentExtension = fileName.substring(idx + 1);;
                if (this.extensions.indexOf(currentExtension) > -1) {
                    fileName = fileName.substring(0, idx);
                }
            }

            fileName = `${fileName}.${this.primaryExtension}`;
        }

        return fileName;
    }

    matches(value: MediaType | string): boolean {
        if (isNullOrUndefined(value)) {
            return false;
        }

        if (isString(value)) {
            value = mediaTypeParse(value);
        }

        return (this.typeName === "*" && this.subTypeName === "*")
            || (this.typeName === value.typeName && (this.subTypeName === "*" || this.subTypeName === value.subTypeName));
    }

    matchesFileName(fileName: string): boolean {
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

    get comment() {
        return comments.get(this);
    }
}

const typePattern = /([^\/]+)\/(.+)/;
export function mediaTypeParse(value: string): MediaType {
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
    const basicType = byValue.get(parsedType.value)
        || parsedType;

    if (isDefined(weight)) {
        return basicType.withParameter("q", weight);
    }
    else {
        return basicType;
    }
}

export function mediaTypesMatch(value: string | MediaType, pattern: string | MediaType): boolean {
    if (isString(value)) {
        value = mediaTypeParse(value);
    }

    if (isString(pattern)) {
        pattern = mediaTypeParse(pattern);
    }

    return pattern.matches(value);
}

export function mediaTypeGuessByFileName(fileName: string): MediaType[] {
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

export function mediaTypeGuessByExtension(ext: string): MediaType[] {
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

export function mediaTypeNormalizeFileName(fileName: string, fileType: string): string {
    if (!fileType && fileName.indexOf(".") > -1) {
        const guesses = mediaTypeGuessByFileName(fileName);
        if (guesses.length > 0) {
            fileType = guesses[0].value;
        }
    }

    return fileType;
}

export function create(group: string, value: string, ...extensions: string[]): MediaType {
    return register(new InternalMediaType(group, value, extensions))
}

export function specialize(group: string) {
    return function (value: string, ...extensions: string[]): MediaType {
        return create(group, value, ...extensions);
    };
}