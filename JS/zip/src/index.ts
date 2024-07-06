import { AsyncZipOptions, strToU8, zip } from "fflate";

export type CompressionLevels = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9;

export interface IZipDirectory {
    /**
     * Add a some data as a buffer to the ZIP archive.
     */
    add(path: string, arr: Uint8Array): IZipDirectory;

    /**
     * Add an object as a JSON file to the ZIP archive.
     */
    addObject(path: string, obj: object): IZipDirectory;

    /**
     * Creates a named entry point for adding files to the ZIP archive.
     */
    addDirectory(path: string): IZipDirectory;

    /**
     * Go up a level in the directory structure.
     */
    exit(): IZipDirectory;

    /**
     * Create the ZIP file as a blob
     * @param level the compression level to apply.
     * @returns 
     */
    create(level?: CompressionLevels): Promise<Blob>
}

/**
 * A constructing object for building ZIP archives.
 */
export class ZipFile implements IZipDirectory {
    #entries: Record<string, Uint8Array> = {};

    /**
     * Add a some data as a buffer to the ZIP
     */
    add(path: string, arr: Uint8Array) {
        this.#entries[path] = arr;
        return this;
    }

    /**
     * Add an object as a JSON file to the ZIP
     */
    addObject(path: string, obj: object) {
        const arr = strToU8(JSON.stringify(obj, null, 2));
        this.add(path, arr);
        return this;
    }

    /**
     * Creates a named entry point for adding files to the ZIP archive.
     */
    addDirectory(path: string): IZipDirectory {
        return new ZipFileDirectory(this, path);
    }

    exit() {
        // no where to go
        return this;
    }

    /**
     * Create the ZIP file as a blob
     * @param level the compression level to apply.
     * @returns 
     */
    create(level?: CompressionLevels) {
        const options: AsyncZipOptions = {
            consume: true,
            level
        }
        return new Promise<Blob>((resolve, reject) => {
            zip(this.#entries, options, (err, data) => {
                if (err) {
                    reject(err);
                }
                else {
                    resolve(new Blob([data], {
                        type: "application/zip"
                    }));
                }
            });
        });
    }
}

/**
 * A redirecting object to prefix file paths with a directory path.
 */
class ZipFileDirectory implements IZipDirectory {

    #parent: IZipDirectory;
    #path: string;

    constructor(zipFile: IZipDirectory, path: string) {
        this.#parent = zipFile;
        this.#path = path;
    }

    /**
     * Add a some data as a buffer to the ZIP
     */
    add(path: string, arr: Uint8Array) {
        this.#parent.add(this.#path + "/" + path, arr);
        return this;
    }

    /**
     * Add an object as a JSON file to the ZIP
     */
    addObject(path: string, obj: object) {
        this.#parent.addObject(this.#path + "/" + path, obj);
        return this;
    }

    /**
     * Creates a named entry point for adding files to the ZIP archive.
     */
    addDirectory(path: string): IZipDirectory {
        return new ZipFileDirectory(this, path);
    }

    exit() {
        return this.#parent;
    }

    /**
     * Create the ZIP file as a blob
     * @param level the compression level to apply.
     * @returns 
     */
    create(level?: CompressionLevels) {
        return this.#parent.create(level);
    }
}
