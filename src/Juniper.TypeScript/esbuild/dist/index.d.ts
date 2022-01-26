import { Plugin } from "esbuild";
declare type Define = [string, string];
declare type DefineFactory = (minify: boolean) => Define;
declare type PluginFactory = (minify: boolean) => Plugin;
export declare class Build {
    private readonly browserEntries;
    private readonly minBrowserEntries;
    private readonly plugins;
    private readonly defines;
    private readonly externals;
    private readonly isWatch;
    private rootDirName;
    private outDirName;
    private bundleOutDirName;
    constructor(args: string[]);
    rootDir(name: string): this;
    outDir(name: string): this;
    bundleOutDir(name: string): this;
    plugin(pgn: PluginFactory): this;
    define(def: DefineFactory): this;
    external(extern: string): this;
    bundle(name: string): this;
    run(): Promise<void>;
    private makeBundle;
}
export {};
