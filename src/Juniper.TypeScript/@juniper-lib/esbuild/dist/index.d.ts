import { ModuleInfo } from "@fal-works/esbuild-plugin-global-externals";
import { Plugin, BuildOptions } from "esbuild";
type Define = [string, string];
type DefineFactory = (minify: boolean) => Define;
type OptionAlterer = (opts: BuildOptions) => void;
type PluginFactory = (minify: boolean) => Plugin;
export declare class Build {
    private readonly buildWorkers;
    private readonly browserEntries;
    private readonly minBrowserEntries;
    private readonly plugins;
    private readonly defines;
    private readonly externals;
    private readonly manualOptionsChanges;
    private readonly globalExternals;
    private readonly isWatch;
    get buildType(): "watch" | "build";
    private entryNames;
    private outbase;
    private outDirName;
    constructor(args: string[], buildWorkers: boolean);
    entryName(name: string): this;
    outDir(name: string): this;
    outBase(name: string): this;
    plugin(pgn: PluginFactory): this;
    define(def: DefineFactory): this;
    external(extern: string): this;
    globalExternal(packageName: string, info: ModuleInfo): this;
    addThreeJS(enabled: boolean): this;
    bundle(name: string): this;
    bundles(...names: string[]): this;
    find(...rootDirs: string[]): this;
    manually(thunk: OptionAlterer): this;
    run(): Promise<void>;
    private makeBundle;
}
export {};
//# sourceMappingURL=index.d.ts.map