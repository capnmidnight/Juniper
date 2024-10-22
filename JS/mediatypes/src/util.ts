import { isDefined, isNullOrUndefined, isString } from "@juniper-lib/util";

export const typePattern = /*@__PURE__*/ (function () { return /([^\/]+)\/(.+)/; })();
const subTypePattern = /*@__PURE__*/ (function () { return /(?:([^\.]+)\.)?([^\+;]+)(?:\+([^;]+))?((?:; *([^=]+)=([^;]+))*)/; })();

export function mediaTypesToAcceptValue(types: MediaType[]): string {
    return types
        .flatMap(type => type.extensions)
        .sort()
        .join(", ");
}

type FileExtension = `.${string}`;
type MIMEType = `${string}/${string}`;

export class MediaType {
    readonly #type: string;
    readonly #fullSubType: string;
    readonly #tree: string;
    readonly #subType: string;
    readonly #suffix: string;
    readonly #parameters: ReadonlyMap<string, string>;

    readonly #value: MIMEType;
    readonly #fullValue: MIMEType;

    readonly #extensions: ReadonlyArray<FileExtension>;
    readonly #primaryExtension: string = null;

    #depMessage: string = null;


    constructor(type: string, fullSubType: string, extensions?: ReadonlyArray<string>) {
        this.#type = type;
        this.#fullSubType = fullSubType;

        const parameters = new Map<string, string>();
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
            .map(ext => ext.startsWith(".") ? ext : `.${ext}`) as FileExtension[];
        this.#primaryExtension = this.#extensions[0] || null;
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
        this.#depMessage = message;
        return this;
    }

    #check() {
        if (isDefined(this.#depMessage)) {
            console.warn(`${this.#value} is deprecated ${this.#depMessage}`);
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
            subTypeName = value.#fullSubType;
        }

        return this.typeName === typeName
            && (this.#fullSubType === "*" || this.#fullSubType === subTypeName);
    }

    withParameter(key: string, value: string): MediaType {
        const newSubType = `${this.#fullSubType}; ${key}=${value}`;
        return new MediaType(this.typeName, newSubType, this.extensions);
    }

    get typeName(): string {
        this.#check();
        return this.#type;
    }

    get tree(): string {
        this.#check();
        return this.#tree;
    }

    get suffix(): string {
        return this.#suffix;
    }

    get subTypeName(): string {
        this.#check();
        return this.#subType;
    }

    get value(): MIMEType {
        this.#check();
        return this.#value;
    }

    __getValueUnsafe() {
        return this.#value;
    }

    get fullValue(): string {
        this.#check();
        return this.#fullValue;
    }

    get parameters(): ReadonlyMap<string, string> {
        this.#check();
        return this.#parameters;
    }

    get extensions(): ReadonlyArray<FileExtension> {
        this.#check();
        return this.#extensions;
    }

    __getExtensionsUnsafe() {
        return this.#extensions;
    }

    get primaryExtension(): string {
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

    toFileSystemAPIAccepts(): Record<MIMEType, FileExtension | FileExtension[]> {
        return {
            [this.value]: Array.from(this.extensions)
        };
    }

    addExtension(fileName: string): string {
        if (!fileName) {
            throw new Error("File name is not defined");
        }

        if (this.primaryExtension) {
            fileName = MediaType.removeExtension(fileName);
            fileName += this.primaryExtension;
        }

        return fileName;
    }

    static removeExtension(fileName: string) {
        const idx = fileName.lastIndexOf(".");
        if (idx > -1) {
            fileName = fileName.substring(0, idx);
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