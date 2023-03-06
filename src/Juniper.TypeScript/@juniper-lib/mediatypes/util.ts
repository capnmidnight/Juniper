import { isDefined, isNullOrUndefined, isString } from "@juniper-lib/tslib/typeChecks";

export const typePattern = /([^\/]+)\/(.+)/;
const subTypePattern = /(?:([^\.]+)\.)?([^\+;]+)(?:\+([^;]+))?((?:; *([^=]+)=([^;]+))*)/;

export function mediaTypesToAcceptValue(types: MediaType[]): string {
    return types
        .flatMap(type =>
            type.extensions.map(ext =>
                "." + ext))
        .sort()
        .join(", ");
}

export class MediaType {
    private readonly _tree: string;
    private readonly _subType: string;
    private readonly _suffix: string;
    private readonly _parameters: ReadonlyMap<string, string>;

    private readonly _value: string;
    private readonly _fullValue: string;

    private readonly _extensions: ReadonlyArray<string>;
    private readonly _primaryExtension: string = null;

    private depMessage: string = null;


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

    static parse(value: string): MediaType {
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

    deprecate(message: string): this {
        this.depMessage = message;
        return this;
    }

    private check() {
        if (isDefined(this.depMessage)) {
            console.warn(`${this._value} is deprecated ${this.depMessage}`);
        }
    }

    matches(value: MediaType | string): boolean {
        if (isNullOrUndefined(value)) {
            return false;
        }

        if (this.typeName === "*" && this.subTypeName === "*") {
            return true;
        }

        let typeName: string = null;
        let subTypeName: string = null;
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

    withParameter(key: string, value: string): MediaType {
        const newSubType = `${this._fullSubType}; ${key}=${value}`;
        return new MediaType(this.typeName, newSubType, this.extensions);
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

    __getValueUnsafe() {
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

    __getExtensionsUnsafe() {
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
}

export function create(group: string, value: string, ...extensions: string[]): MediaType {
    return new MediaType(group, value, extensions);
}

export function specialize(group: string) {
    return create.bind(null, group);
}