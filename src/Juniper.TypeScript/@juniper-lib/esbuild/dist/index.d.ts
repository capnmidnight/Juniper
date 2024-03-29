import { Plugin, BuildOptions } from "esbuild";
type Define = [string, string];
type DefineFactory = (minify: boolean) => Define;
type OptionAlterer = (opts: BuildOptions) => void;
type PluginFactory = (minify: boolean) => Plugin;
type Callback = () => void;
export declare class Build {
    private readonly buildWorkers;
    private readonly browserEntries;
    private readonly minBrowserEntries;
    private readonly plugins;
    private readonly defines;
    private readonly externals;
    private readonly manualOptionsChanges;
    private readonly isWatch;
    get buildType(): "watch" | "build";
    private entryNames;
    private outbase;
    private outDirName;
    private enableSplitting;
    constructor(args: string[], buildWorkers: boolean);
    entryName(name: string): this;
    outDir(name: string): this;
    outBase(name: string): this;
    plugin(pgn: PluginFactory): this;
    define(def: DefineFactory): this;
    external(extern: string, enabled: boolean): this;
    splitting(enable: boolean): this;
    bundle(name: string): this;
    bundles(...names: string[]): this;
    find(...rootDirs: string[]): this;
    manually(thunk: OptionAlterer): this;
    getTasks(onStart: Callback, onEnd: Callback): Promise<void>[];
    private makeBundle;
}
export declare function runBuilds(...builds: Build[]): Promise<void>;
export {};
//# sourceMappingURL=index.d.ts.map