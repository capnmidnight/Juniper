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
    create(level?: CompressionLevels): Promise<Blob>;
}
/**
 * A constructing object for building ZIP archives.
 */
export declare class ZipFile implements IZipDirectory {
    #private;
    /**
     * Add a some data as a buffer to the ZIP
     */
    add(path: string, arr: Uint8Array): this;
    /**
     * Add an object as a JSON file to the ZIP
     */
    addObject(path: string, obj: object): this;
    /**
     * Creates a named entry point for adding files to the ZIP archive.
     */
    addDirectory(path: string): IZipDirectory;
    exit(): this;
    /**
     * Create the ZIP file as a blob
     * @param level the compression level to apply.
     * @returns
     */
    create(level?: CompressionLevels): Promise<Blob>;
}
//# sourceMappingURL=index.d.ts.map