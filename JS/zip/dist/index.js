import { strToU8, zip } from "fflate";
/**
 * A constructing object for building ZIP archives.
 */
export class ZipFile {
    #entries = {};
    /**
     * Add a some data as a buffer to the ZIP
     */
    add(path, arr) {
        this.#entries[path] = arr;
        return this;
    }
    /**
     * Add an object as a JSON file to the ZIP
     */
    addObject(path, obj) {
        const arr = strToU8(JSON.stringify(obj, null, 2));
        this.add(path, arr);
        return this;
    }
    /**
     * Creates a named entry point for adding files to the ZIP archive.
     */
    addDirectory(path) {
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
    create(level) {
        const options = {
            consume: true,
            level
        };
        return new Promise((resolve, reject) => {
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
class ZipFileDirectory {
    #parent;
    #path;
    constructor(zipFile, path) {
        this.#parent = zipFile;
        this.#path = path;
    }
    /**
     * Add a some data as a buffer to the ZIP
     */
    add(path, arr) {
        this.#parent.add(this.#path + "/" + path, arr);
        return this;
    }
    /**
     * Add an object as a JSON file to the ZIP
     */
    addObject(path, obj) {
        this.#parent.addObject(this.#path + "/" + path, obj);
        return this;
    }
    /**
     * Creates a named entry point for adding files to the ZIP archive.
     */
    addDirectory(path) {
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
    create(level) {
        return this.#parent.create(level);
    }
}
//# sourceMappingURL=index.js.map